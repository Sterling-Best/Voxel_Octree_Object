using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Debating Whether to add seed to this or world?

public class Chunk_Manager : MonoBehaviour
{
    //World Parameters
    private int worldSeed;

    //World Boundaries - Null if no boundary exists.
    private bool isBoundaries;
    private float[] boundaries = { float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity };

    private int chunkNumLimit;

    //Chunk Parameters
    private int chunkSize;
    private byte chunkMaxDepth;
    private int blockMinSize;

    GameObject defaultChunk;
    

    //Dictionary of Chunks that is currently loaded into the game.
    public Dictionary<Vector3Int, GameObject> currChunks = new Dictionary<Vector3Int, GameObject>();
    private Queue<GameObject> chunkPool;
    private Queue<KeyValuePair<Vector3Int, GameObject>> loadOrder;

    public Chunk_Renderer chunk_Renderer = new Chunk_Renderer();
    

    
    //Block Manager for Block Details - Mainly to hand off as reference to chunk
    private Block_Manager blockManager;
    IEnumerator render;
    IEnumerator merge;

    bool rendering = false;
    bool loading = false;


    GameObject player;
    Vector3 playerposition;
    Vector3 playeroldpos;

    /*
     Notes for Loading Cubes, to start the game quickly
     -Awake/Start should load about 2x2x2 chunks so the player can stand on something
     -Loading Screen Continues untill there is 4x4x4 worth of chunks available. Then loading screen ends.
     -PLayer Starts game, but loading additional chunk for farther distance happen during game time. 
     */


    private void Awake()
    {
        player = GameObject.Find("Player");
        playerposition = player.transform.position;
        playeroldpos = new Vector3(playerposition.x, playerposition.y, playerposition.z);

        render = RenderLoaded();
        merge = MergeChunkNodes();
        blockMinSize = (int)(chunkSize / (Mathf.Pow(2, chunkMaxDepth)));

        //TODO: Remove Tests
        //Testing Chunks
        
    }

    // Start is called before the first frame update
    void Start()
    {
        chunkloader();
        StartRender();



        //for (int y = 0; y < 16; y++)
        //{
        //    for (int x = 0; x < 16; x++)
        //    {
        //        for (int z = 0; z < 16; z++)
        //        {
        //            AddChunk(new Vector3(x * 16, y * 16, z * 16));

        //        }
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        playerposition = player.transform.position;
        if (Vector3.Distance(playerposition, playeroldpos) > 16 && rendering == false)
        {
            loading = true;
            Debug.Log("Collecting Chunk Area");
            chunkloader();
            playeroldpos = new Vector3(playerposition.x, playerposition.y, playerposition.z);

        }
        if (loadOrder.Count > 0 && rendering == false && loading == true)
        {
            rendering = true;
            StartCoroutine("RenderLoaded");
        }
        if(loadOrder.Count == 0)
        {
            StopCoroutine("RenderLoaded");
            rendering = false;
            loading = false;
        }



    }
     

    public void SetChunkManager(int a_worldseed, int a_chunksize, byte a_chunkmaxdepth, int a_chunkdistance, Block_Manager a_blockmanager)
    {
        blockManager = a_blockmanager;
        chunkPool = new Queue<GameObject>();
        loadOrder = new Queue<KeyValuePair<Vector3Int, GameObject>>();
        defaultChunk = (GameObject)Instantiate(Resources.Load("Prefabs/defaultChunk"));
        chunkSize = a_chunksize;
        chunkMaxDepth = a_chunkmaxdepth;
        chunkNumLimit = (int)Math.Pow((a_chunkdistance * 2), 3);
        defaultChunk.GetComponent<Octree_Controller>().octreeSize = chunkSize;
        defaultChunk.GetComponent<Octree_Controller>().chunkMaxDepth = chunkMaxDepth;
        defaultChunk.GetComponent<Octree_Controller>().chunk_Renderer = chunk_Renderer;
        //Set Procedural Seed
        worldSeed = a_worldseed;
        //Load Chunks inot Pool
        for (int i = 0; i < chunkNumLimit; i++)
        {
            GameObject chunk = Instantiate(defaultChunk);
            chunk.name = "Chunk" + Octree_Controller.count.ToString();
            chunk.transform.parent = this.transform;
            chunk.SetActive(false);
            chunkPool.Enqueue(chunk);
        }
        //Set Block Manager
    }

    private void AddChunk(Vector3 a_pos)
    {
        if (chunkPool.Count > 0)
        {
            Vector3Int target3int = new Vector3Int((int)a_pos.x, (int)a_pos.y, (int)a_pos.z);
            GameObject chunk = chunkPool.Dequeue();
            chunk.SetActive(true);
            chunk.transform.position = a_pos;
            loadOrder.Enqueue(new KeyValuePair<Vector3Int, GameObject>(target3int, chunk));
        }
    }

    public void AddBlock(Vector3 a_pos, int type)
    {
        Vector3 targetVec3 = new Vector3((float)Math.Floor(a_pos.x / chunkSize) * chunkSize, (float)Math.Floor(a_pos.y / chunkSize) * chunkSize, (float)Math.Floor(a_pos.z / chunkSize) * chunkSize);
        Vector3Int targetkey = new Vector3Int((int)targetVec3.x, (int)targetVec3.y, (int)targetVec3.z);
        if (currChunks.ContainsKey(targetkey) == true)
        {
            currChunks[targetkey].GetComponent<Octree_Controller>().AddNodeAbsPos(a_pos, chunkMaxDepth, type);
        }
        else
        {
            AddChunk(new Vector3((float)Math.Floor(a_pos.x / chunkSize) * chunkSize, (float)Math.Floor(a_pos.y / chunkSize) * chunkSize, (float)Math.Floor(a_pos.z / chunkSize) * chunkSize));
            currChunks[targetkey].GetComponent<Octree_Controller>().AddNodeAbsPos(a_pos, chunkMaxDepth, type);
        }
    }

    public void chunkloader()
    {
        GameObject playerload = GameObject.Find("Player");
        Vector3 playerloc = playerload.transform.position;
        Debug.Log(playerloc);
        float playerx = Mathf.FloorToInt(playerloc.x / chunkSize) * chunkSize - 16;
        float playerz = Mathf.FloorToInt(playerloc.z / chunkSize) * chunkSize - 16;
        float x = 0;
        float z = 0;
        float limitx = 16;
        float limitz = 16;
        float maxX = limitx * chunkSize;
        float maxZ = limitz * chunkSize;
        float dx = 0;
        float dz = -1;
        //Remove Chunks outside parameters
        Vector3 target = new Vector3(playerx + (x * chunkSize), 0, playerz + (z * chunkSize));
        List<Vector3Int> remove = new List<Vector3Int>();
        foreach (KeyValuePair<Vector3Int, GameObject> chunk in currChunks)
        {
            Vector3 chunkloc = chunk.Value.transform.position;
            if (chunkloc.x < playerx - (maxX / 2) || chunkloc.x >= playerloc.x + (maxX / 2) || chunkloc.z < playerloc.z - (maxZ / 2)  || chunkloc.z >= playerz + (maxZ / 2))
            {
                //Debug.Log("Remove: " + chunk.Key);
                remove.Add(chunk.Key);
            }
        }
        foreach (Vector3Int chunk in remove)
        {
            currChunks[chunk].GetComponent<Octree_Controller>().octree.Clear();
            currChunks[chunk].GetComponent<MeshFilter>().mesh.Clear();
            currChunks[chunk].SetActive(false);
            chunkPool.Enqueue(currChunks[chunk]);
            currChunks.Remove(chunk);
        }
        remove.Clear();
        //Debug.Log("Player Location: " + playerload.transform.position);
        //Debug.Log("Adjusted Location: " + playerx + ", " + playerz);
        //Debug.Log("Chunks Limit x-: " + (playerx - (maxX / 2)));
        //Debug.Log("Chunks Limit x+: " + (playerx + (maxX / 2)));
        //Debug.Log("Chunks Limit z-: " + (playerz - (maxZ / 2)));
        //Debug.Log("Chunks Limit z+: " + (playerz + (maxZ / 2)));
        for (int i = 0; i < 16*16; i++)
        {
            
            if ((-limitx / 2) <= x && x <= (limitx / 2) && (-limitz / 2) <= z && z <= (limitz / 2))
            {
                for (float y = 0; y < 8; y++)
                {

                    target = new Vector3(playerx + (x*16), y * 16, playerz + (z*16));
                    //Debug.Log("X: " + x + "," + " Z: " + z);
                    //Debug.Log("Player X: " + playerx + "," + " Player Z: " + playerz);
                    //Debug.Log("Modified Coordinates: " + target);
                    //Debug.Log("LoadOrder Count: " + loadOrder.Count);
                    if (!currChunks.ContainsKey(new Vector3Int((int)target.x, (int)target.y, (int)target.z)))
                    {
                        AddChunk(target);
                    }
                }
            }
            if ((x == z) || ((x < 0) && (x == -z)) || ((x > 0) && (x == 1 - z)))
            {
                float olddx = dx;
                dx = -dz;
                dz = olddx;
            }
            x += dx;
            z += dz;
        }
        Debug.Log("chunkPool Count: " + chunkPool.Count);
        Debug.Log("LoadOrder Count: " + loadOrder.Count);
    }

    public void StartRender()
    {
        //StartCoroutine(merge);
        StartCoroutine(render);
    }

    private IEnumerator RenderLoaded()
    {
        Debug.Log("Start Render");
        //float starttime = Time.time;
        int count = 0;
        foreach (KeyValuePair<Vector3Int, GameObject> chunk in loadOrder)
        {
            if (!currChunks.ContainsKey(chunk.Key))
            {
                chunk.Value.SetActive(true);
                chunk.Value.GetComponent<Octree_Controller>().PerlinNoise();
                chunk.Value.GetComponent<Octree_Controller>().MergeAllNodes();
                chunk_Renderer.DrawChunk(chunk.Value);
                if (count > 2)
                {
                    count = 0;
                    yield return 0;
                }
                count++;
                currChunks.Add(chunk.Key, chunk.Value);
            }
            
        }
        loadOrder.Clear();
        yield return null;
    }

    public IEnumerator MergeChunkNodes()
    {

        foreach (KeyValuePair<Vector3Int, GameObject> chunk in currChunks)
        {

            chunk.Value.GetComponent<Octree_Controller>().MergeAllNodes();
            if (Time.deltaTime >= 0.17)
            {
                //starttime = Time.time;
                yield return 0;
            }
        }
        yield return null;
    }

}

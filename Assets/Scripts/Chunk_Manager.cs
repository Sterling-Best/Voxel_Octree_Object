﻿using System;
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
        //chunkloader();
        //StartRender();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerposition = player.transform.position;
        if (Vector3.Distance(playerposition, playeroldpos) > 16 && rendering == false && loading == false)
        {
            chunkloader();
            playeroldpos = new Vector3(playerposition.x, playerposition.y, playerposition.z);
            loading = true;
        }
        else if (rendering == false && loading == true)
        {
            rendering = true;
            StartCoroutine("RenderLoaded");
        }
        else if(loadOrder.Count == 0 && rendering == true && loading == true)
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
        Debug.Log("Chunk Distance: " + a_chunkdistance);
        chunkNumLimit = (int)Math.Pow((a_chunkdistance), 3);
        Debug.Log("Chunk Count Limit: " + chunkNumLimit);
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

    private void AddChunk(Vector3Int a_pos)
    {
        if (chunkPool.Count > 0)
        {
            GameObject chunk = chunkPool.Dequeue();
            chunk.SetActive(true);
            chunk.transform.position = a_pos;
            loadOrder.Enqueue(new KeyValuePair<Vector3Int, GameObject>(a_pos, chunk));
        }
    }

    public void AddBlock(Vector3 a_pos, int type)
    {
        Vector3Int targetVec3 = new Vector3Int((int)Math.Floor(a_pos.x / chunkSize) * chunkSize, (int)Math.Floor(a_pos.y / chunkSize) * chunkSize, (int)Math.Floor(a_pos.z / chunkSize) * chunkSize);
        Vector3Int targetkey = new Vector3Int((int)targetVec3.x, (int)targetVec3.y, (int)targetVec3.z);
        if (currChunks.ContainsKey(targetkey) == true)
        {
            currChunks[targetkey].GetComponent<Octree_Controller>().AddNodeAbsPos(targetVec3, chunkMaxDepth, type);
        }
        else
        {
            AddChunk(new Vector3Int((int)Math.Floor(a_pos.x / chunkSize) * chunkSize, (int)Math.Floor(a_pos.y / chunkSize) * chunkSize, (int)Math.Floor(a_pos.z / chunkSize) * chunkSize));
            currChunks[targetkey].GetComponent<Octree_Controller>().AddNodeAbsPos(targetVec3, chunkMaxDepth, type);
        }
    }

    public void chunkloader()
    {
        GameObject playerload = GameObject.Find("Player");
        Vector3 playerloc = playerload.transform.position;
        Debug.Log("Player Location: " + playerloc);
        float playerx = Mathf.FloorToInt(playerloc.x / chunkSize) * chunkSize - 16;
        float playerz = Mathf.FloorToInt(playerloc.z / chunkSize) * chunkSize - 16;
        Debug.Log("PlayerX: " + playerx);
        Debug.Log("PlayerZ: " + playerz);
        float x = 0;
        float z = 0;
        float limitx = 16;
        float limitz = 16;
        float maxX = limitx * chunkSize;
        float maxZ = limitz * chunkSize;
        Debug.Log("MinX: " + (playerx - (maxX / 2)));
        Debug.Log("MaxX: " + (playerx + (maxX / 2)));
        Debug.Log("MinZ: " + (playerz - (maxZ / 2)));
        Debug.Log("MaxZ: " + (playerz + (maxZ / 2)));
        float dx = 0;
        float dz = -1;
        //Remove Chunks outside parameters
        
        List<Vector3Int> remove = new List<Vector3Int>();
        foreach (KeyValuePair<Vector3Int, GameObject> chunk in currChunks)
        {
            Vector3 chunkloc = chunk.Value.transform.position;
            if (chunkloc.x < playerx - (maxX / 2) || chunkloc.x >= playerx + (maxX / 2) || chunkloc.z < playerz - (maxZ / 2)  || chunkloc.z >= playerz + (maxZ / 2))
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
        Vector3Int target;
        for (int i = 0; i < 16*16; i++)
        {
            if (-(limitx / 2) <= x && x < (limitx / 2) && -(limitz / 2) <= z && z < (limitz / 2) )
            {
                for (float y = 0; y < 16; y++)
                {

                    target = new Vector3Int((int)(playerx + (x * chunkSize)), (int)(y * chunkSize), (int)(playerz + (z * chunkSize)));
                    if (!currChunks.ContainsKey(target))
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
        int chunkloadlimit = 3;
        Debug.Log("Start Time: " + Time.time);
        foreach (KeyValuePair<Vector3Int, GameObject> chunk in loadOrder)
        {
            if (!currChunks.ContainsKey(chunk.Key))
            {
                if (Time.deltaTime <= .017)
                {
                    chunkloadlimit = 3;
                }
                else if (Time.deltaTime <= .034)
                {
                    chunkloadlimit = 2;
                }
                else
                {
                    chunkloadlimit = 2;
                }
                if (count > chunkloadlimit)
                {
                    count = 0;
                    yield return 0;
                }
                chunk.Value.SetActive(true);
                chunk.Value.GetComponent<Octree_Controller>().PerlinNoise();
                chunk.Value.GetComponent<Octree_Controller>().MergeAllNodes();
                chunk_Renderer.DrawChunk(chunk.Value);
                count++;
                currChunks.Add(chunk.Key, chunk.Value);
            }
            
        }
        loadOrder.Clear();
        Debug.Log("currChunks Count: " + currChunks.Count);
        Debug.Log("End Time: " + Time.time);
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

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

    private Queue<GameObject> chunkPool;

    //Dictionary of Chunks that is currently loaded into the game.
    public Dictionary<Vector3Int, GameObject> currChunks = new Dictionary<Vector3Int, GameObject>();

    
    //Block Manager for Block Details - Mainly to hand off as reference to chunk
    private Block_Manager blockManager;

    /*
     Notes for Loading Cubes, to start the game quickly
     -Awake/Start should load about 2x2x2 chunks so the player can stand on something
     -Loading Screen Continues untill there is 4x4x4 worth of chunks available. Then loading screen ends.
     -PLayer Starts game, but loading additional chunk for farther distance happen during game time. 
     */


    private void Awake()
    {
        blockMinSize = (int)(chunkSize / (Mathf.Pow(2, chunkMaxDepth)));

        //TODO: Remove Tests
        //Testing Chunks
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        

    }

    public void SetChunkManager(int a_worldseed, int a_chunksize, byte a_chunkmaxdepth, int a_chunkdistance, Block_Manager a_blockmanager)
    {
        blockManager = a_blockmanager;
        chunkPool = new Queue<GameObject>();
        //Set Procedural Seed
        worldSeed = a_worldseed;

        //Set Chunk Parameters
        chunkSize = a_chunksize;
        chunkMaxDepth = a_chunkmaxdepth;
        chunkNumLimit = (int)Math.Pow((a_chunkdistance * 2), 3);
        for (int i = 0; i < chunkNumLimit; i++)
        {
            GameObject chunk = ChunkSetUp();
            chunkPool.Enqueue(chunk);
        }
        //Set Block Manager
    }

    private GameObject ChunkSetUp()
    {
        GameObject chunk = new GameObject();
        chunk.name = "Chunk" + Octree_Controller.count.ToString();
        chunk.AddComponent(typeof(MeshFilter));
        chunk.AddComponent(typeof(MeshRenderer));
        chunk.AddComponent<Octree_Controller>();
        chunk.GetComponent<Octree_Controller>().octreeSize = chunkSize;
        chunk.GetComponent<Octree_Controller>().chunkMaxDepth = chunkMaxDepth;
        chunk.transform.parent = this.transform;
        //chunk.transform.position = a_pos;
        chunk.SetActive(false);
        return chunk;
    }

    private void AddChunk(Vector3 a_pos)
    {
        GameObject chunk = chunkPool.Dequeue();
        chunk.transform.position = a_pos;
        chunk.SetActive(true);
        currChunks.Add(new Vector3Int((int)a_pos.x, (int)a_pos.y, (int)a_pos.z), chunk);
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

}

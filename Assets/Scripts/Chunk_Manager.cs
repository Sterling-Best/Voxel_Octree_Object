using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Debating Whether to add seed to this or world?

public class Chunk_Manager : MonoBehaviour
{

    private int worldSeed;

    //Chunk Parameters
    private int chunkSize;
    private int chunkMaxDepth;
    private int blockMinSize;


    //Dictionary of Chunks that is currently loaded into the game.
    public Dictionary<Vector3Int, Octree_Controller> currChunks = new Dictionary<Vector3Int, Octree_Controller>();
    
    //Block Manager for Block Details - Mainly to hand off as reference to chunk
    private Block_Manager blockManager;

    // Start is called before the first frame update
    void Start()
    {
        //Determine what the size of the smallest block would be in a chunk based on size and maxDepth
        blockMinSize = (int)(chunkSize / (Mathf.Pow(2, chunkMaxDepth)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetChunkManager(int a_worldseed, int a_chunksize, int a_chunkmaxdepth, Block_Manager a_blockmanager)
    {
        //Set Procedural Seed
        worldSeed = a_worldseed;

        //Set Chunk Parameters
        chunkSize = a_chunksize;
        chunkMaxDepth = a_chunkmaxdepth;

        //Set Block Manager
        blockManager = a_blockmanager;
    }
}

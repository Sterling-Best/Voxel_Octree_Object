using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour

{
    public bool randomSeed = false;
    public int worldSeed;

    public int ChunkDistance = 4;

    public int chunkSize = 16;
    public byte chunkMaxDepth = 4;

    GameObject chunk_Manager;

    private Block_Manager block_Manager;

    //Awake is called before start - Used for initialization
    private void Awake()
    {
        Application.targetFrameRate = -1;
        chunk_Manager = new GameObject();
        block_Manager = new Block_Manager();
        ChunkManagerSetUp();
    }

    // Start is called before the first frame update
    //Used for Set-up
    void Start()
    {
        
    }

    bool startrendering = true;

    // Update is called once per frame
    void Update()
    {
        if (startrendering == true){
            startrendering = false;
            chunk_Manager.GetComponent<Chunk_Manager>().StartRender();
        }
    }


    private void ChunkManagerSetUp()
    {
        chunk_Manager.name = "Chunk Manager";
        chunk_Manager.AddComponent<Chunk_Manager>();
        chunk_Manager.GetComponent<Chunk_Manager>().SetChunkManager(this.worldSeed, this.chunkSize, this.chunkMaxDepth, ChunkDistance, this.block_Manager);

    }

    public void AddBlock(Vector3 a_pos, int type)
    {
        chunk_Manager.GetComponent<Chunk_Manager>().AddBlock(a_pos, type);
    }


}

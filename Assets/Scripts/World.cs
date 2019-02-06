using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour

{
    public bool randomSeed = false;
    public int worldSeed;


    public int chunkSize = 16;
    public byte chunkMaxDepth = 4;

    GameObject chunk_Manager;

    private Block_Manager block_Manager;

    private void Awake()
    {
        chunk_Manager = new GameObject();
        block_Manager = new Block_Manager();
        ChunkManagerSetUp();
    }

    // Start is called before the first frame update
    void Start()
    {
        //AddBlock(new Vector3(0, 0, 0), 1);
        //AddBlock(new Vector3(1, 0, 0), 1);
        for (int x = 0; x < 64; x++)
        {
            for (int z = 0; z < 64; z++)
            {
                int yr = UnityEngine.Random.Range(20, 30);
                for (int y = 0; y < 32; y++)
                {
                    if (y < yr)
                    {
                        AddBlock(new Vector3(x, y, z), 1);
                    }
                    else
                    {
                        AddBlock(new Vector3(x, y, z), 0);
                    }
                }
            }
        }

        //chunk_Manager.GetComponent<Chunk_Manager>().chunkPool[new Vector3Int(0, 0, 0)].GetComponent<Octree_Controller>().MergeNodes();



        //AddBlock(new Vector3(0,0,0), 1);
        //AddBlock(new Vector3(0, 0, 15), 1);
        //AddBlock(new Vector3(17, 17, 17), 1);
        //AddBlock(new Vector3(-1, -1, -1), 1);



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChunkManagerSetUp()
    {
        chunk_Manager.name = "Chunk Manager";
        chunk_Manager.AddComponent<Chunk_Manager>();
        chunk_Manager.GetComponent<Chunk_Manager>().SetChunkManager(this.worldSeed, this.chunkSize, this.chunkMaxDepth, this.block_Manager);

    }

    public void AddBlock(Vector3 a_pos, int type)
    {
        chunk_Manager.GetComponent<Chunk_Manager>().AddBlock(a_pos, type);
    }


}

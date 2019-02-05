using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour

{
    public bool randomSeed = false;
    public int worldSeed;


    public int chunkSize = 16;
    public int chunkMaxDepth = 4;

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
        //Object.Instantiate(chunk_Manager);
        
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


}

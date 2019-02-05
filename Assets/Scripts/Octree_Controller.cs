using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// TODO: Renderer - switch to const for sides
// TODO: Render -  see how uvs map to 

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Renderer))]
public class Octree_Controller : MonoBehaviour
{

    
    public Dictionary<long, int> octree = new Dictionary<long
        , int>(); //<, int> = <locID, blocktype>


    public Dictionary<int, int> materialdic = new Dictionary<int, int>(); //<int, int> = <materialID, materialIndexinChunk>

    public float octreeSize;

    Vector3 octreepos;

    Vector3 octreelimitpos;

    public Block_Manager block_Manager;

    public OT_LocCode olc;

    Renderer octree_MeshRender;

    Mesh octree_mesh;

    Material[] materialsreference;

    public static int count = 0;

    Chunk_Renderer chunk_Renderer = new Chunk_Renderer();

    private void Awake()
    {
        count += 1;
    }

    // Use this for initialization
    void Start()
    {
        this.octreepos = this.transform.position;
        this.octreelimitpos = octreepos + new Vector3(octreeSize, octreeSize, octreeSize);

        this.block_Manager = new Block_Manager();
        this.olc = new OT_LocCode();

        //AddNodeLocID(LocID, Type);
        //AddNodeLocID(8, 0);
        //AddNodeLocID(9, 0);
        //AddNodeLocID(10, 0);
        //AddNodeLocID(11, 0);
        //AddNodeLocID(12, 0);
        //AddNodeLocID(13, 0);
        //AddNodeLocID(14, 0);
        //AddNodeLocID(15, 0);
        //AddNodeLocID(64, 3);
        //AddNodeLocID(65, 3);
        //AddNodeLocID(66, 1);
        //AddNodeLocID(67, 2);
        //AddNodeLocID(536, 1);
        //AddNodeLocID(537, 1);
        //AddNodeLocID(538, 2);
        //AddNodeLocID(539, 2);
        //AddNodeLocID(540, 1);
        //AddNodeLocID(541, 1);
        //AddNodeLocID(542, 2);
        //AddNodeLocID(543, 1);
        //AddNodeLocID(68, 3);
        //AddNodeLocID(69, 3);
        //AddNodeLocID(70, 1);
        //AddNodeLocID(71, 1);

        //AddNodeRelPos(Vector3, Depth, Type);
        AddNodeRelPos(new Vector3(0, 0, 0), 1, 2);
        AddNodeRelPos(new Vector3(1, 7.3f, 9), 1, 0);
        AddNodeRelPos(new Vector3(0, 8, 0), 1, 0);
        AddNodeRelPos(new Vector3(0, 8, 8), 1, 0);
        AddNodeRelPos(new Vector3(8, 0, 0), 1, 0);
        AddNodeRelPos(new Vector3(8, 0, 8), 1, 0);
        AddNodeRelPos(new Vector3(8, 8, 0), 1, 0);
        AddNodeRelPos(new Vector3(8, 8, 8), 1, 0);
        AddNodeRelPos(new Vector3(1, 1, 1), 2, 3);
        AddNodeRelPos(new Vector3(0, 2.4f, 5.3f), 2, 3);
        AddNodeRelPos(new Vector3(3.6f, 6, 1.2f), 2, 1);
        AddNodeRelPos(new Vector3(1, 7.7f, 4), 2, 2);
        //AddNodeRelPos(536, 1);
        //AddNodeRelPos(537, 1);
        //AddNodeRelPos(538, 2);
        //AddNodeRelPos(539, 2);
        //AddNodeRelPos(540, 1);
        //AddNodeRelPos(541, 1);
        //AddNodeRelPos(542, 2);
        //AddNodeRelPos(543, 1);
        AddNodeRelPos(new Vector3(4, 2, 1), 2, 3);
        AddNodeRelPos(new Vector3(4, 3, 4), 2, 3);
        AddNodeRelPos(new Vector3(6.1f, 4.11f, 1.69f), 2, 1);
        AddNodeRelPos(new Vector3(4, 4, 4), 2, 1);

        //Test 2

        chunk_Renderer.DrawChunk(gameObject);

    }



    // Update is called once per frame
    void Update()
    {
    
    }

    public void AddNodeAbsPos(Vector3 position, byte depth, int type)
    {


    }

    public void AddNodeRelPos(Vector3 position, byte depth, int type) {
        int depthcoord = (int)this.octreeSize / (2 * depth);
        position = new Vector3((int)Math.Floor(position.x) / depthcoord, (int)Math.Floor(position.y) / depthcoord, (int)Math.Floor(position.z) / depthcoord);
        AddNodeLocID(olc.Vec3ToLoc(position, depth), type);
    }

    public void AddNodeLocID(long locID, int type)
    {
        if (!materialdic.ContainsKey(type))
        {
            this.materialdic.Add(type, this.materialdic.Count);
        }
        this.octree.Add(locID, type);
    }
   
}

   
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

    
    public Dictionary<ushort, int> octree = new Dictionary<ushort, int>(); //<, int> = <locID, blocktype>


    public Dictionary<ushort, int> materialdic = new Dictionary<ushort, int>(); //<int, int> = <materialID, materialIndexinChunk>

    public float octreeSize;

    public byte chunkMaxDepth;

    Vector3 octreepos;

    Vector3 octreelimitpos;

    public Block_Manager block_Manager;

    public OT_LocCode olc;

    Renderer octree_MeshRender;

    Mesh octree_mesh;

    Material[] materialsreference;

    public static int count = 0;

    public Chunk_Renderer chunk_Renderer = new Chunk_Renderer();

    private void Awake()
    {
        count += 1;
        this.block_Manager = new Block_Manager();
        this.olc = new OT_LocCode();
        this.octreepos = this.gameObject.transform.position;
        this.octreelimitpos = octreepos + new Vector3(octreeSize, octreeSize, octreeSize);
    }

        // Use this for initialization
        void Start()
    {
    }



    // Update is called once per frame
    void Update()
    {
    
    }

    public void PerlinNoise()
    {
        Vector3 offset = this.transform.position;
        for (ushort i = 4096; i < (4096 * 2); i++)
        {
            Vector3Int point = olc.LocToVec3(i);
            float yr = Mathf.PerlinNoise((offset.x + point.x) * .019f, (offset.z + point.z) * .019f) * 70 + 90;
            if (offset.y + point.y < yr)
            {
                AddNodeRelPos(point, chunkMaxDepth, 1);
            }
            else
            {
                AddNodeRelPos(point, chunkMaxDepth, 0);
            }
        }
    }

    public void AddNodeAbsPos(Vector3Int a_position, byte depth, int type)
    {
        //Vector3Int position = new Vector3Int(Mathf(a_position.x - this.transform.position.x), a_position.y - this.transform.position.y, a_position.z - this.transform.position.z);
        //AddNodeRelPos(position, depth, type);
    }

    public void AddNodeRelPos(Vector3Int a_position, byte depth, int type) {
        byte depthcoord = (byte)(this.octreeSize / Math.Pow(2, depth));
        Vector3Int position = new Vector3Int(a_position.x / depthcoord, a_position.y / depthcoord, a_position.z / depthcoord);
        this.octree.Add(olc.Vec3ToLoc(position, depth), type); 
    }

    public void AddNodeLocID(ushort locID, int type)
    {
        this.octree.Add(locID, type);
    }

    public void MergeAllNodes()
    {
        bool same;
        for (int d = chunkMaxDepth - 1; d >= 0; d--) //Start counting down from max depth
        {
            ushort depthcode = (ushort)Math.Pow(8, d);
            for (ushort i = depthcode; i < (depthcode * 2); i++) //Going through each possibility of LocationCode for given depth
            {
                if (octree.ContainsKey(i))
                {
                    break;
                }
                else
                {
                    same = true;
                    for (byte c = 0; c < 8; c++) //Check through Children
                    {
                        if (octree.ContainsKey((ushort)((i << 3) | c)))
                        {
                            if (octree[((ushort)((i << 3) | c))] != octree[(ushort)(i << 3)]) //If child key exists and its not the same as sibling
                            {
                                same = false;
                                break;
                            }
                        }
                        else
                        {
                            same = false;
                            break;
                        }
                    }
                    if (same == true)
                    {
                        octree.Add(i, octree[(ushort)(i << 3)]);
                        for (byte c = 0; c < 8; c++) //Check through Children
                        {
                            octree.Remove((ushort)((i << 3) | c));
                        }
                    }
                }
            }
        }
    }
}














        //for (int d = chunkMaxDepth; d > 0; d--) //Start counting down from max depth
        //{
        //    Dictionary<long, int> deletedic = new Dictionary<long, int>();
        //    Dictionary<long, int> adddic = new Dictionary<long, int>();
        //    for (int i = (int)Math.Pow(8, d); i < ((int)Math.Pow(8, d) *2); i++) //Going through each possibility of LocationCode for given depth
        //    {
        //        if (octree.ContainsKey(i) && !deletedic.ContainsKey(i) )
        //        {
        //            long parent = olc.CalculateParent(i);
        //            Array children = olc.CollectChildrenAll(parent);
        //            bool same = true;
        //            //Check for any children that are different
        //            for (int c = 0; c < children.Length; c++) //Check through Children
        //            {
        //                if (octree.ContainsKey((long)children.GetValue(c))) 
        //                {
        //                    if (octree[(long)children.GetValue(c)] != octree[(long)children.GetValue(0)])
        //                    {
        //                        same = false;
        //                        break;
        //                    }
        //                    else
        //                    {

        //                    }
        //                }
        //                else
        //                {
        //                    same = false;
        //                    break;
        //                }
        //            }
        //            if (same == true)
        //            {
        //                adddic.Add(parent, octree[i]);
        //                foreach (long child in children)
        //                {
        //                    if (!deletedic.ContainsKey(child))
        //                    {
        //                        deletedic.Add(child, octree[child]);
        //                    }
        //                }
        //            }
        //            //If children are all the same, add to remove, and add parent to octree
        //        }
        //    }
        //    foreach (KeyValuePair<long, int> deleted in deletedic)
        //    {
        //        octree.Remove(deleted.Key);
        //    }
        //    foreach (KeyValuePair<long, int> added in adddic)
        //    {
        //        octree.Add(added.Key, added.Value);
        //    }
        //}

   
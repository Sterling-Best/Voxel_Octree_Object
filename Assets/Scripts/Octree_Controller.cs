using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Renderer))]
public class Octree_Controller : MonoBehaviour
{

    public Dictionary<int, int> octree = new Dictionary<int
        , int>();

    public float octreesize = 16.0f;

    Vector3 octreepos;

    Vector3 octreelimitpos;

    public Block_Manager block_Manager;

    public OT_LocCode olc;

    Renderer octree_MeshRender;

    Mesh octree_mesh;

    Material[] materialsreference;


    // Use this for initialization
    void Start()
    {

        this.block_Manager = new Block_Manager();
        this.olc = new OT_LocCode();

        this.octreepos = this.transform.position;
        this.octreelimitpos = octreepos + new Vector3(octreesize, octreesize, octreesize);


        //Test 1
        //this.octree.Add(0, 0);
        this.octree.Add(8, 0);
        this.octree.Add(9, 0);
        this.octree.Add(10, 0);
        this.octree.Add(11, 0);
        this.octree.Add(12, 0);
        this.octree.Add(13, 0);
        this.octree.Add(14, 0);
        this.octree.Add(15, 0);
        this.octree.Add(64, 3);
        this.octree.Add(65, 3);
        this.octree.Add(66, 1);
        this.octree.Add(67, 2);
        this.octree.Add(536, 1);
        this.octree.Add(537, 1);
        this.octree.Add(538, 2);
        this.octree.Add(539, 2);
        this.octree.Add(540, 1);
        this.octree.Add(541, 1);
        this.octree.Add(542, 2);
        this.octree.Add(543, 2);
        this.octree.Add(68, 3);
        this.octree.Add(69, 3);
        this.octree.Add(70, 1);
        this.octree.Add(71, 1);

        //Test 2

        PreRender();

    }



    // Update is called once per frame
    void Update()
    {

    }

    private void PreRender()
    {
        //Set up Mesh
        this.octree_mesh = GetComponent<MeshFilter>().mesh;
        this.octree_mesh.Clear();
        this.octree_mesh.subMeshCount = this.octree.Count;
        //Set Up Materials
        this.octree_MeshRender = GetComponent<MeshRenderer>();
        List<Material> materiallist = new List<Material>();
        //Record Chunk's Current Position
        Vector3 octreepos = this.transform.position;
        //Display Default Information for Chunk
        int count = 0;
        //Check each node in the octree if it should be rendered. 
        foreach (int code in octree.Keys)
        {
            //Debug.Log("Current Code: " + code);
            //Only Render if node is  (has no children, identified if a key exists with string (current node.locationCode + "000"))
            if (octree.ContainsKey(code << 3) == false)
            {

                bool[] sidestorender = { DetermineSideRender(code, olc.CalculateAdjacent(code, 0, -1)), DetermineSideRender(code, olc.CalculateAdjacent(code, 1, 1)), // -z, +y
                     DetermineSideRender(code, olc.CalculateAdjacent(code, 2, 1)), DetermineSideRender(code, olc.CalculateAdjacent(code, 2, -1)), // +x, -x
                     DetermineSideRender(code, olc.CalculateAdjacent(code, 0, 1)), DetermineSideRender(code, olc.CalculateAdjacent(code, 1, -1))}; // +z, -y

                if (sidestorender.Any(x => x)) //If at least one side can be rendered
                {

                    float tier_size = this.octreesize * (1 / (float)Math.Pow(2, olc.CalculateDepth(code)));
                    Vector3 locpos = olc.LocToVec3(code) * tier_size;

                    Vector3[] verts = {
                    octreepos + locpos + new Vector3 (0, 0, 0),
                    octreepos + locpos + new Vector3 (tier_size, 0, 0),
                    octreepos + locpos + new Vector3 (tier_size, tier_size, 0),
                    octreepos + locpos + new Vector3 (0, tier_size, 0),
                    octreepos + locpos + new Vector3 (0, tier_size, tier_size),
                    octreepos + locpos + new Vector3 (tier_size, tier_size, tier_size),
                    octreepos + locpos + new Vector3 (tier_size, 0, tier_size),
                    octreepos + locpos + new Vector3 (0, 0, tier_size),
                    };

                    List<int> facetriangles = new List<int>();

                    //face front: -z
                    if (sidestorender[0])
                    {
                        int[] sidetriangles = {
                            (count * 8) + 0, (count * 8) + 2, (count * 8) + 1, //f-z t1
                            (count * 8) + 0, (count * 8) + 3, (count * 8) + 2, //f-z t2
                         };
                        facetriangles.AddRange(sidetriangles);
                    }

                    //face top: +y
                    if (sidestorender[1])
                    {
                        int[] sidetriangles = {
                            (count * 8) + 2, (count * 8) + 3, (count * 8) + 4, //t+y t1
			                (count * 8) + 2, (count * 8) + 4, (count * 8) + 5, //t+y t2
                    };
                        facetriangles.AddRange(sidetriangles);
                    }

                    //face right: +x
                    if (sidestorender[2])
                    {
                        int[] sidetriangles = {
                            (count * 8) + 1, (count * 8) + 2, (count * 8) + 5, //r+x t1
                            (count * 8) + 1, (count * 8) + 5, (count * 8) + 6, //r+x t2
                         };
                        facetriangles.AddRange(sidetriangles);
                    }

                    //face left: -x
                    if (sidestorender[3])
                    {
                        int[] sidetriangles = {
                            (count * 8) + 0, (count * 8) + 7, (count * 8) + 4, //l-x t1
                            (count * 8) + 0, (count * 8) + 4, (count * 8) + 3, //l-x t2
                         };
                        facetriangles.AddRange(sidetriangles);
                    }

                    //face back: +z
                    if (sidestorender[4])
                    {
                        int[] sidetriangles = {
                            (count * 8) + 5, (count * 8) + 4, (count * 8) + 7, //l-z t1
                            (count * 8) + 5, (count * 8) + 7, (count * 8) + 6, //l-z t2
                         };
                        facetriangles.AddRange(sidetriangles);
                    }

                    //face bottom: -y
                    if (sidestorender[5])
                    {
                        int[] sidetriangles = {
                            (count * 8) + 0, (count * 8) + 6, (count * 8) + 7, //b-y t1
                            (count * 8) + 0, (count * 8) + 1, (count * 8) + 6  //b-y t2
                    };
                        facetriangles.AddRange(sidetriangles);
                    }
                    this.octree_mesh.vertices = CombineVector3Arrays(this.octree_mesh.vertices, verts);
                    this.octree_mesh.SetTriangles(facetriangles, count);
                    materiallist.Add(this.block_Manager.blockMaterialList[octree[code]]);
                    count++;

                }

                else
                {
                    continue;
                }

            }
            else
            {
                continue;
            }
            this.octree_MeshRender.materials = materiallist.ToArray();
            this.octree_mesh.RecalculateNormals();
            this.octree_mesh.RecalculateBounds();
        }
    }

    //DeterminSideRender()
    //TODO: might need to make octree Dictionary<long,int>
    //TODO: Documentation: DeterminSideRender()
    //TODO: UnitTest: DetermineSideRender()
    bool DetermineSideRender(long code, long adjacent)
    {
        if (adjacent == code) // Adjacent is the same as code if they are at the edge of a chunk, should render.
        {
            return true;
        }
        else if (octree.ContainsKey(Convert.ToInt32(adjacent))) //Check to see if Adjacent is in Octree Dictionary
        {
            bool codetransparency = block_Manager.blocklist[octree[Convert.ToInt32(code)]].transparency;
            bool adjacenttransparency = block_Manager.blocklist[octree[Convert.ToInt32(adjacent)]].transparency;

            if (adjacenttransparency == true &&  codetransparency != adjacenttransparency) //Is adjacent transparent
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Array parents = this.olc.CollectParents(adjacent);
            foreach (long parent in parents)
            {
                if (octree.ContainsKey(Convert.ToInt32(parent)))
                {
                    bool codetransparency = block_Manager.blocklist[octree[Convert.ToInt32(code)]].transparency;
                    bool adjacenttransparency = block_Manager.blocklist[octree[Convert.ToInt32(parent)]].transparency;

                    if (adjacenttransparency == true && codetransparency != adjacenttransparency)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        return false;
    }

    private bool ChildrenSideTransparencyCheck(long m, byte axis, bool positive)
    {

        return true;
    }

    Vector3[] CombineVector3Arrays(Vector3[] array1, Vector3[] array2)
    {
        Vector3[] array3 = new Vector3[array1.Count() + array2.Count()];
        System.Array.Copy(array1, array3, array1.Count());
        System.Array.Copy(array2, 0, array3, array1.Count(), array2.Count());
        return array3;
    }

    Vector2[] CombineVector2Arrays(Vector2[] array1, Vector2[] array2)
    {
        Vector2[] array3 = new Vector2[array1.Count() + array2.Count()];
        System.Array.Copy(array1, array3, array1.Count());
        System.Array.Copy(array2, 0, array3, array1.Count(), array2.Count());
        return array3;
    }

    int[] CombineIntArrays(int[] array1, int[] array2)
    {
        int[] array3 = new int[array1.Count() + array2.Count()];
        System.Array.Copy(array1, array3, array1.Count());
        System.Array.Copy(array2, 0, array3, array1.Count(), array2.Count());
        return array3;
    }

    string Vector3ArrayList(Vector3[] target)
    {
        string vectstr = "";
        foreach (var vect in target)
        {
            vectstr = vectstr + vect + ", ";
        }
        return vectstr;
    }

    string Vector2ArrayList(Vector2[] target)
    {
        string vectstr = "";
        foreach (var vect in target)
        {
            vectstr = vectstr + vect + ", ";
        }
        return vectstr;
    }

    string IntArrayList(int[] target)
    {
        string vectstr = "";
        foreach (var vect in target)
        {
            vectstr = vectstr + vect + ", ";
        }
        return vectstr;
    }
}

   
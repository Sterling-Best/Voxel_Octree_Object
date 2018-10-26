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

    
    public Dictionary<long, int> octree = new Dictionary<long
        , int>(); //<, int> = <locID, blocktype>


    public Dictionary<int, int> materialdic = new Dictionary<int, int>(); //<int, int> = <materialID, materialIndexinChunk>

    public float octreesize;

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
        this.octreepos = this.transform.position;
        this.octreesize = 16;
        this.octreelimitpos = octreepos + new Vector3(octreesize, octreesize, octreesize);

        this.block_Manager = new Block_Manager();
        this.olc = new OT_LocCode();

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
        //AddNodeRelPos(67, 2);
        //AddNodeRelPos(536, 1);
        //AddNodeRelPos(537, 1);
        //AddNodeRelPos(538, 2);
        //AddNodeRelPos(539, 2);
        //AddNodeRelPos(540, 1);
        //AddNodeRelPos(541, 1);
        //AddNodeRelPos(542, 2);
        //AddNodeRelPos(543, 1);
        //AddNodeRelPos(68, 3);
        //AddNodeRelPos(69, 3);
        //AddNodeRelPos(70, 1);
        //AddNodeRelPos(71, 1);

        //Test 2

        PreRender();

    }



    // Update is called once per frame
    void Update()
    {
    
    }

    public void AddNodeAbsPos(Vector3 position, byte depth, int type)
    {

    }

    public void AddNodeRelPos(Vector3 position, byte depth, int type) {
        int boop = (int)this.octreesize / (2 * depth);
        position = new Vector3((int)Math.Floor(position.x) / boop, (int)Math.Floor(position.y) / boop, (int)Math.Floor(position.z) / boop);
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

    private void PreRender()
    {
        //Set up Mesh
        this.octree_mesh = GetComponent<MeshFilter>().mesh;
        this.octree_mesh.Clear();
        this.octree_mesh.subMeshCount = this.materialdic.Count;
        //Set Up Materials
        this.octree_MeshRender = GetComponent<MeshRenderer>();
        //Collect Materials that belong in this chunk
        Material[] materiallist = new Material[this.materialdic.Count];
        foreach (int key in this.materialdic.Keys)
        {
            materiallist[this.materialdic[key]] = this.block_Manager.blockMaterialList[key];
        }
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
                    //octreepos +
                    locpos + new Vector3 (0, 0, 0),
                    locpos + new Vector3 (tier_size, 0, 0),
                    locpos + new Vector3 (tier_size, tier_size, 0),
                    locpos + new Vector3 (0, tier_size, 0),
                    locpos + new Vector3 (0, tier_size, tier_size),
                    locpos + new Vector3 (tier_size, tier_size, tier_size),
                    locpos + new Vector3 (tier_size, 0, tier_size),
                    locpos + new Vector3 (0, 0, tier_size),
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
                    this.octree_mesh.SetTriangles(CombineIntArrays(this.octree_mesh.GetTriangles(this.materialdic[octree[code]]), facetriangles.ToArray()), this.materialdic[octree[code]]);
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

   
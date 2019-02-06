using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk_Renderer
{
    OT_LocCode olc = new OT_LocCode();
    Block_Manager block_Manager;
    


    public void DrawChunk(GameObject chunk)

            {
        block_Manager = new Block_Manager();
        Mesh octree_mesh = chunk.GetComponent<MeshFilter>().mesh;
        Dictionary<int, int> materialdic = chunk.GetComponent<Octree_Controller>().materialdic;
        MeshRenderer octree_MeshRender = chunk.GetComponent<MeshRenderer>();
        int count = 0;
        Dictionary<long, int> octree = chunk.GetComponent<Octree_Controller>().octree;
        float octreeSize = chunk.GetComponent<Octree_Controller>().octreeSize;

        //Set up Mesh
        octree_mesh.Clear(); 
        //Set Up Materials
        //octree_mesh.subMeshCount = materialdic.Count;
        //Material[] materiallist = new Material[materialdic.Count];
        //Collect Materials that belong in this chunk
        //foreach (int key in materialdic.Keys)
        //{
        //    materiallist[materialdic[key]] = chunk.GetComponent<Octree_Controller>().block_Manager.blockMaterialList[key];
        //}
        //Check each node in the octree if it should be rendered. 
        foreach (int code in octree.Keys)
        {
            if (octree.ContainsKey(code << 3) == false)
            {
            //Only Render if node is  (has no children, identified if a key exists with string (current node.locationCode + "000"))

                bool[] sidestorender = { DetermineSideRender(octree, code, olc.CalculateAdjacent(code, 0, -1)), DetermineSideRender(octree, code, olc.CalculateAdjacent(code, 1, 1)), // -z, +y
                     DetermineSideRender(octree, code, olc.CalculateAdjacent(code, 2, 1)), DetermineSideRender(octree, code, olc.CalculateAdjacent(code, 2, -1)), // +x, -x
                     DetermineSideRender(octree, code, olc.CalculateAdjacent(code, 0, 1)), DetermineSideRender(octree, code, olc.CalculateAdjacent(code, 1, -1))}; // +z, -y

                if (sidestorender.Any(x => x)) //If at least one side can be rendered
                {

                    float tier_size = octreeSize * (1 / (float)Math.Pow(2, olc.CalculateDepth(code)));
                    Vector3 test = olc.LocToVec3(code);
                    Vector3 locpos = test * tier_size;

                    Vector3[] verts = {
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
                    octree_mesh.vertices = CombineVector3Arrays(octree_mesh.vertices, verts);
                    //octree_mesh.SetTriangles(CombineIntArrays(octree_mesh.GetTriangles(materialdic[octree[code]]), facetriangles.ToArray()), materialdic [octree[code]]);
                    octree_mesh.triangles = CombineIntArrays(octree_mesh.triangles, facetriangles.ToArray());
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
            //octree_MeshRender.materials = materiallist.ToArray();
            
            
        }
        octree_MeshRender.material = (Material)Resources.Load("Default", typeof(Material));
        octree_mesh.RecalculateNormals();
        octree_mesh.RecalculateBounds();
    }

    private bool DetermineSideRender(Dictionary<long, int> octree, long code, long adjacent)
    {
        bool codeopaque = block_Manager.blocklist[octree[Convert.ToInt32(code)]].opaque;
        if (codeopaque == false)
        {
            return false;
        }
        if (adjacent == code) // Adjacent is the same as code if they are at the edge of a chunk, should render.
        {
            return true;
        }
        else if (octree.ContainsKey(Convert.ToInt32(adjacent))) //Check to see if Adjacent is in Octree Dictionary
        {
            bool codetranslucent = block_Manager.blocklist[octree[Convert.ToInt32(code)]].translucent;
            bool adjacenttransparency = block_Manager.blocklist[octree[Convert.ToInt32(adjacent)]].translucent;

            if (adjacenttransparency == true && codetranslucent != adjacenttransparency) //Is adjacent transparent
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
                    bool codetransparency = block_Manager.blocklist[octree[Convert.ToInt32(code)]].translucent;
                    bool adjacenttransparency = block_Manager.blocklist[octree[Convert.ToInt32(parent)]].translucent;

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
        return true;
    }

    private bool ChildrenSideTransparencyCheck(long m, byte axis, bool positive)
    {
        return true;
    }

    private Vector3[] CombineVector3Arrays(Vector3[] array1, Vector3[] array2)
    {
        Vector3[] array3 = new Vector3[array1.Count() + array2.Count()];
        System.Array.Copy(array1, array3, array1.Count());
        System.Array.Copy(array2, 0, array3, array1.Count(), array2.Count());
        return array3;
    }



    private Vector2[] CombineVector2Arrays(Vector2[] array1, Vector2[] array2)
    {
        Vector2[] array3 = new Vector2[array1.Count() + array2.Count()];
        System.Array.Copy(array1, array3, array1.Count());
        System.Array.Copy(array2, 0, array3, array1.Count(), array2.Count());
        return array3;
    }

    private int[] CombineIntArrays(int[] array1, int[] array2)
    {
        int[] array3 = new int[array1.Count() + array2.Count()];
        System.Array.Copy(array1, array3, array1.Count());
        System.Array.Copy(array2, 0, array3, array1.Count(), array2.Count());
        return array3;
    }

    private string Vector3ArrayList(Vector3[] target)
    {
        string vectstr = "";
        foreach (var vect in target)
        {
            vectstr = vectstr + vect + ", ";
        }
        return vectstr;
    }

    private string Vector2ArrayList(Vector2[] target)
    {
        string vectstr = "";
        foreach (var vect in target)
        {
            vectstr = vectstr + vect + ", ";
        }
        return vectstr;
    }

    private string IntArrayList(int[] target)
    {
        string vectstr = "";
        foreach (var vect in target)
        {
            vectstr = vectstr + vect + ", ";
        }
        return vectstr;
    }
}

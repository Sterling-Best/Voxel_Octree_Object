using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk_Renderer
{
    OT_LocCode olc = new OT_LocCode();
    Block_Manager block_Manager;
    const byte axisZ = 0;
    const byte axisY = 1;
    const byte axisX = 2;






    public void DrawChunk(GameObject chunk)

            {
        block_Manager = new Block_Manager();
        Mesh octree_mesh = chunk.GetComponent<MeshFilter>().mesh;
        //Dictionary<ushort, int> materialdic = chunk.GetComponent<Octree_Controller>().materialdic;
        MeshRenderer octree_MeshRender = chunk.GetComponent<MeshRenderer>();

        Dictionary<ushort, int> octree = chunk.GetComponent<Octree_Controller>().octree;
        float octreeSize = chunk.GetComponent<Octree_Controller>().octreeSize;

        //Set up Mesh
        octree_mesh.Clear();
        byte lengthverts = 18;
        Vector3[] meshverts = new Vector3[octree.Count * lengthverts];
        List<int> facetriangles = new List<int>();
        int count = 0;
        if (octree.Keys.Count == 1) 
        {
            if !(block_Manager.blocklist[octree[1]].opaque)
            {
                return;
            }
        }
        foreach (ushort code in octree.Keys)
        {
                bool[] sidestorender = { DetermineSideRender(octree, code, olc.CalculateAdjacent(code, axisZ, -1)), DetermineSideRender(octree, code, olc.CalculateAdjacent(code, axisY, 1)), // -z, +y
                     DetermineSideRender(octree, code, olc.CalculateAdjacent(code, axisX, 1)), DetermineSideRender(octree, code, olc.CalculateAdjacent(code, axisX, -1)), // +x, -x
                     DetermineSideRender(octree, code, olc.CalculateAdjacent(code, axisZ, 1)), DetermineSideRender(octree, code, olc.CalculateAdjacent(code, axisY, -1))}; // +z, -y

                if (sidestorender.Any(x => x)) //If at least one side can be rendered
                {

                    float tier_size = octreeSize * (1 / (float)Math.Pow(2, olc.CalculateDepth(code)));
                    Vector3 test = olc.LocToVec3(code);
                    Vector3 locpos = test * tier_size;

                    //MultiSide
                    meshverts[(count * lengthverts) + 0] = locpos + new Vector3(0, 0, 0);
                    meshverts[(count * lengthverts) + 1] = locpos + new Vector3(0, tier_size, 0);
                    meshverts[(count * lengthverts) + 2] = locpos + new Vector3(0, tier_size, tier_size);
                    meshverts[(count * lengthverts) + 3] = locpos + new Vector3(0, 0, tier_size);
                    //Side Z-
                    meshverts[(count * lengthverts) + 4] = locpos + new Vector3(tier_size * GreedyAdjacent(octree, code, axisX, axisZ, -1), 0, 0);
                    meshverts[(count * lengthverts) + 5] = locpos + new Vector3(tier_size * GreedyAdjacent(octree, code, axisX, axisZ, -1), tier_size, 0);
                    //Side Y+
                    meshverts[(count * lengthverts) + 6] = locpos + new Vector3(tier_size * GreedyAdjacent(octree, code, axisX, axisY, 1), tier_size, 0);
                    meshverts[(count * lengthverts) + 7] = locpos + new Vector3(tier_size * GreedyAdjacent(octree, code, axisX, axisY, 1), tier_size, tier_size);
                    //Side X+
                    meshverts[(count * lengthverts) + 8] = locpos + new Vector3(tier_size, 0, 0);
                    meshverts[(count * lengthverts) + 9] = locpos + new Vector3(tier_size, tier_size, 0);
                    meshverts[(count * lengthverts) + 10] = locpos + new Vector3(tier_size, tier_size, tier_size * GreedyAdjacent(octree, code, axisZ, axisX, 1));
                    meshverts[(count * lengthverts) + 11] = locpos + new Vector3(tier_size, 0, tier_size * GreedyAdjacent(octree, code, axisZ, axisX, 1));
                    //Side X-
                    meshverts[(count * lengthverts) + 12] = locpos + new Vector3(0, tier_size, tier_size * GreedyAdjacent(octree, code, axisZ, axisX, -1));
                    meshverts[(count * lengthverts) + 13] = locpos + new Vector3(0, 0, tier_size * GreedyAdjacent(octree, code, axisZ, axisX, -1));
                    //Side Z+
                    meshverts[(count * lengthverts) + 14] = locpos + new Vector3(tier_size * GreedyAdjacent(octree, code, axisX, axisZ, 1), tier_size, tier_size);
                    meshverts[(count * lengthverts) + 15] = locpos + new Vector3(tier_size * GreedyAdjacent(octree, code, axisX, axisZ, 1), 0, tier_size);
                    //Side Y-
                    meshverts[(count * lengthverts) + 16] = locpos + new Vector3(tier_size * GreedyAdjacent(octree, code, axisX, axisY, -1), 0, 0);
                    meshverts[(count * lengthverts) + 17] = locpos + new Vector3(tier_size * GreedyAdjacent(octree, code, axisX, axisY, -1), 0, tier_size);


                    //face front: -z
                    if (sidestorender[0] && !GreedySubAdjacent(octree, code, axisX, axisZ, -1)) //Should this side show? Plus Greedmesh Check
                    {

                        facetriangles.Add((count * lengthverts) + 0);
                        facetriangles.Add((count * lengthverts) + 5);
                        facetriangles.Add((count * lengthverts) + 4); //f-z t1
                        facetriangles.Add((count * lengthverts) + 0);
                        facetriangles.Add((count * lengthverts) + 1);
                        facetriangles.Add((count * lengthverts) + 5); //f-z t2
                    }

                    //face top: +y
                    if (sidestorender[1] && !GreedySubAdjacent(octree, code, axisX, axisY, 1)) //Should this side show? Plus Greedmesh Check
                    {

                        facetriangles.Add((count * lengthverts) + 6);
                        facetriangles.Add((count * lengthverts) + 1);
                        facetriangles.Add((count * lengthverts) + 2); //t+y t1
                        facetriangles.Add((count * lengthverts) + 6);
                        facetriangles.Add((count * lengthverts) + 2);
                        facetriangles.Add((count * lengthverts) + 7); //t+y t2

                    }

                    //face right: +x
                    if (sidestorender[2] && !GreedySubAdjacent(octree, code, axisZ, axisX, 1)) //Should this side show? Plus Greedmesh Check
                    {

                        facetriangles.Add((count * lengthverts) + 8);
                        facetriangles.Add((count * lengthverts) + 9);
                        facetriangles.Add((count * lengthverts) + 10); //r+x t1
                        facetriangles.Add((count * lengthverts) + 8);
                        facetriangles.Add((count * lengthverts) + 10);
                        facetriangles.Add((count * lengthverts) + 11); //r+x t2
                    }

                    //face left: -x
                    if (sidestorender[3] && !GreedySubAdjacent(octree, code, axisZ, axisX, -1)) //Should this side show? Plus Greedmesh Check
                    {
                        facetriangles.Add((count * lengthverts) + 0);
                        facetriangles.Add((count * lengthverts) + 13);
                        facetriangles.Add((count * lengthverts) + 12); //l-x t1
                        facetriangles.Add((count * lengthverts) + 0);
                        facetriangles.Add((count * lengthverts) + 12);
                        facetriangles.Add((count * lengthverts) + 1); //l-x t2

                    }

                    //face back: +z
                    if (sidestorender[4] && !GreedySubAdjacent(octree, code, axisX, axisZ, 1)) //Checking side - Plus Greedmesh Check
                    {
                        facetriangles.Add((count * lengthverts) + 14);
                        facetriangles.Add((count * lengthverts) + 2);
                        facetriangles.Add((count * lengthverts) + 3); //l-z t1
                        facetriangles.Add((count * lengthverts) + 14);
                        facetriangles.Add((count * lengthverts) + 3);
                        facetriangles.Add((count * lengthverts) + 15); //l-z t2

                    }

                    //face bottom: -y
                    if (sidestorender[5] && !GreedySubAdjacent(octree, code, axisX, axisY, -1)) //Checking Side -  Plus Greedmesh Check
                    {
                        facetriangles.Add((count * lengthverts) + 0);
                        facetriangles.Add((count * lengthverts) + 17);
                        facetriangles.Add((count * lengthverts) + 3); //b-y t1
                        facetriangles.Add((count * lengthverts) + 0);
                        facetriangles.Add((count * lengthverts) + 16);
                        facetriangles.Add((count * lengthverts) + 17); //b-y t2
                    }
                    count++;
                }       
                else
                {
                    continue;
                }          
            
        }
        octree_mesh.vertices = meshverts;
        octree_mesh.triangles = facetriangles.ToArray();
        octree_MeshRender.material = (Material)Resources.Load("Default", typeof(Material));
        octree_mesh.RecalculateNormals();
        octree_mesh.RecalculateBounds();
        chunk.GetComponent<MeshCollider>().sharedMesh = octree_mesh;
    }

    private bool DetermineSideRender(Dictionary<ushort, int> octree, ushort code, ushort adjacent)
    {
        if (!block_Manager.blocklist[octree[code]].opaque)
        {
            return false;
        }
        else if (adjacent == code) // Adjacent is the same as code if they are at the edge of a chunk, should render.
        {
            return true;
        }
        else if (octree.ContainsKey(adjacent)) //Check to see if Adjacent is in Octree Dictionary
        {
            bool adjacenttransparency = block_Manager.blocklist[octree[adjacent]].translucent;

            if (adjacenttransparency == true && block_Manager.blocklist[octree[code]].translucent != adjacenttransparency) //Is adjacent transparent
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
            for (int i = 1; i < olc.CalculateDepth(code); i++ )
            {
                if (octree.ContainsKey((ushort)(code >> (i*3))))
                {
                    bool adjacenttransparency = block_Manager.blocklist[octree[(ushort)(code >> (i * 3))]].translucent;

                    if (adjacenttransparency == true && block_Manager.blocklist[octree[code]].translucent != adjacenttransparency)
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

    private bool GreedySubAdjacent(Dictionary<ushort,int> octree, ushort m, byte axis, byte side, int dir)
    {
        ushort adjacent = olc.CalculateAdjacent(m, axis, -1);
        
        if (octree.ContainsKey(adjacent))
        {
            if (m != adjacent && block_Manager.blocklist[octree[m]].itemcode == block_Manager.blocklist[octree[adjacent]].itemcode &&
                    olc.CalculateDepth(m) == olc.CalculateDepth(adjacent) && DetermineSideRender(octree, adjacent, olc.CalculateAdjacent(adjacent, side, dir))) //TODO: Interior nodes may still render
            {
                return true;
           }
        }
        return false;
    }

    private int GreedyAdjacent(Dictionary<ushort, int> octree, ushort m, byte axis, byte side, int dir)
    {
        int modifiercount = 1;
        ushort adjacent = olc.CalculateAdjacent(m, axis, 1);
        if (octree.ContainsKey(adjacent))
        {
            if (m != adjacent && block_Manager.blocklist[octree[m]].itemcode == block_Manager.blocklist[octree[adjacent]].itemcode &&
                    olc.CalculateDepth(m) == olc.CalculateDepth(adjacent) && DetermineSideRender(octree, adjacent, olc.CalculateAdjacent(adjacent, side, dir)))
            {
                modifiercount += GreedyAdjacent(octree, adjacent, axis, side, dir);
            }
        }
        return modifiercount;
    }
}
    

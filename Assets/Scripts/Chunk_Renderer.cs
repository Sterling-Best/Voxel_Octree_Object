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
    Dictionary<ushort, int> octree = new Dictionary<ushort, int>();
    Dictionary<ushort, bool?[]> deniedSides = new Dictionary<ushort, bool?[]>();

    public void DrawChunk(GameObject chunk)

    {
        block_Manager = new Block_Manager();
        Mesh octree_mesh = chunk.GetComponent<MeshFilter>().mesh;
        //Dictionary<ushort, int> materialdic = chunk.GetComponent<Octree_Controller>().materialdic;
        MeshRenderer octree_MeshRender = chunk.GetComponent<MeshRenderer>();

        octree = chunk.GetComponent<Octree_Controller>().octree;
        deniedSides = new Dictionary<ushort, bool?[]>();
        float octreeSize = chunk.GetComponent<Octree_Controller>().octreeSize;

        //Set up Mesh
        octree_mesh.Clear();
        byte lengthverts = 18;
        Vector3[] meshverts = new Vector3[octree.Count * lengthverts];
        List<int> facetriangles = new List<int>();
        int count = 0;

        //Calculate which nodes to render based on exposed sides
        foreach (ushort code in octree.Keys)
        {
            bool sideHasBeenDenied = deniedSides.ContainsKey(code);
            bool?[] sidesToRender = new bool?[6];
            if (!block_Manager.blocklist[octree[code]].opaque)
            {
                continue;
            }
            if (!deniedSides.ContainsKey(code))
            { 
                sidesToRender = new bool?[] { DetermineSideRender(octree, code, 'z',  'x', -1, 0), DetermineSideRender(octree, code, 'y', 'x', 1, 1), // -z, +y
                     DetermineSideRender(octree, code, 'x', 'z', 1, 2), DetermineSideRender(octree, code, 'x', 'z', -1, 3), // +x, -x
                     DetermineSideRender(octree, code, 'z', 'x', 1, 4), DetermineSideRender(octree, code, 'x', 'x', -1, 5) }; // +z, -y
                deniedSides.Add(code, sidesToRender);
            }
            else
            {
                sidesToRender = deniedSides[code];
                if (!(sidesToRender[0] ?? false))
                {
                    sidesToRender[0] = DetermineSideRender(octree, code, 'z', 'x', -1, 0);
                }
                if (!(sidesToRender[1] ?? false))
                {
                    sidesToRender[1] = DetermineSideRender(octree, code, 'y', 'x', 1, 1);
                }
                if (!(sidesToRender[2] ?? false))
                {
                    sidesToRender[2] = DetermineSideRender(octree, code, 'x', 'z', 1, 2);
                }
                if (!(sidesToRender[3] ?? false))
                {
                    sidesToRender[3] = DetermineSideRender(octree, code, 'x', 'z', -1, 3);
                }
                if (!(sidesToRender[4] ?? false))
                {
                    sidesToRender[4] = DetermineSideRender(octree, code, 'z', 'x', 1, 4);
                }
                if (!(sidesToRender[5] ?? false))
                {
                    sidesToRender[5] = DetermineSideRender(octree, code, 'y', 'x', -1, 5);
                }

                deniedSides[code] = sidesToRender;
            }
        }
        foreach (ushort code in octree.Keys)
        {
            if (!block_Manager.blocklist[octree[code]].opaque)
            {
                continue;
            }
            bool?[] sidesToRender = new bool?[6];
            if (deniedSides.ContainsKey(code))
            {
                sidesToRender = deniedSides[code];
            }
/*            else
            {
                sidesToRender = new bool?[6] { true, true, true, true, true, true };
            }*/
            if (block_Manager.blocklist[octree[code]].opaque == true && octree.Count == 1)
            {
                Debug.Log(string.Join(", ", sidesToRender.Select(b => b.ToString()).ToArray()));
            }
            if (sidesToRender.Any(x => x ?? false)) //If at least one side can be rendered
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
                meshverts[(count * lengthverts) + 4] = locpos + new Vector3(tier_size * GreedyAdjacent(octree, code, 'x', 'z', -1, 0), 0, 0);
                meshverts[(count * lengthverts) + 5] = locpos + new Vector3(tier_size * GreedyAdjacent(octree, code, 'x', 'z', -1, 0), tier_size, 0);
                //Side Y+
                meshverts[(count * lengthverts) + 6] = locpos + new Vector3(tier_size * GreedyAdjacent(octree, code, 'x', 'y', 1, 1), tier_size, 0);
                meshverts[(count * lengthverts) + 7] = locpos + new Vector3(tier_size * GreedyAdjacent(octree, code, 'x', 'y', 1, 1), tier_size, tier_size);
                //Side X+
                meshverts[(count * lengthverts) + 8] = locpos + new Vector3(tier_size, 0, 0);
                meshverts[(count * lengthverts) + 9] = locpos + new Vector3(tier_size, tier_size, 0);
                meshverts[(count * lengthverts) + 10] = locpos + new Vector3(tier_size, tier_size, tier_size * GreedyAdjacent(octree, code, 'z', 'x', 1, 2));
                meshverts[(count * lengthverts) + 11] = locpos + new Vector3(tier_size, 0, tier_size * GreedyAdjacent(octree, code, 'z', 'x', 1, 2));
                //Side X-
                meshverts[(count * lengthverts) + 12] = locpos + new Vector3(0, tier_size, tier_size * GreedyAdjacent(octree, code, 'z', 'x', -1, 3));
                meshverts[(count * lengthverts) + 13] = locpos + new Vector3(0, 0, tier_size * GreedyAdjacent(octree, code, 'z', 'x', -1, 3));
                //Side Z+
                meshverts[(count * lengthverts) + 14] = locpos + new Vector3(tier_size * GreedyAdjacent(octree, code, 'x', 'z', 1, 4), tier_size, tier_size);
                meshverts[(count * lengthverts) + 15] = locpos + new Vector3(tier_size * GreedyAdjacent(octree, code, 'x', 'z', 1, 4), 0, tier_size);
                //Side Y-
                meshverts[(count * lengthverts) + 16] = locpos + new Vector3(tier_size * GreedyAdjacent(octree, code, 'x', 'y', -1, 5), 0, 0);
                meshverts[(count * lengthverts) + 17] = locpos + new Vector3(tier_size * GreedyAdjacent(octree, code, 'x', 'y', -1, 5), 0, tier_size);


                //face front: -z
                if ((sidesToRender[0] ?? true)) //Should this side show? Plus Greedmesh Check
                {

                    facetriangles.Add((count * lengthverts) + 0);
                    facetriangles.Add((count * lengthverts) + 5);
                    facetriangles.Add((count * lengthverts) + 4); //f-z t1
                    facetriangles.Add((count * lengthverts) + 0);
                    facetriangles.Add((count * lengthverts) + 1);
                    facetriangles.Add((count * lengthverts) + 5); //f-z t2
                } 

                //face top: +y
                if ((sidesToRender[1] ?? true)) //Should this side show? Plus Greedmesh Check
                {

                    facetriangles.Add((count * lengthverts) + 6);
                    facetriangles.Add((count * lengthverts) + 1);
                    facetriangles.Add((count * lengthverts) + 2); //t+y t1
                    facetriangles.Add((count * lengthverts) + 6);
                    facetriangles.Add((count * lengthverts) + 2);
                    facetriangles.Add((count * lengthverts) + 7); //t+y t2

                }

                //face right: +x
                if (sidesToRender[2] ?? true) //Should this side show? Plus Greedmesh Check
                {

                    facetriangles.Add((count * lengthverts) + 8);
                    facetriangles.Add((count * lengthverts) + 9);
                    facetriangles.Add((count * lengthverts) + 10); //r+x t1
                    facetriangles.Add((count * lengthverts) + 8);
                    facetriangles.Add((count * lengthverts) + 10);
                    facetriangles.Add((count * lengthverts) + 11); //r+x t2
                }

                //face left: -x
                if (sidesToRender[3] ?? true) //Should this side show? Plus Greedmesh Check
                {
                    facetriangles.Add((count * lengthverts) + 0);
                    facetriangles.Add((count * lengthverts) + 13);
                    facetriangles.Add((count * lengthverts) + 12); //l-x t1
                    facetriangles.Add((count * lengthverts) + 0);
                    facetriangles.Add((count * lengthverts) + 12);
                    facetriangles.Add((count * lengthverts) + 1); //l-x t2

                }

                //face back: +z
                if (sidesToRender[4] ?? true) //Checking side - Plus Greedmesh Check
                {
                    facetriangles.Add((count * lengthverts) + 14);
                    facetriangles.Add((count * lengthverts) + 2);
                    facetriangles.Add((count * lengthverts) + 3); //l-z t1
                    facetriangles.Add((count * lengthverts) + 14);
                    facetriangles.Add((count * lengthverts) + 3);
                    facetriangles.Add((count * lengthverts) + 15); //l-z t2

                }

                //face bottom: -y
                if (sidesToRender[5] ?? true) //Checking Side -  Plus Greedmesh Check
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

    private bool? DetermineSideRender(Dictionary<ushort, int> octree, ushort code, char adjacentAxis, char sideAxis, int dir, int index)

    {
        if (!block_Manager.blocklist[octree[code]].opaque)
        {
            return false;
        }
        ushort adjacent = olc.CalculateAdjacent(code, adjacentAxis, dir);
        if (adjacent == code) // Adjacent is the same as code if they are at the edge of a chunk, should render.
        {
            return true;
        }

        else if (octree.ContainsKey(adjacent)) //Check to see if Adjacent is in Octree Dictionary
        {
            bool ogTransparent = block_Manager.blocklist[octree[code]].translucent;
            bool adjacenttransparency = block_Manager.blocklist[octree[adjacent]].translucent;

            if (adjacenttransparency == true && ogTransparent != adjacenttransparency) //Is adjacent transparent
            {
                return GreedySubAdjacent(octree, code, sideAxis, adjacentAxis, dir, index);
            }
            else
            {
                return false;
            }
        }
        else
        {
            bool ogTransparent = block_Manager.blocklist[octree[code]].translucent;
            for (int i = 1; i < olc.CalculateDepth(code); i++)
            {
                ushort depthCode = (ushort)(code >> (i * 3));
                if (octree.ContainsKey(depthCode))
                {
                    
                    bool adjacenttransparency = block_Manager.blocklist[octree[depthCode]].translucent;

                    if (adjacenttransparency == true && ogTransparent != adjacenttransparency)
                    {
                        return GreedySubAdjacent(octree, depthCode, sideAxis, adjacentAxis, dir, index);
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

    private bool? GreedySubAdjacent(Dictionary<ushort, int> octree, ushort code, char sideAxis, char adjacentAxis, int dir, int index)
    {
        ushort adjacent = olc.CalculateAdjacent(code , sideAxis, dir);
        if (octree.ContainsKey(adjacent))
        {
            if (code != adjacent && block_Manager.blocklist[octree[code]].itemcode == block_Manager.blocklist[octree[adjacent]].itemcode &&
                    olc.CalculateDepth(code) == olc.CalculateDepth(adjacent) && (DetermineSideRender(octree,  adjacent, sideAxis, adjacentAxis, dir, index) ?? false)) //TODO: Interior nodes may still render
            {
                /*if (deniedSides.ContainsKey(code))
                {
                    deniedSides[code][index] = false;

                }
                else
                {
                    deniedSides.Add(code, new bool[6] { false, false, false, false, false, false });
                }*/
                return false;
            }
        }
        return true;
    }

    private int GreedyAdjacent(Dictionary<ushort, int> octree, ushort m, char axis, char side, int dir, int index)
    {
        int modifiercount = 1;
        ushort adjacent = olc.CalculateAdjacent(m, axis, 1);
        if (octree.ContainsKey(adjacent))
        {
            if (m != adjacent && block_Manager.blocklist[octree[m]].itemcode == block_Manager.blocklist[octree[adjacent]].itemcode &&
                    olc.CalculateDepth(m) == olc.CalculateDepth(adjacent) && (DetermineSideRender(octree, adjacent, side, axis, dir, index) ?? true))
            {
                modifiercount += GreedyAdjacent(octree, adjacent, axis, side, dir, index);
            }
        }
        return modifiercount;
    }
}


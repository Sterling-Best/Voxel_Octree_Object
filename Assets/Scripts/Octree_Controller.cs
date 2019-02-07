﻿using System;
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

    public byte chunkMaxDepth;

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
        this.block_Manager = new Block_Manager();
        this.olc = new OT_LocCode();
        this.octreepos = this.gameObject.transform.position;
        this.octreelimitpos = octreepos + new Vector3(octreeSize, octreeSize, octreeSize);
    }

        // Use this for initialization
        void Start()
    {
        MergeAllNodes();

        chunk_Renderer.DrawChunk(gameObject);
    }



    // Update is called once per frame
    void Update()
    {
    
    }

    public void AddNodeAbsPos(Vector3 a_position, byte depth, int type)
    {
        Vector3 position = new Vector3(a_position.x - this.transform.position.x, a_position.y - this.transform.position.y, a_position.z - this.transform.position.z);
        AddNodeRelPos(position, depth, type);
    }

    public void AddNodeRelPos(Vector3 a_position, byte depth, int type) {
        int depthcoord = (int)this.octreeSize / (int)(Math.Pow(2, depth));
        Vector3 position = new Vector3((int)Math.Floor(a_position.x) / depthcoord, (int)Math.Floor(a_position.y) / depthcoord, (int)Math.Floor(a_position.z) / depthcoord);
        AddNodeLocID(olc.Vec3ToLoc(position, depth), type);
    }

    public void AddNodeLocID(long locID, int type)
    {
        
        //if (!materialdic.ContainsKey(type))
        //{
        //    this.materialdic.Add(type, this.materialdic.Count);
        //}
        this.octree.Add(locID, type);
        
    }

    public void MergeAllNodes()
    {
        for (int d = chunkMaxDepth; d > 0; d--)
        {
            Dictionary<long, int> deletedic = new Dictionary<long, int>();
            Dictionary<long, int> adddic = new Dictionary<long, int>();
            for (int i = (int)Math.Pow(8, d); i < ((int)Math.Pow(8, d) *2); i++)
            {
                if (octree.ContainsKey(i) && !deletedic.ContainsKey(i) )
                {
                    long parent = olc.CalculateParent(i);
                    Array children = olc.CollectChildrenAll(parent);
                    bool same = true;
                    //Check for any children that are different
                    for (int c = 0; c < children.Length; c++)
                    {
                        if (octree.ContainsKey((long)children.GetValue(c)))
                        {
                            if (octree[(long)children.GetValue(c)] != octree[(long)children.GetValue(0)])
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
                        adddic.Add(parent, octree[i]);
                        foreach (long child in children)
                        {
                            if (!deletedic.ContainsKey(child))
                            {
                                deletedic.Add(child, octree[child]);
                            }
                        }
                    }
                    //If children are all the same, add to remove, and add parent to octree
                }
            }
            foreach (KeyValuePair<long, int> deleted in deletedic)
            {
                octree.Remove(deleted.Key);
            }
            foreach (KeyValuePair<long, int> added in adddic)
            {
                octree.Add(added.Key, added.Value);
            }
        }
    }



}

   
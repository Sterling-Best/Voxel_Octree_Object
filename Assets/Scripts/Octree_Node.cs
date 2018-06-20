using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octree_Node: Object {

    public string locationCode;

    public Block type;

    private int submeshIndex;

    public Octree_Node(Octree_Controller_v2 controller, string loccode, int a_type = 0)
    {
        this.locationCode = loccode;
        this.type = controller.block_Manager.blocklist[a_type];
    }

    public int get_submesh()
    {
        return this.submeshIndex;
    }

    public void set_submesh(int x)
    {
        this.submeshIndex = x;
    }

    public override string ToString()
    {
        return this.locationCode; 
    }
}

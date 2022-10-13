using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct VoxelRenderCache
{

    private ushort collection;

    private ushort itemCode;

    private bool opaque;

    private bool translucent;

    private string materialFile; 

    public VoxelRenderCache(ushort col, ushort cod, bool opaq, bool trans, string mfile)
    {
        this.collection = col;
        this.itemCode = cod;
        this.opaque = opaq;
        this.translucent = trans;
        this.materialFile = mfile;
    }

}
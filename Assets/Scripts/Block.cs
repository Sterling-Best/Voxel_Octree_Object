using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Block { 
    public ushort collection;
    public ushort itemcode;
    public bool transparency; 
    public string materialfile;

    public Block (ushort col, ushort cod, bool trans, string mfile)
    {
        this.collection = col;
        this.itemcode = cod;
        this.transparency = trans;
        this.materialfile = mfile;
    }

}
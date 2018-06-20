using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {

    public int collection;
    public int code; // Collection-BlockID; ex' 0-0 is default blocks - void block

    //Details of Block
    public int transparency; //0-2; 0 = Completely Transparent, 1 = Transluscent, 2 = Opaque 

    public string materialfile;

}

public class AirBlock: Block
{
    public AirBlock()
    {
        this.collection = 0;
        this.code = 0;
        this.transparency = 0;
        this.materialfile = "[0]AirBlock";
    }
}

public class DirtBlock : Block
{
    public DirtBlock()
    {
        this.collection = 0;
        this.code = 1;
        this.transparency = 2;
        this.materialfile = "[1]DirtBlock";
    }
}

public class GrassBlock : Block
{
    public GrassBlock()
    {
        this.collection = 0;
        this.code = 2;
        this.transparency = 2;
        this.materialfile = "[2]GrassBlock";
    }
}

public class StoneBlock : Block
{
    public StoneBlock()
    {
        this.collection = 0;
        this.code = 3;
        this.transparency = 2;
        this.materialfile = "[3]StoneBlock";
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_Manager
{

    public Block[] blocklist =
    {
        new Block(0,0,false, true,"[0]AirBlock"), new Block(0,1,true, false,"[1]DirtBlock"),
        new Block(0,2,true, false, "[2]GrassBlock"), new Block(0,3,true, false,"[3]StoneBlock")
    };

   public List<Material> blockMaterialList = new List<Material>();

    public Block_Manager()
    {
        foreach (Block block in this.blocklist)
        {
            string fileString = "Materials/" + block.materialfile;
            Material material = Resources.Load(fileString, typeof(Material)) as Material;
            blockMaterialList.Add(material);
        }
    }
}
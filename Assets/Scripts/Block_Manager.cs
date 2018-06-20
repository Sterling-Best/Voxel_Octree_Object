using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_Manager :MonoBehaviour
{

    public Block[] blocklist =
    {
        new AirBlock(),
        new DirtBlock(),
        new GrassBlock(),
        new StoneBlock(),
    };

   public List<Material> blockMaterialList = new List<Material>();

    public void Start()
    {

        
    }

    public Block_Manager()
    {
        foreach (Block block in this.blocklist)
        {
            string fileString = "Materials/" + block.materialfile;
            Material material = Resources.Load(fileString, typeof(Material)) as Material;
            blockMaterialList.Add(material);
        }
        Debug.Log("Block_Manager Material List: " + blockMaterialList);
    }
}
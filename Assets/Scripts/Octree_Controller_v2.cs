using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Renderer))]
public class Octree_Controller_v2: MonoBehaviour {

    public Dictionary<string, Octree_Node> octree = new Dictionary<string, Octree_Node>();

    public float octreesize = 16.0f;

    Vector3 octreepos;

    Vector3 octreelimitpos;

    public Block_Manager block_Manager;

    Renderer octree_MeshRender;

    Mesh octree_mesh;

    Material[] materialsreference;
    

    // Use this for initialization
    void Start () {

        this.block_Manager = new Block_Manager();

        this.octreepos = this.transform.position;
        this.octreelimitpos = octreepos + new Vector3(octreesize, octreesize, octreesize);

        //Test 1
        this.octree.Add("", new Octree_Node(this, "", 0));
        this.octree.Add("000", new Octree_Node(this, "000", 0));
        this.octree.Add("001", new Octree_Node(this, "001", 0));
        this.octree.Add("010", new Octree_Node(this, "010", 0));
        this.octree.Add("011", new Octree_Node(this, "011", 0));
        this.octree.Add("100", new Octree_Node(this, "100", 0));
        this.octree.Add("101", new Octree_Node(this, "101", 0));
        this.octree.Add("110", new Octree_Node(this, "110", 0));
        this.octree.Add("111", new Octree_Node(this, "111", 0));
        this.octree.Add("000000", new Octree_Node(this, "000000", 3));
        this.octree.Add("000001", new Octree_Node(this, "000001", 3));
        this.octree.Add("000010", new Octree_Node(this, "000010", 1));
        this.octree.Add("000011", new Octree_Node(this, "000011", 2));
        this.octree.Add("000011000", new Octree_Node(this, "000011000", 1));
        this.octree.Add("000011001", new Octree_Node(this, "000011001", 1));
        this.octree.Add("000011010", new Octree_Node(this, "000011010", 2));
        this.octree.Add("000011011", new Octree_Node(this, "000011011", 2));
        this.octree.Add("000011100", new Octree_Node(this, "000011100", 1));
        this.octree.Add("000011101", new Octree_Node(this, "000011101", 1));
        this.octree.Add("000011110", new Octree_Node(this, "000011110", 2));
        this.octree.Add("000011111", new Octree_Node(this, "000011111", 2));
        this.octree.Add("000100", new Octree_Node(this, "000100", 3));
        this.octree.Add("000101", new Octree_Node(this, "000101", 1));
        this.octree.Add("000110", new Octree_Node(this, "000110", 1));
        this.octree.Add("000111", new Octree_Node(this, "000111", 1));

        //Test 2

        PreRender();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void PreRender()
    {
        //Set up Mesh
        this.octree_mesh = GetComponent<MeshFilter>().mesh;
        this.octree_mesh.Clear();
        this.octree_mesh.subMeshCount = this.octree.Count;
        //Set Up Materials
        this.octree_MeshRender = GetComponent<MeshRenderer>();
        List<Material> materiallist = new List<Material>();
        //Record Chunk's Current Position
        Vector3 octreepos = this.transform.position;
        //Display Default Information for Chunk
        Debug.Log("Mesh.Vertices: " + Vector3ArrayList(this.octree_mesh.vertices));
        Debug.Log("Mesh.Triangles: " + IntArrayList(this.octree_mesh.triangles));
        Debug.Log(octreepos);
        int count = 0;
        //Check each node in the octree if it should be rendered. 
        foreach (Octree_Node node in octree.Values)
        {
            //Only Render if node is  (has no children, identified if a key exists with string (current node.locationCode + "000"))
            if (octree.ContainsKey(node.locationCode + "000") == false)
            {
                //If it is a leaf, check to see if this block type is transparent (void, atmosphere, item blocks (blocks with models), etc...). If it is don't render. 
                Debug.Log(node.locationCode);
                float tier_size = this.octreesize * (1 / (float)Math.Pow(2, (node.locationCode.Length / 3)));
                Vector3 locpos = LocationCodeToVector3(node.locationCode);
                Vector3[] verts = {
                    octreepos + locpos + new Vector3 (0, 0, 0),
                    octreepos + locpos + new Vector3 (tier_size, 0, 0),
                    octreepos + locpos + new Vector3 (tier_size, tier_size, 0),
                    octreepos + locpos + new Vector3 (0, tier_size, 0),
                    octreepos + locpos + new Vector3 (0, tier_size, tier_size),
                    octreepos + locpos + new Vector3 (tier_size, tier_size, tier_size),
                    octreepos + locpos + new Vector3 (tier_size, 0, tier_size),
                    octreepos + locpos + new Vector3 (0, 0, tier_size),
                };

                int[] facetriangles = {
                    //face front
                    (count * 8) + 0, (count * 8) + 2, (count * 8) + 1, //f t1
			        (count * 8) + 0, (count * 8) + 3, (count * 8) + 2, //f t2
                    //face top
                    (count * 8) + 2, (count * 8) + 3, (count * 8) + 4, //t t1
			        (count * 8) + 2, (count * 8) + 4, (count * 8) + 5, //t t2
                    //face right
                    (count * 8) + 1, (count * 8) + 2, (count * 8) + 5, //r t1
			        (count * 8) + 1, (count * 8) + 5, (count * 8) + 6, //r t2
                    //face left
                    (count * 8) + 0, (count * 8) + 7, (count * 8) + 4, //l t1
			        (count * 8) + 0, (count * 8) + 4, (count * 8) + 3, //l t2
                    //face back
                    (count * 8) + 5, (count * 8) + 4, (count * 8) + 7, //l t1
			        (count * 8) + 5, (count * 8) + 7, (count * 8) + 6, //l t2
                    //face bottom
                    (count * 8) + 0, (count * 8) + 6, (count * 8) + 7, //b t1
                    (count * 8) + 0, (count * 8) + 1, (count * 8) + 6  //b t2
                    };

                this.octree_mesh.vertices = CombineVector3Arrays(this.octree_mesh.vertices, verts);
                Debug.Log("Mesh.Vertices: " + Vector3ArrayList(this.octree_mesh.vertices));
                //this.octree_mesh.triangles = CombineIntArrays(this.octree_mesh.triangles, facetriangles);
                Debug.Log("Mesh.Triangles: " + IntArrayList(this.octree_mesh.triangles));
                this.octree_mesh.SetTriangles(facetriangles, count);

                Debug.Log("Type Number: " + node.type.code);
                materiallist.Add(this.block_Manager.blockMaterialList[node.type.code]);

                this.octree_MeshRender.materials = materiallist.ToArray();

                this.octree_mesh.RecalculateNormals();
                this.octree_mesh.RecalculateBounds();
                count++;
            }
            else
            {
                continue;
            }
        }
    }

    Vector3[] CombineVector3Arrays(Vector3[] array1, Vector3[] array2) {
        Vector3[] array3 = new Vector3[array1.Count() + array2.Count()];
    System.Array.Copy(array1, array3, array1.Count());
        System.Array.Copy(array2, 0, array3, array1.Count(), array2.Count());
        return array3;
    }

    Vector2[] CombineVector2Arrays(Vector2[] array1, Vector2[] array2)
    {
        Vector2[] array3 = new Vector2[array1.Count() + array2.Count()];
        System.Array.Copy(array1, array3, array1.Count());
        System.Array.Copy(array2, 0, array3, array1.Count(), array2.Count());
        return array3;
    }

    int[] CombineIntArrays(int[] array1, int[] array2)
    {
        int[] array3 = new int[array1.Count() + array2.Count()];
        System.Array.Copy(array1, array3, array1.Count());
        System.Array.Copy(array2, 0, array3, array1.Count(), array2.Count());
        return array3;
    }

    string Vector3ArrayList(Vector3[] target)
    {
        string vectstr = "";
        foreach (var vect in target)
        {
            vectstr = vectstr + vect + ", ";
        }
        return vectstr;
    }

    string Vector2ArrayList(Vector2[] target)
    {
        string vectstr = "";
        foreach (var vect in target)
        {
            vectstr = vectstr + vect + ", ";
        }
        return vectstr;
    }

    string IntArrayList(int[] target)
    {
        string vectstr = "";
        foreach (var vect in target)
        {
            vectstr = vectstr + vect + ", ";
        }
        return vectstr;
    }

    Vector3 LocationCodeToVector3(string locationcode)
    {
        Vector3 Vec3Sum = new Vector3(0, 0, 0);
        for (int i = 1; i <= (locationcode.Length / 3); i++)
        {
            float tier_size = this.octreesize * (1 / (float)(Math.Pow(2, i)));
            float x = Convert.ToInt32(locationcode[((i - 1) * 3) + 0].ToString());
            float y = Convert.ToInt32(locationcode[((i - 1) * 3) + 1].ToString());
            float z = Convert.ToInt32(locationcode[((i - 1) * 3) + 2].ToString());
            x = tier_size * x;
            y = tier_size * y;
            z = tier_size * z;

            //Debug.Log(x.ToString() + ", " + y.ToString() + ", " + z.ToString());

            Vec3Sum = Vec3Sum + new Vector3(x, y, z);
        }

        Debug.Log("Location Code to Vert - LocCode: " + locationcode + " LocPos: " + Vec3Sum.ToString());

        return Vec3Sum;
    }

    String AbsVec3toLocCode(Vector3 a_coord, int a_targetsize)
    {
        string loccode = "";
        return loccode;
    }

    String RelVec3toLocCode(Vector3 a_coord, int a_targetsize)
    {
        string loccode = "";
        if (((this.octreepos.x < a_coord.x) && (a_coord.x < this.octreelimitpos.x)) && ((this.octreepos.y < a_coord.y) && (a_coord.y < this.octreelimitpos.y)) && ((this.octreepos.z < a_coord.z) && (a_coord.z < this.octreelimitpos.z)))
        //Checks to see if coordinates are within octree's boundaries 
        {
            if (a_targetsize < this.octreesize)
            //Check to see if target size for the voxel is within octree, else Error if it is too large
            {
                Vector3 remaining_coord = a_coord; //Vec3 used to store remaining values while determing morton code teirs.
                for (float i = (this.octreesize); i >= a_targetsize; i = i / 2)
                //Goes through each teir of the octree down to desired teir/targetsize
                {
                    //X
                    if (remaining_coord.x >= i)
                    {
                        loccode = loccode + "1";

                    }
                    //Y

                    //Z
                }
            }
            else if (a_targetsize == this.octreesize)
            {
                loccode = "";
            }
            else
            //Target size of voxel is not compatible with octree, too large
            {
                Debug.LogError("RelVec3toLocCode: Target Size of Voxel does not fit with the size of the octree.");
                return null;
            }
        }
        else
        //Target coordinates do not fall within octree's boundaries 
        {
            Debug.LogError("RelVec3toLocCode: Coordinate is not within octree.");
            return null;
        }
        Debug.LogError("RelVec3toLocCode: Has failed to generated a location code for block at " + a_coord.ToString() + " for a size of " + a_targetsize.ToString());
        return null;
    }



    //Voxels are any region of the octree, must be equal to or smaller than the octree
    //AddVoxelCode - using location code
    //AddVoxelCoord - using coordinates, refering using AddBlockCode
    //ReplaceVoxelCode - using location code
    //ReplaceVoxelCoord 
    //RemoveBlockCode - using location code
    //RemoveBlockCoord

    //Blocks are a voxel of 1 unit in size, these methods will utilize voxel methods, but are restricted to size of 1
    //AddBlock
    //ReplaceBlock
    //Remove Block

} 

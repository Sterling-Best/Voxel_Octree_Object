using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Renderer))]
public class Octree_Controller_v0 : MonoBehaviour {

    public Dictionary<string, Octree_Node> octree = new Dictionary<string, Octree_Node>();

    public float size = 10.0f;

    public Block_Manager block_Manager = new Block_Manager();

    Octree_Render octree_Render = new Octree_Render();

    Mesh octree_mesh;

    Vector2[] octree_uvs;

    Vector3[] vertices;

    // Use this for initialization
    void Start () {

        //Test 1
        //this.octree.Add("001", new Octree_Node(this, "001", 0));
        //this.octree.Add("010", new Octree_Node(this, "010", 0));
        //this.octree.Add("011", new Octree_Node(this, "011", 0));
        //this.octree.Add("100", new Octree_Node(this, "100", 0));
        //this.octree.Add("101", new Octree_Node(this, "101", 0));
        //this.octree.Add("110", new Octree_Node(this, "110", 0));
        //this.octree.Add("111", new Octree_Node(this, "111", 0));
        //this.octree.Add("000000", new Octree_Node(this, "000000", 1));
        //this.octree.Add("000001", new Octree_Node(this, "000001", 1));
        //this.octree.Add("000010", new Octree_Node(this, "000010", 1));
        //this.octree.Add("000011", new Octree_Node(this, "000011", 1));
        //this.octree.Add("000100", new Octree_Node(this, "000100", 1));
        //this.octree.Add("000101", new Octree_Node(this, "000101", 1));
        //this.octree.Add("000110", new Octree_Node(this, "000110", 1));
        //this.octree.Add("000111", new Octree_Node(this, "000111", 1));

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
        var renderer = GetComponent<Renderer>();
        Material[] material_placeholder_list = {
        };
        renderer.materials = material_placeholder_list;
        Vector3 pos = this.transform.position;
        Debug.Log("Mesh.Vertices: " + Vector3ArrayList(this.octree_mesh.vertices));
        Debug.Log("Mesh.Triangles: " + IntArrayList(this.octree_mesh.triangles));
        int count = 0;
        foreach (Octree_Node node in octree.Values) {
            Debug.Log(node.locationCode);
            float tier_size = this.size * (1 / (float) Math.Pow(2, (node.locationCode.Length/3)));
            Vector3 locpos = LocationCodeToVector3(node.locationCode);
            Vector3[] verts = {
            pos + locpos + new Vector3 (0, 0, 0),
            pos + locpos + new Vector3 (tier_size, 0, 0),
            pos + locpos + new Vector3 (tier_size, tier_size, 0),
            pos + locpos + new Vector3 (0, tier_size, 0),
            pos + locpos + new Vector3 (0, tier_size, tier_size),
            pos + locpos + new Vector3 (tier_size, tier_size, tier_size),
            pos + locpos + new Vector3 (tier_size, 0, tier_size),
            pos + locpos + new Vector3 (0, 0, tier_size),
            };

            int[] facetriangles =
            {
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
            
            this.vertices = verts;
            this.octree_mesh.vertices = CombineVector3Arrays(this.octree_mesh.vertices, this.vertices);
            Debug.Log("Mesh.Vertices: " + Vector3ArrayList(this.octree_mesh.vertices));
            //this.octree_mesh.triangles = CombineIntArrays(this.octree_mesh.triangles, facetriangles);
            Debug.Log("Mesh.Triangles: " + IntArrayList(this.octree_mesh.triangles));
            this.octree_mesh.SetTriangles(facetriangles, count);
            this.octree_mesh.RecalculateNormals();
            this.octree_mesh.RecalculateBounds();
            count++;
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
            float tier_size = this.size * (1 / (float)(Math.Pow(2, i)));
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
} 

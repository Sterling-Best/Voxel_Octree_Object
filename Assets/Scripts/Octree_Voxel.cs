using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Renderer))]
public class Octree_Voxel : MonoBehaviour {

    public Transform pos;

    public string locationcode; //Morton Code

    public float size = 1;

    private ArrayList octreeVoxel;

    private Vector3 realpos;

    private Vector3[] vertices;

    private int[] facetraingles =
    {
            0, 2, 1, //face front
			0, 3, 2,
            2, 3, 4, //face top
			2, 4, 5,
            1, 2, 5, //face right
			1, 5, 6,
            0, 7, 4, //face left
			0, 4, 3,
            5, 4, 7, //face back
			5, 7, 6,
            0, 6, 7, //face bottom
            0, 1, 6
    };

    private Vector2[] uvs;

    private Mesh mesh;

	// Use this for initialization
	void Start () {
        //this.locationcode = "0";
        //GenerateVertices();
        //GenerateMesh();
        //GenerateUVs();

    }
	
	// Update is called once per frame
	void Update () {
	}

    void GenerateVertices()
    {
        this.pos = this.transform;
        this.realpos = pos.position;
        Vector3[] verts = {
            this.realpos + new Vector3 (0, 0, 0),
            this.realpos + new Vector3 (this.size, 0, 0),
            this.realpos + new Vector3 (this.size, this.size, 0),
            this.realpos + new Vector3 (0, this.size, 0),
            this.realpos + new Vector3 (0, this.size, this.size),
            this.realpos + new Vector3 (this.size, this.size, this.size),
            this.realpos + new Vector3 (this.size, 0, this.size),
            this.realpos + new Vector3 (0, 0, this.size),
        };
        this.vertices = verts;
    }

    void GenerateMesh()
    {
        this.mesh = GetComponent<MeshFilter>().mesh;
        this.mesh.Clear();
        this.mesh.vertices = this.vertices;
        this.mesh.triangles = this.facetraingles;
        this.mesh.RecalculateNormals();
        this.uvs = new Vector2[this.vertices.Length];
        for (int i = 0; i < this.uvs.Length; i++)
        {
            this.uvs[i] = new Vector2(this.vertices[i].x, this.vertices[i].z);
        }
        this.mesh.uv = this.uvs;

    }

    void GenerateUVs()
    {
        
        var renderer = GetComponent<Renderer>();
        var materials = renderer.sharedMaterials;
        materials[0] = Resources.Load("Materials/Octree_Tier", typeof(Material)) as Material;
        materials[0].shader = Shader.Find("Standard");
        materials[0].SetColor("Standard", Color.green);
        renderer.sharedMaterials = materials;
    }
}

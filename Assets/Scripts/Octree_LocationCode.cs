using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octree_LocationCode : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        //Morton Test
        Vector3 formercode = new Vector3(7, 4, 13);
        Debug.Log("Morton Code Test: " + formercode.ToString());
        long code = this.Vec3ToLoc(formercode, 4);
        Debug.Log("Morton Code: " + code.ToString());
        Vector3 vec = this.LocToVec3(code);
        Debug.Log("Vector3 Locatio: " + vec.ToString());

    }

    // Update is called once per frame
    void Update()
    {

    }


    //Encoding Location Code (Morton Code)

    public long Vec3ToLoc(Vector3 vec, int depth)
    {
        long answer = 0;
        //interleave vectors
        answer |= part1by2(vec.x) | part1by2(vec.y) << 1 | part1by2(vec.z) << 2;
        //Add depth Identifier
        if (depth > 0)
        {
            int depthmodifier = Convert.ToInt32(Math.Pow(8, depth));
            answer = (answer | depthmodifier);
        }
        return answer;
    }

    private long part1by2(long x)
    {
        x &= 0x000003ff;
        x = (x | x << 16) & 0x1f0000ff0000ff;
        x = (x | x << 8) & 0x100f00f00f00f00f;
        x = (x | x << 4) & 0x10c30c30c30c30c3;
        x = (x | x << 2) & 0x1249249249249249;
        return x;
    }

    //public long part1by2(int a)
    //{
    //    return part1by2(Convert.ToInt32(a));
    //}

    public long part1by2(float a)
    {
        //TODO: Make sure 
        return part1by2(Convert.ToInt32(a));
    }

    //Decoding Morton Code

    public Vector3 LocToVec3(long x)
    {
        Vector3 vec = new Vector3(0, 0, 0);
        int depthmodifier = (Convert.ToInt32(Math.Pow(8, Convert.ToInt32(Math.Log(x, 8)))));
        x = (x ^ depthmodifier);
        vec.x = collapseby2(x);
        vec.y = collapseby2(x >> 1);
        vec.z = collapseby2(x >> 2);
        return vec;
    }

    private long collapseby2(long x)
    {
        x &= 0x1249249249249249;
        x = (x ^ (x >> 2)) & 0x10c30c30c30c30c3;
        x = (x ^ (x >> 4)) & 0x100f00f00f00f00f;
        x = (x ^ (x >> 8)) & 0x1f0000ff0000ff;
        x = (x ^ (x >> 16)) & 0x1f00000000ffff;
        return x;
    }
}

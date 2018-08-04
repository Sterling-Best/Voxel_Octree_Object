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
        Vector3 formercode = new Vector3(7, 3, 2);
        Debug.Log("Morton Code Test: " + formercode.ToString());
        long code = this.Vec3ToLoc(formercode, 3);
        Debug.Log("Morton Code: " + code.ToString());
        Vector3 vec = this.LocToVec3(code);
        Debug.Log("Vector3 Locatio: " + vec.ToString());
        long code2 = this.CalculateAdjacent(code, 0, 1);
        Debug.Log("Adjacent Search Up: " + code2);
        Vector3 vec2 = this.LocToVec3(code2);
        Debug.Log("Vector3 Location: " + vec2.ToString());
    }

    // Update is called once per frame
    void Update()
    {

    }

    //--Encoding/Decoding--//

    //Encoding Location Code (Morton Code)

    //TODO: Finish Vec3ToLoc() Documentation
    public long Vec3ToLoc(Vector3 vec, int depth)
    {
        long answer = 0;
        //interleave vectors
        answer |= part1by2(vec.z) | part1by2(vec.y) << 1 | part1by2(vec.x) << 2;
        //Add depth Identifier
        if (depth > 0)
        {
            int depthmodifier = Convert.ToInt32(Math.Pow(8, depth));
            answer = (answer | depthmodifier);
        }
        return answer;
    }

    //TODO: Finish part1by2(long) Documentation
    private long part1by2(long n)
    {
        n &= 0x000003ff;
        n = (n | n << 16) & 0x1f0000ff0000ff;
        n = (n | n << 8) & 0x100f00f00f00f00f;
        n = (n | n << 4) & 0x10c30c30c30c30c3;
        n = (n | n << 2) & 0x1249249249249249;
        return n;
    }


    //TODO: part1by2(int) figure out why this wasn't working
    //public long part1by2(int a)
    //{
    //    return part1by2(Convert.ToInt32(a));
    //}

    //TODO: Finish part1by2(float) Documentation
    public long part1by2(float a)
    {
        return part1by2(Convert.ToInt32(a));
    }

    //Decoding Morton Code

    //TODO: Finish LocToVec3() Documentation
    public Vector3 LocToVec3(long m)
    {
        Vector3 vec = new Vector3(0, 0, 0);
        int depthmodifier = (Convert.ToInt32(Math.Pow(8, Convert.ToInt32(Math.Log(m, 8)))));
        m = (m ^ depthmodifier);
        vec.z = collapseby2(m);
        vec.y = collapseby2(m >> 1);
        vec.x = collapseby2(m >> 2);
        return vec;
    }

    //TODO: Finish collapseby2() Documentation
    private long collapseby2(long n)
    {
        n &= 0x1249249249249249;
        n = (n ^ (n >> 2)) & 0x10c30c30c30c30c3;
        n = (n ^ (n >> 4)) & 0x100f00f00f00f00f;
        n = (n ^ (n >> 8)) & 0x1f0000ff0000ff;
        n = (n ^ (n >> 16)) & 0x1f00000000ffff;
        return n;
    }

    //--Searches--// 

    //Neighbor/Adjacent "Search"

    //Calculates what the adjacent node (with offset) is from another code.
    //TODO: Documentation: CalculateAdjacent() 
    //TODO: UnitTests: CalculateAdjacent() 
    /// <summary>
    /// Calculates adjacent code (with offset <paramref name="distance"/>) from the specified int code <paramref name="m"/>. Returns an int code.
    /// </summary>
    /// <param name="m"></param>
    /// <param name="axis"></param>
    /// <param name="distance"></param>
    /// <returns>A long. The desired Location code.</returns>
    /// <remarks>
    /// 
    /// </remarks>
    public long CalculateAdjacent(long m, byte axis, int distance)
    {
        int depth = Convert.ToInt32(Math.Log(m, 8));
        int depthmodifier = (Convert.ToInt32(Math.Pow(8, depth)));
        long[] axismask = { 0x6DB6DB6DB6DB6DB6, 0x5B6DB6DB6DB6DB6D, 0x36DB6DB6DB6DB6DB };
        m = (m ^ depthmodifier);
        long n = collapseby2(m >> axis);
        if (0 < (n + distance) && (n + distance) < Math.Pow(2, depth))
        {
            // TODO: Optimization: Possible optimization in injecting (n + distance) value back into m
            m = (m & (axismask[axis])) | (part1by2(n + distance) << axis);
        }
        m = (m | depthmodifier);
        return m;
    }

    //Offset Search

    //TODO: Documentation: CalculateOffset() 
    //TODO: UnitTests: CalculateOffest() 
    public long CalculateOffset(long m, Vector3 offset)
    {
        //TODO: Optimization: Look into similar set-up to CalculateAdjacent() to reduce operations (unpacking/packing).
        int depth = Convert.ToInt32(Math.Log(m, 8));
        Vector3 mvec = LocToVec3(m);
        mvec += offset;
        m = Vec3ToLoc(mvec, depth);
        return m;
    }

}

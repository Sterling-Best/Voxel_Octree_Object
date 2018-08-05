using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Octree_LocationCode
{
    //--Encoding/Decoding--//

    //Encoding Location Code (Morton Code)

    //TODO: Finish Vec3ToLoc() Documentation
    //TODO: Allow for depths above 7 to be used.
    public long Vec3ToLoc(Vector3 vec, int depth)
    {
        long m = 0;
        int max = Convert.ToInt32(new[] { vec.x, vec.y, vec.z }.Max());
        if (max >= Math.Pow(2, depth))
        {
            throw new ArgumentException(String.Format("{0} is not within bounds of depth {1}", vec, depth), 
                                      "vec");
        }
        else
        {
            
            m |= part1by2(vec.z) | part1by2(vec.y) << 1 | part1by2(vec.x) << 2;
            //Add depth Identifier
            if (depth > 0)
            {
                int depthmodifier = Convert.ToInt32(Math.Pow(8, depth));
                m = (m | depthmodifier);
            }
        }
        //interleave vectors
        return m;
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
        vec.z = Collapseby2(m);
        vec.y = Collapseby2(m >> 1);
        vec.x = Collapseby2(m >> 2);
        return vec;
    }

    //TODO: Finish collapseby2() Documentation
    private long Collapseby2(long n)
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
        byte depth = CalculateDepth(m);
        int depthmodifier = (Convert.ToInt32(Math.Pow(8, depth)));
        long[] axismask = { 0x6DB6DB6DB6DB6DB6, 0x5B6DB6DB6DB6DB6D, 0x36DB6DB6DB6DB6DB };
        m = (m ^ depthmodifier);
        long n = Collapseby2(m >> axis);
        if (WithinOctreeCheckInt(n + distance, depth))
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
        byte depth = CalculateDepth(m);
        Vector3 mvec = LocToVec3(m);
        mvec += offset;
        if (WithinOctreeCheckVec3(mvec, depth))
        {
            m = Vec3ToLoc(mvec, depth);
        }
        return m;
    }

    //--Location Code Helpers--//

    //Caluclate Depth
    //TODO: Documentation: CalculateDepth()
    //TODO: UnitTest: CalculateDepth()
    public byte CalculateDepth(long m)
    {
        return Convert.ToByte(Math.Log(m, 8));
    }

    //Check if Within Octree

    //TODO: Documentation: WithinOctreeInt()
    //TODO: UnitTest: WithinOctreeInt()
    private bool WithinOctreeCheckInt(long n, byte depth)
    {
        if (0 < n && n < Math.Pow(2, depth))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //TODO: Documentation: WithinOctreeVec3()
    //TODO: UnitTest: WithinOctreeVec3()
    private bool WithinOctreeCheckVec3(Vector3 vec, byte depth)
    {
        double upperlimit = Math.Pow(2, depth);
        if (new[] { vec.x, vec.y, vec.z }.All(x => (0 < x && x < upperlimit)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}

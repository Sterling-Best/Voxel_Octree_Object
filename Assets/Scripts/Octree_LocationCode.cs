using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OT_LocCode
{
    //--Encoding/Decoding--//

    //Encoding Location Code (Morton Code)

    //TODO: Documentation: Vec3ToLoc
    //TODO: Allow for depths above 7 to be used.
    public long Vec3ToLoc(Vector3 vec, byte depth)
    {
        long m = 0;
        m |= Party1By2(vec.z) | Party1By2(vec.y) << 1 | Party1By2(vec.x) << 2;
        return (m | Convert.ToInt32(Math.Pow(8, depth)));
    }

    //TODO: Documentation: Party1By2(long)
    private long Party1By2(long n)
    {
        n &= 0x000003ff;
        n = (n | n << 16) & 0x1f0000ff0000ff;
        n = (n | n << 8) & 0x100f00f00f00f00f;
        n = (n | n << 4) & 0x10c30c30c30c30c3;
        return (n | n << 2) & 0x1249249249249249;
    }

    public long Party1By2(float a)
    {
        return Party1By2(Convert.ToInt32(a));
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
        return (n ^ (n >> 16)) & 0x1f00000000ffff;
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
    /// <returns>Code for Adjacent code as a long.</returns>
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
            if (distance >= 0)
            {
                // TODO: Optimization: Possible optimization in injecting (n + distance) value back into m
                m = (m & (axismask[axis])) | (Party1By2(n + distance) << axis);
            } else
            {
                m = (m & (axismask[axis])) ^ (Party1By2(n + distance) << axis);
            }
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
        Vector3 mvec = LocToVec3(m) + offset;
        if (WithinOctreeCheckVec3(mvec, depth))
        {
            m = Vec3ToLoc(mvec, depth);
        }
        return m;
    }

    //Parent Search
    //Calculate Parent
    //TODO: Documentation: CalculateParent()
    public long CalculateParent(long m)
    {
        return m = (m >> 3);
    }

    //Parents Search
    //TODO: Documentation: CollectParents()
    public Array CollectParents(long m)
    {
        byte depth = CalculateDepth(m);
        List<long> parentlist = new List<long>();
        for (int i = 1; i < depth; ++i)
        {
            parentlist.Add(m >> 3 * i);
        }
        Array array = parentlist.ToArray();
        return array;
    }

    //Child Search

   public Array CollectChildrenAll(long m)
    {
        m = (m << 3);
        long[] children = {m, (m | 1), (m | 2), (m | 3), (m | 4), (m | 5), (m | 6), (m | 7) };
        return children;
    }

    //public Array CollectChildrenSide(long m, byte axis, bool positive)
    //{

    //}

    //TODO: Documentation: CalculateChildSpecific()
    //TODO: UnitTest: CalculateChildSpecific()
    public long CalculateChildSpecific(long m, byte c)
    {
        if (c > 7)
        {
            throw new ArgumentException(String.Format("{0} is not valued between 0 and 7", c),
                                      "c");
        }
        else
        {
            return ((m << 3) | c);
        }
    }

    //--Property Methods--//
    //Methods that provide/reveal information from the code
    
    //Caluclate Depth
    //TODO: Documentation: CalculateDepth()
    public byte CalculateDepth(long m)
    {
        return Convert.ToByte(Math.Log(m, 8));
    }
    
    //Calculate Child Check
    //TODO: Documentation: CalculateChildCheck()

    public long CalculateChildCheck(long m)
    {
        return m = (m << 3);
    }

    //--Location Code Helpers--//

    //Check if Within Octree

    //TODO: Documentation: WithinOctreeInt()
    private bool WithinOctreeCheckInt(long n, byte depth)
    {
        if (0 <= n && n < Math.Pow(2, depth))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //TODO: Documentation: WithinOctreeVec3()
    private bool WithinOctreeCheckVec3(Vector3 vec, byte depth)
    {
        double upperlimit = Math.Pow(2, depth);
        if (new[] { vec.x, vec.y, vec.z }.All(x => (0 <= x && x < upperlimit)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public long CalculateOppositEdge(long n)
    {
        long[] axismask = { 0x6DB6DB6DB6DB6DB6, 0x5B6DB6DB6DB6DB6D, 0x36DB6DB6DB6DB6DB };
        int depthmodifier = (Convert.ToInt32(Math.Pow(8, Convert.ToInt32(Math.Log(n, 8)))));
        n = (n ^ depthmodifier); //Removes Depth Modifier at far right
        //n = n & (axismask[axis]); //Modifies 
        n = (n | depthmodifier);
        return n;
    }
}

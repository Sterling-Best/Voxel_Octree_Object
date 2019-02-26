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
    public ushort Vec3ToLoc(Vector3Int vec, byte depth)
    {
        int m = 0;
        m |= Party1By2(vec.z) | Party1By2(vec.y) << 1 | Party1By2(vec.x) << 2;
        return (ushort)(m | (int)(Math.Pow(8, depth)));
    }

    //TODO: Documentation: Party1By2(long)
    private int Party1By2(int n)
    {
        n &= 0x3ff;
        n = (n | n << 16) & 0xff;
        n = (n | n << 8) & 0xf00f00f;
        n = (n | n << 4) & 0x30c30c3;
        return (n | n << 2) & 0x9249249;
    }

    private long Party1By2long(long n)
    {
        n &= 0x000003ff;
        n = (n | n << 16) & 0x1f0000ff0000ff;
        n = (n | n << 8) & 0x100f00f00f00f00f;
        n = (n | n << 4) & 0x10c30c30c30c30c3;
        return (n | n << 2) & 0x1249249249249249;
    }

    public long Party1By2float(float a)
    {
        return Party1By2(Convert.ToInt32(a));
    }

    //Decoding Morton Code

    //TODO: Finish LocToVec3() Documentation
    public Vector3Int LocToVec3(int m)
    {
        int depthmodifier = (Convert.ToInt32(Math.Pow(8, Convert.ToInt32(Math.Log(m, 8)))));
        m = (m ^ depthmodifier);
        return new Vector3Int(Collapseby2(m >> 2), Collapseby2(m >> 1), Collapseby2(m));
    }

    //TODO: Finish collapseby2() Documentation
    private int Collapseby2(int n)
    {
        n &= 0x49249249;
        n = (n ^ (n >> 2)) & 0x30c30c3;
        n = (n ^ (n >> 4)) & 0xf00f00f;
        n = (n ^ (n >> 8)) & 0xff;
        return (n ^ (n >> 16)) & 0xffff;
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
    public ushort CalculateAdjacent(int m, byte axis, int distance)
    {
        byte depth = CalculateDepth(m);
        int depthmodifier = (Convert.ToInt32(Math.Pow(8, depth)));
        int[] axismask = { 0x6DB6DB6, 0x6DB6DB6D, 0xB6DB6DB };
        m = (m ^ depthmodifier);
        int n = Collapseby2(m >> axis);
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
        return (ushort)m;
    }

    //Offset Search
    //TODO: Documentation: CalculateOffset() 
    //TODO: UnitTests: CalculateOffest() 
    public ushort CalculateOffset(int m, Vector3Int offset)
    {
        //TODO: Optimization: Look into similar set-up to CalculateAdjacent() to reduce operations (unpacking/packing).
        byte depth = CalculateDepth(m);
        Vector3Int mvec = LocToVec3(m) + offset;
        if (WithinOctreeCheckVec3(mvec, depth))
        {
            m = Vec3ToLoc(mvec, depth);
        }
        return (ushort)m;
    }

    //Parent Search
    //Calculate Parent
    //TODO: Documentation: CalculateParent()
    public ushort CalculateParent(ushort m)
    {
        return m = (ushort)(m >> 3);
    }

    //Parents Search
    //TODO: Documentation: CollectParents()
    public Array CollectParents(ushort m)
    {
        byte depth = CalculateDepth(m);
        List<ushort> parentlist = new List<ushort>();
        for (int i = 1; i < depth; ++i)
        {
            parentlist.Add((ushort)(m >> 3 * i));
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Location Codes are a compressed form of 3D octant (voxels), based on a modified version of Morton Codes. 
/// 
/// Octree
/// ------------
/// Octree is a datastructure where each parent node must have 8 children.
/// When dealing with 3D space an octree is used to split each regions into 8 Octants, of equal size. 
/// 
/// Morton Codes
/// ------------
/// Using Octree format every 3 bit indicates positional region at a certain depth.  
/// Each bit represents a binary shift on the given axis. First bit (from left to right) is X axis, second is Y axis, and Third is Z axis. 
/// 
/// A binary Shift along the xaxis for morton codes  would look like this
/// 
/// --------------
/// | 000 |  100 |
/// |  0  |   4  |
/// --------------
/// 
/// Bit Code 000 = Morton Code 0 = Bottom Corner closest to origin
/// Bit Code 111 = Morton Code 7 = Upper Corner farthest from origin
/// 
/// 0 (x axis) + 0 (y axis) + 0 (z axis) = 0
/// 0 (x axis) + 1 (y axis) + 0 (z axis) = 2
/// 1 (x axis) + 0 (y axis) + 0 (z axis) = 4 
/// 1 (x axis) + 1 (y axis) + 0 (z axis) = 6
/// 1 (x axis) + 1 (y axis) + 1 (z axis) = 7
/// 
/// Note: Examples illustrated will be a quadtree or 2D slice of an octree for simplicity.
/// Top Binary,
/// 
/// --------------
/// | 010 |  110 |
/// |  2  |   6  |
/// |------------|
/// | 000 |  100 |
/// |  0  |   4  |
/// --------------
/// 
/// Adding the 3-bit codes together specifies a octant within one of the larger octants.
/// Bit Code (Depth 1) 000 + Depth (2) 010 = Morton Code 000010 -> 2
/// 
/// -------------------------------------
/// | 010010 | 010110 | 110010 | 110110 |
/// |   18   |   22   |   50   |   54   |
/// |-----------------|-----------------|
/// | 010000 | 010100 | 110000 | 110100 |
/// |   16   |   20   |   48   |   52   |
/// |-----------------------------------|
/// | 000010 | 000110 | 100010 | 100110 |
/// |    2   |    6   |   34   |   38   |
/// |-----------------|-----------------|
/// | 000000 | 000100 | 100000 | 100100 |
/// |    0   |    4   |   32   |   36   |
/// -------------------------------------
///
/// Then having codes for various sized octants within the same octree
/// 
/// -------------------------------------
/// | 010010 | 010110 |                 |
/// |   18   |   22   |      110        |
/// |-----------------|       6         |
/// | 010000 | 010100 |                 |
/// |   16   |   20   |                 |
/// |-----------------------------------|
/// |                 | 100010 | 100110 |
/// |       000       |   34   |   38   |
/// |        0        |-----------------|
/// |                 | 100000 | 100100 |
/// |                 |   32   |   36   |
/// -------------------------------------
/// 
/// However this does create some conflict when dealing with an octree with nodes of various sizes.
/// 000 and 000000 both being 0 even though they are technically different regions. 
/// So adding an extra bit indicating depth/size is required
///
/// Depth Value Bits
/// -----------------
/// 
/// Added on to the End of the Morton Code, 
/// modifying values to differentiate nodes of difference size within similar spaces that
/// have similar location code values.
/// Issues: 000 and 000000 both have a location code of 0 even though the represent codes of different sizes.
///
/// Depth 1 + Morton Code 000 = Bit Code 1000 = 8
/// 
/// -----------------------------------------
/// | 1010010 | 1010110 |                   |
/// |   82    |   86    |      1110         |
/// |-------------------|       14          | 
/// | 1010000 | 1010100 |                   |
/// |   80    |   84    |                   |
/// |---------------------------------------|
/// |                   | 1100010 | 1100110 |
/// |      1000         |   98    |   102   |
/// |        8          |-------------------|
/// |                   | 1100000 | 1100100 |
/// |                   |    96   |   100   |
/// -----------------------------------------
/// 
/// Codes
/// -----------------
/// 
/// --ushort Supported--
/// Total Codes: 37,449 -- Depths: 0-5
/// Codes 1 -> Depth 0
/// Codes 8 to 15 -> Depth 1
/// Codes 64 to 127 -> Depth 2
/// Codes 512 to 1,023 -> Depth 3
/// Codes 4,096 to 8,191 -> Depth 4
/// Codes 32,768 to 65,535 -> Depth 5
/// --int or uint supported-- (includes ushort code values)
/// Total Codes: 1,227,096,064 -- Depths: 0-10
/// Codes 262,144 to 524,287 -> Depth 6
/// Codes 2,097,152 to 4,194,303 -> Depth 7
/// Codes 16,777,216 to 33,554,431 -> Depth 8
/// Codes 134,217,728 to 268,435,455 -> Depth 9
/// Codes 1,073,741,824 to 2,147,483,647 -> Depth 10
/// --long supported-- (includes ushort and int code values)
/// Codes ‭8,589,934,592‬ to 17,179,869,183 -> Detph 11
/// etc....
/// 
/// </summary>
public class OT_LocCode
{
    //--Encoding/Decoding--//

    //Encoding Location Code (Morton Code)

    //TODO: Documentation: Vec3ToLoc
    //TODO: Allow for depths above 7 to be used.

    
    /// <param name="vec"></param>
    /// <param name="depth"></param>
    /// <returns>Returns the 16-bit Location code, dirived from Vector 3 coordinates</returns>
    /// <remarks>
    /// 
    /// </remarks>
    public ushort Vec3ToLoc(Vector3Int vec, byte depth)
    {
        int m = Party1By2(vec.z) | Party1By2(vec.y) << 1 | Party1By2(vec.x) << 2;
        return (ushort)(m | (int)(Math.Pow(8, depth)));
    }

    private int Party1By2(int n)
    {
        n &= 0x3ff;
        n = (n | n << 16) & 0xff;
        n = (n | n << 8) & 0xf00f00f;
        n = (n | n << 4) & 0x30c30c3;
        return (n | n << 2) & 0x9249249;
    }

    //TODO: Documentation: Party1By2long(long)
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
        return false;
    }

    //TODO: Documentation: WithinOctreeVec3()
    private bool WithinOctreeCheckVec3(Vector3 vec, byte depth)
    {
        double upperlimit = Math.Pow(2, depth);
        if (new[] { vec.x, vec.y, vec.z }.All(x => (0 <= x && x < upperlimit)))
        {
            return true;
        }
        return false;
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

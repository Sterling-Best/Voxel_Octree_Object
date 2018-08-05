using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System;

public class Octree_LocationCode_Tests{

    //VecToLoc() TESTS
    [Test]
    //Vec3ToLoc_Test1 - Manual Depth Test
    //TODO: Finish Vec3toLoc() to work with depths 7 or higher.
    public void Vec3ToLoc_Test1_DepthManual()
    {
        Octree_LocationCode lc = new Octree_LocationCode();
        long[] TestVec = { lc.Vec3ToLoc(new Vector3(0,0,0), 0), lc.Vec3ToLoc(new Vector3(0, 0, 0), 1),
            lc.Vec3ToLoc(new Vector3(0, 0, 0), 2), lc.Vec3ToLoc(new Vector3(0, 0, 0), 3),
            lc.Vec3ToLoc(new Vector3(0, 0, 0), 4), lc.Vec3ToLoc(new Vector3(0, 0, 0), 5),
            lc.Vec3ToLoc(new Vector3(0, 0, 0), 6)};
        long[] TestAssert = {0, 8, 64, 512, 4096, 32768, 262144};
        CollectionAssert.AreEqual(TestVec, TestAssert);
    }

    [Test]
    //Vec3ToLoc_Test2 - Random Depth Test
    //TODO: Finish Vec3toLoc() to work with depths 7 or higher.
    public void Vec3ToLoc_Test2_DepthRandom()
    {
        Octree_LocationCode lc = new Octree_LocationCode();
        System.Random rnd = new System.Random();
        for (int i = 0; i < 16; i++) {
            int d = rnd.Next(0,7);
            long n = lc.Vec3ToLoc(new Vector3(0, 0, 0), d);
            if (d > 0)
            {
                Assert.AreEqual(n, Convert.ToInt32(Mathf.Pow(8, d)));
            } else { Assert.AreEqual(n, 0); }
        }
    }

    [Test]
    //Vec3ToLocTest3 - Manual Code Test
    public void Vec3ToLoc_Test3_CodeManual()
    {
        Octree_LocationCode lc = new Octree_LocationCode();
        long[] TestVec = { lc.Vec3ToLoc(new Vector3(0,0,0), 1), lc.Vec3ToLoc(new Vector3(1, 1, 1), 1),
            lc.Vec3ToLoc(new Vector3(0, 0, 0), 2), lc.Vec3ToLoc(new Vector3(3, 3, 3), 2),
            lc.Vec3ToLoc(new Vector3(4, 4, 4), 3), lc.Vec3ToLoc(new Vector3(1, 0, 0), 1),
            lc.Vec3ToLoc(new Vector3(1, 0, 0), 2), lc.Vec3ToLoc(new Vector3(5, 6, 2), 3), lc.Vec3ToLoc(new Vector3(15, 15, 15), 4)};
        long[] TestAssert = { 8, 15, 64, 127, 960, 12, 68, 924, 8191 };
        CollectionAssert.AreEqual(TestAssert, TestVec);
    }

    [Test]
    //TODO: Create Vec3ToLoc_Test4_CodeRandom()
    public void Vec3ToLoc_Test4_CodeRandom()
    {

    }

    [Test]
    public void Vec3ToLoc_Test5_AssertVecAndDepthManual()
    {
        Octree_LocationCode lc = new Octree_LocationCode();
        long m;
        Assert.Throws<ArgumentException>( () => m = lc.Vec3ToLoc(new Vector3(4, 4, 4), 2));
    }

    [Test]
    //TODO: Update Vec3ToLoc() for depths larger than 6.
    public void Vec3ToLoc_Test5_CodeRangesRandom()
    {
        Octree_LocationCode lc = new Octree_LocationCode();
        System.Random rnd = new System.Random();
        for (int i = 0; i < 4681; i++)
        {
            int d = rnd.Next(1, 6);
            int coordlimit = Convert.ToInt32(Math.Pow(2, d));
            int valuelimit = Convert.ToInt32(Math.Pow(8, d));
            Vector3 vec = new Vector3(rnd.Next(0, coordlimit),
                rnd.Next(0, coordlimit), rnd.Next(0, coordlimit));
            long n = lc.Vec3ToLoc(vec, d);
            Assert.IsTrue(valuelimit <= n && n < valuelimit*2);
        }
    }

    //LocToVec3 - Tests

    //TODO; Create LocToVec3_Test1_VectorsManual
    
    [Test]
    public void LocToVec3_Test2_VectorsRandom()
    {
        Octree_LocationCode lc = new Octree_LocationCode();
        System.Random rnd = new System.Random();
        for (int i = 0; i < 4681; i++)
        {
            int d = rnd.Next(1, 6);
            int coordlimit = Convert.ToInt32(Math.Pow(2, d));
            int valuelimit = Convert.ToInt32(Math.Pow(8, d));
            Vector3 vec = new Vector3(rnd.Next(0, coordlimit),
                rnd.Next(0, coordlimit), rnd.Next(0, coordlimit));
            long n = lc.Vec3ToLoc(vec, d);
            Assert.AreEqual(vec, lc.LocToVec3(n));
        }
    }


    //CalculateAdjacent() - Tests

    [Test]
    public void CalculateAdjacent_Test2_Offset1Random()
    {
        {
            Octree_LocationCode lc = new Octree_LocationCode();
            System.Random rnd = new System.Random();
            for (int i = 0; i < 4681; i++)
            {
                int d = rnd.Next(1, 6);
                int coordlimit = Convert.ToInt32(Math.Pow(2, d));
                int valuelimit = Convert.ToInt32(Math.Pow(8, d));
                Vector3 vec = new Vector3(rnd.Next(0, coordlimit -1),
                    rnd.Next(0, coordlimit -1), rnd.Next(0, coordlimit -1));
                long n = lc.Vec3ToLoc(vec, d);
                Vector3 vec1 = (new Vector3(1, 0, 0) + vec);
                long n1 = lc.Vec3ToLoc(vec1, d);
                long a = lc.CalculateAdjacent(n, 2, 1);
                Assert.IsTrue(n1 == a || a == n);
                vec1 = (new Vector3(-1, 0, 0) + vec);
                n1 = lc.Vec3ToLoc(vec1, d);
                a = lc.CalculateAdjacent(n, 2, -1);
                Assert.IsTrue(n1 == a || a == n);
                vec1 = (new Vector3(0, 1, 0) + vec);
                n1 = lc.Vec3ToLoc(vec1, d);
                a = lc.CalculateAdjacent(n, 1, 1);
                Assert.IsTrue(n1 == a || a == n); ;
                vec1 = (new Vector3(0, -1, 0) + vec);
                n1 = lc.Vec3ToLoc(vec1, d);
                a = lc.CalculateAdjacent(n, 1, -1);
                Assert.IsTrue(n1 == a || a == n);
                vec1 = (new Vector3(0, 0, 1) + vec);
                n1 = lc.Vec3ToLoc(vec1, d);
                a = lc.CalculateAdjacent(n, 0, 1);
                Assert.IsTrue(n1 == a || a == n);
                vec1 = (new Vector3(0, 0, -1) + vec);
                n1 = lc.Vec3ToLoc(vec1, d);
                a = lc.CalculateAdjacent(n, 0, -1);
                Assert.IsTrue(n1 == a || a == n);
            }
        }
    }
    
}

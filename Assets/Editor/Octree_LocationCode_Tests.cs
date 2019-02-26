//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using NUnit.Framework;
//using System;

//public class Octree_LocationCode_Tests{

//    //VecToLoc() TESTS
//    [Test]
//    //Vec3ToLoc_Test1 - Manual Depth Test
//    //TODO: Finish Vec3toLoc() to work with depths 7 or higher.
//    public void Vec3ToLoc_Test1_DepthManual()
//    {
//        OT_LocCode lc = new OT_LocCode();
//        long[] TestVec = { lc.Vec3ToLoc(new Vector3(0,0,0), 0), lc.Vec3ToLoc(new Vector3(0, 0, 0), 1),
//            lc.Vec3ToLoc(new Vector3(0, 0, 0), 2), lc.Vec3ToLoc(new Vector3(0, 0, 0), 3),
//            lc.Vec3ToLoc(new Vector3(0, 0, 0), 4), lc.Vec3ToLoc(new Vector3(0, 0, 0), 5),
//            lc.Vec3ToLoc(new Vector3(0, 0, 0), 6)};
//        long[] TestAssert = {1, 8, 64, 512, 4096, 32768, 262144};
//        CollectionAssert.AreEqual(TestVec, TestAssert);
//    }

//    [Test]
//    //Vec3ToLoc_Test2 - Random Depth Test
//    //TODO: Finish Vec3toLoc() to work with depths 7 or higher.
//    public void Vec3ToLoc_Test2_DepthRandom()
//    {
//        OT_LocCode lc = new OT_LocCode();
//        System.Random rnd = new System.Random();
//        for (int i = 0; i < 16; i++) {
//            byte d = Convert.ToByte(rnd.Next(1, 6));
//            long n = lc.Vec3ToLoc(new Vector3(0, 0, 0), d);
//            if (d > 0)
//            {
//                Assert.AreEqual(n, Convert.ToInt32(Mathf.Pow(8, d)));
//            } else { Assert.AreEqual(n, 0); }
//        }
//    }

//    [Test]
//    //Vec3ToLocTest3 - Manual Code Test
//    public void Vec3ToLoc_Test3_CodeManual()
//    {
//        OT_LocCode lc = new OT_LocCode();
//        long[] TestVec = { lc.Vec3ToLoc(new Vector3(0,0,0), 1), lc.Vec3ToLoc(new Vector3(1, 1, 1), 1),
//            lc.Vec3ToLoc(new Vector3(0, 0, 0), 2), lc.Vec3ToLoc(new Vector3(3, 3, 3), 2),
//            lc.Vec3ToLoc(new Vector3(4, 4, 4), 3), lc.Vec3ToLoc(new Vector3(1, 0, 0), 1),
//            lc.Vec3ToLoc(new Vector3(1, 0, 0), 2), lc.Vec3ToLoc(new Vector3(5, 6, 2), 3), lc.Vec3ToLoc(new Vector3(15, 15, 15), 4)};
//        long[] TestAssert = { 8, 15, 64, 127, 960, 12, 68, 924, 8191 };
//        CollectionAssert.AreEqual(TestAssert, TestVec);
//    }

//    [Test]
//    //TODO: Update Vec3ToLoc() for depths larger than 6.
//    public void Vec3ToLoc_Test5_CodeRangesRandom()
//    {
//        OT_LocCode lc = new OT_LocCode();
//        System.Random rnd = new System.Random();
//        for (int i = 0; i < 4681; i++)
//        {
//            byte d = Convert.ToByte(rnd.Next(1, 6));
//            int coordlimit = Convert.ToInt32(Math.Pow(2, d));
//            int valuelimit = Convert.ToInt32(Math.Pow(8, d));
//            Vector3 vec = new Vector3(rnd.Next(0, coordlimit),
//                rnd.Next(0, coordlimit), rnd.Next(0, coordlimit));
//            long n = lc.Vec3ToLoc(vec, d);
//            Assert.IsTrue(valuelimit <= n && n < valuelimit*2);
//        }
//    }

//    //LocToVec3 - Tests

//    //TODO; Create LocToVec3_Test1_VectorsManual
    
//    [Test]
//    public void LocToVec3_Test2_VectorsRandom()
//    {
//        OT_LocCode lc = new OT_LocCode();
//        System.Random rnd = new System.Random();
//        for (int i = 0; i < 4681; i++)
//        {
//            byte d = Convert.ToByte(rnd.Next(1, 6));
//            int coordlimit = Convert.ToInt32(Math.Pow(2, d));
//            int valuelimit = Convert.ToInt32(Math.Pow(8, d));
//            Vector3 vec = new Vector3(rnd.Next(0, coordlimit),
//                rnd.Next(0, coordlimit), rnd.Next(0, coordlimit));
//            long n = lc.Vec3ToLoc(vec, d);
//            Assert.AreEqual(vec, lc.LocToVec3(n));
//        }
//    }


//    //CalculateAdjacent() - Tests
//    //TODO: CalculateAdjacent(), Manual use more examples
//    [Test]
//    public void CalculateAdjacent_Test1_Offset1Manual()
//    {
//        OT_LocCode lc = new OT_LocCode();
//        long[] Testcodes = { lc.CalculateAdjacent(12,2,-1), lc.CalculateAdjacent(15,1,-1)};
//        long[] TestAssert = {8,13};
//        CollectionAssert.AreEqual(TestAssert, Testcodes);
//    }

//    [Test]
//    //TODO: CalculateAdjacent Offset Random, do negative values
//    public void CalculateAdjacent_Test2_Offset1Random()
//    {
//        {
//            OT_LocCode lc = new OT_LocCode();
//            System.Random rnd = new System.Random();
//            for (int i = 0; i < 4681; i++)
//            {
//                byte d = Convert.ToByte(rnd.Next(1, 6));
//                int coordlimit = Convert.ToInt32(Math.Pow(2, d));
//                int valuelimit = Convert.ToInt32(Math.Pow(8, d));
//                Vector3 vec = new Vector3(rnd.Next(0, coordlimit -1),
//                    rnd.Next(0, coordlimit -1), rnd.Next(0, coordlimit -1));
//                long n = lc.Vec3ToLoc(vec, d);
//                Vector3 vec1 = (new Vector3(1, 0, 0) + vec);
//                long n1 = lc.Vec3ToLoc(vec1, d);
//                long a = lc.CalculateAdjacent(n, 2, 1);
//                Assert.IsTrue(n1 == a || a == n);
//                vec1 = (new Vector3(-1, 0, 0) + vec);
//                n1 = lc.Vec3ToLoc(vec1, d);
//                a = lc.CalculateAdjacent(n, 2, -1);
//                Assert.IsTrue(n1 == a || a == n);
//                vec1 = (new Vector3(0, 1, 0) + vec);
//                n1 = lc.Vec3ToLoc(vec1, d);
//                a = lc.CalculateAdjacent(n, 1, 1);
//                Assert.IsTrue(n1 == a || a == n); ;
//                vec1 = (new Vector3(0, -1, 0) + vec);
//                n1 = lc.Vec3ToLoc(vec1, d);
//                a = lc.CalculateAdjacent(n, 1, -1);
//                Assert.IsTrue(n1 == a || a == n);
//                vec1 = (new Vector3(0, 0, 1) + vec);
//                n1 = lc.Vec3ToLoc(vec1, d);
//                a = lc.CalculateAdjacent(n, 0, 1);
//                Assert.IsTrue(n1 == a || a == n);
//                vec1 = (new Vector3(0, 0, -1) + vec);
//                n1 = lc.Vec3ToLoc(vec1, d);
//                a = lc.CalculateAdjacent(n, 0, -1);
//                Assert.IsTrue(n1 == a || a == n);
//            }
//        }
//    }

//    //CollectParents()

//    [Test]
//    public void CollectParents_Test1_Manual()
//    {
//        OT_LocCode lc = new OT_LocCode();
//        Array parents1 = lc.CollectParents(64);
//        long[] check1 = {8};
//        CollectionAssert.AreEqual(check1, parents1);
//    }

//    [Test]
//    public void CollectParents_Test2_Manual()
//    {
//        OT_LocCode lc = new OT_LocCode();
//        Array parents2 = lc.CollectParents(514);
//        long[] check2 = { 64, 8 };
//        CollectionAssert.AreEqual(check2, parents2);
//    }

//    [Test]
//    public void CollectParents_Test3_Manual()
//    {
//        OT_LocCode lc = new OT_LocCode();
//        Array parents3 = lc.CollectParents(5394);
//        long[] check3 = { 674, 84, 10 };
//        CollectionAssert.AreEqual(check3, parents3);
//    }

//    [Test]
//    public void CollectParents_Test4_Manual()
//    {
//        OT_LocCode lc = new OT_LocCode();
//        Array parents4 = lc.CollectParents(60353);
//        long[] check4 = { 7544, 943, 117, 14 };
//        CollectionAssert.AreEqual(check4, parents4);
//    }

//    //CalculateDepth()
//    [Test]
//    public void CalculateDepth_Test1_Manual()
//    {
//        OT_LocCode lc = new OT_LocCode();
//        long[] testdepths = {lc.CalculateDepth(8), lc.CalculateDepth(15),lc.CalculateDepth(12),
//            lc.CalculateDepth(64), lc.CalculateDepth(127), lc.CalculateDepth(93),
//            lc.CalculateDepth(512), lc.CalculateDepth(1023), lc.CalculateDepth(856),
//            lc.CalculateDepth(4096), lc.CalculateDepth(8191), lc.CalculateDepth(5237),
//            lc.CalculateDepth(32768), lc.CalculateDepth(65535), lc.CalculateDepth(60353) };
//        long[] checkdepths = { 1, 1, 1,
//            2, 2, 2,
//            3, 3, 3,
//            4, 4, 4,
//            5, 5, 5 };
//        CollectionAssert.AreEqual(checkdepths, testdepths);
//    }

//    //CalculateParent()
//    [Test]
//    public void CalculateParent_Test1_Manual()
//    {
//        OT_LocCode lc = new OT_LocCode();
//        long[] testdepths = {lc.CalculateParent(8), lc.CalculateParent(15),lc.CalculateParent(12),
//            lc.CalculateParent(64), lc.CalculateParent(127), lc.CalculateParent(93),
//            lc.CalculateParent(512), lc.CalculateParent(1023), lc.CalculateParent(856),
//            lc.CalculateParent(4096), lc.CalculateParent(8191), lc.CalculateParent(5237),
//            lc.CalculateParent(32768), lc.CalculateParent(65535), lc.CalculateParent(60353) };
//        long[] checkdepths = { 1, 1, 1,
//            8, 15, 11,
//            64, 127, 107,
//            512, 1023, 654,
//            4096, 8191, 7544 };
//        CollectionAssert.AreEqual(checkdepths, testdepths);
//    }

//    //CalculateChildCheck()
//    [Test]
//    public void CalculateChildCheck_Test1_Manual()
//    {
//        OT_LocCode lc = new OT_LocCode();
//        long[] testcodes = {lc.CalculateChildCheck(8), lc.CalculateChildCheck(15),lc.CalculateChildCheck(12),
//            lc.CalculateChildCheck(64), lc.CalculateChildCheck(127), lc.CalculateChildCheck(93),
//            lc.CalculateChildCheck(512), lc.CalculateChildCheck(1023), lc.CalculateChildCheck(856),
//            lc.CalculateChildCheck(4096), lc.CalculateChildCheck(8191), lc.CalculateChildCheck(5237)};
//        long[] checkchild = { 64, 120, 96,
//            512, 1016, 744,
//            4096, 8184, 6848,
//            32768, 65528, 41896};
//        CollectionAssert.AreEqual(checkchild, testcodes);
//    }

//    //CalculateChildSpecific()
//    [Test]
//    public void CalculateChildSpecific_Test1_Manual()
//    {
//        OT_LocCode lc = new OT_LocCode();
//        long[] testcodes = {lc.CalculateChildSpecific(1,0), lc.CalculateChildSpecific(1,1), lc.CalculateChildSpecific(1, 2),
//            lc.CalculateChildSpecific(1,3), lc.CalculateChildSpecific(1,4), lc.CalculateChildSpecific(1,5),
//            lc.CalculateChildSpecific(1,6), lc.CalculateChildSpecific(1,7),};
//        long[] checkchild = { 8, 9, 10, 11, 12, 13, 14, 15 };
//        CollectionAssert.AreEqual(checkchild, testcodes);
//    }

//    [Test]
//    public void CalculateChildSpecific_Test2_ExceptionOutofBounds()
//    {
//        OT_LocCode lc = new OT_LocCode();
//        long m;
//        Assert.Throws<ArgumentException>(() => m = lc.CalculateChildSpecific(1,8));
//    }

//    [Test]
//    public void CalculateOppositeEdge_Test1()
//    {
//        OT_LocCode lc = new OT_LocCode();
//        long[] Testcodes = { lc.CalculateOppositEdge(127), lc.CalculateOppositEdge(100) };
//        long[] TestAssert = {91, 64};
//        CollectionAssert.AreEqual(TestAssert, Testcodes);
//    }


//}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System;

public class Octree_Controller_Tests : MonoBehaviour {

    //Class target = new Class();
    //var obj = new PrivateObject(target);
    //var retVal = obj.Invoke("PrivateMethod");
    //Assert.AreEqual(retVal);

    [Test]
    public void ChildrenSideTransparencyCheck()
    {
        Octree_Controller oc = new Octree_Controller();

        oc.octree.Add(64, 3);
        oc.octree.Add(65, 3);
        oc.octree.Add(66, 1);
        oc.octree.Add(536, 1);
        oc.octree.Add(537, 1);
        oc.octree.Add(538, 2);
        oc.octree.Add(539, 2);
        oc.octree.Add(540, 1);
        oc.octree.Add(541, 1);
        oc.octree.Add(542, 2);
        oc.octree.Add(543, 0);
        oc.octree.Add(68, 3);
        oc.octree.Add(69, 3);
        oc.octree.Add(70, 1);
        oc.octree.Add(71, 1);
        oc.octree.Add(9, 0);
        oc.octree.Add(10, 0);
        oc.octree.Add(11, 0);
        oc.octree.Add(12, 0);
        oc.octree.Add(13, 0);
        oc.octree.Add(14, 0);
        oc.octree.Add(15, 0);

        //Assert.AreEqual (oc.Chi)



    }
}

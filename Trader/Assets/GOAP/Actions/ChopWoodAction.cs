using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopWoodAction : GoapAction {

    private bool chopped = false;
    //private TreeController targetTree;

    private float startTime = 0;
    public float choppingDuration = 5; //seconds

    public ChopWoodAction()
    {
        addPrecondition("hasTool", true); //we need a tool to do this
        addPrecondition("felledTree", true);

        addEffect("hasLogs", true);
    }

    public override void reset()
    {
        chopped = false;
        //targetTree = null;
        startTime = 0;
    }

    public override bool isDone()
    {
        return chopped;
    }

    public override bool requiresInRange()
    {
        return true; //need to be in range of tree to chop it
    }
    /// <summary>
    /// Find the nearest tree to chop
    /// </summary>
    /// <param name="agent"></param>
    /// <returns></returns>
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        return false;
        //TreeController[] trees = FindObjectsOfType(typeof(TreeController)) as TreeController[];
        //TreeController closest = null;
        //float closestDistance = 0;
        
        //foreach( TreeController tree in trees)
        //{
        //    if (tree.model.treeState == TreeState.Fell)
        //    {
        //        float dist = (tree.transform.position - agent.transform.position).magnitude;

        //        if (closest == null)
        //        {
        //            closest = tree;
        //            closestDistance = dist;
        //        }
        //        else if (dist < closestDistance)
        //        {
        //            closest = tree;
        //            closestDistance = dist;
        //        }
        //    } 
        //}

        //targetTree = closest;
        //if (closest != null)
        //target = targetTree.gameObject;

        //return closest != null;
    }

    public override bool perform(GameObject agent)
    {
        if (startTime == 0)
        {
            //startTime = MyGameManager.instance.world.date.time;
        }

        //if (MyGameManager.instance.world.date.time - startTime > choppingDuration)
        //{
        //    //finished chopping tree
        //    BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));           
        //    backpack.numLogs += targetTree.TreeChopped();
        //    chopped = true;
        //    ToolComponent tool = backpack.tool.GetComponent(typeof(ToolComponent)) as ToolComponent;
        //    tool.use(0.34f);
        //    if (tool.destroyed())
        //    {
        //        Destroy(backpack.tool);
        //        backpack.tool = null;
        //    }
        //}
        return true;
    }
}

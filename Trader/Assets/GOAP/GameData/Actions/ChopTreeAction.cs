
using System;
using UnityEngine;

public class ChopTreeAction : GoapAction
{
	private bool chopped = false;
	//private TreeController targetTree; // where we get the logs from
	
	private float startTime = 0;
	public float workDuration = 5; // seconds
	
	public ChopTreeAction () {
		addPrecondition ("hasTool", true); // we need a tool to do this
		addEffect ("felledTree", true);
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
        //TreeController[] trees = FindObjectsOfType(typeof(TreeController)) as TreeController[];
        //TreeController closest = null;
        //float closestDistance = 0;

        //foreach (TreeController tree in trees)
        //{
        //    if (tree.model.treeState == TreeState.Whole)
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
        //    target = targetTree.gameObject;

        //return closest != null;
        return true;
    }

    public override bool perform(GameObject agent)
    {
        if (startTime == 0)
        {
            //startTime = MyGameManager.instance.world.date.time;
        }

        //if (MyGameManager.instance.world.date.time - startTime > workDuration)
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
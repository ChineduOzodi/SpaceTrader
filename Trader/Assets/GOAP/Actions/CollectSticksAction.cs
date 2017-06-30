using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectSticksAction : GoapAction {

    private bool chopped = false;
    private TreeController targetTree;

    private float startTime = 0;
    public float choppingDuration = 15; //seconds

    public CollectSticksAction()
    {
        //addPrecondition("hasLogs", false); //don't need more wood if we already have wood
        addEffect("hasSticks", true);
    }

    public override void reset()
    {
        chopped = false;
        targetTree = null;
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
        TreeController[] trees = FindObjectsOfType(typeof(TreeController)) as TreeController[];
        TreeController closest = null;
        float closestDistance = 0;
        
        foreach( TreeController tree in trees)
        {
            float dist = (tree.transform.position - agent.transform.position).magnitude;

            if (closest == null)
            {
                closest = tree;
                closestDistance = dist;
            }
            else if (dist < closestDistance)
            {
                closest = tree;
                closestDistance = dist;
            }
        }

        targetTree = closest;
        if (closest != null)
        target = targetTree.gameObject;

        return closest != null;
    }

    public override bool perform(GameObject agent)
    {
        if (startTime == 0)
        {
            startTime = MyGameManager.instance.world.date.time;
        }

        if (MyGameManager.instance.world.date.time - startTime > choppingDuration)
        {
            //finished chopping tree
            BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));
            backpack.numLogs += 1;
            chopped = true;
        }
        return true;
    }
}

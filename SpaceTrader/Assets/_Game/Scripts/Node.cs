using CodeControl;
using System;
using System.Collections.Generic;
using UnityEngine;
public class Node : IHeapItem<Node>
{
    public float walkSpeed;
    public Vector3d worldPosition;
    public Coord coord;
    /// <summary>
    /// Used to trace back path
    /// </summary>
    public Node parent;
    public List<Node> neighbors;
    public int index;
    public double distance;
    public float gCost; //cost from node position to the start node
    public float hCost; //cost from the node position to the target node
    int heapIndex;

    public float fCost
    {
        get { return gCost + hCost; }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }
    public Node(int _index, float _walkSpeed, Vector3d _worldPosition)
    {
        index = _index;
        walkSpeed = _walkSpeed;
        worldPosition = _worldPosition;
        coord = new Coord(worldPosition);
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }

    internal static Node NodeFromStar(int index, ModelRefs<SolarModel> stars)
    {
        Node node = new Node(index, 1, (Vector3d) stars[index].solar.referencePosition);
        node.index = index;
        return node;
    }

    public static double GetDistance(Node nodeA, Node nodeB)
    {

        return Vector3d.Distance(nodeA.worldPosition, nodeB.worldPosition);
    }
}
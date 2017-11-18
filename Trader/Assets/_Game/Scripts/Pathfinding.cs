using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using CodeControl;

public class Pathfinding : MonoBehaviour
{

    public bool displayGizmos = false;
    PathRequestManager requestManager;
    GameManager game;
    
    Node[] nodes;
    List<Node> path;

    private int maxLocalSize = 100 * 100;
    private GalaxyManager galaxy;
    // Use this for initialization
    public void Awake()
    {
        game = GameManager.instance;
        galaxy = GetComponent<GalaxyManager>();
        maxLocalSize = game.data.stars.Count;
        nodes = new Node[game.data.stars.Count];
        requestManager = GetComponent<PathRequestManager>();

    }

    internal void StartFindPath(int startIndex, int targetIndex, float maxDistance)
    {
        StartCoroutine(FindPath(startIndex, targetIndex, maxDistance));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position, new Vector3(10, 10));
        if (nodes != null && displayGizmos)
        {
            foreach (Node n in nodes)
            {
                if (n != null)
                {
                    Gizmos.color = new Color(n.fCost / 100f, 1, 1, .8f);
                    Gizmos.DrawCube(CameraController.CameraOffsetGalaxyPosition(n.worldPosition / GameDataModel.galaxyDistanceMultiplication), Vector3.one);
                }


            }
        }
    }
    IEnumerator FindPath(int startIndex, int targetIndex, float maxDistance)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Node[] waypoints = new Node[0];
        bool pathSuccess = false;

        //Save Nodes to Grid
        if (nodes[startIndex] == null)
        {
            nodes[startIndex] = Node.NodeFromStar(startIndex, game.data.stars);
        }
        if (nodes[targetIndex] == null)
        {
            nodes[targetIndex] = Node.NodeFromStar(targetIndex, game.data.stars);
        }

        Node startNode = nodes[startIndex];
        Node targetNode = nodes[targetIndex];

        startNode.coord = new Coord(startNode.worldPosition);
        targetNode.coord = new Coord(targetNode.worldPosition);

        

        if (startNode.walkSpeed > 0 && targetNode.walkSpeed > 0)
        {
            Heap<Node> openSet = new Heap<Node>(maxLocalSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path found:" + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbor in GetNeighbors(currentNode, maxDistance, game.data.stars))
                {

                    if (neighbor.walkSpeed == 0 || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    double newMovementCostToNeighbor = currentNode.gCost + Node.GetDistance(currentNode, neighbor) / (currentNode.walkSpeed);
                    if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = (float) newMovementCostToNeighbor;
                        neighbor.hCost = (float) Node.GetDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                        else
                            openSet.UpdateItem(neighbor);
                    }

                }

            }
        }

        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(nodes[startIndex], nodes[targetIndex]);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Node[] RetracePath(Node startNode, Node endNode)
    {
        path = new List<Node>();
        Node[] waypoints;
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        //waypoints = SimplifyPath(path);
        waypoints = path.ToArray();
        Array.Reverse(waypoints);

        return waypoints;
    }

    Node[] SimplifyPath(List<Node> path)
    {
        List<Node> waypoints = new List<Node>();

        Coord directionOld = Coord.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Coord directionNew = path[i - 1].coord - path[i].coord;

            if (directionNew != directionOld)
            {
                waypoints.Add(path[i]);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    public List<Node> GetNeighbors(Node node, float maxDistance, ModelRefs<SolarModel> stars)
    {
        //Set neighbors

        node.neighbors = new List<Node>(stars.Count);
        for (int i = 0; i < stars.Count; i++)
        {

            if (nodes[i] == null)
            {
                nodes[i] = Node.NodeFromStar(i, stars);
            }
            node.neighbors.Add(nodes[i]);
            node.neighbors[i].distance = Node.GetDistance(node, node.neighbors[i]);
        }
        node.neighbors.Sort((n1, n2) => n1.distance.CompareTo(n2.distance));
        //Get neighbors
        List<Node> neighbors = new List<Node>();
        for (int i = 0; i < node.neighbors.Count; i++)
        {
            if (node.neighbors[i].distance < maxDistance && node.neighbors[i].distance != 0)
            {
                neighbors.Add(node.neighbors[i]);
            }
            else if (node.neighbors[i].distance != 0) break;
        }

        return neighbors;
    }
}

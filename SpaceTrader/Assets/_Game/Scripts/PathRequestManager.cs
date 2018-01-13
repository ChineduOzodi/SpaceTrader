using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PathRequestManager : MonoBehaviour {

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    Pathfinding pathfinding;

    bool isProcessingPath;

    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    public static void RequestPath(int startIndex, int endIndex, float maxDistance, Action<Node[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(startIndex, endIndex, maxDistance, callback);

        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.maxDistance);
        }
    }

    public void FinishedProcessingPath(Node[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public int pathStart;
        public int pathEnd;
        public float maxDistance;
        public Action<Node[], bool> callback;

        public PathRequest(int _start, int _end, float _max, Action<Node[],bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            maxDistance = _max;
            callback = _callback;
        }
    }
}

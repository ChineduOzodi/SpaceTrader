using UnityEngine;
using System.Collections;
using System;

public class Unit : MonoBehaviour {

    private int targetIndex;
    internal int target;
    internal int oldTarget;
    internal int currentIndex;
    internal CreateGalaxy galaxy;
    internal ShipController ship;
    public float speed = 2;
    public float jumpDistance = 100;
    internal LineRenderer line;
    Node[] path;
    

	// Use this for initialization
	void Start () {
        line = GetComponent<LineRenderer>();
        galaxy = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CreateGalaxy>();
        ship = GetComponent<ShipController>();
        target = ship.starIndex;
        oldTarget = ship.starIndex;
    }
    void Update()
    {
        if (oldTarget != target)
        {
            if (target == currentIndex)
                target += 1;
            
            oldTarget = target;
        }
    }
    public void OnPathFound(Node[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            //Setting LineRender vertex positions
            line.numPositions = path.Length + 1;
            line.SetPosition(0, transform.position);
            
            for (int i = 0; i < path.Length; i++)
            {
                line.SetPosition(1 + i, path[i].worldPosition);
            }

            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
        else
        {
            print("no path");
            target = UnityEngine.Random.Range(0, galaxy.starCount);
        }
    }

    IEnumerator FollowPath()
    {
        Node currentWaypoint = path[0];
        targetIndex = 0;
        while (true)
        {
            if (transform.position == currentWaypoint.worldPosition)
            {
                currentIndex = currentWaypoint.index;
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    ship.HyperSpaceDone();
                    yield break;
                }
                currentWaypoint = path[targetIndex];
                
                //update linerenderer
                line.numPositions--;
                line.SetPosition(0, transform.position);

                for (int i = targetIndex; i < path.Length; i++)
                {
                    line.SetPosition(1 + i - targetIndex, path[i].worldPosition);
                }
                yield return new WaitForSeconds(5);
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.worldPosition, speed * Time.deltaTime);
            line.SetPosition(0, transform.position);
            yield return null;
        }
    }

    internal void HyperSpaceTravel(int startIndex, int endIndex, float speed)
    {
        this.speed = speed;
        PathRequestManager.RequestPath(startIndex, endIndex, jumpDistance, OnPathFound);
    }
}

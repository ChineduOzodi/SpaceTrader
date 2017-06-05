using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinatesTest : MonoBehaviour {

	// Use this for initialization
	void Start() {

        SystemPosition position = new SystemPosition(new Vector3[] { Vector3.zero, Vector3.one, Vector3.right});

        print(string.Format("{0} - {1} - {2}",position.location[0],position.location[1],position.location[2]));
        SystemPosition.Subtract(position.location,Vector3.one * 52.1f,1);
        print(string.Format("{0} - {1} - {2}", position.location[0], position.location[1], position.location[2]));
        print(position.ToWorldPosition(0));
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

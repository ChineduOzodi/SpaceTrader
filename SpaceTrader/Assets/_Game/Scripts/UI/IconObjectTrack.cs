using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconObjectTrack : MonoBehaviour {

    internal GameObject target;
    internal Camera cameraTarget;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (target != null)
        {
            var pos = cameraTarget.WorldToScreenPoint(target.transform.position);
            transform.position = pos;
            transform.position = new Vector3(transform.position.x, transform.position.y, 10);
        }
	}

    public void SetTarget(GameObject _target, Camera _camera)
    {
        target = _target;
        cameraTarget = _camera;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaledCamera : MonoBehaviour {

    public Camera mainCamera;
    [ExecuteInEditMode]
    [Range(1,1000)]
    public int scaleFactor = 1000;

    private Camera thisCam;

	// Use this for initialization
	void Start () {
        thisCam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {

        transform.position = mainCamera.transform.position / scaleFactor;
        transform.rotation = mainCamera.transform.rotation;
        transform.localScale = mainCamera.transform.localScale;
        thisCam.fieldOfView = mainCamera.fieldOfView;
	}
}

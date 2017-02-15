using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float camMoveSpeed = 1;
    public float zoomSpeed = 1;


    private Camera mainCam;

	// Use this for initialization
	void Start () {

        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
	
	}
	
	// Update is called once per frame
	void Update () {

        //Camera Controls
        float moveModifier = camMoveSpeed * mainCam.orthographicSize;
        mainCam.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed * moveModifier;

        float transX = Input.GetAxis("Horizontal") * moveModifier * Time.deltaTime / Time.timeScale;
        float transY = Input.GetAxis("Vertical") * moveModifier * Time.deltaTime / Time.timeScale;

        Camera.main.transform.Translate(new Vector3(transX, transY));

    }
}

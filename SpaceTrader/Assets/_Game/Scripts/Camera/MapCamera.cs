using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCamera : MonoBehaviour {
    public GameObject cameraHorizontal;
    public GameObject cameraVertical;
    public Camera mainCamera;

    public float camMoveSpeed = 1;
    public float zoomSpeed = 1;
    public float cameraScaleModPow = .7f;
    public float cameraScaleModMult = .1f;

    public float[] cameraUpperBound = { 500, 5000, 10000, 1000, 5000 }; //Ly, Gm, Mm, km, m
    public float[] cameraLowerBound = { .5f, 1, 1, 5, 5 }; //Ly, Gm, Mm, km, m

    public float dragSpeedVert = 50;
    public float dragSpeedHor = 100;
    private Vector3 dragOrigin;

    internal bool cameraControl = false;

    private Vector3 savedCamPos  = Vector3.zero;
    private Quaternion saveCamRot;

    void Start () {
		
	}

	
	// Update is called once per frame
	void LateUpdate () {
        if (cameraControl)
        {
            //Camera Controls
            Vector3 moveModifier = camMoveSpeed * mainCamera.transform.localPosition;
            var orthTransform = Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed * moveModifier;

            float transX = Input.GetAxis("Horizontal") * moveModifier.magnitude * Time.deltaTime / Time.timeScale;
            float transY = Input.GetAxis("Vertical") * moveModifier.magnitude * Time.deltaTime / Time.timeScale;

            cameraHorizontal.transform.Translate(transX, transY, 0, Space.Self);
            mainCamera.transform.Translate(orthTransform);

            if (Input.GetMouseButtonDown(2))
            {
                dragOrigin = Input.mousePosition;
                return;
            }

            if (!Input.GetMouseButton(2)) return;

            Vector3 pos = mainCamera.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            //Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);
            cameraHorizontal.transform.Rotate(new Vector3(0, 0, pos.x * dragSpeedHor), Space.Self);
            cameraVertical.transform.Rotate(new Vector3(pos.y * dragSpeedVert, 0, 0), Space.Self);
            //transform.Translate(move, Space.World);
            dragOrigin = Input.mousePosition;
        }
        
    }

    public void SetCameraControlTrue()
    {
        cameraControl = true;
        if (savedCamPos != Vector3.zero)
        {
            mainCamera.transform.position = savedCamPos;
            mainCamera.transform.rotation = saveCamRot;
            savedCamPos = Vector3.zero;
        }
        
        //TODO: set camera defaults if appropriate

    }

    public void SetCameraView(Vector3 position, Quaternion rotation, Vector3 localScale, float fieldOfView )
    {
        if (savedCamPos == Vector3.zero)
        {
            savedCamPos = mainCamera.transform.position;
            saveCamRot = mainCamera.transform.rotation;
        }
        cameraControl = false;
        mainCamera.transform.position = position;
        mainCamera.transform.rotation = rotation;
        mainCamera.transform.localScale = localScale;
        mainCamera.fieldOfView = fieldOfView;
    }
}

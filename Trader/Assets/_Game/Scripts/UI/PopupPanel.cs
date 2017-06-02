using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupPanel : MonoBehaviour {

    public GameObject panel;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && panel.activeSelf)
        {
            panel.SetActive(false);
        }
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInfo : MonoBehaviour {

    public int index;
    public InfoPanelController controller;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetPanelToggle(int index, InfoPanelController controller)
    {
        this.index = index;
        this.controller = controller;

        GetComponent<Button>().onClick.AddListener(() => controller.SelectPanel(index));
    }

    public void OnDestroy()
    {
        GetComponent<Button>().onClick.RemoveListener(() => controller.SelectPanel(index));
    }
}

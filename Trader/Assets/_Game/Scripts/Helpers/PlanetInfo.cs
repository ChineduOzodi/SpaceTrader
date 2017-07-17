using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetInfo : MonoBehaviour {

    internal string planetName;
    internal string planetInfo;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnMouseEnter()
    {
        ToolTip.instance.SetTooltip(planetName, planetInfo);
    }
    public void OnMouseExit()
    {
        ToolTip.instance.CancelTooltip();
    }
}

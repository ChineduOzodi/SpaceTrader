﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class PlanetInfo : MonoBehaviour {

    internal SolarBody solar;
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
        ToolTip.instance.SetTooltip(solar.name, solar.GetInfo(GameManager.instance.data.date.time));
    }
    public void OnMouseOver()
    {
        ToolTip.instance.SetTooltip(solar.name, solar.GetInfo(GameManager.instance.data.date.time));
    }
    public void OnMouseExit()
    {
        ToolTip.instance.CancelTooltip();
    }
    public void OnMouseDown()
    {
        GameManager.instance.OpenInfoPanel(solar.solarIndex);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using UnityEngine.EventSystems;

public class PlanetInfo : MonoBehaviour {

    internal SolarBody solar;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            ToolTip.instance.SetTooltip(solar.name, solar.GetInfo(GameManager.instance.data.date.time));
    }
    public void OnMouseOver()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            ToolTip.instance.SetTooltip(solar.name, solar.GetInfo(GameManager.instance.data.date.time));
    }
    public void OnMouseExit()
    {
        ToolTip.instance.CancelTooltip();
    }
    public void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //GameManager.instance.OpenInfoPanel(solar.solarIndex);
            PlanetView.instance.CreatePlanetSystem(solar);
            SolarView.instance.SelectPlanet(solar);
        }
            
    }
}

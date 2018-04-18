using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipInteract : MonoBehaviour {

    public SolarBody solar;

    public void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            ToolTip.instance.SetTooltip(solar.name, String.Format("Satellites: {0}", solar.satelites.Count));
    }
    public void OnMouseExit()
    {
        ToolTip.instance.CancelTooltip();
    }

    public void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            SolarView.instance.CreateSystem(solar);
            GalaxyView.instance.SelectSolarSystem(solar);
            PlanetView.instance.DestroySystem();
        }
            
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipShip : MonoBehaviour {

    public Ship ship;
    LineRenderer line;
    public GameObject visual;

    public void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    public void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            ToolTip.instance.SetTooltip(ship.name, String.Format("Destination: {0}", GameManager.instance.locations[GameManager.instance.contracts[ship.contractId].destinationId].name));
    }
    public void OnMouseExit()
    {
        ToolTip.instance.CancelTooltip();
    }

    public void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            GameManager.instance.OpenInfoPanel(ship);
    }
    public void Update()
    {
        if (ship != null)
        {
            
            if ((GameManager.instance.locations[ship.referenceId] as SolarBody)!= null && (GameManager.instance.locations[ship.referenceId] as SolarBody).solarType != SolarType.Star)
            {
                line.widthMultiplier = .05f;
                transform.localScale = Vector3.one * .01f;
            }
            else
            {
                line.widthMultiplier = 5f;
                transform.localScale = Vector3.one * 1f;
            }
            if (ship.shipTargetId != null)
            {
                var target = GameManager.instance.locations[ship.shipTargetId];

                if ((GameManager.instance.locations[ship.referenceId] as SolarBody) != null && (GameManager.instance.locations[ship.referenceId] as SolarBody).solarType != SolarType.Star)
                {
                    if (target.referenceId != ship.referenceId)
                    {
                        Vector3 distance = (Vector3) (target.SystemPosition - ship.SystemPosition).normalized * 100 + transform.position;
                        transform.LookAt(distance);
                        line.SetPosition(0, transform.position);
                        line.SetPosition(1, distance);
                        line.endColor = Color.yellow;
                    }
                    else
                    {
                        Vector3 targetPosition = (Vector3)(target.referencePosition / Position.SystemConversion[2]);
                        transform.LookAt(targetPosition);
                        line.endColor = Color.blue;
                    }
                }
                else
                {
                    
                    Vector3 targetPosition = (Vector3)(target.SystemPosition / Position.SystemConversion[1]);
                    transform.LookAt(targetPosition);
                    line.SetPosition(0, transform.position);
                    line.SetPosition(1, targetPosition);
                    line.endColor = Color.blue;
                }
            }
        }
    }

    public void SetLayer(int layer)
    {
        gameObject.layer = layer;
        visual.layer = layer;
    }
}

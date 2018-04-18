using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SatelliteIconCreator : MonoBehaviour {

    public Structure target;
    public GameObject station;
    public GameObject factoryPrefab;
    public GameObject driller;
    public GameObject distributionCenterPrefab;
    // Use this for initialization
    void Start () {    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetTarget(Structure _structure)
    {
        target = _structure;
        GameObject structure;
        if (target.GetType() == typeof(Factory))
        {
            structure = Instantiate(factoryPrefab, this.transform);
            structure.transform.LookAt(transform.localPosition * 2, structure.transform.up);
            structure.transform.Rotate(Vector3.right * 90);
        }
        else if (target.GetType() == typeof(Driller))
        {
            structure = Instantiate(driller, this.transform);
            structure.transform.LookAt(transform.localPosition * 2, structure.transform.up);
            structure.transform.Rotate(Vector3.right * 90);
        }
         else if (target.GetType() == typeof(DistributionCenter))
        {
            structure = Instantiate(distributionCenterPrefab, this.transform);
            structure.transform.LookAt(transform.localPosition * 2,structure.transform.up);
            structure.transform.Rotate(Vector3.right * 90);
        }
        else //if (target.GetType() == typeof(Station))
        {
            structure = Instantiate(station, this.transform);
            structure.transform.Rotate(Vector3.right * -90);
        }
        
        structure.transform.localScale = Vector3.one * .1f;
    }

    public void OnMouseEnter()
    {
        if (target != null && (!EventSystem.current.IsPointerOverGameObject()))
        {
            ToolTip.instance.SetTooltip(target.name, target.Info);
        }
        
    }
    public void OnMouseOver()
    {
        if (target != null && (!EventSystem.current.IsPointerOverGameObject()))
        {
            ToolTip.instance.SetTooltip(target.name, target.Info);
        }
    }
    public void OnMouseExit()
    {
            ToolTip.instance.CancelTooltip();
    }
    public void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (target.GetType() == typeof(Station))
            {
                //GameManager.instance.OpenInfoPanel(solar.solarIndex);
                PlanetView.instance.SelectStructure(target);
                NormalView.instance.CreateNormalView(target);
                return;
            }
            GameManager.instance.OpenInfoPanel(target.id, TargetType.Structure);
        }
    }
}

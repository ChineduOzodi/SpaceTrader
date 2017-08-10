using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetPanel : MonoBehaviour {

    public GameObject planetPanel;
    public PlanetGridPanel gridPanel;

    public static PlanetPanel instance;
	// Use this for initialization
	void Start () {
        instance = this;
        planetPanel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SelectPlanet(SolarBody body)
    {
        planetPanel.SetActive(true);
        gridPanel.SelectPlanet(body);
    }

    public void GetInfo(PlanetTile _tile)
    {
        gridPanel.GetInfo(_tile);
    }
}

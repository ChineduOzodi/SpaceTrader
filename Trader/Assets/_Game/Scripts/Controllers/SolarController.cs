﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CodeControl;
using System;

public class SolarController : Controller<SolarModel> {

    public GameObject sunObj;
    public GameObject planetObj;
    public GameObject moonObj;
    internal GameObject sun;
    internal GameObject[] planets;
    internal StationController[] stations;
    internal List<GameObject> moons = new List<GameObject>();
    internal List<SolarBody> moonModels = new List<SolarBody>();
    internal GameManager game;
    internal GalaxyManager galaxy;
    private SpriteRenderer sprite;
    protected override void OnInitialize()
    {
        game = GameManager.instance;
        galaxy = GalaxyManager.instance;
        transform.position = model.position[0];
        name = model.name;
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = model.sun.color;
        transform.localScale = Vector3.one * Mathf.Pow(model.sun.mass / Mathf.PI,.3f);

        if (model.isActive)
        {
            CreateSystem();
        }
    }
	
	// Update is called once per frame
	void Update () {
        ToggleSystem();
        if (model.isActive)
        {
            //sun.transform.localScale = Vector3.one * Mathf.Sqrt(model.sun.mass / Mathf.PI) * Mathf.Pow(game.localScaleMod, .9f);
            sun.transform.localScale = Vector3.one * model.sun.bodyRadius * 2;
            game.localScaleMod = Mathf.Pow(Camera.main.orthographicSize, .5f);

            for ( int i = 0; i < model.planets.Length; i++)
            {
                SolarBody body = model.planets[i];
                Vector3 position = body.solar.GetLocalPosition(game.data.date.time).cartesian;
                planets[i].transform.position = position;
                //planets[i].transform.localScale = Vector3.one * Mathf.Sqrt(body.mass / Mathf.PI) * Mathf.Pow(game.localScaleMod, 1.3f);
                planets[i].transform.localScale = Vector3.one * body.bodyRadius * 2;

                LineRenderer line = planets[i].GetComponent<LineRenderer>();

                line.startWidth = planets[i].transform.localScale.x * .3f;
                line.endWidth = planets[i].transform.localScale.x * .3f;

                
            }

            for (int i = 0; i < moons.Count; i++)
            {               
                SolarBody moon = moonModels[i];
                Vector3 position = moon.solar.GetWorldPosition(game.data.date.time);
                moons[i].transform.position = position;
                //moons[i].transform.localScale = Vector3.one * Mathf.Sqrt(moon.mass / Mathf.PI) * Mathf.Pow(game.localScaleMod, 1.1f);
                moons[i].transform.localScale = Vector3.one * moon.bodyRadius * 2;

                LineRenderer line = moons[i].GetComponent<LineRenderer>();
                line.startWidth = moons[i].transform.localScale.x * .3f;
                line.endWidth = moons[i].transform.localScale.x * .3f;

                //int numPoints = (int) (body.radius * .1f);
                int numPoints = 360;
                float angleStep = (2 * Mathf.PI / numPoints);

                //Creates the line rendering for the orbit of planets

                Vector3[] orbitPos = new Vector3[numPoints + 1];

                for (int b = 0; b < numPoints + 1; b++)
                {
                    orbitPos[b] = moon.solar.parent.solar.GetWorldPosition(game.data.date.time) + new Polar2(moon.solar.radius, angleStep * b).cartesian;
                }
                line.positionCount = numPoints;
                line.SetPositions(orbitPos);
            }
        }
		
	}

    protected override void OnModelChanged()
    {

        sprite.color = model.color;
        if (model.isActive)
            transform.localScale = Vector3.one * model.localScale;
    }

    public void ToggleSystem()
    {
        SystemToggle();
    }
    //TODO: new way to save and load focus star system
    private void SystemToggle()
    {
        if (galaxy.solar == this && Camera.main.cullingMask == galaxy.solarMask && !model.isActive)
        {
            model.isActive = true;
            game.nameOfSystem.text = model.name;
            CreateSystem();
        }
        else if ((galaxy.solar != this || Camera.main.cullingMask != galaxy.solarMask) && model.isActive)
        {
            model.isActive = false;
            game.nameOfSystem.text = "Galaxy";
            DestroySystem();
        }
    }

    public void CreateSystem()
    {
        model.localScale = 1;
        transform.localScale = Vector3.one;
        sun = Instantiate(sunObj, transform);
        sun.name = model.name + " Sun";
        sun.transform.position = Vector3.zero;
        sun.transform.localScale = Vector3.one * Mathf.Sqrt(model.sun.mass / Mathf.PI);
        sun.GetComponent<SpriteRenderer>().color = model.sun.color;
        sun.GetComponent<SpriteRenderer>().sortingOrder = 5;
        planets = new GameObject[model.planets.Length];
        for (int i = 0; i < model.planets.Length; i++)
        {
            SolarBody body = model.planets[i];
            Vector3 position = body.solar.GetLocalPosition(game.data.date.time).cartesian;
            planets[i] = Instantiate(planetObj,transform);
            planets[i].name = body.name;
            planets[i].transform.localPosition = position;
            planets[i].GetComponent<SpriteRenderer>().color = body.color;
            planets[i].GetComponent<SpriteRenderer>().sortingOrder = 4;
            planets[i].transform.localScale = Vector3.one * (Mathf.Sqrt(body.mass / Mathf.PI));

            //int numPoints = (int) (body.radius * .1f);
            int numPoints = 360;
            float angleStep = (2 * Mathf.PI / numPoints);

            //Creates the line rendering for the orbit of planets

            Vector3[] orbitPos = new Vector3[numPoints + 1];

            for (int b = 0; b < numPoints + 1; b++)
            {
                orbitPos[b] = new Polar2(body.solar.radius, angleStep * b).cartesian;
            }
            LineRenderer line = planets[i].GetComponent<LineRenderer>();
            line.positionCount = numPoints;
            line.SetPositions(orbitPos);

            //Create Moons
            for (int m = 0; m < model.planets[i].children.Length; m++)
            {
                SolarBody moon = model.planets[i].children[m];
                position = moon.solar.GetLocalPosition(game.data.date.time).cartesian;
                moonModels.Add(moon);
                moons.Add(Instantiate(moonObj, transform));
                moons[moons.Count - 1].name = moon.name;
                moons[moons.Count - 1].transform.localPosition = position;
                moons[moons.Count - 1].GetComponent<SpriteRenderer>().color = moon.color;
                moons[moons.Count - 1].GetComponent<SpriteRenderer>().sortingOrder = 3;
                moons[moons.Count - 1].transform.localScale = Vector3.one * (Mathf.Sqrt(body.mass / Mathf.PI));
            }
        }

        //Create Stations
        stations = new StationController[model.stations.Count];
        int c = 0;
        foreach (StationModel station in model.stations)
        {
            stations[c] = Controller.Instantiate<StationController>("station", station);
            c++;
        }

    }

    public void DestroySystem()
    {
        //if (model.nameText != "" && model.nameText != null)
        //{
        //    nameButton.enabled = false;
        //}
        transform.localScale = Vector3.one * Mathf.Pow(model.sun.mass / Mathf.PI, .3f);
        model.localScale = Mathf.Pow(model.sun.mass / Mathf.PI, .3f);
        Destroy(sun);
        for (int i = 0; i < model.planets.Length; i++)
        {
            Destroy(planets[i]);
            
        }
        planets = new GameObject[0];
        for (int i = 0; i < moons.Count; i++)
        {
            Destroy(moons[i]);
            
        }
        moons = new List<GameObject>();
        for (int i = 0; i < stations.Length; i++)
        {
            Destroy(stations[i].gameObject);

        }
        stations = new StationController[0];
    }

    public void OnMouseEnter()
    {
        ToolTip.instance.SetTooltip(model.name, String.Format("Planets: {0}", model.planets.Length));
    }
    public void OnMouseExit()
    {
        ToolTip.instance.CancelTooltip();
    }
}

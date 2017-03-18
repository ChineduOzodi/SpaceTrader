using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using System;

public class SolarController : Controller<SolarModel> {

    public GameObject sunObj;
    public GameObject planetObj;
    public GameObject moonObj;

    internal GameObject sun;
    internal GameObject[] planets;
    internal GameObject[] moons;

    public float localScaleMod = 1;

    
    protected override void OnInitialize()
    {
        transform.position = model.position;

        GetComponent<SpriteRenderer>().color = model.sun.color;
        transform.localScale = Vector3.one * Mathf.Sqrt(model.sun.mass / Mathf.PI);

        if (model.isActive)
        {
            CreateSystem();
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (model.isActive)
        {
            sun.transform.localScale = Vector3.one * Mathf.Sqrt(model.sun.mass / Mathf.PI) * localScaleMod;

            for ( int i = 0; i < model.planets.Length; i++)
            {
                localScaleMod = Mathf.Pow(Camera.main.orthographicSize, .5f);
                SolarBody body = model.planets[i];
                Vector3 position = body.GetPosition(Time.time).cartesian;
                planets[i].transform.position = position;
                planets[i].transform.localScale = Vector3.one * Mathf.Sqrt(model.planets[i].mass / Mathf.PI) * Mathf.Pow(localScaleMod, 1.3f);
                LineRenderer line = planets[i].GetComponent<LineRenderer>();
                line.startWidth = planets[i].transform.localScale.x * .3f;
                line.endWidth = planets[i].transform.localScale.x * .3f;
            }
        }
		
	}

    public void ToggleSystem()
    {
        model.isActive = !model.isActive;

        if (model.isActive)
        {
            CreateSystem();
        }
        else
            DestroySystem();
    }

    public void CreateSystem()
    {
        transform.localScale = Vector3.one;
        sun = Instantiate(sunObj, transform);
        sun.transform.position = Vector3.zero;
        sun.transform.localScale = Vector3.one * Mathf.Sqrt(model.sun.mass / Mathf.PI);
        sun.GetComponent<SpriteRenderer>().color = model.sun.color;

        planets = new GameObject[model.planets.Length];
        for (int i = 0; i < model.planets.Length; i++)
        {
            SolarBody body = model.planets[i];
            Vector3 position = body.GetPosition(Time.time).cartesian;
            planets[i] = Instantiate(planetObj,transform);
            planets[i].name = body.name;
            planets[i].transform.position = position;
            planets[i].GetComponent<SpriteRenderer>().color = body.color;
            planets[i].transform.localScale = Vector3.one * Mathf.Sqrt(body.mass / Mathf.PI);

            //int numPoints = (int) (body.radius * .1f);
            int numPoints = 360;
            float angleStep = (2 * Mathf.PI / numPoints);

            //Creates the line rendering for the orbit of planets

            Vector3[] orbitPos = new Vector3[numPoints + 1];

            for (int b = 0; b < numPoints + 1; b++)
            {
                orbitPos[b] = new Polar2(body.radius, angleStep * b).cartesian;
            }
            LineRenderer line = planets[i].GetComponent<LineRenderer>();
            line.numPositions = numPoints;
            line.SetPositions(orbitPos);
        }
    }

    public void DestroySystem()
    {
        transform.localScale = Vector3.one * Mathf.Sqrt(model.sun.mass / Mathf.PI);

        Destroy(sun);
        for (int i = 0; i < model.planets.Length; i++)
        {
            Destroy(planets[i]);
        }
    }
}

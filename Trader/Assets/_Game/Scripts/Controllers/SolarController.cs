using System.Collections.Generic;
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
    internal List<GameObject> moons = new List<GameObject>();
    internal List<SolarBody> moonModels = new List<SolarBody>();
    internal GameManager game;
    internal CreateGalaxy galaxy;
    protected override void OnInitialize()
    {
        game = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();
        galaxy = game.GetComponent<CreateGalaxy>();
        transform.position = model.position;
        name = model.name;
        GetComponent<SpriteRenderer>().color = model.sun.color;
        transform.localScale = Vector3.one * Mathf.Sqrt(model.sun.mass / Mathf.PI);

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
            sun.transform.localScale = Vector3.one * Mathf.Sqrt(model.sun.mass / Mathf.PI) * Mathf.Pow(game.localScaleMod, .9f);
            game.localScaleMod = Mathf.Pow(Camera.main.orthographicSize, .5f);

            for ( int i = 0; i < model.planets.Length; i++)
            {
                SolarBody body = model.planets[i];
                Vector3 position = body.GetLocalPosition(game.data.date.time).cartesian;
                planets[i].transform.position = position;
                planets[i].transform.localScale = Vector3.one * Mathf.Sqrt(body.mass / Mathf.PI) * Mathf.Pow(game.localScaleMod, 1.3f);
                LineRenderer line = planets[i].GetComponent<LineRenderer>();

                line.startWidth = planets[i].transform.localScale.x * .3f;
                line.endWidth = planets[i].transform.localScale.x * .3f;

                
            }

            for (int i = 0; i < moons.Count; i++)
            {               
                SolarBody moon = moonModels[i];
                Vector3 position = moon.GetWorldPosition(game.data.date.time);
                moons[i].transform.position = position;
                moons[i].transform.localScale = Vector3.one * Mathf.Sqrt(moon.mass / Mathf.PI) * Mathf.Pow(game.localScaleMod, 1.1f);

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
                    orbitPos[b] = moon.parent.GetWorldPosition(game.data.date.time) + new Polar2(moon.radius, angleStep * b).cartesian;
                }
                line.numPositions = numPoints;
                line.SetPositions(orbitPos);
            }
        }
		
	}

    public void ToggleSystem()
    {
        if (galaxy.solar == this && Camera.main.cullingMask == galaxy.solarMask && !model.isActive)
        {
            model.isActive = true;
            CreateSystem();
        }
        else if ((galaxy.solar != this || Camera.main.cullingMask != galaxy.solarMask) && model.isActive)
        {
            model.isActive = false;
            DestroySystem();
        }
        //else if (galaxy.solar != this || Camera.main.cullingMask != galaxy.solarMask)
        //{
        //    model.isActive = false;
        //}

    }

    public void CreateSystem()
    {
        transform.localScale = Vector3.one;
        sun = Instantiate(sunObj, transform);
        sun.transform.position = Vector3.zero;
        sun.transform.localScale = Vector3.one * Mathf.Sqrt(model.sun.mass / Mathf.PI);
        sun.GetComponent<SpriteRenderer>().color = model.sun.color;
        sun.GetComponent<SpriteRenderer>().sortingOrder = 5;
        planets = new GameObject[model.planets.Length];
        for (int i = 0; i < model.planets.Length; i++)
        {
            SolarBody body = model.planets[i];
            Vector3 position = body.GetLocalPosition(game.data.date.time).cartesian;
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
                orbitPos[b] = new Polar2(body.radius, angleStep * b).cartesian;
            }
            LineRenderer line = planets[i].GetComponent<LineRenderer>();
            line.numPositions = numPoints;
            line.SetPositions(orbitPos);

            //Create Moons
            for (int m = 0; m < model.planets[i].children.Length; m++)
            {
                SolarBody moon = model.planets[i].children[m];
                position = moon.GetLocalPosition(game.data.date.time).cartesian;
                moonModels.Add(moon);
                moons.Add(Instantiate(moonObj, transform));
                moons[moons.Count - 1].name = moon.name;
                moons[moons.Count - 1].transform.localPosition = position;
                moons[moons.Count - 1].GetComponent<SpriteRenderer>().color = moon.color;
                moons[moons.Count - 1].GetComponent<SpriteRenderer>().sortingOrder = 3;
                moons[moons.Count - 1].transform.localScale = Vector3.one * (Mathf.Sqrt(body.mass / Mathf.PI));
            }
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
        planets = new GameObject[0];
        for (int i = 0; i < moons.Count; i++)
        {
            Destroy(moons[i]);
            
        }
        moons = new List<GameObject>();
    }
}

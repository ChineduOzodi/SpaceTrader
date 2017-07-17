using System.Collections.Generic;
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
    internal List<GameObject> planets;
    internal StationController[] stations;
    internal List<GameObject> moons = new List<GameObject>();
    internal List<SolarBody> moonModels = new List<SolarBody>();
    internal GameManager game;
    internal GalaxyManager galaxy;
    private SpriteRenderer sprite;
    public float solarSpriteScale = .25f;
    protected override void OnInitialize()
    {
        game = GameManager.instance;
        galaxy = GalaxyManager.instance;
        transform.position = model.galacticPosition;
        name = model.name;
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = model.solar.color;
        transform.localScale = Vector3.one * (float)Mathd.Pow((model.solar.bodyRadius), .02f);

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
            game.localScaleMod = Mathf.Pow(Camera.main.orthographicSize, .6f) / 90;
            sun.transform.localScale = Vector3.one * (float)Mathd.Pow((model.solar.bodyRadius), solarSpriteScale) * Mathf.Pow(game.localScaleMod, .9f);

            for ( int i = 0; i < model.solar.satelites.Count; i++)
            {
                SolarBody body = model.solar.satelites[i];
                Vector3 position = (Vector2) body.GamePosition(game.data.date.time,model.solar.mass);
                planets[i].transform.position = sun.transform.position + position;
                //planets[i].transform.localScale = Vector3.one * Mathf.Sqrt(body.mass / Mathf.PI) * Mathf.Pow(game.localScaleMod, 1.3f);
                planets[i].transform.localScale = Vector3.one * (float)Mathd.Pow((body.bodyRadius), solarSpriteScale) * Mathf.Pow(game.localScaleMod, .9f);

                LineRenderer line = planets[i].GetComponent<LineRenderer>();

                line.widthMultiplier = planets[i].transform.localScale.x * .3f;
                
            }

            for (int i = 0; i < moons.Count; i++)
            {               
                SolarBody moon = moonModels[i];
                Vector3 position = (Vector2) moon.GamePosition(game.data.date.time,model.solar.satelites[moon.solarIndex[1]].mass);
                moons[i].transform.position = position + model.solar.satelites[moon.solarIndex[1]].GamePosition(game.data.date.time, model.solar.mass);
                //moons[i].transform.localScale = Vector3.one * Mathf.Sqrt(moon.mass / Mathf.PI) * Mathf.Pow(game.localScaleMod, 1.1f);
                moons[i].transform.localScale = Vector3.one * (float)Mathd.Pow((moon.bodyRadius), solarSpriteScale) * Mathf.Pow(game.localScaleMod, .9f);

                LineRenderer line = moons[i].GetComponent<LineRenderer>();

                line.widthMultiplier = moons[i].transform.localScale.x * .3f;

                //Creates the line rendering for the orbit of moons

                line = moons[i].GetComponent<LineRenderer>();

                var positions = new Vector3[361];

                var timeInc = (moon.OrbitalPeriod(model.solar.mass) / 360);
                double time = game.data.date.time;

                for (var b = 0; b < 361; b++)
                {
                    positions[b] = model.solar.satelites[moon.solarIndex[1]].GamePosition(game.data.date.time,model.solar.mass) + moon.GamePosition(time, model.solar.mass);
                    time += timeInc;
                }
                line.positionCount = 361;
                line.SetPositions(positions);
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
        sun.transform.localScale = Vector3.one * (float)Mathd.Pow((model.solar.bodyRadius / GameDataModel.solarDistanceMultiplication), solarSpriteScale);
        sun.GetComponent<SpriteRenderer>().color = model.solar.color;
        sun.GetComponent<SpriteRenderer>().sortingOrder = 5;

        var info = sun.GetComponent<PlanetInfo>();
        info.planetName = sun.name;
        info.planetInfo = model.solar.GetInfo(model.solar.mass);

        planets = new List<GameObject>();
        for (int i = 0; i < model.solar.satelites.Count; i++)
        {
            SolarBody body = model.solar.satelites[i];
            Vector3 position = sun.transform.position + body.GamePosition(game.data.date.time, model.solar.mass);
            planets.Add(Instantiate(planetObj, transform));
            planets[i].name = body.name;
            planets[i].transform.localPosition = position;
            planets[i].GetComponent<SpriteRenderer>().color = body.color;
            planets[i].GetComponent<SpriteRenderer>().sortingOrder = 4;
            planets[i].transform.localScale = Vector3.one * (float)Mathd.Pow((body.bodyRadius), solarSpriteScale);

            info = planets[i].GetComponent<PlanetInfo>();
            info.planetName = body.name;
            info.planetInfo = body.GetInfo(model.solar.mass);

            //Creates the line rendering for the orbit of planets

            LineRenderer line = planets[i].GetComponent<LineRenderer>();

            Vector3[] positions = new Vector3[361];

            double timeInc = (body.OrbitalPeriod(model.solar.mass) / 360);
            double time = game.data.date.time;

            for (var b = 0; b < 361; b++)
            {
                positions[b] = body.GamePosition(time,model.solar.mass);
                time += timeInc;
            }
            line.positionCount = 361;
            line.SetPositions(positions);

            //Create Moons
            for (int m = 0; m < model.solar.satelites[i].satelites.Count; m++)
            {
                SolarBody moon = model.solar.satelites[i].satelites[m];
                position = moon.GamePosition(game.data.date.time, model.solar.satelites[i].mass);
                moonModels.Add(moon);

                moons.Add(Instantiate(moonObj, transform));
                moons[moons.Count - 1].name = moon.name;
                moons[moons.Count - 1].transform.localPosition = position;
                moons[moons.Count - 1].GetComponent<SpriteRenderer>().color = moon.color;
                moons[moons.Count - 1].GetComponent<SpriteRenderer>().sortingOrder = 3;
                moons[moons.Count - 1].transform.localScale = Vector3.one * (float)Mathd.Pow((moon.bodyRadius), solarSpriteScale);

                info = moons[moons.Count - 1].GetComponent<PlanetInfo>();
                info.planetName = moon.name;
                info.planetInfo = moon.GetInfo(body.mass);
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
        transform.localScale = Vector3.one * (float) Mathd.Pow((model.solar.bodyRadius), .01f);
        model.localScale = (float) Mathd.Pow((model.solar.bodyRadius), .01f);
        Destroy(sun);
        for (int i = 0; i < planets.Count; i++)
        {
            Destroy(planets[i]);
            
        }
        planets = new List<GameObject>();
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
        ToolTip.instance.SetTooltip(model.name, String.Format("Planets: {0}", model.solar.satelites.Count));
    }
    public void OnMouseExit()
    {
        ToolTip.instance.CancelTooltip();
    }
}

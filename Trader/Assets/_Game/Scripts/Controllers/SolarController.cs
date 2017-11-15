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

    private Vector3 lastCamPosition = Vector3.zero;
    protected override void OnInitialize()
    {
        lastCamPosition.z = -10;
        game = GameManager.instance;
        galaxy = GalaxyManager.instance;
        transform.position = model.galacticPosition;
        name = model.name;
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = model.solar.color;
        transform.localScale = Vector3.one * (float)Mathd.Pow((model.solar.bodyRadius), .02f);
        sprite.enabled = false;

        if (model.isActive)
        {
            CreateSystem();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (model.isActive)
        {
            game.localScaleMod = Mathf.Pow(Camera.main.orthographicSize, .7f) / 90;
            sun.transform.localScale = Vector3.one * (float)Mathd.Pow((model.solar.bodyRadius), solarSpriteScale) * Mathf.Pow(game.localScaleMod, .9f);

            for (int i = 0; i < model.solar.satelites.Count; i++)
            {
                //planets[i].transform.localScale = Vector3.one * (float)Mathd.Pow((body.bodyRadius), solarSpriteScale) * Mathf.Pow(game.localScaleMod, .9f);
                planets[i].transform.localScale = sun.transform.localScale * .5f;

                LineRenderer line = planets[i].GetComponent<LineRenderer>();

                line.widthMultiplier = planets[i].transform.localScale.x * .3f;
            }
            for (int i = 0; i < moons.Count; i++)
            {
                SolarBody moon = moonModels[i];

                var visible = CheckVisibility(moon);
                moons[i].SetActive(visible);
                LineRenderer line = moons[i].GetComponent<LineRenderer>();

                line.widthMultiplier = moons[i].transform.localScale.x * .3f;
                moons[i].transform.localScale = sun.transform.localScale * .15f;
            }
        }
        
        ToggleSystem();
		
	}

    public IEnumerator UpdateSolarObjects()
    {
        while (model.isActive)
        {
            for (int i = 0; i < model.solar.satelites.Count; i++)
            {
                SolarBody body = model.solar.satelites[i];
                Vector3 position = (Vector2)body.GamePosition(game.data.date.time);

                Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
                bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

                if (onScreen)
                {
                    var visible = CheckVisibility(body);

                    planets[i].SetActive(visible);

                    planets[i].transform.position = sun.transform.position + position;
                    yield return null;
                }
            }

            for (int i = 0; i < moons.Count; i++)
            {
                SolarBody moon = moonModels[i];
                Vector3 position = (Vector2)moon.GamePosition(game.data.date.time);
                position += model.solar.satelites[moon.solarIndex[1]].GamePosition(game.data.date.time);

                Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
                bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

                if (onScreen)
                {
                    var visible = CheckVisibility(moon);
                    moons[i].SetActive(visible);
                    LineRenderer line = moons[i].GetComponent<LineRenderer>();

                    moons[i].transform.position = position;

                    //Creates the line rendering for the orbit of moons

                    var positions = new Vector3[361];

                    var timeInc = (moon.OrbitalPeriod(model.solar.satelites[moon.solarIndex[1]].mass) / 360);
                    double time = game.data.date.time;

                    for (var b = 0; b < 361; b++)
                    {
                        positions[b] = model.solar.satelites[moon.solarIndex[1]].GamePosition(game.data.date.time) + moon.GamePosition(time);
                        time += timeInc;
                    }
                    line.positionCount = 361;
                    line.SetPositions(positions);

                    yield return null;
                }

            }
            yield return null;
        }
    }

    private bool CheckVisibility(SolarBody body)
    {
        if (body.solarType == SolarType.Planet)
        {
            if (!MapTogglePanel.instance.planet.isOn)
            {
                return false;
            }
            else
            {
                return MapTogglePanel.instance.subtypes[body.solarSubType].isOn;
            }
        }
        if (body.solarType == SolarType.DwarfPlanet)
        {
            if (!MapTogglePanel.instance.dwarfPlanet.isOn)
            {
                return false;
            }
            else
            {
                return MapTogglePanel.instance.subtypes[body.solarSubType].isOn;
            }
        }
        if (body.solarType == SolarType.Comet)
        {
            if (!MapTogglePanel.instance.comet.isOn)
            {
                return false;
            }
            else
            {
                return MapTogglePanel.instance.subtypes[body.solarSubType].isOn;
            }
        }
        if (body.solarType == SolarType.Asteroid)
        {
            if (!MapTogglePanel.instance.asteroid.isOn)
            {
                return false;
            }
            else
            {
                return MapTogglePanel.instance.subtypes[body.solarSubType].isOn;
            }
        }

        if (body.solarType == SolarType.Moon)
        {
            if (!MapTogglePanel.instance.moons.isOn)
            {
                return false;
            }
            else if (planets[body.solarIndex[1]].activeSelf)
            {
                return MapTogglePanel.instance.subtypes[body.solarSubType].isOn;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    internal SolarModel GetModel()
    {
        return model;
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
            var mainCam = Camera.main;
            var newCamPos = mainCam.transform.position;
            mainCam.transform.position = lastCamPosition;
            lastCamPosition = newCamPos;
            CreateSystem();
        }
        else if ((galaxy.solar != this || Camera.main.cullingMask != galaxy.solarMask) && model.isActive)
        {
            var mainCam = Camera.main;
            var newCamPos = mainCam.transform.position;
            mainCam.transform.position = lastCamPosition;
            lastCamPosition = newCamPos;
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
        info.solar = model.solar;

        planets = new List<GameObject>();
        for (int i = 0; i < model.solar.satelites.Count; i++)
        {
            SolarBody body = model.solar.satelites[i];
            Vector3 position = sun.transform.position + body.GamePosition(game.data.date.time);
            planets.Add(Instantiate(planetObj, transform));
            planets[i].name = body.name;
            planets[i].transform.localPosition = position;
            planets[i].GetComponent<SpriteRenderer>().color = body.color;
            planets[i].GetComponent<SpriteRenderer>().sortingOrder = 4;
            planets[i].transform.localScale = Vector3.one * (float)Mathd.Pow((body.bodyRadius), solarSpriteScale);

            info = planets[i].GetComponent<PlanetInfo>();
            info.solar = body;

            //Creates the line rendering for the orbit of planets

            LineRenderer line = planets[i].GetComponent<LineRenderer>();

            Vector3[] positions = new Vector3[361];

            double timeInc = (body.OrbitalPeriod(model.solar.mass) / 360);
            double time = game.data.date.time;

            for (var b = 0; b < 361; b++)
            {
                positions[b] = body.GamePosition(time);
                time += timeInc;
            }
            line.positionCount = 361;
            line.SetPositions(positions);
            
            if (body.solarSubType == SolarSubType.GasGiant)
            {
                Color col = Color.blue;
                col.a = .25f;
                line.startColor = col;
                line.endColor = col;
            }
            else if (body.solarType == SolarType.DwarfPlanet)
            {
                Color col = Color.yellow;
                col.a = .25f;
                line.startColor = col;
                line.endColor = col;
            }
            else if (body.solarType == SolarType.Comet)
            {
                Color col = Color.white;
                col.a = .25f;
                line.startColor = col;
                line.endColor = col;
            }
            else
            {
                Color col = Color.green;
                col.a = .25f;
                line.startColor = col;
                line.endColor = col;
            }

            //Create Moons
            for (int m = 0; m < model.solar.satelites[i].satelites.Count; m++)
            {
                SolarBody moon = model.solar.satelites[i].satelites[m];
                position = moon.GamePosition(game.data.date.time);
                position += model.solar.satelites[moon.solarIndex[1]].GamePosition(game.data.date.time);
                moonModels.Add(moon);

                moons.Add(Instantiate(moonObj, transform));
                moons[moons.Count - 1].name = moon.name;
                moons[moons.Count - 1].transform.position = position;
                moons[moons.Count - 1].GetComponent<SpriteRenderer>().color = moon.color;
                moons[moons.Count - 1].GetComponent<SpriteRenderer>().sortingOrder = 3;
                moons[moons.Count - 1].transform.localScale = Vector3.one * (float)Mathd.Pow((moon.bodyRadius), solarSpriteScale);

                info = moons[moons.Count - 1].GetComponent<PlanetInfo>();
                info.solar = moon;
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

        StartCoroutine("UpdateSolarObjects");
    }

    public void DestroySystem()
    {
        //if (model.nameText != "" && model.nameText != null)
        //{
        //    nameButton.enabled = false;
        //}
        StopAllCoroutines();
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
        ToolTip.instance.SetTooltip(model.name, String.Format("Satellites: {0}", model.solar.satelites.Count));
    }
    public void OnMouseExit()
    {
        ToolTip.instance.CancelTooltip();
    }
}

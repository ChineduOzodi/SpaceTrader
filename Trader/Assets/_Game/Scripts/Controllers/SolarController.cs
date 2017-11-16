using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CodeControl;
using System;
using Vectrosity;

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
    private CircleCollider2D circleCollider;
    private float solarSpriteScale = .02f;
    private float moonViewOrthoSize = .001f;
    //private VectorObject3D line;
    private VectorLine vectorLine;
    public Texture lineTexture;

    private Vector3 lastCamPosition = Vector3.zero;
    protected override void OnInitialize()
    {
        lastCamPosition.z = -10;
        game = GameManager.instance;
        galaxy = GalaxyManager.instance;
        transform.position = CameraController.CameraOffsetPoistion(model.galacticPosition);
        name = model.name;
        circleCollider = GetComponent<CircleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = model.solar.color;
        transform.localScale = Vector3.one;
        var points = new List<Vector3>();
        foreach (SolarModel solar in model.nearStars)
        {
            points.Add(transform.position);
            points.Add(CameraController.CameraOffsetPoistion(solar.galacticPosition));
        }
        vectorLine = new VectorLine("model.name Connections", points, (float)(Mathd.Pow((model.solar.bodyRadius), .02f) * game.data.cameraScaleMod));
        //line.SetVectorLine(vectorLine, lineTexture, sprite.material);
        vectorLine.Draw3D();
        sprite.enabled = false;
        if (model.isActive)
        {
            CreateSystem();
        }
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = CameraController.CameraOffsetPoistion(model.galacticPosition);
        circleCollider.radius = (float)(Mathd.Pow((model.solar.bodyRadius), .02f) * game.data.cameraScaleMod * 5);

        if (model.isActive)
        {
            sun.transform.position = transform.position;
            sun.transform.localScale = Vector3.one * (float)(Mathd.Pow((model.solar.bodyRadius), solarSpriteScale) * Mathd.Pow(game.data.cameraScaleMod, .9f));

            for (int i = 0; i < model.solar.satelites.Count; i++)
            {
                SolarBody body = model.solar.satelites[i];
                //planets[i].transform.localScale = Vector3.one * (float)Mathd.Pow((body.bodyRadius), solarSpriteScale) * Mathf.Pow(game.data.cameraScaleMod, .9f);
                planets[i].transform.localScale = sun.transform.localScale * .5f;

                LineRenderer line = planets[i].GetComponent<LineRenderer>();

                line.widthMultiplier = planets[i].transform.localScale.x * .3f;
                planets[i].transform.position = CameraController.CameraOffsetPoistion(model.galacticPosition + body.lastKnownPosition);

                //Creates the line rendering for the orbit of planets

                Vector3[] positions = new Vector3[36];

                for (var b = 0; b < 36; b++)
                {
                    
                    positions[b] = CameraController.CameraOffsetPoistion(model.galacticPosition + body.approximatePositions[b * 10]);
                }
                line.positionCount = 36;
                line.SetPositions(positions);
            }
            for (int i = 0; i < moons.Count; i++)
            {
                SolarBody moon = moonModels[i];
                moons[i].transform.position = CameraController.CameraOffsetPoistion(model.galacticPosition + model.solar.satelites[moon.solarIndex[1]].lastKnownPosition
                        + moon.lastKnownPosition);

                if (game.data.cameraOrth < moonViewOrthoSize && moons[i].transform.position.sqrMagnitude < 62500)
                {
                    var visible = CheckVisibility(moon);
                    moons[i].SetActive(visible);
                    LineRenderer line = moons[i].GetComponent<LineRenderer>();

                    line.widthMultiplier = moons[i].transform.localScale.x * .3f;
                    moons[i].transform.localScale = sun.transform.localScale * .15f;



                    //Creates the line rendering for the orbit of moons

                    var positions = new Vector3[36];

                    for (var b = 0; b < 36; b++)
                    {
                        positions[b] = CameraController.CameraOffsetPoistion(model.galacticPosition + model.solar.satelites[moon.solarIndex[1]].lastKnownPosition
                    + moon.approximatePositions[b * 10]);
                    }
                    line.positionCount = 36;
                    line.SetPositions(positions);
                }
                else
                {
                    moons[i].SetActive(false);
                }

                

            }

            if (game.data.cameraOrth > 15)
            {
                galaxy.GalaxyView();
            }
        }
        else
        {
            if (transform.position.sqrMagnitude < 40000)
            {
                if (MapTogglePanel.instance.galaxyConnections.isOn)
                {
                    var points = new List<Vector3>();
                    foreach (SolarModel solar in model.nearStars)
                    {
                        points.Add(transform.position);
                        points.Add(CameraController.CameraOffsetPoistion(solar.galacticPosition));
                    }
                    vectorLine.points3 = points;
                    vectorLine.SetWidth((float)(Mathd.Pow((model.solar.bodyRadius), .02f) * game.data.cameraScaleMod) * 5);
                    if (MapTogglePanel.instance.galaxyTerritory.isOn)
                    {
                        if (model.government.Model != null)
                        {
                            var govModel = model.government.Model;
                            vectorLine.color = new Color32((byte)(govModel.spriteColor.r * 255), (byte)(govModel.spriteColor.g * 255), (byte)(govModel.spriteColor.b * 255), 50);
                        }
                        else
                        {
                            vectorLine.color = new Color32((byte)(50), (byte)(50), (byte)(50), 10);
                        }
                        
                    }
                    else
                    {
                        vectorLine.color = new Color32((byte)(model.solar.color.r * 255), (byte)(model.solar.color.g * 255), (byte)(model.solar.color.b * 255), 10);
                    }
                    vectorLine.Draw3D();
                }
                else
                {
                    if (vectorLine.lineWidth != 0)
                    {
                        vectorLine.SetWidth(0);
                        vectorLine.Draw3D();
                    }
                }
            }
            else
            {
                if (vectorLine.lineWidth != 0)
                {
                    vectorLine.SetWidth(0);
                    vectorLine.Draw3D();
                }
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
                Vector2 position = CameraController.CameraOffsetPoistion(model.galacticPosition + body.GamePosition(game.data.date.time));

                Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
                bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

                if (onScreen)
                {
                    var visible = CheckVisibility(body);

                    planets[i].SetActive(visible);

                    planets[i].transform.position = position;

                    LineRenderer line = planets[i].GetComponent<LineRenderer>();

                    //Creates the line rendering for the orbit of planets

                    Vector3[] positions = new Vector3[361];
                    double time = game.data.date.time;
                    body.SetOrbit(time, model.solar.mass);


                    for (var b = 0; b < 360; b++)
                    {
                        positions[b] = CameraController.CameraOffsetPoistion(model.galacticPosition + body.approximatePositions[b]);
                    }
                    line.positionCount = 360;
                    line.SetPositions(positions);

                    yield return null;
                }
            }

            for (int i = 0; i < moons.Count; i++)
            {
                SolarBody moon = moonModels[i];
                Vector3 position = CameraController.CameraOffsetPoistion(model.galacticPosition + model.solar.satelites[moon.solarIndex[1]].lastKnownPosition
                    + moon.GamePosition(game.data.date.time));

                Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
                bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

                if (onScreen)
                {
                    LineRenderer line = moons[i].GetComponent<LineRenderer>();

                    moons[i].transform.position = position;

                    //Creates the line rendering for the orbit of moons

                    var positions = new Vector3[361];
                    double time = game.data.date.time;
                    moon.SetOrbit(time, model.solar.satelites[moon.solarIndex[1]].mass);

                    for (var b = 0; b < 360; b++)
                    {
                        positions[b] = CameraController.CameraOffsetPoistion(model.galacticPosition + model.solar.satelites[moon.solarIndex[1]].lastKnownPosition
                    + moon.approximatePositions[b]);
                    }
                    line.positionCount = 360;
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
        if (galaxy.solarModel == model && Camera.main.cullingMask == galaxy.solarMask && !model.isActive)
        {
            model.isActive = true;
            game.nameOfSystem.text = model.name;
            var mainCam = Camera.main;
            var newCamPos = mainCam.transform.position;
            mainCam.transform.position = lastCamPosition;
            lastCamPosition = newCamPos;
            CreateSystem();
        }
        else if ((galaxy.solarModel != model || Camera.main.cullingMask != galaxy.solarMask) && model.isActive)
        {
            var mainCam = Camera.main;
            var newCamPos = mainCam.transform.position;
            mainCam.transform.position = lastCamPosition;
            lastCamPosition = newCamPos;
            model.isActive = false;
            game.nameOfSystem.text = game.data.galaxyName;
            DestroySystem();
        }
    }

    public void CreateSystem()
    {
        model.localScale = 1;
        transform.localScale = Vector3.one;
        sun = Instantiate(sunObj, transform);
        sun.name = model.name + " Sun";
        sun.transform.localPosition = Vector3.zero;
        sun.transform.localScale = Vector3.one * (float)(Mathd.Pow((model.solar.bodyRadius), solarSpriteScale) * Mathd.Pow(game.data.cameraScaleMod, .9f));
        sun.GetComponent<SpriteRenderer>().color = model.solar.color;
        sun.GetComponent<SpriteRenderer>().sortingOrder = 5;

        var info = sun.GetComponent<PlanetInfo>();
        info.solar = model.solar;

        planets = new List<GameObject>();
        for (int i = 0; i < model.solar.satelites.Count; i++)
        {
            SolarBody body = model.solar.satelites[i];
            Vector3 position = CameraController.CameraOffsetPoistion(model.galacticPosition + body.GamePosition(game.data.date.time));
            planets.Add(Instantiate(planetObj, transform));
            planets[i].name = body.name;
            planets[i].transform.position = position;
            planets[i].GetComponent<SpriteRenderer>().color = body.color;
            planets[i].GetComponent<SpriteRenderer>().sortingOrder = 4;
            planets[i].transform.localScale = sun.transform.localScale * .5f;

            info = planets[i].GetComponent<PlanetInfo>();
            info.solar = body;

            LineRenderer line = planets[i].GetComponent<LineRenderer>();

            //Creates the line rendering for the orbit of planets

            Vector3[] positions = new Vector3[361];
            double time = game.data.date.time;
            body.SetOrbit(time, model.solar.mass);


            for (var b = 0; b < 360; b++)
            {
                positions[b] = CameraController.CameraOffsetPoistion(model.galacticPosition + body.approximatePositions[b]);
            }
            line.positionCount = 360;
            line.SetPositions(positions);


            if (body.solarSubType == SolarSubType.GasGiant)
            {
                Color col = Color.blue;
                col.a = .1f;
                line.startColor = col;
                line.endColor = col;
            }
            else if (body.solarType == SolarType.DwarfPlanet)
            {
                Color col = Color.yellow;
                col.a = .1f;
                line.startColor = col;
                line.endColor = col;
            }
            else if (body.solarType == SolarType.Comet)
            {
                Color col = Color.white;
                col.a = .1f;
                line.startColor = col;
                line.endColor = col;
            }
            else
            {
                Color col = Color.green;
                col.a = .1f;
                line.startColor = col;
                line.endColor = col;
            }

            //Create Moons
            for (int m = 0; m < model.solar.satelites[i].satelites.Count; m++)
            {
                SolarBody moon = model.solar.satelites[i].satelites[m];
                position = CameraController.CameraOffsetPoistion(model.galacticPosition + model.solar.satelites[moon.solarIndex[1]].lastKnownPosition
                    + moon.GamePosition(game.data.date.time));
                moonModels.Add(moon);

                moons.Add(Instantiate(moonObj, transform));
                moons[moons.Count - 1].name = moon.name;
                moons[moons.Count - 1].transform.position = position;
                moons[moons.Count - 1].GetComponent<SpriteRenderer>().color = moon.color;
                moons[moons.Count - 1].GetComponent<SpriteRenderer>().sortingOrder = 3;
                moons[moons.Count - 1].transform.localScale = sun.transform.localScale * .15f;

                info = moons[moons.Count - 1].GetComponent<PlanetInfo>();
                info.solar = moon;

                //Creates the line rendering for the orbit of moons

                positions = new Vector3[361];
                time = game.data.date.time;
                moon.SetOrbit(time, model.solar.satelites[moon.solarIndex[1]].mass);

                for (var b = 0; b < 360; b++)
                {
                    positions[b] = CameraController.CameraOffsetPoistion(model.galacticPosition + model.solar.satelites[moon.solarIndex[1]].lastKnownPosition
                + moon.approximatePositions[b]);
                }
                line.positionCount = 360;
                line.SetPositions(positions);
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

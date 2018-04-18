using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SolarView : IconManager {

    public GameObject sunObj;
    public PlanetCreator planetObj;
    public MapCamera solarCamera;
    public MapCamera galaxyCamera;
    public TooltipShip shipObj;

    internal static SolarView instance;

    internal GameManager game;
    internal ViewManager galaxy;

    

    internal GameObject sun;
    internal List<PlanetCreator> planets = new List<PlanetCreator>();
    internal List<TooltipShip> ships = new List<TooltipShip>();
    internal bool control = false;

    private SolarBody solar;

    //private float solarSpriteScale = .02f;

    private ParticleSystem particles;
    private ParticleSystem.Particle[] points;
    private int pointsMax;

    //Manage Icons
    public Canvas iconsCanvas;
    public Image hasStation;

    // Use this for initialization
    void Start () {
        if (instance == null)
        {
            instance = this;
            game = GameManager.instance;
            galaxy = ViewManager.instance;
            particles = GetComponent<ParticleSystem>();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
	
	// Update is called once per frame
	void LateUpdate () {
		if (control)
        {
            galaxyCamera.SetCameraView((Vector3)(solar.referencePosition + (((Vector3d) solarCamera.mainCamera.transform.position) / Position.SystemConversion[0] * Position.SystemConversion[1])), solarCamera.mainCamera.transform.rotation, solarCamera.mainCamera.transform.localScale, solarCamera.mainCamera.fieldOfView);

            for (int i = 1; i < pointsMax; i++)
            {
                points[i].position = solar.satelites[i-1].GamePosition(game.data.date.time);
                planets[i-1].transform.position = points[i].position;
            }
            particles.SetParticles(points, points.Length);

            //Create Ships
            foreach (string structureId in solar.structureIds)
            {
                var location = GameManager.instance.locations[structureId];

                
                if (location.GetType() == typeof(Ship))
                {
                    Ship ship = location as Ship;


                    //Find ship GameObject
                    

                    if (ships.Exists(x => x.ship == ship))
                    {
                        TooltipShip shipTooltip = ships.Find(x => x.ship == ship);
                        shipTooltip.transform.position = (Vector3)(ship.referencePosition / Position.SystemConversion[1]);
                    }
                    else
                    {
                        //Create ship
                        TooltipShip shipTooltip = Instantiate(shipObj, this.transform);
                        shipTooltip.ship = ship;
                        ships.Add(shipTooltip);
                        shipTooltip.transform.position = (Vector3)(ship.referencePosition / Position.SystemConversion[1]);

                    }
                    foreach (TooltipShip shipTooltip in ships)
                    {
                        //Delete if moved to another SOI
                        if (shipTooltip.ship.referenceId != solar.id)
                        {
                            ships.Remove(shipTooltip);
                            Destroy(shipTooltip.gameObject);
                            break;
                        }
                    }

                    
                }
            }
        }
    }

    public void CreateSystem(SolarBody _solar)
    {
        DestroySystem();
        PlanetView.instance.DestroySystem();
        NormalView.instance.DestroySystem();
        solar = _solar;
        control = true;
        solarCamera.SetCameraControlTrue();
        galaxyCamera.SetCameraView((Vector3)(solar.referencePosition + (((Vector3d)solarCamera.mainCamera.transform.position) / Position.SystemConversion[0] * Position.SystemConversion[1])), solarCamera.mainCamera.transform.rotation, solarCamera.mainCamera.transform.localScale, solarCamera.mainCamera.fieldOfView);
        //game.nameOfSystem.text = solar.name;

        transform.localScale = Vector3.one;
        sun = Instantiate(sunObj, transform);
        sun.name = solar.name;
        sun.transform.localPosition = Vector3.zero;
        sun.transform.localScale = Vector3.one * 50 ;//(float)(Mathd.Pow((solar.bodyRadius), solarSpriteScale)) ;//* Mathd.Pow(game.data.mainCameraOrtho[1], .9f));
        sun.GetComponent<Renderer>().material.color = solar.color;
        sun.GetComponent<Renderer>().enabled = false;

        pointsMax = solar.satelites.Count + 1;
        points = new ParticleSystem.Particle[pointsMax];

        points[0].position = Vector3.zero;
        points[0].startColor = solar.color;
        points[0].startSize = 2000;

        var info = sun.GetComponent<PlanetInfo>();
        info.solar = solar;

        //Destroy Icons
        ClearIcons();

        planets = new List<PlanetCreator>();
        for (int i = 0; i < solar.satelites.Count; i++)
        {
            SolarBody body = solar.satelites[i];
            Vector3 position = body.GamePosition(game.data.date.time);
            planets.Add(Instantiate(planetObj, transform));
            planets[i].name = body.name;
            planets[i].transform.position = position;
            planets[i].planet.GetComponent<Renderer>().material.color = new Color(body.color.r, body.color.g,body.color.b, .1f);
            //planets[i].planet.transform.localScale = Vector3.one * (float) (body.SOI() / Position.SystemConversion[1]);
            //planets[i].planet.layer = 9;
            planets[i].planet.GetComponent<Renderer>().enabled = false;
            //planets[i].GetComponent<SpriteRenderer>().sortingOrder = 4;
            planets[i].SetPlanetSize( sun.transform.localScale.x * .5f);
              
            info = planets[i].planet.GetComponent<PlanetInfo>();
            info.solar = body;

            //Create station indicator
            if (body.HasStations)
            {
                CreateIcon(i, planets[i].gameObject);
            }

            points[i + 1].position = position;
            points[i+1].startColor = solar.satelites[i].color;
            points[i+1].startSize = 1000;

            LineRenderer line = planets[i].planet.GetComponent<LineRenderer>();

            //Creates the line rendering for the orbit of planets

            Vector3[] positions = new Vector3[361];
            double time = game.data.date.time;
            body.SetOrbit(time, solar.mass);


            for (var b = 0; b < 360; b++)
            {
                positions[b] = body.approximatePositions[b];
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

            line.widthMultiplier = sun.transform.localScale.x * .5f;
        }
        particles.SetParticles(points, points.Length);
        //StartCoroutine("UpdateSolarObjects");
    }

    private void CreateIcon(int i, GameObject obj)
    {
        //Create station indicator
        if (solar.satelites[i].HasStations)
        {
            var icon = Instantiate(hasStation, iconsCanvas.transform);
            icon.GetComponent<IconObjectTrack>().SetTarget(obj, solarCamera.mainCamera);
            icons.Add(icon.gameObject);
        }
    }

    public void DestroySystem()
    {
        //if (model.nameText != "" && model.nameText != null)
        //{
        //    nameButton.enabled = false;
        //}
        //StopAllCoroutines();
        //game.nameOfSystem.text = game.data.galaxyName;

        //transform.localScale = Vector3.one * (float)Mathd.Pow((model.solar.bodyRadius), .01f);
        //model.localScale = (float)Mathd.Pow((model.solar.bodyRadius), .01f);
        Destroy(sun);
        for (int i = 0; i < planets.Count; i++)
        {
            Destroy(planets[i].gameObject);

        }
        //Destroy ships
        for (int i = 0; i < ships.Count; i++)
        {
            Destroy(ships[i].gameObject);

        }
        ships = new List<TooltipShip>();
        planets = new List<PlanetCreator>();
        control = false;
        solarCamera.cameraControl = false;
        particles.SetParticles(points, 0);
    }

    public void SelectPlanet(SolarBody planet)
    {
        //points[0].startSize = (float)(solar.bodyRadius / 500);
        for (int i = 1; i < pointsMax; i++)
        {
            
            LineRenderer line = planets[i - 1].planet.GetComponent<LineRenderer>();
            line.widthMultiplier = 0;
            points[i].startSize = (float)(Mathd.Pow(solar.satelites[i - 1].bodyRadius, .5));
        }
        points[planet.ReferenceBody.satelites.FindIndex(x => x.name == planet.name) + 1].startSize = 0;
        particles.SetParticles(points, points.Length);
        ClearIcons();
    }

    public void SelectSolarView()
    {
        ClearIcons();
        GalaxyView.instance.ClearIcons();
        points[0].startSize = 2000;
        for (int i = 1; i < pointsMax; i++)
        {
            points[i].startSize = 1000;
            LineRenderer line = planets[i-1].planet.GetComponent<LineRenderer>();
            line.widthMultiplier = sun.transform.localScale.x * .5f;
            CreateIcon(i-1, planets[i - 1].gameObject);
        }
        particles.SetParticles(points, points.Length);
        solarCamera.SetCameraControlTrue();
        PlanetView.instance.DestroySystem();
        NormalView.instance.DestroySystem();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetView : MonoBehaviour {

    public PlanetCreator planetObj;
    public SatelliteIconCreator structureObj;
    public TooltipShip shipObj;
    public Light sunLight;
    public MapCamera planetCamera;
    public MapCamera solarCamera;
    public MapCamera galaxyCamera; 

    internal static PlanetView instance;

    internal GameManager game;
    internal ViewManager galaxy;
    

    internal PlanetCreator mainPlanet;
    internal List<PlanetCreator> moons = new List<PlanetCreator>();
    internal List<TooltipShip> ships = new List<TooltipShip>();
    internal bool control = false;

    private SolarBody solar;

    private ParticleSystem particles;
    private ParticleSystem.Particle[] points;
    private int pointsMax;

    //private float solarSpriteScale = .02f;

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
            solarCamera.SetCameraView((solar.GamePosition(game.data.date.time) + (planetCamera.mainCamera.transform.position) / (float)Position.SystemConversion[1] * (float)Position.SystemConversion[2]), planetCamera.mainCamera.transform.rotation, planetCamera.mainCamera.transform.localScale, planetCamera.mainCamera.fieldOfView);

            //Controls direction of directional light
            sunLight.transform.LookAt(solarCamera.mainCamera.transform);

            for (int i = 1; i < pointsMax; i++)
            {
                points[i].position = solar.satelites[i - 1].GamePosition(game.data.date.time);
                moons[i - 1].transform.position = solar.satelites[i - 1].lastKnownPosition;
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
                        shipTooltip.transform.position = (Vector3)(ship.referencePosition / Position.SystemConversion[2]);
                    }
                    else
                    {
                        //Create ship
                        TooltipShip shipTooltip = Instantiate(shipObj, this.transform);
                        shipTooltip.SetLayer(10);
                        shipTooltip.ship = ship;
                        ships.Add(shipTooltip);
                        shipTooltip.transform.position = (Vector3)(ship.referencePosition / Position.SystemConversion[2]);

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

    public void CreatePlanetSystem(SolarBody _solar)
    {
        NormalView.instance.DestroySystem();
        DestroySystem();
        solar = _solar;
        control = true;

        planetCamera.SetCameraControlTrue();
        solarCamera.SetCameraView((solar.GamePosition(game.data.date.time) + (planetCamera.mainCamera.transform.position) / (float)Position.SystemConversion[1] * (float) Position.SystemConversion[2]), planetCamera.mainCamera.transform.rotation, planetCamera.mainCamera.transform.localScale, planetCamera.mainCamera.fieldOfView);
        //game.nameOfSystem.text = solar.name;
        sunLight.colorTemperature = (float) solar.Star.surfaceTemp;
        transform.localScale = Vector3.one;
        mainPlanet = Instantiate(planetObj, transform);
        mainPlanet.name = solar.name;
        mainPlanet.transform.localPosition = Vector3.zero;
        mainPlanet.SetPlanetSize((float)((solar.bodyRadius) / Position.SystemConversion[2])) ;//* Mathd.Pow(game.data.mainCameraOrtho[1], .9f));
        mainPlanet.planet.GetComponent<Renderer>().material.color = solar.color;
        mainPlanet.gameObject.layer = 10;
        mainPlanet.planet.layer = 10;
        pointsMax = solar.satelites.Count + 1;
        points = new ParticleSystem.Particle[pointsMax];

        points[0].position = Vector3.zero;
        points[0].startColor = solar.color;
        points[0].startSize = 1;

        var info = mainPlanet.planet.GetComponent<PlanetInfo>();
        info.solar = solar;

        //Set up any station sprites
        for (int i = 0; i < solar.structureIds.Count; i++)
        {
            var structure = GameManager.instance.locations[solar.structureIds[i]] as Structure;
            if (structure.GetType() != typeof(Ship))
            {
                var obj = Instantiate(structureObj, mainPlanet.transform);

                obj.transform.localPosition = (Vector3)(structure.referencePosition / Position.SystemConversion[2]);
                obj.SetTarget(structure);
            }
        }

            moons = new List<PlanetCreator>();
        for (int i = 0; i < solar.satelites.Count; i++)
        {
            SolarBody body = solar.satelites[i];
            Vector3 position = body.GamePosition(game.data.date.time);
            moons.Add(Instantiate(planetObj, transform));
            moons[i].name = body.name;
            moons[i].transform.position = position;
            moons[i].planet.GetComponent<Renderer>().material.color = body.color;
            moons[i].planet.GetComponent<Renderer>().enabled = true;
            moons[i].gameObject.layer = 10;
            moons[i].planet.layer = 10;
            //planets[i].GetComponent<SpriteRenderer>().sortingOrder = 4;
            moons[i].SetPlanetSize((float)((body.bodyRadius) / Position.SystemConversion[2]));

            info = moons[i].planet.GetComponent<PlanetInfo>();
            info.solar = body;

            //Set up any station sprites
            for (int b = 0; b < body.structureIds.Count; b++)
            {
                var structure = GameManager.instance.locations[solar.structureIds[b]] as Structure;
                if (structure.GetType() != typeof(Ship))
                {
                    var obj = Instantiate(structureObj, moons[i].transform);
                    obj.name = structure.name;
                    obj.transform.localPosition = (Vector3)(structure.referencePosition / Position.SystemConversion[2]);
                    obj.SetTarget(structure);
                }
                
            }

            points[i + 1].position = position;
            points[i + 1].startColor = solar.satelites[i].color;
            points[i + 1].startSize = 10;

            LineRenderer line = moons[i].planet.GetComponent<LineRenderer>();

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

            line.widthMultiplier = mainPlanet.transform.localScale.x * .1f;
        }
        particles.SetParticles(points, points.Length);
        //StartCoroutine("UpdateSolarObjects");
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
        if (mainPlanet != null)
            Destroy(mainPlanet.gameObject);
        if (moons.Count > 0)
        {
            for (int i = 0; i < moons.Count; i++)
            {
                Destroy(moons[i].gameObject);

            }
            //particles.SetParticles(points, 0);
            moons = new List<PlanetCreator>();
        }
        //Destroy ships
        for (int i = 0; i < ships.Count; i++)
        {
            Destroy(ships[i].gameObject);

        }
        ships = new List<TooltipShip>();
        planetCamera.cameraControl = false;
        control = false;
        particles.SetParticles(points, 0);
    }

    public void SelectStructure(Structure structure)
    {
        //points[0].startSize = (float)(solar.bodyRadius / 500);
        for (int i = 1; i < pointsMax; i++)
        {

            LineRenderer line = moons[i - 1].planet.GetComponent<LineRenderer>();
            line.widthMultiplier = 0;
            points[i].startSize = (float)(Mathd.Pow(solar.satelites[i - 1].bodyRadius, .5));
        }
        //points[structure.ReferenceBody.satelites.FindIndex(x => x.name == structure.name) + 1].startSize = 0;
        if (structure.ReferenceBody.name == mainPlanet.name)
        {
            mainPlanet.gameObject.SetActive(false);
        }
        particles.SetParticles(points, points.Length);
    }

    public void SelectPlanetView()
    {
        mainPlanet.gameObject.SetActive(true);
        points[0].startSize = 1;
        for (int i = 1; i < pointsMax; i++)
        {
            points[i].startSize = 10;
            LineRenderer line = moons[i - 1].planet.GetComponent<LineRenderer>();
            line.widthMultiplier = mainPlanet.transform.localScale.x * .1f;
        }
        particles.SetParticles(points, points.Length);
        planetCamera.SetCameraControlTrue();
        NormalView.instance.DestroySystem();
    }
}

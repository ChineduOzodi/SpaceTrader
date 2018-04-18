using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalView : MonoBehaviour {

    public PlanetCreator planetObj;
    public GameObject stationPrefab;
    public Camera moonViewCamera;
    public MapCamera normalCamera;
    public MapCamera satelliteCamera;
    public MapCamera planetCamera;
    public MapCamera solarCamera;
    public MapCamera galaxyCamera;

    internal static NormalView instance;

    internal GameManager game;
    internal ViewManager galaxy;
    

    internal PlanetCreator mainPlanet;
    internal GameObject station;
    //internal List<GameObject> moons = new List<GameObject>();
    internal bool control = false;

    private SolarBody solar;
    private Structure structure;

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
	void Update () {
		if (control)
        {
            planetCamera.SetCameraView((Vector3)((structure.referencePosition / Position.SystemConversion[2]) + ((Vector3d)normalCamera.mainCamera.transform.position) / Position.SystemConversion[2] * Position.SystemConversion[4]), normalCamera.mainCamera.transform.rotation, normalCamera.mainCamera.transform.localScale, normalCamera.mainCamera.fieldOfView);
            satelliteCamera.SetCameraView((Vector3)((structure.referencePosition / Position.SystemConversion[3]) + ((Vector3d)normalCamera.mainCamera.transform.position) / Position.SystemConversion[3] * Position.SystemConversion[4]), normalCamera.mainCamera.transform.rotation, normalCamera.mainCamera.transform.localScale, normalCamera.mainCamera.fieldOfView);

            //for (int i = 1; i < pointsMax; i++)
            //{
            //    points[i].position = solar.satelites[i - 1].GamePosition(game.data.date.time);
            //    moons[i - 1].transform.position = solar.satelites[i - 1].lastKnownPosition;
            //}
            //particles.SetParticles(points, points.Length);
        }
    }

    public void CreateNormalView(Structure _structure)
    {
        DestroySystem();
        structure = _structure;
        solar = structure.ReferenceBody;

        control = true;
        normalCamera.SetCameraControlTrue();
        planetCamera.SetCameraView((Vector3)((structure.referencePosition / Position.SystemConversion[2])  + ((Vector3d)normalCamera.mainCamera.transform.position) / Position.SystemConversion[2] * Position.SystemConversion[4]), normalCamera.mainCamera.transform.rotation, normalCamera.mainCamera.transform.localScale, normalCamera.mainCamera.fieldOfView);
        satelliteCamera.SetCameraView((Vector3)((structure.referencePosition / Position.SystemConversion[3]) + ((Vector3d)normalCamera.mainCamera.transform.position) / Position.SystemConversion[3] * Position.SystemConversion[4]), normalCamera.mainCamera.transform.rotation, normalCamera.mainCamera.transform.localScale, normalCamera.mainCamera.fieldOfView);
        //game.nameOfSystem.text = solar.name;
        transform.localScale = Vector3.one;
        mainPlanet = Instantiate(planetObj, transform);
        mainPlanet.name = solar.name;
        mainPlanet.transform.localPosition = Vector3.zero;// (Vector3)(-structure.referencePosition / Position.SystemConversion[3]);
        mainPlanet.SetPlanetSize((float)((solar.bodyRadius) / Position.SystemConversion[3])) ;//* Mathd.Pow(game.data.mainCameraOrtho[1], .9f));
        //mainPlanet.planetGetComponent<Renderer>().material.color = solar.color;
        mainPlanet.gameObject.layer = 11;
        mainPlanet.planet.layer = 11;

        //Create Station
        station = Instantiate(stationPrefab, transform);
        station.name = structure.name;
        station.transform.localPosition = Vector3.zero;
        station.gameObject.layer = 12;

        //pointsMax = solar.satelites.Count + 1;
        //points = new ParticleSystem.Particle[pointsMax];

        //points[0].position = Vector3.zero;
        //points[0].startColor = solar.color;
        //points[0].startSize = 1;

        var info = mainPlanet.planet.GetComponent<PlanetInfo>();
        info.solar = solar;

        //moons = new List<GameObject>();
        //for (int i = 0; i < solar.satelites.Count; i++)
        //{
        //    SolarBody body = solar.satelites[i];
        //    Vector3 position = body.GamePosition(game.data.date.time);
        //    moons.Add(Instantiate(planetObj, transform));
        //    moons[i].name = body.name;
        //    moons[i].transform.position = position;
        //    moons[i].GetComponent<Renderer>().material.color = body.color;
        //    moons[i].GetComponent<Renderer>().enabled = true;
        //    moons[i].layer = 10;
        //    //planets[i].GetComponent<SpriteRenderer>().sortingOrder = 4;
        //    moons[i].transform.localScale = Vector3.one * (float)((body.bodyRadius) / Position.SystemConversion[2]);

        //    info = moons[i].GetComponent<PlanetInfo>();
        //    info.solar = body;

        //    points[i + 1].position = position;
        //    points[i + 1].startColor = solar.satelites[i].color;
        //    points[i + 1].startSize = 10;

        //    LineRenderer line = moons[i].GetComponent<LineRenderer>();

        //    //Creates the line rendering for the orbit of planets

        //    Vector3[] positions = new Vector3[361];
        //    double time = game.data.date.time;
        //    body.SetOrbit(time, solar.mass);


        //    for (var b = 0; b < 360; b++)
        //    {
        //        positions[b] = body.approximatePositions[b];
        //    }
        //    line.positionCount = 360;
        //    line.SetPositions(positions);


        //    if (body.solarSubType == SolarSubType.GasGiant)
        //    {
        //        Color col = Color.blue;
        //        col.a = .1f;
        //        line.startColor = col;
        //        line.endColor = col;
        //    }
        //    else if (body.solarType == SolarType.DwarfPlanet)
        //    {
        //        Color col = Color.yellow;
        //        col.a = .1f;
        //        line.startColor = col;
        //        line.endColor = col;
        //    }
        //    else if (body.solarType == SolarType.Comet)
        //    {
        //        Color col = Color.white;
        //        col.a = .1f;
        //        line.startColor = col;
        //        line.endColor = col;
        //    }
        //    else
        //    {
        //        Color col = Color.green;
        //        col.a = .1f;
        //        line.startColor = col;
        //        line.endColor = col;
        //    }

        //    line.widthMultiplier = mainPlanet.transform.localScale.x * .1f;
        //}
        //particles.SetParticles(points, points.Length);
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
        if (mainPlanet == null) return;
        Destroy(mainPlanet.gameObject);
        Destroy(station);
        //for (int i = 0; i < moons.Count; i++)
        //{
        //    Destroy(moons[i]);

        //}
        //particles.SetParticles(points, 0);
        //moons = new List<GameObject>();
        normalCamera.cameraControl = false;
        control = false;
        //particles.SetParticles(points, 0);
    }

    public void SelectStructure(Structure structure)
    {
        //points[0].startSize = (float)(solar.bodyRadius / 500);
        //for (int i = 1; i < pointsMax; i++)
        //{

        //    LineRenderer line = moons[i - 1].GetComponent<LineRenderer>();
        //    line.widthMultiplier = 0;
        //    points[i].startSize = (float)(Mathd.Pow(solar.satelites[i - 1].bodyRadius, .5));
        //}
        //points[structure.ReferenceBody.satelites.FindIndex( x => x.name == structure.name) + 1].startSize = 0;
        //particles.SetParticles(points, points.Length);
    }

    public void SelectNormalView()
    {

        //points[0].startSize = 1;
        //for (int i = 1; i < pointsMax; i++)
        //{
        //    points[i].startSize = 10;
        //    LineRenderer line = moons[i - 1].GetComponent<LineRenderer>();
        //    line.widthMultiplier = mainPlanet.transform.localScale.x * .1f;
        //}
        //particles.SetParticles(points, points.Length);
        normalCamera.SetCameraControlTrue();
    }
}

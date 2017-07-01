using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeControl;
using System.IO;

public class OrbitTest : MonoBehaviour {

    public Text text;
    public InputField inputField;
    public GameObject planetObject;
    private Orbit orbit;
    private Dated date;
    private List<OrbitTestModel> models;
    private List<GameObject> modelObjects;

    public double G = 6.67408;
    public double Mass = 5.97237;
    public double SemiMajor = 1.496;
    public double ECC = .016708;
    public double MeanAnom = 358.617 * Mathd.Deg2Rad;
    public double Longitude = 288.1 * Mathd.Deg2Rad;
    public double SunMass = 1.989;
    public bool showOrbit = true;

    internal float timeScale = 1;

	// Use this for initialization
	void Start () {
        

        modelObjects = new List<GameObject>();
        models = new List<OrbitTestModel>();
        date = new Dated(2017,6,33,20,4,0);

        Orbit sun = new Orbit(SunMass * Mathd.Pow(10, 30));
        OrbitTestModel sunModel = new OrbitTestModel(sun);
        models.Add(sunModel);
        

        orbit = new Orbit(Mass * Orbit.massConversion, SemiMajor * Orbit.distanceConversion, ECC, MeanAnom,0, Longitude, sun.Mass);
        orbit.G = G;
        OrbitTestModel orbitModel = new OrbitTestModel(orbit);
        models.Add(orbitModel);
        orbitModel.parent = new ModelRef<OrbitTestModel>(sunModel);
        InstanceModels();
        
	}
	
	// Update is called once per frame
	void Update () {
        if (models.Count > 1)
        {
            date.AddTime(Time.deltaTime * timeScale);
            text.text = date.GetFormatedDateTime() + "\nTimescale: " + timeScale + "\ndeltaTime: "+ Time.deltaTime + "\nTime: " + date.time
                + "\nPosition: " + models[1].orbit.Radius(date.time).x + " - " + models[1].orbit.Radius(date.time).y
                + "\n" + models[1].orbit.GetOrbitalInfo(date.time);

            UpdateModelPositions();

            if (Input.GetKeyDown(KeyCode.Period))
            {
                timeScale *= 10;
            }

            if (Input.GetKeyDown(KeyCode.Comma))
            {
                timeScale *= .1f;
            }

            if (Input.GetKeyDown(KeyCode.Slash))
            {
                timeScale *= -1;
            }
        }
    }
        

    private void UpdateModelPositions()
    {
        for (var i = 0; i < models.Count; i++)
        { 
            modelObjects[i].transform.position = models[i].orbit.Radius(date.time);

            if (showOrbit)
            {
                LineRenderer line = modelObjects[i].GetComponent<LineRenderer>();

                Vector3[] positions = new Vector3[101];

                double timeInc = (double)(orbit.OrbitalPeriod / 100);
                double time = date.time;

                for (var b = 0; b < 101; b++)
                {
                    positions[b] = orbit.Radius(time);
                    time += timeInc;
                }
                line.positionCount = 101;
                line.SetPositions(positions);
            }
        }
    }

    private void InstanceModels()
    {
        foreach (var model in models)
        {
            GameObject planet = Instantiate(planetObject);
            planet.transform.position = model.orbit.Radius(date.time);
            modelObjects.Add(planet);
        }
    }
    private void DeleteModelObjs()
    {
        foreach(var modObj in modelObjects)
        {
            Destroy(modObj);
        }
        modelObjects = new List<GameObject>();
    }

    public void ChangeParam()
    {
        models[0].orbit = new Orbit() { Mass = SunMass * Mathd.Pow(10, 30) };
        models[1].orbit = new Orbit(Mass * Orbit.massConversion, SemiMajor * Orbit.distanceConversion, ECC, MeanAnom, 0, Longitude, models[0].orbit.Mass);
        models[1].orbit.G = G * Mathd.Pow(10, -11);
    }
    public void SaveOrbitTest()
    {
        string path = Application.dataPath + "orbitTestSave.xml";
        if (!File.Exists(path))
        {
            // Function for overriting saves.
        }

        ModelBlobs save = Model.SaveAll();
        File.WriteAllText(path, save.ToString());

        
    }

    public void LoadOrbitTest()
    {
        string path = Application.dataPath + "orbitTestSave.xml";
        ModelBlobs save = ModelBlobs.FromString(File.ReadAllText(path));
        Model.Load(save, OnLoadStart, OnLoadProgress, OnLoadDone, OnLoadError);
    }

    private void OnLoadStart()
    {
        Model.DeleteAll();
        DeleteModelObjs();
    }

    private void OnLoadProgress(float p)
    {
        
    }

    private void OnLoadDone()
    {
        models = Model.GetAll<OrbitTestModel>();
        InstanceModels();
    }

    private void OnLoadError(string obj)
    {
        print("Load error: " + obj);
    }
}
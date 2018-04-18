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
    internal List<GameObject> moons = new List<GameObject>();
    internal List<SolarBody> moonModels = new List<SolarBody>();
    internal GameManager game;
    internal ViewManager galaxy;
    private SpriteRenderer sprite;
    private CircleCollider2D circleCollider;
    public float viewPopulationSize = 3f;
    public float viewCompanySize = 1;
    private float solarSpriteScale = .02f;
    private float moonViewOrthoSize = .001f;
    private float lineSize = 2;
    //private VectorObject3D line;
    //private VectorLine solarLine;
    public Texture lineTexture;
    

    internal double populationUpdateProgress = 0;
    internal double populationUpdateTime = Dated.Day;

    private Vector3 lastCamPosition = Vector3.zero;
    private bool isActive;

    protected override void OnInitialize()
    {
        lastCamPosition.z = -10;
        game = GameManager.instance;
        galaxy = ViewManager.instance;
        transform.position = (Vector3) model.solar.referencePosition;
        name = model.name;
        circleCollider = GetComponent<CircleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = model.solar.color;
        transform.localScale = Vector3.one;
        var points = new List<Vector3>();

        //solarLine = new VectorLine(model.name+ " Connections", points, (float)(Mathd.Pow((model.solar.bodyRadius), .02f) * game.data.mainCameraOrtho[0]));
        //line.SetVectorLine(vectorLine, lineTexture, sprite.material);
        //sprite.enabled = false;

        //staggers the updating of supplydemand and population updates
        populationUpdateProgress = UnityEngine.Random.Range(0, 300);
    }
	
	// Update is called once per frame
	//void Update () {
 //       if (true)
 //       {
 //           circleCollider.radius = (float)(Mathd.Pow((model.solar.bodyRadius), .02f) * game.data.mainCameraOrtho[0] * 5);
 //           //Population
 //           populationUpdateProgress += game.data.date.deltaTime;
 //           if (populationUpdateProgress > populationUpdateTime)
 //           {

 //               for (int i = 0; i < model.solar.satelites.Count; i++)
 //               {
 //                   SolarBody body = model.solar.satelites[i];
 //                   if (body.inhabited)
 //                   {
 //                       body.population.Update(body, populationUpdateProgress);
 //                       if (body.sateliteInhabited)
 //                       {
 //                           for (int m = 0; m < body.satelites.Count; m++)
 //                           {
 //                               SolarBody moon = body.satelites[m];
 //                               if (moon.inhabited)
 //                               {
 //                                   moon.population.Update(moon, populationUpdateProgress);
 //                               }
 //                           }
 //                       }
 //                   }

 //               }
 //               populationUpdateProgress = 0;
 //           }


 //           //List<Vector3> linePoints = new List<Vector3>();
 //           //List<Color32> lineColors = new List<Color32>();
 //           //List<float> lineWidths = new List<float>();
 //           if (isActive)
 //           {
 //               if (game.data.mainCamerViewMode == ViewMode.Planet)
 //               {
 //                   sun.SetActive(true);

 //                   var localScale = (float)(Mathd.Pow((model.solar.bodyRadius), solarSpriteScale) * Mathd.Pow(game.data.mainCameraOrtho[1], .9f));

 //                   if (localScale * game.data.mainCameraOrtho[1] / Camera.main.orthographicSize * GameDataModel.galaxyDistanceMultiplication < model.solar.bodyRadius)
 //                   {
 //                       localScale = (float)(model.solar.bodyRadius / GameDataModel.galaxyDistanceMultiplication * Camera.main.orthographicSize / game.data.mainCameraOrtho[1]);
 //                   }
 //                   sun.transform.localScale = Vector3.one * localScale;

 //               }
 //               else
 //               {
 //                   sun.SetActive(false);
 //               }
 //               for (int i = 0; i < model.solar.satelites.Count; i++)
 //               {
 //                   SolarBody body = model.solar.satelites[i];

 //                   if (game.data.mainCamerViewMode == ViewMode.Planet)
 //                   {
 //                       planets[i].SetActive(true);

 //                       var localScale = (float)(Mathd.Pow((body.bodyRadius), solarSpriteScale) * Mathd.Pow(game.data.mainCameraOrtho[1], .9f));
 //                       if (localScale * game.data.mainCameraOrtho[1] / Camera.main.orthographicSize * GameDataModel.galaxyDistanceMultiplication < body.bodyRadius)
 //                       {
 //                           localScale = (float)(body.bodyRadius / GameDataModel.galaxyDistanceMultiplication * Camera.main.orthographicSize / game.data.mainCameraOrtho[1]);
 //                       }
 //                       planets[i].transform.localScale = Vector3.one * localScale;

 //                       planets[i].transform.position = body.lastKnownPosition;

 //                       //Orbit Line Rendering
 //                       LineRenderer line = planets[i].GetComponent<LineRenderer>();

 //                       if (MapTogglePanel.instance.solarOrbits.isOn)
 //                       {
 //                           line.widthMultiplier = planets[i].transform.localScale.x * .3f;
 //                       }
 //                       else
 //                       {
 //                           line.widthMultiplier = 0;
 //                       }

 //                       //Population rendering
 //                       if (MapTogglePanel.instance.populations.isOn && (body.inhabited || body.sateliteInhabited))
 //                       {
 //                           if (body.inhabited || body.sateliteInhabited)
 //                           {
 //                               //linePoints.Add(planets[i].transform.position + Vector3.up * planets[i].transform.localScale.x * .5f);
 //                               //linePoints.Add(planets[i].transform.position + Vector3.up * planets[i].transform.localScale.x * .5f + Vector3.up * viewPopulationSize);
 //                               //lineColors.Add(new Color32(100, 100, 255, 200));
 //                               //lineWidths.Add(localScale);
 //                           }
 //                       }

 //                       //Companies rendering
 //                       if (MapTogglePanel.instance.compines.isOn)
 //                       {
 //                           //linePoints.Add(planets[i].transform.position + Vector3.right * planets[i].transform.localScale.x * .5f);
 //                           //linePoints.Add(planets[i].transform.position + Vector3.right * planets[i].transform.localScale.x * .5f + Vector3.right * Mathf.Pow(body.companies.Count, viewCompanySize) * lineSize);

 //                           //var index = linePoints.Count - 1;
 //                           //body.satelites.ForEach(x => linePoints[index] += Vector3.right * Mathf.Pow(x.companies.Count, viewCompanySize));

 //                           //lineColors.Add(new Color32(255, 50, 255, 150));
 //                           //lineWidths.Add(localScale);
 //                       }
 //                   }
 //                   else if (game.data.mainCamerViewMode == ViewMode.Moon && CheckCameraVisibility(body.solarIndex))
 //                   {
 //                       planets[i].SetActive(true);
 //                       var localScale = (float)(Mathd.Pow((body.bodyRadius), solarSpriteScale) * Mathd.Pow(game.data.mainCameraOrtho[1], .9f));
 //                       if (localScale * game.data.mainCameraOrtho[1] / Camera.main.orthographicSize * GameDataModel.galaxyDistanceMultiplication < body.bodyRadius)
 //                       {
 //                           localScale = (float)(body.bodyRadius / GameDataModel.galaxyDistanceMultiplication * Camera.main.orthographicSize / game.data.mainCameraOrtho[1]);
 //                       }
 //                       planets[i].transform.localScale = Vector3.one * localScale;

 //                       planets[i].transform.position = Vector3.zero;
 //                   }
 //                   else
 //                   {
 //                       planets[i].SetActive(false);
 //                   }

 //               }

 //               for (int i = 0; i < moons.Count; i++)
 //               {
 //                   SolarBody body = moonModels[i];


 //                   if (game.data.mainCameraSolarIndex[1] == body.solarIndex[1] && game.data.mainCamerViewMode == ViewMode.Moon) //Checks to see if the camera is focusing on that specific planet
 //                   {
 //                       moons[i].SetActive(true);

 //                       moons[i].transform.position = body.lastKnownPosition;

 //                       var visible = CheckVisibility(body);
 //                       moons[i].SetActive(visible);
 //                       LineRenderer line = moons[i].GetComponent<LineRenderer>();

 //                       //moons[i].transform.localScale = sun.transform.localScale * .15f;
 //                       var localScale = (float)(Mathd.Pow((body.bodyRadius), solarSpriteScale) * Mathd.Pow(game.data.mainCameraOrtho[1], .5f));
 //                       if (localScale * game.data.mainCameraOrtho[1] / Camera.main.orthographicSize * GameDataModel.galaxyDistanceMultiplication < body.bodyRadius)
 //                       {
 //                           localScale = (float)(body.bodyRadius / GameDataModel.galaxyDistanceMultiplication * Camera.main.orthographicSize / game.data.mainCameraOrtho[1]);
 //                       }
 //                       moons[i].transform.localScale = Vector3.one * localScale;

 //                       //Creates the line rendering for the orbit of moons

 //                       if (MapTogglePanel.instance.solarOrbits.isOn)
 //                       {
 //                           line.widthMultiplier = moons[i].transform.localScale.x * .3f;
 //                       }
 //                       else
 //                       {
 //                           line.widthMultiplier = 0;
 //                       }

 //                       if (moons[i].activeSelf)
 //                       {
 //                           //Population rendering
 //                           if (MapTogglePanel.instance.populations.isOn && body.inhabited)
 //                           {
 //                               //linePoints.Add(moons[i].transform.position + Vector3.up * moons[i].transform.localScale.x * .5f);
 //                               //linePoints.Add(moons[i].transform.position + Vector3.up * moons[i].transform.localScale.x * .5f + Vector3.up * viewPopulationSize);
 //                               //lineColors.Add(new Color32(100, 100, 255, 200));
 //                               //lineWidths.Add(localScale);
 //                           }

 //                           //Companies rendering
 //                           if (MapTogglePanel.instance.compines.isOn)
 //                           {
 //                               //linePoints.Add(moons[i].transform.position + Vector3.right * moons[i].transform.localScale.x * .5f);
 //                               //linePoints.Add(moons[i].transform.position + Vector3.right * moons[i].transform.localScale.x * .5f + Vector3.right * Mathf.Pow(body.companies.Count, viewCompanySize) * lineSize);

 //                               //lineColors.Add(new Color32(255, 50, 255, 150));
 //                               //lineWidths.Add(localScale);
 //                           }
 //                       }

 //                   }
 //                   else
 //                   {
 //                       moons[i].SetActive(false);
 //                   }
 //               }


 //               //if (game.data.mainCameraOrtho[1] * GameDataModel.galaxyDistanceMultiplication > 1.5 * Units.ly)
 //               //{
 //               //    galaxy.GalaxyView();
 //               //}
 //           }
 //           else
 //           {
 //               if ((game.data.mainCameraPosition[0] - transform.position).sqrMagnitude < 40000)
 //               {
 //                   if (MapTogglePanel.instance.galaxyConnections.isOn)
 //                   {
 //                       foreach (SolarModel solar in model.nearStars)
 //                       {
 //                           //linePoints.Add(transform.position);
 //                           //linePoints.Add(solar.position[0]);
 //                           //lineWidths.Add((float)circleCollider.radius);
 //                           //if (MapTogglePanel.instance.galaxyTerritory.isOn)
 //                           //{
 //                           //    if (model.government.Model != null)
 //                           //    {
 //                           //        var govModel = model.government.Model;
 //                           //        lineColors.Add(new Color32((byte)(govModel.spriteColor.r * 255), (byte)(govModel.spriteColor.g * 255), (byte)(govModel.spriteColor.b * 255), 50));
 //                           //    }
 //                           //    else
 //                           //    {
 //                           //        lineColors.Add(new Color32((byte)(50), (byte)(50), (byte)(50), 10));
 //                           //    }

 //                           //}
 //                           //else
 //                           //{
 //                           //    lineColors.Add(new Color32((byte)(model.solar.color.r * 255), (byte)(model.solar.color.g * 255), (byte)(model.solar.color.b * 255), 10));
 //                           //}
 //                       }

 //                   }
 //                   else
 //                   {
 //                       //if (solarLine.lineWidth != 0)
 //                       //{
 //                       //    solarLine.SetWidth(0);
 //                       //    solarLine.Draw3D();
 //                       //}
 //                   }

 //                   //Population rendering
 //                   if (MapTogglePanel.instance.populations.isOn && model.solar.inhabited)
 //                   {
 //                       //linePoints.Add(transform.position + Vector3.up * circleCollider.radius * .5f);
 //                       //linePoints.Add(transform.position + Vector3.up * circleCollider.radius + Vector3.up * viewPopulationSize);
 //                       //lineColors.Add(new Color32(100, 100, 255, 200));
 //                       //lineWidths.Add(circleCollider.radius * 2f);
 //                   }
 //               }
 //               else
 //               {
 //                   //if (solarLine.lineWidth != 0)
 //                   //{
 //                   //    solarLine.SetWidth(0);
 //                   //    solarLine.Draw3D();
 //                   //}
 //               }
 //           }
 //           //Stats Display
 //           //if (linePoints.Count > 0)
 //           //{
 //           //    solarLine.points3 = linePoints;
 //           //    solarLine.SetWidths(lineWidths);
 //           //    solarLine.SetColors(lineColors);
 //           //    solarLine.Draw3D();
 //           //}
 //           //else
 //           //{
 //           //    solarLine.points3 = new List<Vector3>();
 //           //    solarLine.lineWidth = 0;
 //           //    solarLine.Draw();
 //           //}
 //       }
        
		
	//}

 //   private void LateUpdate()
 //   {
 //       //transform.forward = Camera.main.transform.forward;
 //   }

 //   //public IEnumerator UpdateSolarObjects()
 //   //{
 //   //    while (model.isActive)
 //   //    {
 //   //        for (int i = 0; i < model.solar.satelites.Count; i++)
 //   //        {
 //   //            SolarBody body = model.solar.satelites[i];
 //   //            Vector2 position = CameraController.CameraOffsetGalaxyPosition(model.position + body.GamePosition(game.data.date.time));

 //   //            Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
 //   //            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

 //   //            if (onScreen)
 //   //            {
 //   //                var visible = CheckVisibility(body);

 //   //                planets[i].SetActive(visible);

 //   //                planets[i].transform.position = position;

 //   //                LineRenderer line = planets[i].GetComponent<LineRenderer>();

 //   //                //Creates the line rendering for the orbit of planets

 //   //                Vector3[] positions = new Vector3[361];
 //   //                double time = game.data.date.time;
 //   //                body.SetOrbit(time, model.solar.mass);


 //   //                for (var b = 0; b < 360; b++)
 //   //                {
 //   //                    positions[b] = CameraController.CameraOffsetGalaxyPosition(model.position + body.approximatePositions[b]);
 //   //                }
 //   //                line.positionCount = 360;
 //   //                line.SetPositions(positions);

 //   //                yield return null;
 //   //            }
 //   //        }

 //   //        for (int i = 0; i < moons.Count; i++)
 //   //        {
 //   //            SolarBody moon = moonModels[i];
 //   //            Vector3 position = CameraController.CameraOffsetGalaxyPosition(model.position + model.solar.satelites[moon.solarIndex[1]].lastKnownPosition
 //   //                + moon.GamePosition(game.data.date.time));

 //   //            Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
 //   //            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

 //   //            if (onScreen)
 //   //            {
 //   //                LineRenderer line = moons[i].GetComponent<LineRenderer>();

 //   //                moons[i].transform.position = position;

 //   //                //Creates the line rendering for the orbit of moons

 //   //                var positions = new Vector3[361];
 //   //                double time = game.data.date.time;
 //   //                moon.SetOrbit(time, model.solar.satelites[moon.solarIndex[1]].mass);

 //   //                for (var b = 0; b < 360; b++)
 //   //                {
 //   //                    positions[b] = CameraController.CameraOffsetGalaxyPosition(model.position + model.solar.satelites[moon.solarIndex[1]].lastKnownPosition
 //   //                + moon.approximatePositions[b]);
 //   //                }
 //   //                line.positionCount = 360;
 //   //                line.SetPositions(positions);
 //   //                yield return null;
 //   //            }

 //   //        }
 //   //        yield return null;
 //   //    }
 //   //}

 //   public bool CheckCameraVisibility(List<int> solarIndex)
 //   {
 //       if (solarIndex.Count == 2 || solarIndex.Count == 3) //Go to moon view of planet
 //       {
 //           return solarIndex[1] == game.data.mainCameraSolarIndex[1];
 //       }

 //       return false;
 //   }

 //   private bool CheckVisibility(SolarBody body)
 //   {
 //       if (body.solarType == SolarType.Planet)
 //       {
 //           if (!MapTogglePanel.instance.planet.isOn)
 //           {
 //               return false;
 //           }
 //           else
 //           {
 //               return MapTogglePanel.instance.subtypes[body.solarSubType].isOn;
 //           }
 //       }
 //       if (body.solarType == SolarType.DwarfPlanet)
 //       {
 //           if (!MapTogglePanel.instance.dwarfPlanet.isOn)
 //           {
 //               return false;
 //           }
 //           else
 //           {
 //               return MapTogglePanel.instance.subtypes[body.solarSubType].isOn;
 //           }
 //       }
 //       if (body.solarType == SolarType.Comet)
 //       {
 //           if (!MapTogglePanel.instance.comet.isOn)
 //           {
 //               return false;
 //           }
 //           else
 //           {
 //               return MapTogglePanel.instance.subtypes[body.solarSubType].isOn;
 //           }
 //       }
 //       if (body.solarType == SolarType.Asteroid)
 //       {
 //           if (!MapTogglePanel.instance.asteroid.isOn)
 //           {
 //               return false;
 //           }
 //           else
 //           {
 //               return MapTogglePanel.instance.subtypes[body.solarSubType].isOn;
 //           }
 //       }

 //       if (body.solarType == SolarType.Moon)
 //       {
 //           if (!MapTogglePanel.instance.moons.isOn)
 //           {
 //               return false;
 //           }
 //           else if (planets[body.solarIndex[1]].activeSelf)
 //           {
 //               return MapTogglePanel.instance.subtypes[body.solarSubType].isOn;
 //           }
 //           else
 //           {
 //               return false;
 //           }
 //       }
 //       return true;
 //   }

 //   internal SolarModel GetModel()
 //   {
 //       return model;
 //   }

 //   protected override void OnModelChanged()
 //   {

 //       sprite.color = model.color;
 //       if (model.isActive)
 //           transform.localScale = Vector3.one * model.localScale;
 //   }

 //   public void CreateSystem()
 //   {
 //       game.nameOfSystem.text = model.name;

 //       model.localScale = 1;
 //       transform.localScale = Vector3.one;
 //       sun = Instantiate(sunObj, transform);
 //       sun.name = model.name + " Sun";
 //       sun.transform.localPosition = Vector3.zero;
 //       sun.transform.localScale = Vector3.one * (float)(Mathd.Pow((model.solar.bodyRadius), solarSpriteScale) * Mathd.Pow(game.data.mainCameraOrtho[1], .9f));
 //       sun.GetComponent<SpriteRenderer>().color = model.solar.color;
 //       sun.GetComponent<SpriteRenderer>().sortingOrder = 5;

 //       var info = sun.GetComponent<PlanetInfo>();
 //       info.solar = model.solar;

 //       planets = new List<GameObject>();
 //       for (int i = 0; i < model.solar.satelites.Count; i++)
 //       {
 //           SolarBody body = model.solar.satelites[i];
 //           Vector3 position = body.GamePosition(game.data.date.time);
 //           planets.Add(Instantiate(planetObj, transform));
 //           planets[i].name = body.name;
 //           planets[i].transform.position = position;
 //           planets[i].GetComponent<SpriteRenderer>().color = body.color;
 //           planets[i].GetComponent<SpriteRenderer>().sortingOrder = 4;
 //           planets[i].transform.localScale = sun.transform.localScale * .5f;

 //           info = planets[i].GetComponent<PlanetInfo>();
 //           info.solar = body;

 //           LineRenderer line = planets[i].GetComponent<LineRenderer>();

 //           //Creates the line rendering for the orbit of planets

 //           Vector3[] positions = new Vector3[361];
 //           double time = game.data.date.time;
 //           body.SetOrbit(time, model.solar.mass);


 //           for (var b = 0; b < 360; b++)
 //           {
 //               positions[b] = body.approximatePositions[b];
 //           }
 //           line.positionCount = 360;
 //           line.SetPositions(positions);


 //           if (body.solarSubType == SolarSubType.GasGiant)
 //           {
 //               Color col = Color.blue;
 //               col.a = .1f;
 //               line.startColor = col;
 //               line.endColor = col;
 //           }
 //           else if (body.solarType == SolarType.DwarfPlanet)
 //           {
 //               Color col = Color.yellow;
 //               col.a = .1f;
 //               line.startColor = col;
 //               line.endColor = col;
 //           }
 //           else if (body.solarType == SolarType.Comet)
 //           {
 //               Color col = Color.white;
 //               col.a = .1f;
 //               line.startColor = col;
 //               line.endColor = col;
 //           }
 //           else
 //           {
 //               Color col = Color.green;
 //               col.a = .1f;
 //               line.startColor = col;
 //               line.endColor = col;
 //           }

 //           //Create Moons
 //           for (int m = 0; m < model.solar.satelites[i].satelites.Count; m++)
 //           {
 //               SolarBody moon = model.solar.satelites[i].satelites[m];
 //               position = moon.GamePosition(game.data.date.time);
 //               moonModels.Add(moon);

 //               moons.Add(Instantiate(moonObj, transform));
 //               moons[moons.Count - 1].name = moon.name;
 //               moons[moons.Count - 1].transform.position = position;
 //               moons[moons.Count - 1].GetComponent<SpriteRenderer>().color = moon.color;
 //               moons[moons.Count - 1].GetComponent<SpriteRenderer>().sortingOrder = 3;
 //               moons[moons.Count - 1].transform.localScale = sun.transform.localScale * .15f;

 //               info = moons[moons.Count - 1].GetComponent<PlanetInfo>();
 //               info.solar = moon;

 //               //Creates the line rendering for the orbit of moons

 //               positions = new Vector3[361];
 //               time = game.data.date.time;
 //               moon.SetOrbit(time, model.solar.satelites[moon.solarIndex[1]].mass);

 //               for (var b = 0; b < 360; b++)
 //               {
 //                   positions[b] = moon.approximatePositions[b];
 //               }
 //               line.positionCount = 360;
 //               line.SetPositions(positions);
 //           }
 //       }

 //       //StartCoroutine("UpdateSolarObjects");
 //   }

 //   public void DestroySystem()
 //   {
 //       //if (model.nameText != "" && model.nameText != null)
 //       //{
 //       //    nameButton.enabled = false;
 //       //}
 //       //StopAllCoroutines();
 //       game.nameOfSystem.text = game.data.galaxyName;

 //       transform.localScale = Vector3.one * (float) Mathd.Pow((model.solar.bodyRadius), .01f);
 //       model.localScale = (float) Mathd.Pow((model.solar.bodyRadius), .01f);
 //       Destroy(sun);
 //       for (int i = 0; i < planets.Count; i++)
 //       {
 //           Destroy(planets[i]);
            
 //       }
 //       planets = new List<GameObject>();
 //       for (int i = 0; i < moons.Count; i++)
 //       {
 //           Destroy(moons[i]);
            
 //       }
 //       moons = new List<GameObject>();
 //   }

    
}

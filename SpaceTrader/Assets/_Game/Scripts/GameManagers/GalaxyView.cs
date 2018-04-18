using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GalaxyView : IconManager {

    public static GalaxyView instance;
    public MapCamera galaxyCamera;
    public GameObject galaxyStar;
    public float starPow = .02f;
    public float starScale = 1f;

    internal GameManager game;
    internal ViewManager galaxy;

    private ParticleSystem particles;
    private ParticleSystem.Particle[] points;
    private int pointsMax;

    private List<GameObject> starList = new List<GameObject>();

    //Manage Icons
    public Canvas iconsCanvas;
    public Image hasStation;
    

    // Use this for initialization
    void Start () {
        if (instance == null)
        {
            instance = this;
            galaxy = ViewManager.instance;
            game = GameManager.instance;
            particles = GetComponent<ParticleSystem>();
            LoadStars();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
		//if (points != null)
  //      {
  //          for (int i = 0; i < pointsMax; i++)
  //          {
  //              points[i].position = game.data.stars[i].position.Local;
  //              points[i].startColor = game.data.stars[i].color;
  //              points[i].startSize = 1;
  //          }
  //      }
	}

    /// <summary>
    /// Loads all stars references in the main data file
    /// </summary>
    public void LoadStars()
    {
        if (game == null)
        {
            SceneManager.LoadScene("main_menu");
            return;
        }
        var stars = new GameObject("Stars");
        pointsMax = game.data.stars.Count;
        points = new ParticleSystem.Particle[pointsMax];

        //Destroy Icons
        ClearIcons();

        for (int i = 0; i < pointsMax; i++)
        {
            points[i].position = (Vector3) game.data.stars[i].solar.referencePosition;
            points[i].startColor = game.data.stars[i].solar.color;
            points[i].startSize = 3;

            

            var star = Instantiate(galaxyStar, stars.transform);
            star.GetComponent<TooltipInteract>().solar = game.data.stars[i].solar;
            star.transform.position = (Vector3) game.data.stars[i].solar.referencePosition;
            starList.Add(star);
            CreateIcon(i, star);
        }
        particles.SetParticles(points, points.Length);
        //foreach (SolarModel star in game.data.stars)
        //{
        //    Controller.Instantiate<SolarController>("solar", star, stars.transform);
        //}
        game.galaxyView = true;
        galaxyCamera.SetCameraControlTrue();
        game.setup = false;
        game.gameInitiated = true;
    }

    private void CreateIcon(int i, GameObject star)
    {
        //Create station indicator
        if (game.data.stars[i].solar.HasStations)
        {
            var icon = Instantiate(hasStation, iconsCanvas.transform);
            icon.GetComponent<IconObjectTrack>().SetTarget(star, galaxyCamera.mainCamera);
            icons.Add(icon.gameObject);
        }
    }

    public void SelectSolarSystem(SolarBody solar)
    {
        for (int i = 0; i < pointsMax; i++)
        {
            points[i].startSize = 1;
            if (game.data.stars[i].solar.id == solar.id)
            {
                points[i].startSize = 0;
            }
        }
        particles.SetParticles(points, points.Length);
        ClearIcons();
    }

    public void SelectGalaxyView()
    {
        ClearIcons();
        SolarView.instance.ClearIcons();
        for (int i = 0; i < pointsMax; i++)
        {
            points[i].startSize = 1;
            CreateIcon(i, starList[i]);
        }
        particles.SetParticles(points, points.Length);
        galaxyCamera.SetCameraControlTrue();
        SolarView.instance.DestroySystem();
        PlanetView.instance.DestroySystem();
        NormalView.instance.DestroySystem();
    }
}

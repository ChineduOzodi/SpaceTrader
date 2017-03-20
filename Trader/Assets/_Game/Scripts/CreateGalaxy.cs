using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class CreateGalaxy : MonoBehaviour {

    public LayerMask mapMask;
    public LayerMask solarMask;
    public Gradient sunSizeColor;
    public Gradient planetSizeColor;
    public Vector2 mapField;
    public int starCount;

    public Text infoText;

    internal static float G = .01f;

    private GameObject selectedObj;
    private Camera cam;
    internal SolarController solar;

    internal SolarModel[] stars;
    internal SolarController[] starControllers;

    private void Awake()
    {
        CreateStars(starCount);
        LoadStars();
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (hit)
                {
                    selectedObj = hit.transform.gameObject;
                    if (selectedObj.tag == "solar")
                    {
                        solar = selectedObj.GetComponent<SolarController>();
                        SolarView();
                        
                        
                    }

                }
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (cam.cullingMask == solarMask)
                GalaxyView();
            else
                SolarView();         
        }
        if (infoText != null)
            infoText.text = "Timescale: " + Time.timeScale + "\n";
    }

    public void GalaxyView()
    {
        cam.cullingMask = mapMask;
        transform.position = new Vector3(solar.transform.position.x, solar.transform.position.y, -10);
        cam.orthographicSize = 100;
    }

    public void SolarView()
    {

        cam.cullingMask = solarMask;
        transform.position = new Vector3(0, 0, -10);
        cam.orthographicSize = 1000;
    }

    public void CreateStars(int count)
    {
        stars = new SolarModel[count];

        for (int i = 0; i < count; i++)
        {
            Vector2 position = new Vector2(UnityEngine.Random.Range(-mapField.x * .5f, mapField.x * .5f), UnityEngine.Random.Range(-mapField.y * .5f, mapField.y * .5f));
            float sunMass = UnityEngine.Random.Range(1f, 1000);
            int numPlanets = UnityEngine.Random.Range(0, (int) Mathf.Sqrt(sunMass / 10));
            stars[i] = new SolarModel("Solar " + i + 1, i, position, sunMass, G, numPlanets, sunSizeColor, sunMass *.001f);
        }
    }
    public void LoadStars()
    {
        starControllers = new SolarController[stars.Length];
        foreach (SolarModel star in stars)
        {
            starControllers[star.index] = Controller.Instantiate<SolarController>("solar", star);
            starControllers[star.index].name = star.name;
        }
    }
	
}

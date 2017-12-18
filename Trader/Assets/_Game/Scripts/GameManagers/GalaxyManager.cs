using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class GalaxyManager : MonoBehaviour {

    public Canvas mapCanvas;
    public LayerMask mapMask;
    public LayerMask solarMask;
    
    
    public Canvas buttonInstanceCanvas;
    
    internal List<Canvas> mapButtonCanvases;
    public float mapButtonCanvasScaleMod = 10;
    public float galaxyInfluenceMod = .6f;
    private List<Vector2d> mapButtonStarPositions;
    public GameObject solarVisualOptions;
    public GameObject galaxyVisualOptions;
    public GameObject solarDisplayOptions;
    public GameObject galaxyDisplayOptions;

    

    private GameObject selectedObj;
    private Camera cam;
    internal SolarModel solarModel;

    //internal ModelRefs<Ship> hyperSpaceShips;
    internal static GalaxyManager instance;
    private GameManager game;
    private void Awake()
    {
        instance = this;
        mapButtonCanvases = new List<Canvas>();
        
        //hyperSpaceShips = new ModelRefs<Ship>();
        game = GameManager.instance;
        cam = game.GetComponent<Camera>();
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
                        solarModel = selectedObj.GetComponent<SolarController>().GetModel();
                        GoToSolarView();
                        
                        
                    }

                }
            }
        }
        if (game.data.cameraGalaxyOrtho * GameDataModel.galaxyDistanceMultiplication < 1 * Units.ly && CameraController.ClosestSolar() != solarModel)
        {
            solarModel = CameraController.ClosestSolar();
            SolarView();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (cam.cullingMask == solarMask)
                GalaxyView();
            else
                GoToSolarView();         
        }
        if (mapCanvas.enabled)
        {
            for(int i = 0; i < mapButtonCanvases.Count; i++)
            {
                mapButtonCanvases[i].transform.localScale = Vector3.one * (float) Mathd.Pow(game.data.cameraGalCameraScaleMod, 1f) * mapButtonCanvasScaleMod;
                mapButtonCanvases[i].transform.position = CameraController.CameraOffsetGalaxyPosition(mapButtonStarPositions[i]);
            }
        }
    }

    public void GalaxyView()
    {
        foreach (Canvas canvas in mapButtonCanvases)
        {
            canvas.enabled = true;
        }
        GameManager.instance.galaxyView = true;
        mapCanvas.enabled = true;
        cam.cullingMask = mapMask;
        game.data.cameraGalaxyOrtho = 100;
        solarModel = null;
        solarDisplayOptions.SetActive(false);
        solarVisualOptions.SetActive(false);
        galaxyDisplayOptions.SetActive(true);
        galaxyVisualOptions.SetActive(true);

        if (mapButtonCanvases.Count > 0)
        {
            foreach (SolarModel star in game.data.stars)
            {
                if (star.government.Model == null)
                {
                    star.color = Color.grey;
                }
                else
                {
                    
                }
                star.NotifyChange();
            }
        }
    }

    public void SolarView()
    {
        foreach (Canvas canvas in mapButtonCanvases)
        {
            canvas.enabled = false;
        }
        GameManager.instance.galaxyView = false;
        mapCanvas.enabled = false;
        solarDisplayOptions.SetActive(true);
        solarVisualOptions.SetActive(true);
        galaxyDisplayOptions.SetActive(false);
        galaxyVisualOptions.SetActive(false);
        cam.cullingMask = solarMask;
    }

    public void GoToSolarView()
    {
        game.data.cameraGalaxyPosition = solarModel.galaxyPosition;
        game.data.cameraGalaxyOrtho = .01;
        SolarView();        
    }

    public void CheckDisplayGovernment()
    {
        if (MapTogglePanel.instance.galaxyTerritory.isOn)
        {
            SetStarsGovernment();
        }
        else
        {
            SetStarsRegularColor();
        }
    }

    public void SetStarsGovernment()
    {
        for (int i = 0; i < mapButtonCanvases.Count; i++)
        {
            Destroy(mapButtonCanvases[i].gameObject);
        }
        mapButtonCanvases = new List<Canvas>();
        mapButtonStarPositions = new List<Vector2d>();
        foreach (SolarModel star in game.data.stars)
        {
            if (star.government.Model == null)
            {
                star.color = Color.grey;
            }
            else
            {
                star.color = star.government.Model.spriteColor;
                if (star.isCapital)
                {
                    Canvas textCanvas = Instantiate(buttonInstanceCanvas, CameraController.CameraOffsetGalaxyPosition(star.galaxyPosition), Quaternion.identity);
                    Button textButton = textCanvas.GetComponentInChildren<Button>();
                    Text text = textCanvas.GetComponentInChildren<Text>();
                    mapButtonCanvases.Add(textCanvas);
                    mapButtonStarPositions.Add(star.galaxyPosition);
                    text.text = star.government.Model.name;
                    textButton.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(star.government.Model));
                }
            }
            star.NotifyChange();
        }
    }
    public void SetStarsRegularColor()
    {
        foreach (SolarModel star in game.data.stars)
        {
            star.color = star.solar.color;
            star.localScale = (float)(star.solar.bodyRadius /1000000);
            star.NotifyChange();
        }

        for (int i = 0; i < mapButtonCanvases.Count; i++)
        {
            Destroy(mapButtonCanvases[i].gameObject);
        }
        mapButtonCanvases = new List<Canvas>();
    } 
}

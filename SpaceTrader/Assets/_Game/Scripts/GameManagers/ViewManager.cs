using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class ViewManager : MonoBehaviour {

    public Canvas mapCanvas;
    public LayerMask galaxyMask;
    public LayerMask planetsMask;
    public LayerMask moonsMask;
    public LayerMask satellitesMask;
    public LayerMask thirdPersonMask;
    
    
    public Canvas buttonInstanceCanvas;
    
    internal List<Canvas> mapButtonCanvases;
    public float mapButtonCanvasScaleMod = 10;
    public float galaxyInfluenceMod = .6f;
    private List<Vector3> mapButtonStarPositions;
    public GameObject solarVisualOptions;
    public GameObject galaxyVisualOptions;
    public GameObject solarDisplayOptions;
    public GameObject galaxyDisplayOptions;

    

    private GameObject selectedObj;
    private Camera cam;

    //internal ModelRefs<Ship> hyperSpaceShips;
    internal static ViewManager instance;
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
                        //game.data.mainCameraSolarIndex = selectedObj.GetComponent<SolarController>().GetModel().solarIndex;
                        //GoToSolarView(game.data.mainCameraSolarIndex);
                        
                        
                    }

                }
            }
        }
        //if (game.data.cameraGalaxyOrtho * GameDataModel.galaxyDistanceMultiplication < 1 * Units.ly && CameraController.ClosestSolar() != solarModel)
        //{
        //    solarModel = CameraController.ClosestSolar();
        //    SolarView();
        //}

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (cam.cullingMask == planetsMask)
                GalaxyView();
            else
                SolarView();         
        }
        if (mapCanvas.enabled)
        {
            for(int i = 0; i < mapButtonCanvases.Count; i++)
            {
                mapButtonCanvases[i].transform.localScale = Vector3.one * (float) Mathd.Pow(game.data.mainCameraOrtho[0], 1f) * mapButtonCanvasScaleMod;
                mapButtonCanvases[i].transform.position = mapButtonStarPositions[i];
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
        cam.cullingMask = galaxyMask;
        game.data.mainCameraOrtho[0] = 100;
        game.data.mainCameraSolarIndex = new List<int>();

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
                    //star.color = Color.grey;
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

        cam.cullingMask = planetsMask;
    }

    public void GoToSolarView(List<int> solarIndex)
    {
        if (solarIndex.Count == 1)
        {
            game.data.mainCameraSolarIndex = solarIndex;
        }
        if (solarIndex.Count > 2)
        {
            game.data.mainCameraSolarIndex = new List<int>() { solarIndex[0] };
        }
        else
        {
            throw new Exception("Incorrect solar index, " + solarIndex.Count);
        }
        game.data.mainCameraOrtho[1] = 100;
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
        mapButtonStarPositions = new List<Vector3>();
        foreach (SolarModel star in game.data.stars)
        {
            //if (star.government.Model == null)
            //{
            //    star.color = Color.grey;
            //}
            //else
            //{
            //    star.color = star.government.Model.spriteColor;
            //    if (star.isCapital)
            //    {
            //        Canvas textCanvas = Instantiate(buttonInstanceCanvas, star.position[0], Quaternion.identity);
            //        Button textButton = textCanvas.GetComponentInChildren<Button>();
            //        Text text = textCanvas.GetComponentInChildren<Text>();
            //        mapButtonCanvases.Add(textCanvas);
            //        mapButtonStarPositions.Add(star.position[0]);
            //        text.text = star.government.Model.name;
            //        textButton.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(star.government.Model));
            //    }
            //}
            //star.NotifyChange();
        }
    }
    public void SetStarsRegularColor()
    {
        foreach (SolarModel star in game.data.stars)
        {
            //star.color = star.solar.color;
            //star.localScale = (float)(star.solar.bodyRadius /1000000);
            //star.NotifyChange();
        }

        for (int i = 0; i < mapButtonCanvases.Count; i++)
        {
            Destroy(mapButtonCanvases[i].gameObject);
        }
        mapButtonCanvases = new List<Canvas>();
    } 
}

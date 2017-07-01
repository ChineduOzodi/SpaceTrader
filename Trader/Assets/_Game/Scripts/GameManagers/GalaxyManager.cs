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

    

    private GameObject selectedObj;
    private Camera cam;
    internal SolarController solar;

    internal ModelRefs<ShipModel> hyperSpaceShips;
    internal static GalaxyManager instance;
    private GameManager game;
    private void Awake()
    {
        instance = this;
        mapButtonCanvases = new List<Canvas>();
        hyperSpaceShips = new ModelRefs<ShipModel>();
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
        if (mapCanvas.enabled)
        {
            foreach (Canvas canvas in mapButtonCanvases)
            {
                canvas.transform.localScale = Vector3.one * Mathf.Pow(cam.orthographicSize, .5f) * 1f;
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
        if (solar != null)
            transform.position = new Vector3(solar.transform.position.x, solar.transform.position.y, -10);
        cam.orthographicSize = 100;

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
        cam.cullingMask = solarMask;
        transform.position = new Vector3(0, 0, -10);
        cam.orthographicSize = 1000;
    }

    
	public void SetStarsGovernment()
    {
        for (int i = 0; i < mapButtonCanvases.Count; i++)
        {
            Destroy(mapButtonCanvases[i].gameObject);
        }
        mapButtonCanvases = new List<Canvas>();
        foreach (SolarModel star in game.data.stars)
        {
            if (star.government.Model == null)
            {
                star.color = Color.grey;
                star.localScale = Mathf.Pow(star.governmentInfluence, .6f) + .5f;
            }
            else
            {
                star.color = star.government.Model.spriteColor;
                star.localScale = Mathf.Pow(star.governmentInfluence, .6f) + .5f;
                if (star.isCapital)
                {
                    Canvas textCanvas = Instantiate(buttonInstanceCanvas, star.galacticPosition, Quaternion.identity);
                    Button textButton = textCanvas.GetComponentInChildren<Button>();
                    Text text = textCanvas.GetComponentInChildren<Text>();
                    mapButtonCanvases.Add(textCanvas);
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

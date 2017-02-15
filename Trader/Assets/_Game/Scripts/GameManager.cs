using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeControl;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {

    internal GameObject selectedObj;
    internal Text infoText;
    internal Date date;
    internal ModelRefs<ShipModel> ships;
    internal ModelRefs<StationModel> stations;
    internal int statsDisplay = 0;

	// Use this for initialization
	void Start () {
        ships = new ModelRefs<ShipModel>();
        stations = new ModelRefs<StationModel>();

        date = new Date(0);
        int numStation = 100;
        for (int i = 0; i < numStation; i++)
        {
            int index = UnityEngine.Random.Range(0, 10);
            Vector3 randomLocation = new Vector3(UnityEngine.Random.Range(-1000, 1000), UnityEngine.Random.Range(-1000, 1000));
            stations.Add(StationCreator.CreateStation((FactoryType) index, randomLocation));
        }

        //--------------Create Ships---------------------------//
        int numShip = 25;
        for (int i = 0; i < numShip; i++)
        {
            Vector3 randomLocation = new Vector3(UnityEngine.Random.Range(-1000, 1000), UnityEngine.Random.Range(-1000, 1000));
            ships.Add(ShipCreator.CreateShip("Freight Ship "+ i, randomLocation));
        }
        //------------------------------------------------------------------------------------------------//
        infoText = GameObject.FindGameObjectWithTag("InfoText").GetComponent<Text>();

	
	}
	
	// Update is called once per frame
	void Update () {

        //Mouse Hit
        date.AddTime(Time.deltaTime);
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (hit)
                {
                    selectedObj = hit.transform.gameObject;
                }
                else
                {
                    selectedObj = null;
                    statsDisplay = 0;
                }
            
                
            }
        }

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            Time.timeScale *= .5f;
        }
        if (Input.GetKeyDown(KeyCode.Period))
        {
            Time.timeScale *= 2;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (selectedObj != null && transform.parent == null)
            {
                transform.parent = selectedObj.transform;
                transform.localPosition = new Vector3(0, 0, -10);
            }
            else transform.parent = null;
        }

        if (selectedObj != null)
        {
            if (selectedObj.tag == "station")
                infoText.text = selectedObj.GetComponent<StationController>().info;
            else
                infoText.text = selectedObj.GetComponent<ShipController>().info;
        }
        else
        {
            infoText.text = date.GetDateTime();
            if (statsDisplay == 1)
            {
                infoText.text += ShipStats();
            }
            else if (statsDisplay == 2)
            {
                infoText.text += StationStats();
            }
            else
            {
                infoText.text += BasicStats();
            }
        }
    }

    private string BasicStats()
    {
        string stats = " Timescale: " + Time.timeScale;
        Dictionary<string, Items> stationData = new Dictionary<string, Items>();

        foreach (StationModel station in stations)
        {
            foreach (Items item in station.factory.inputItems)
            {
                if (!item.selfProducing)
                {
                    if (stationData.ContainsKey(item.name))
                    {
                        stationData[item.name].price += item.price;
                        stationData[item.name].amount++;
                    }
                    else
                    {
                        Items addItem = new Items(item.name, 1);
                        addItem.price = item.price;
                        addItem.basePrice = item.basePrice;
                        addItem.selfProducing = item.selfProducing;
                        stationData.Add(item.name, addItem);
                    }
                }
                    
            }
            foreach (Items item in station.factory.outputItems)
            {
                if (!item.selfProducing)
                {
                    if (stationData.ContainsKey(item.name))
                    {
                        stationData[item.name].price += item.price;
                        stationData[item.name].amount++;
                    }
                    else
                    {
                        Items addItem = new Items(item.name, 1);
                        addItem.price = item.price;
                        addItem.basePrice = item.basePrice;
                        stationData.Add(item.name, addItem);
                        addItem.selfProducing = item.selfProducing;
                    }
                }
                
            }
        }
        List<Items> sortedItems = new List<Items>();
        foreach (Items item in stationData.Values)
        {
            sortedItems.Add(item);
        }
        sortedItems.Sort(delegate (Items c1, Items c2) { return (c2.price/ c2.amount /c2.basePrice).CompareTo(c1.price/ c1.amount /c1.basePrice); });
        for (int i = 0; i < sortedItems.Count; i++)
        {
            stats += string.Format("\n{0}. {1} - {2} | {3}/{4}", i + 1, sortedItems[i].name, sortedItems[i].amount, (sortedItems[i].price / sortedItems[i].amount).ToString("0.00"), sortedItems[i].basePrice.ToString("0.00"));
        }

        return stats;
    }

    public void SetShipStats()
    {
        selectedObj = null;
        statsDisplay = 1;
    }
    public string ShipStats()
    {
        string stats = "";
        List<ShipModel> sortedShips = new List<ShipModel>();
        foreach (ShipModel shipModel in ships)
        {
            sortedShips.Add(shipModel);
        }
        sortedShips.Sort(delegate (ShipModel c1, ShipModel c2) { return c2.money.CompareTo(c1.money); });
        for (int i = 0; i < sortedShips.Count; i++)
        {
            stats += string.Format("\n{0}. {1} - {2} | {3}", i+1, sortedShips[i].name, sortedShips[i].money.ToString("0.00"), sortedShips[i].capacity);
        }
        return stats;
    }
    public void SetStationStats()
    {
        selectedObj = null;
        statsDisplay = 2;
    }
    public string StationStats()
    {
        string stats = "";
        List<StationModel> sortedStations = new List<StationModel>();
        foreach (StationModel station in stations)
        {
            sortedStations.Add(station);
        }
        sortedStations.Sort(delegate (StationModel c1, StationModel c2) { return c2.money.CompareTo(c1.money); });
        for (int i = 0; i < sortedStations.Count; i++)
        {
            stats += string.Format("\n{0}. {1} - {2} | {3}/{4}", i+1, sortedStations[i].name, sortedStations[i].money.ToString("0.00"), sortedStations[i].factory.productionTime.ToString("0.00"), sortedStations[i].factory.unitTime.ToString("0.00"));
        }

        return stats;
    }
}

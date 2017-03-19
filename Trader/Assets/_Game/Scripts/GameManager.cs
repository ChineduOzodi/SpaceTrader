using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeControl;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {

    internal float localScaleMod;

    internal GameObject selectedObj;
    internal Text infoText;
    internal GameDataModel data;
    
    internal int statsDisplay = 0;
    internal CreateGalaxy galaxy;

	// Use this for initialization
	void Start () {
        galaxy = GetComponent<CreateGalaxy>();
        data = new GameDataModel();
        data.ships = new ModelRefs<ShipModel>();
        data.stations = new ModelRefs<StationModel>();
        data.creatures = new ModelRefs<CreatureModel>();
        
        data.date = new Date(0);
        int numStation = 100;
        for (int i = 0; i < numStation; i++)
        {
            int factoryIndex = UnityEngine.Random.Range(0, 10);
            int starIndex = UnityEngine.Random.Range(0, galaxy.starCount);
            int planetIndex = UnityEngine.Random.Range(0, galaxy.stars[starIndex].planets.Length);
            SolarBody parent;
            if (galaxy.stars[starIndex].planets.Length > 0)
                parent = galaxy.stars[starIndex].planets[planetIndex];
            else
                parent = galaxy.stars[starIndex].sun;

            Polar2 position = new Polar2(UnityEngine.Random.Range(parent.bodyRadius + 2, parent.SOI), UnityEngine.Random.Range(0, 2 * Mathf.PI));

            StationModel station = StationCreator.CreateStation((FactoryType)factoryIndex, starIndex, parent, position, null);
            data.stations.Add(station);
            UpdateCreatures(station);
        }

        numStation = 10;
        for (int i = 0; i < numStation; i++)
        {
            int factoryIndex = 9;
            int starIndex = UnityEngine.Random.Range(0, galaxy.starCount);
            int planetIndex = UnityEngine.Random.Range(0, galaxy.stars[starIndex].planets.Length);
            SolarBody parent;
            if (galaxy.stars[starIndex].planets.Length > 0)
                parent = galaxy.stars[starIndex].planets[planetIndex];
            else
                parent = galaxy.stars[starIndex].sun;

            Polar2 position = new Polar2(UnityEngine.Random.Range(parent.bodyRadius + 2, parent.SOI), UnityEngine.Random.Range(0, 2 * Mathf.PI));

            StationModel station = StationCreator.CreateStation((FactoryType)factoryIndex, starIndex, parent, position, null);
            data.stations.Add(station);
            UpdateCreatures(station);
        }

        //--------------Create Ships---------------------------//
        int numShip = 75;
        for (int i = 0; i < numShip; i++)
        {
            StationModel startStation = data.stations[UnityEngine.Random.Range(0, data.stations.Count)];
            Vector3 randomLocation = new Vector3(UnityEngine.Random.Range(-1000, 1000), UnityEngine.Random.Range(-1000, 1000));
            ShipModel ship = ShipCreator.CreateShip("Freight Ship " + i, startStation.solar.starIndex, startStation.solar.parent, startStation.solar.GetLocalPosition(data.date.time));
            data.ships.Add(ship);
            UpdateCreatures(ship);
        }
        //------------------------------------------------------------------------------------------------//
        infoText = GameObject.FindGameObjectWithTag("InfoText").GetComponent<Text>();

	
	}
	
	// Update is called once per frame
	void Update () {

        //Mouse Hit
        data.date.AddTime(Time.deltaTime);
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (hit)
                {
                    selectedObj = hit.transform.gameObject;
                    if (selectedObj.tag == "station")
                    {
                        //BreakStationLinks();
                        //SetStationLinks();
                    }
                        
                }
                else
                {
                    //BreakStationLinks();
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

        infoText.text = data.date.GetDateTime() + "\nTimescale: " + Time.timeScale + "\n";

        if (selectedObj != null)
        {
            if (selectedObj.tag == "station")
                infoText.text += selectedObj.GetComponent<StationController>().GetInfo();
            else if (selectedObj.tag == "ship")
                infoText.text += selectedObj.GetComponent<ShipController>().GetInfo();
        }
        else
        {
            
            if (statsDisplay == 1)
            {
                infoText.text += ShipStats();
            }
            else if (statsDisplay == 2)
            {
                infoText.text += StationStats();
            }
            else if (statsDisplay == 3)
            {
                infoText.text += CreatureStats();
            }
            else
            {
                infoText.text += BasicStats();
            }
        }
    }

    //private void BreakStationLinks()
    //{
    //    foreach (StationModel station in data.stations)
    //    {
    //        station.lineTarget = station.position;
    //        station.NotifyChange();
    //    }
    //}

    private void SetStationLinks()
    {
        StationController thisStation = selectedObj.GetComponent<StationController>();

        foreach (StationModel station in data.stations)
        {
            foreach (Items item in station.factory.inputItems)
            {
                foreach (Items thisItem in thisStation.GetOutputItems())
                {
                    if (thisItem.name == item.name)
                    {
                        station.lineTarget = thisStation.transform.position;
                        station.lineColor = item.color;
                        station.NotifyChange();
                    }
                }
                

            }
            foreach (Items item in station.factory.outputItems)
            {
                foreach (Items thisItem in thisStation.GetInputItems())
                {
                    if (thisItem.name == item.name)
                    {
                        station.lineTarget = thisStation.transform.position;
                        station.lineColor = item.color;
                        station.NotifyChange();
                    }
                }

            }
        }
    }

    private string BasicStats()
    {
        string stats = "";
        Dictionary<string, Items> stationData = new Dictionary<string, Items>();

        foreach (StationModel station in data.stations)
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
            stats += string.Format("\n{0}. {1} - {2} | {3}/{4}", i + 1, sortedItems[i].coloredName, sortedItems[i].amount, (sortedItems[i].price / sortedItems[i].amount).ToString("0.00"), sortedItems[i].basePrice.ToString("0.00"));
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
        string stats = "\nTotal Ships: " + data.ships.Count + "\n";

        List<ShipModel> sortedShips = new List<ShipModel>();
        foreach (ShipModel shipModel in data.ships)
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
        string stats = "\nTotal Stations: " + data.stations.Count + "\n";
        List<StationModel> sortedStations = new List<StationModel>();
        foreach (StationModel station in data.stations)
        {
            sortedStations.Add(station);
        }
        sortedStations.Sort(delegate (StationModel c1, StationModel c2) { return c2.money.CompareTo(c1.money); });
        for (int i = 0; i < sortedStations.Count; i++)
        {
            stats += string.Format("\n{0}. {1} - {2} | {3}/{4}", i+1, "<color=" + ColorTypeConverter.ToRGBHex(sortedStations[i].color) + ">" + sortedStations[i].name + "</color>", sortedStations[i].money.ToString("0.00"), sortedStations[i].factory.productionTime.ToString("0.00"), sortedStations[i].factory.unitTime.ToString("0.00"));
        }

        return stats;
    }
    public void SetCreatureStats()
    {
        selectedObj = null;
        statsDisplay = 3;
    }
    public string CreatureStats()
    {
        string stats = "\nTotal Creatures:" + data.creatures.Count;
        List<CreatureModel> sortedCreatures = new List<CreatureModel>();
        foreach (CreatureModel creature in data.creatures)
        {
            sortedCreatures.Add(creature);
        }
        sortedCreatures.Sort(delegate (CreatureModel c1, CreatureModel c2) { return c2.money.CompareTo(c1.money); });
        for (int i = 0; i < sortedCreatures.Count; i++)
        {
            stats += string.Format("\n{0}. {1} - {2} | {3} - {4}", i + 1, sortedCreatures[i].name, sortedCreatures[i].money.ToString("0.00"), sortedCreatures[i].ships.Count, sortedCreatures[i].stations.Count);
            if (i >= 99)
                break;
        }
        return stats;
    }
    public void UpdateCreatures(StructureModel model)
    {
        if (!data.creatures.Contains(model.owner.Model))
            data.creatures.Add(model.owner.Model);
        if (!data.creatures.Contains(model.captain.Model))
            data.creatures.Add(model.captain.Model);

        foreach( CreatureModel creature in model.workers)
        {
            if (!data.creatures.Contains(creature))
                data.creatures.Add(creature);
        }
    }
    public void SaveGame()
    {
        Model.SaveAll("TraderSaves");
        print("Game Saved");
    }

    public void LoadGame()
    {
        Model.DeleteAll();
        Model.Load("TraderSaves", OnStart, OnProgress, OnDone, OnError);
    }

    private void OnProgress(float obj)
    {
        print("Loading Progress: " + obj);
    }

    private void OnDone()
    {
        print("Loading Done");
        GameObject stationEmpty = new GameObject("Stations");
        GameObject shipEmpty = new GameObject("Ships");
        data = Model.First<GameDataModel>();
        foreach (Model model in Model.GetAll<StationModel>())
        {
            StationController station = Controller.Instantiate<StationController>("station", model, stationEmpty.transform);
        }
        foreach (Model model in Model.GetAll<ShipModel>())
        {
            ShipController ship = Controller.Instantiate<ShipController>("ship", model, shipEmpty.transform);
        }
    }

    private void OnError(string obj)
    {
        print("Error: " + obj);
    }

    private void OnStart()
    {
        print("Loading Save");
    }
}

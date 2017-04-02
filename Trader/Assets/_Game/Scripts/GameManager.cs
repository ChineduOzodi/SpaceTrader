using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeControl;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {
    public int numStation;
    public int numShip;
    public int numGov;
    public int numComp;
    internal float localScaleMod = 1;
    bool galaxyView = true;
    public int numShipsPerFrame;
    public int numStationsPerFrame;
    internal GameObject selectedObj;
    internal Text infoText;
    internal GameDataModel data;
    internal bool setup = false;
    internal int statsDisplay = 0;
    internal CreateGalaxy galaxy;
    internal TradeRouteRequestManager tradeRequestManager;
    internal static GameManager instance;
    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start () {
        galaxy = GetComponent<CreateGalaxy>();
        tradeRequestManager = GetComponent<TradeRouteRequestManager>();
        infoText = GameObject.FindGameObjectWithTag("InfoText").GetComponent<Text>();
        data = new GameDataModel();
        data.date = new Date(0);
        setup = true;
        StartCoroutine("CreateGame");
    }
	
	// Update is called once per frame
	void Update () {

        //Mouse Hit
        if (!setup)
        {
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

            if (transform.parent != null && transform.parent.tag == "ship")
            {
                ShipController ship = transform.parent.GetComponent<ShipController>();
                galaxy.solar = galaxy.starControllers[ship.starIndex];
                if (ship.hyperspace != galaxyView)
                {
                    if (ship.hyperspace)
                    {
                        galaxyView = true;
                        galaxy.GalaxyView();
                        transform.localPosition = new Vector3(0, 0, -10);
                        transform.localScale = Vector3.one * (1 / transform.parent.localScale.x);

                    }
                    else
                    {
                        galaxyView = false;
                        galaxy.SolarView();
                        transform.localPosition = new Vector3(0, 0, -10);
                        transform.localScale = Vector3.one * (1 / transform.parent.localScale.x);
                    }
                }

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

    IEnumerator CreateGame()
    {
        NameGen names = new NameGen();
        while (setup)
        {
            for (int a = 0; a < numGov; a++)
            {
                //Create Government
                GovernmentModel gov = new GovernmentModel(names.GenerateWorldName() + " Government");
                data.governments.Add(gov);
                int starIndex = UnityEngine.Random.Range(0, galaxy.starCount);
                int planetIndex = UnityEngine.Random.Range(0, galaxy.stars[starIndex].planets.Length);
                SolarBody parent;
                if (galaxy.stars[starIndex].planets.Length > 0)
                    parent = galaxy.stars[starIndex].planets[planetIndex];
                else
                    parent = galaxy.stars[starIndex].sun;
                Polar2 position = new Polar2(UnityEngine.Random.Range(parent.bodyRadius + 2, parent.SOI), UnityEngine.Random.Range(0, 2 * Mathf.PI));
                if (parent.planetType == PlanetType.Regular)
                {
                    position = new Polar2(0,0);
                }

                //Add Government Capital
                CreatureModel owner = new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 1000000);
                CreatureModel manager = new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 100000);
                StationModel station = StationCreator.CreateStation(names.GenerateRegionName() + " Station", galaxy.stars[starIndex], parent, position, owner, manager);
                data.stations.Add(station);
                data.creatures.Add(owner);
                data.creatures.Add(manager);
                gov.location.Model = station;
                for (int c = 0; c < numComp; c++)
                {
                    owner = new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 1000000);
                    CompanyModel comp = new CompanyModel(names.GenerateRegionName() + " Company", gov, owner);
                    gov.companyAcess.Add(comp);
                    data.creatures.Add(owner);

                    //Add Company Headquarter Station

                    int factoryIndex = UnityEngine.Random.Range(0, 10);
                    starIndex = UnityEngine.Random.Range(0, galaxy.starCount);
                    planetIndex = UnityEngine.Random.Range(0, galaxy.stars[starIndex].planets.Length);
                    if (galaxy.stars[starIndex].planets.Length > 0)
                        parent = galaxy.stars[starIndex].planets[planetIndex];
                    else
                        parent = galaxy.stars[starIndex].sun;

                    position = new Polar2(UnityEngine.Random.Range(parent.bodyRadius + 2, parent.SOI), UnityEngine.Random.Range(0, 2 * Mathf.PI));
                    owner = new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 1000000);
                    manager = new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 100000);
                    station = StationCreator.CreateStation(names.GenerateRegionName() + " Station", galaxy.stars[starIndex], parent, position, owner, manager);
                    data.creatures.Add(owner);
                    data.creatures.Add(manager);
                    data.stations.Add(station);
                    yield return null;

                    for (int i = 0; i < numStation - 1; i++)
                    {
                        factoryIndex = UnityEngine.Random.Range(0, 10);
                        starIndex = UnityEngine.Random.Range(0, galaxy.starCount);
                        planetIndex = UnityEngine.Random.Range(0, galaxy.stars[starIndex].planets.Length);
                        if (galaxy.stars[starIndex].planets.Length > 0)
                            parent = galaxy.stars[starIndex].planets[planetIndex];
                        else
                            parent = galaxy.stars[starIndex].sun;

                        position = new Polar2(UnityEngine.Random.Range(parent.bodyRadius + 2, parent.SOI), UnityEngine.Random.Range(0, 2 * Mathf.PI));
                        owner = new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 1000000);
                        manager = new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 100000);
                        station = StationCreator.CreateStation(names.GenerateRegionName() + " Station", galaxy.stars[starIndex], parent, position, owner, manager);
                        data.creatures.Add(owner);
                        data.creatures.Add(manager);
                        data.stations.Add(station);
                        infoText.text = string.Format("Creating stations: {0} of {1}", i, numStation);
                        yield return null;
                    }

                    //--------------Create Ships---------------------------//
                    for (int i = 0; i < numShip; i++)
                    {
                        StationModel startStation = data.stations[UnityEngine.Random.Range(0, data.stations.Count)];
                        Vector3 randomLocation = new Vector3(UnityEngine.Random.Range(-1000, 1000), UnityEngine.Random.Range(-1000, 1000));
                        manager = new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 10000);
                        ShipModel ship = ShipCreator.CreateShip(comp.name + " Ship " + i, startStation.solar.starIndex, startStation.solar.parent, startStation.solar.GetLocalPosition(data.date.time), comp, manager);
                        data.ships.Add(ship);
                        data.creatures.Add(manager);
                        infoText.text = string.Format("Creating ships: {0} of {1}", i, numShip);
                        yield return null;
                    }
                }
                
            }
            
            //------------------------------------------------------------------------------------------------//
            
            StartCoroutine("UpdateShips", numShipsPerFrame);
            StartCoroutine("UpdateStations", numStationsPerFrame);
            setup = false;
        }
    }
    IEnumerator UpdateShips(int updatesPerFrame)
    {
        int shipIndex = 0;
        while (true)
        {
            if (shipIndex >= data.ships.Count)
                shipIndex = 0;
            for (int i =0; i < updatesPerFrame; i++)
            {
                ShipControl(shipIndex);
                shipIndex++;
            }

            yield return null;
        }
    }

    IEnumerator UpdateStations(int updatesPerFrame)
    {
        int stationIndex = 0;
        while (true)
        {
            if (stationIndex >= data.stations.Count)
            {
                stationIndex = 0;

            }
                
            for (int i = 0; i < updatesPerFrame; i++)
            {
                StationControl(stationIndex);
            }
            stationIndex++;
            yield return null;
        }
    }

    private void StationControl(int stationIndex)
    {
        StationModel model = data.stations[stationIndex];
        float deltaTime = data.date.time - model.age.time;
        model.age.AddTime(deltaTime);
        int factoryStatus = model.factory.UpdateProduction(deltaTime);

        foreach (ProductionItem item in model.factory.inputItems)
        {
            if (model.factory.outputItems.Length == 0 && factoryStatus > 0)
            {
                model.money += item.productionAmount * factoryStatus * item.price;
            }
        }

        if (model.timeUpdate < model.age.time)
        {
            model.timeUpdate = model.age.time + Date.Hour;

            //Money Evaluation
            model.money -= model.runningCost;
            model.money -= 10 * model.workers;

            if (model.manager.Model != model.owner.Model)
            {
                model.manager.Model.money += 85;
                model.money -= 85;
            }

            float moneyEarned = model.money - model.moneyStats.data["Money"][model.moneyStats.data["Money"].Count - 1].y;

            if (moneyEarned > 0)
            {
                model.manager.Model.money += moneyEarned * .1f;
                model.money -= moneyEarned * .1f;
            }
            moneyEarned = model.money - model.moneyStats.data["Money"][model.moneyStats.data["Money"].Count - 1].y;
            model.moneyChange = moneyEarned;
            model.moneyStats.data["Money Change"].Add(new Stat(model.age.time, moneyEarned));
            model.moneyStats.data["Money"].Add(new Stat(model.age.time, model.money));

            model.owner.Model.money += model.money;
            model.money = 0;


        }

        if (model.money < 0)
        {
            foreach (ShipModel ship in model.incomingShips)
            {
                ship.NotifyChange();
            }
            print(model.name + " Died");
            model.Delete();
        }
        //else if (model.money > 5000000)
        //{
        //    int factoryIndex = UnityEngine.Random.Range(0, 10);
        //    int starIndex;
        //    if (UnityEngine.Random.Range(0, 2) == 1)
        //        starIndex = UnityEngine.Random.Range(0, galaxy.starCount);
        //    else starIndex = model.solar.starIndex;
        //    int planetIndex = UnityEngine.Random.Range(0, galaxy.stars[starIndex].planets.Length);
        //    SolarBody parent;
        //    if (galaxy.stars[starIndex].planets.Length > 0)
        //        parent = galaxy.stars[starIndex].planets[planetIndex];
        //    else
        //        parent = galaxy.stars[starIndex].sun;

        //    Polar2 position = new Polar2(UnityEngine.Random.Range(parent.bodyRadius + 2, parent.SOI), UnityEngine.Random.Range(0, 2 * Mathf.PI));

        //    StationModel station = StationCreator.CreateStation((FactoryType)factoryIndex, galaxy.stars[starIndex], parent, position, model.owner.Model);
        //    data.stations.Add(station);
        //    UpdateCreatures(station);
        //    model.money -= 1750000;
        //    model.owner.Model.money += 1000000;
        //    print(name + " Bought " + (FactoryType)factoryIndex);
        //}
    }

    private void ShipControl(int shipIndex)
    {
        ShipModel model = data.ships[shipIndex];
        float deltaTime = data.date.time - model.age.time;
        model.age.AddTime(deltaTime);

        if (model.timeUpdate < model.age.time)
        {
            model.timeUpdate += Date.Hour;

            //Money Evaluation
            model.money -= model.runningCost;
            model.money -= model.workers * 15;

            if (model.captain.Model != model.owner.Model)
            {
                model.captain.Model.money += 35;
                model.money -= 35;
            }

            float moneyEarned = model.money - model.moneyStats.data["Money"][model.moneyStats.data["Money"].Count - 1].y;

            if (moneyEarned > 0)
            {
                model.captain.Model.money += moneyEarned * .1f;
                model.money -= moneyEarned * .1f;
            }
            moneyEarned = model.money - model.moneyStats.data["Money"][model.moneyStats.data["Money"].Count - 1].y;
            model.moneyChange = moneyEarned;
            model.moneyStats.data["Money Change"].Add(new Stat(model.age.time, model.moneyChange));
            model.moneyStats.data["Money"].Add(new Stat(model.age.time, model.money));
            model.owner.Model.money += model.money;
            model.money = 0;
            //if (model.money < 0)
            //{
            //    if (model.mode == ShipMode.Sell && model.target != null && model.target.Model != null)
            //    {
            //        ((StationModel) model.target.Model).SellIncomplete(model.item);
            //    }
            //    print(model.name + " Died");
            //    model.Delete();
            //    return;
            //}

        }

        if ((float)model.fuel.amount / model.fuelCapacity < .25f && model.target.Model == null)
        {
            model.target = new ModelRef<StructureModel>(FindClosestStation( ItemTypes.Fuel.ToString() , model));
            if (model.target.Model != null)
            {
                StationModel station = (StationModel) model.target.Model;
                Items buyItem = new Items(ItemTypes.Fuel, model.fuelCapacity);
                model.items.Add(station.Buy(buyItem, model));
                model.spriteColor = Color.yellow;
                model.mode = ShipMode.Buy;
                station.incomingShips.Add(model);
                model.NotifyChange();
                return;
            }
            else if (model.fuel.amount < 0) {  }
        }

        foreach (Items item in model.items)
        {
            if (item.name == ItemTypes.Ship.ToString() && item.amount > 0)
            {
                for (int i = 0; i < item.amount; i++)
                {
                    CreatureModel captain = new CreatureModel(model.owner.Model.name + " Ship Captain " + model.owner.Model.itemsBought, 1000);
                    ShipModel ship = ShipCreator.CreateShip(model.owner.Model.name + "." + model.owner.Model.itemsBought, model.solar.starIndex, model.solar.parent, model.solar.GetLocalPosition(data.date.time), model.owner.Model, captain);
                    data.ships.Add(ship);
                    data.creatures.Add(captain);
                    model.owner.Model.itemsBought++;
                }

                item.amount = 0;
            }

            if (item.amount == 0 && item.pendingAmount == 0)
                model.items.Remove(item);
        }
        

        //if (model.money > 10000 && model.target.Model == null)
        //{
        //    model.target.Model = FindClosestStation("Ship", model);
        //    if (model.target.Model != null)
        //    {
        //        StationModel station = (StationModel) model.target.Model;
        //        foreach (Items outputItem in station.factory.outputItems)
        //        {
        //            if (outputItem.name == "Ship")
        //            {
        //                if (outputItem.price + 2000 < model.money)
        //                {
        //                    model.item = new Items("Ship", 1);
        //                    model.spriteColor = Color.cyan;
        //                    model.mode = ShipMode.Buy;
        //                    station.incomingShips.Add(model);
        //                    model.NotifyChange();
        //                    return;
        //                }
        //                else
        //                {
        //                    model.target.Model = null;
        //                }
        //            }
        //        }
        //    }
        //}
        if (model.mode == ShipMode.Idle)
        {
            model.mode = ShipMode.SearchingTradeRoute;
            TradeRouteRequestManager.RequestTradeRoute(model, OnFindRouteFinished);
        }
    }

    private void OnFindRouteFinished(ShipModel model, Items buyItem, StructureModel[] targets, bool success)
    {
        if (success)
        {
            model.target.Model = targets[0];
            model.sellTarget.Model = targets[1];
            model.mode = ShipMode.Buy;
            StationModel station = (StationModel)model.target.Model;
            model.items.Add(station.Buy(buyItem, model));
            station.incomingShips.Add(model);
            model.NotifyChange();
        }
        else
        {
            model.mode = ShipMode.Idle;
        }
    }

    private StationModel FindClosestStation(string itemName, ShipModel model)
    {
        float distance = 100000000;
        StationModel foundStation = null;
        foreach (StationModel station in data.stations)
        {
            foreach (Items outputItem in station.factory.outputItems)
            {
                float closestDistance = Vector2.Distance(galaxy.stars[station.solar.starIndex].position, model.hyperSpacePosition) * 1000;
                closestDistance += Vector2.Distance(station.solar.GetWorldPosition(data.date.time), model.solar.GetWorldPosition(data.date.time));
                if (outputItem.name == itemName && closestDistance < distance && outputItem.amount > 0)
                {
                    distance = closestDistance;
                    foundStation = station;

                }
            }
        }

        return foundStation;
    }
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

        //foreach (StationModel station in data.stations)
        //{
        //    foreach (Items item in station.factory.inputItems)
        //    {
        //        if (!item.selfProducing)
        //        {
        //            if (stationData.ContainsKey(item.name))
        //            {
        //                stationData[item.name].price += item.price;
        //                stationData[item.name].amount++;
        //            }
        //            else
        //            {
        //                Items addItem = new Items(item.name, 1);
        //                addItem.price = item.price;
        //                addItem.basePrice = item.basePrice;
        //                addItem.selfProducing = item.selfProducing;
        //                stationData.Add(item.name, addItem);
        //            }
        //        }
                    
        //    }
        //    foreach (Items item in station.factory.outputItems)
        //    {
        //        if (!item.selfProducing)
        //        {
        //            if (stationData.ContainsKey(item.name))
        //            {
        //                stationData[item.name].price += item.price;
        //                stationData[item.name].amount++;
        //            }
        //            else
        //            {
        //                Items addItem = new Items(item.name, 1);
        //                addItem.price = item.price;
        //                addItem.basePrice = item.basePrice;
        //                stationData.Add(item.name, addItem);
        //                addItem.selfProducing = item.selfProducing;
        //            }
        //        }
                
        //    }
        //}
        List<Items> sortedItems = new List<Items>();
        foreach (Items item in stationData.Values)
        {
            sortedItems.Add(item);
        }
        //sortedItems.Sort(delegate (Items c1, Items c2) { return (c2.price/ c2.amount /c2.basePrice).CompareTo(c1.price/ c1.amount /c1.basePrice); });
        //for (int i = 0; i < sortedItems.Count; i++)
        //{
        //    stats += string.Format("\n{0}. {1} - {2} | {3}/{4}", i + 1, sortedItems[i].coloredName, sortedItems[i].amount, (sortedItems[i].price / sortedItems[i].amount).ToString("0.00"), sortedItems[i].basePrice.ToString("0.00"));
        //}

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

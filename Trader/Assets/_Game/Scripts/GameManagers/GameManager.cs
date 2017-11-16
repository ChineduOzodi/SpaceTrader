﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeControl;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
/// <summary>
/// Manages the main aspects of the game, including player interaction, main ai, game ui, and view changes. Anything else should be moved to a more specific file
/// </summary>
public class GameManager : MonoBehaviour {

    public GameObject galaxyManager;
    public GameObject pathfindingManager;
    public LoadingPanel loadingPanel;
    public GameObject exitPanel;
    internal float localScaleMod = 1;
    internal bool galaxyView = true;
    public int numShipsPerFrame;
    public int numStationsPerFrame;
    internal GameObject selectedObj;
    public Text dateInfo;
    public Text nameOfSystem;
    internal GameDataModel data;
    
    internal int statsDisplay = 0;
    internal GalaxyManager galaxy;
    internal TradeRouteRequestManager tradeRequestManager;
    internal static GameManager instance;

    //Used to setup the game and make sure nothing is running while things are being configured
    internal bool gameInitiated = false;
    internal bool setup = false;

    internal float timeScale = 1;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    // Use this for initialization
    void Start ()
    {
        galaxy = galaxyManager.GetComponent<GalaxyManager>();
        tradeRequestManager = pathfindingManager.GetComponent<TradeRouteRequestManager>();
        data = new GameDataModel();
        setup = true;
        GetLoadingPanel().LoadAssets();

    }

    private LoadingPanel GetLoadingPanel()
    {
        return loadingPanel;
    }

    // Update is called once per frame
    void Update () {
        if (gameInitiated)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (exitPanel.activeSelf)
                {
                    ExitGame();
                }
                else
                {
                    OpenExitPanel();
                }
            }
            //Mouse Hit
            if (!setup)
            {
                data.date.AddTime(Time.deltaTime * timeScale);
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
                                OpenInfoPanel(selectedObj.GetComponent<StationController>().Model);
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
                    timeScale *= .5f;
                }
                if (Input.GetKeyDown(KeyCode.Period))
                {
                    timeScale *= 2;
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

                dateInfo.text = data.date.GetDateTime() + "\nTimescale: " + timeScale + "\n";

                //if (selectedObj != null)
                //{
                //    if (selectedObj.tag == "station")
                //        dateInfo.text += selectedObj.GetComponent<StationController>().GetInfo();
                //    else if (selectedObj.tag == "ship")
                //        dateInfo.text += selectedObj.GetComponent<ShipController>().GetInfo();
                //}
                //else
                //{

                //    if (statsDisplay == 1)
                //    {
                //        dateInfo.text += ShipStats();
                //    }
                //    else if (statsDisplay == 2)
                //    {
                //        dateInfo.text += StationStats();
                //    }
                //    else if (statsDisplay == 3)
                //    {
                //        dateInfo.text += CreatureStats();
                //    }
                //    else
                //    {
                //        dateInfo.text += BasicStats();
                //    }
                //}
                // ---------------------------------- TODO: Need to find a mor MVC way to run on click events on the controllers themselves -------------------------//
                //if (transform.parent != null && transform.parent.tag == "ship")
                //{
                //    ShipController ship = transform.parent.GetComponent<ShipController>();
                //    galaxy.solar = galaxy.starControllers[ship.starIndex];
                //    if (ship.hyperspace != galaxyView)
                //    {
                //        if (ship.hyperspace)
                //        {
                //            galaxyView = true;
                //            galaxy.GalaxyView();
                //            transform.localPosition = new Vector3(0, 0, -10);
                //            transform.localScale = Vector3.one * (1 / transform.parent.localScale.x);

                //        }
                //        else
                //        {
                //            galaxyView = false;
                //            galaxy.SolarView();
                //            transform.localPosition = new Vector3(0, 0, -10);
                //            transform.localScale = Vector3.one * (1 / transform.parent.localScale.x);
                //        }
                //    }

                //}
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

   public void StartGame()
    {
        //StartCoroutine("UpdateShips", numShipsPerFrame);
        //StartCoroutine("UpdateStations", numStationsPerFrame);
        //StartCoroutine("UpdateGovernments");
        setup = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void OpenExitPanel()
    {
        exitPanel.SetActive(true);
    }
    internal void OpenInfoPanel(IdentityModel model)
    {
        InfoPanelModel infoModel = new InfoPanelModel(model);
        Controller.Instantiate<InfoPanelController>("infopanelcanvas", infoModel);
    }

    
    /// <summary>
    /// Update all ship positions and actions
    /// </summary>
    /// <param name="updatesPerFrame"> how many ships to update per frame. Should be calculated to make it more effective</param>
    /// <returns></returns>
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
    /// <summary>
    /// Updates all stations per frame
    /// </summary>
    /// <param name="updatesPerFrame">how many stations to update per frame.</param>
    /// <returns></returns>
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
    /// <summary>
    /// Updates all the government bodies per frame
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateGovernments()
    {
        int govIndex = 0;
        while (true)
        {
            GovernmentModel model = data.governments[govIndex];
            
            foreach (SolarModel star in model.stars)
            {
                double deltaTime = data.date.time - model.age.time - model.dateCreated.time;
                model.age.AddTime(deltaTime);
                double totalPop = 0;
                //foreach (SolarBody body in star.solar.satelites)
                //{
                //    totalPop += body.population;
                //    foreach (SolarBody moon in body.satelites)
                //    {
                //        totalPop += moon.population;
                //    }
                //}
                //foreach (StationModel station in star.stations)
                //{
                //    totalPop += station.population;
                //}

                //star.governmentInfluence += (totalPop * .001f * (float) data.date.deltaTime) - (star.governmentInfluence * .1f * (float)data.date.deltaTime);

                foreach (SolarModel nearStar in star.nearStars)
                {
                    nearStar.governmentInfluence += star.governmentInfluence * .1f * (float) data.date.deltaTime;
                    star.governmentInfluence -= star.governmentInfluence *.1f * (float) data.date.deltaTime;
                    if (galaxy.mapButtonCanvases.Count > 0 && galaxyView)
                        nearStar.localScale = Mathf.Pow(nearStar.governmentInfluence, .6f) + 1f;
                    if (nearStar.governmentInfluence > 250 && nearStar.government.Model == null)
                    {
                        nearStar.government.Model = star.government.Model;
                        model.stars.Add(nearStar);
                        
                        if (galaxy.mapButtonCanvases.Count > 0)
                        {
                            nearStar.color = star.government.Model.spriteColor;
                        }
                        
                    }
                    nearStar.NotifyChange();

                }
                star.NotifyChange();
                //if (star.governmentInfluence < 250)
                //{
                //    star.government.Model = null;
                //    star.color = Color.grey;
                //    model.stars.Remove(star);
                //    
                //}
                yield return null;
            }
            govIndex++;
            if (govIndex >= data.governments.Count)
                govIndex = 0;
        }
    }
    /// <summary>
    /// runs commands on a station when called
    /// </summary>
    /// <param name="stationIndex">index of station in the station data to call</param>
    private void StationControl(int stationIndex)
    {
        StationModel model = data.stations[stationIndex];
        double deltaTime = data.date.time - model.age.time - model.dateCreated.time;
        model.age.AddTime(deltaTime);
        //int factoryStatus = model.factory.UpdateProduction(deltaTime);

        //foreach (ProductionItem item in model.factory.inputItems)
        //{
        //    if (model.factory.outputItems.Length == 0 && factoryStatus > 0)
        //    {
        //        model.money += item.productionAmount * factoryStatus * item.price;
        //    }
        //}

        if (model.timeUpdate < model.age.time)
        {
            model.timeUpdate = model.age.time + Dated.Hour;

            //Money Evaluation
            model.money -= model.runningCost;
            model.money -= 10 * model.workers;

            if (model.manager.Model != model.owner.Model)
            {
                model.manager.Model.money += 85;
                model.money -= 85;
            }

            double moneyEarned = model.money - model.moneyStats.data["Money"][model.moneyStats.data["Money"].Count - 1].y;

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
        //    else starIndex = model.solarIndex;
        //    int planetIndex = UnityEngine.Random.Range(0, game.data.stars[starIndex].planets.Length);
        //    SolarBody parent;
        //    if (game.data.stars[starIndex].planets.Length > 0)
        //        parent = game.data.stars[starIndex].planets[planetIndex];
        //    else
        //        parent = game.data.stars[starIndex].sun;

        //    Polar2 position = new Polar2(UnityEngine.Random.Range(parent.bodyRadius + 2, parent.SOI), UnityEngine.Random.Range(0, 2 * Mathf.PI));

        //    StationModel station = StationCreator.CreateStation((FactoryType)factoryIndex, game.data.stars[starIndex], parent, position, model.owner.Model);
        //    data.stations.Add(station);
        //    UpdateCreatures(station);
        //    model.money -= 1750000;
        //    model.owner.Model.money += 1000000;
        //    print(name + " Bought " + (FactoryType)factoryIndex);
        //}
    }
    /// <summary>
    /// Runs commands on ships when called
    /// </summary>
    /// <param name="shipIndex">index of ship in the ship data to call</param>
    private void ShipControl(int shipIndex)
    {
        ShipModel model = data.ships[shipIndex];
        double deltaTime = data.date.time - model.age.time - model.dateCreated.time;
        model.age.AddTime(deltaTime);

        if (model.timeUpdate < model.age.time)
        {
            model.timeUpdate += Dated.Hour;

            //Money Evaluation
            model.money -= model.runningCost;
            model.money -= model.workers * 15;

            if (model.captain.Model != model.owner.Model)
            {
                model.captain.Model.money += 35;
                model.money -= 35;
            }

            double moneyEarned = model.money - model.moneyStats.data["Money"][model.moneyStats.data["Money"].Count - 1].y;

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
            //model.target = new ModelRef<StructureModel>(FindClosestStation( ItemTypes.Fuel.ToString() , model));
            if (model.target.Model != null)
            {
                StationModel station = (StationModel) model.target.Model;
                Items buyItem = new Items(ItemTypes.Fuel, model.fuelCapacity);
                //model.items.Add(station.Buy(buyItem, model));
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
                    ShipModel ship = ShipCreator.CreateShip(model.owner.Model.name + "." + model.owner.Model.itemsBought, model.solarIndex, model.parentIndex, model.orbit, model.owner.Model, captain);
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

        ShipTravel(model, deltaTime);
    }
    /// <summary>
    /// Controls ship movement, taking into the account of when the ship was last called
    /// </summary>
    /// <param name="model">ship model</param>
    /// <param name="deltaTime">time since the ship was last called</param>
    private void ShipTravel(ShipModel model, double deltaTime)
    {
        if (model.target != null && model.target.Model != null && !model.hyperSpace)
        {
            //Vector3 distance = target.transform.position - transform.position;
            //Polar2 angleOfAttack = new Polar2(distance);
            //float rotateAmount = angleOfAttack.angle * Mathf.Rad2Deg - transform.eulerAngles.z;
            //transform.Rotate(0, 0, rotateAmount * model.rotateSpeed * Time.deltaTime);
            //distance.Normalize();
            //Vector2 targetPosition = model.target.Model.GamePosition(data.date.time);
            //model.orbit.SetWorldPosition( Vector3d.MoveTowards((Vector3d) model.orbit.Radius(data.date.time), (Vector2d) targetPosition, model.speed * deltaTime), data.date.time);
            model.fuel.amount -= (float) (deltaTime / model.fuelEfficiency);

            //Vector2d distance = targetPosition - model.orbit.Radius(data.date.time);
            

            //if (distance.sqrMagnitude < 1)
            //{

            //    StationModel station = (StationModel)model.target.Model;
            //    model.target = null;
            //    if (model.mode == ShipMode.Buy)
            //    {
            //        station.incomingShips.Remove(model);

            //        foreach (Items item in model.items)
            //        {
            //            foreach (Items buyItem in station.factory.outputItems)
            //            {
            //                if (item.name == ItemTypes.Fuel.ToString())
            //                {
            //                    model.fuel.amount += item.pendingAmount;
            //                    model.mode = ShipMode.Idle;
            //                    //yield break;
            //                }
            //                else if (item.name == ItemTypes.Ship.ToString())
            //                {
            //                    item.amount += (item.pendingAmount - item.amount);
            //                    buyItem.amount -= (item.pendingAmount - item.amount);
            //                    model.mode = ShipMode.Idle;
            //                    //yield break;
            //                }
            //                else if (item.name == buyItem.name)
            //                {
            //                    item.amount += (item.pendingAmount - item.amount);
            //                    buyItem.amount -= (item.pendingAmount - item.amount);
            //                    //new WaitForSeconds(item.amount * .1f);
            //                }
            //            }

            //        }
            //        if (model.sellTarget != null && model.sellTarget.Model != null)
            //        {
            //            model.mode = ShipMode.Sell;
            //            model.target = model.sellTarget;
            //            model.sellTarget.Model = null;
            //            station = (StationModel)model.target.Model;
            //            foreach (Items item in model.items)
            //            {
            //                item.amount -= station.Sell(item, model).amount;
            //            }
            //            //sprite.color = Color.green; //Used to color the ship
            //        }
            //    }
            //    else if (model.mode == ShipMode.Sell)
            //    {
            //        foreach (Items item in model.items)
            //        {
            //            foreach (Items sellItem in station.factory.inputItems)
            //            {
            //                if (item.name == sellItem.name)
            //                {
            //                    item.amount -= (item.amount - item.pendingAmount);
            //                    sellItem.amount += (item.amount - item.pendingAmount);
            //                    //new WaitForSeconds(item.amount * .1f);
            //                }
            //            }
            //        }
            //        station.incomingShips.Remove(model);
            //        model.mode = ShipMode.Idle;
            //    }
            //}

        }
    }
    /// <summary>
    /// Allows the user view to go to the target when a button or shortcut is clecked
    /// </summary>
    /// <param name="model"></param>
    internal void GoToTarget(IdentityModel model)
    {
        if (model.identityType == IdentityType.Government)
        {
            galaxyView = true;
            galaxy.GalaxyView();
            //transform.position = new Vector3(CameraController.CameraOffsetPoistion(data.stars[model.solarIndex].galacticPosition).x, CameraController.CameraOffsetPoistion(data.stars[model.solarIndex].galacticPosition).y, -10);

        }
    }
    internal void GoToTarget(SolarModel model)
    {
        galaxyView = true;
        galaxy.GalaxyView();
        //transform.position = new Vector3(CameraController.CameraOffsetPoistion(model.galacticPosition).x, CameraController.CameraOffsetPoistion(model.galacticPosition).y, -10);
    }
    private void OnFindRouteFinished(ShipModel model, Items buyItem, StructureModel[] targets, bool success)
    {
        if (success)
        {
            model.target.Model = targets[0];
            model.sellTarget.Model = targets[1];
            model.mode = ShipMode.Buy;
            StationModel station = (StationModel)model.target.Model;
            //model.items.Add(station.Buy(buyItem, model));
            station.incomingShips.Add(model);
            model.NotifyChange();
        }
        else
        {
            model.mode = ShipMode.Idle;
        }
    }

    //private StationModel FindClosestStation(string itemName, ShipModel model)
    //{
    //    double distance = 100000000;
    //    StationModel foundStation = null;
    //    foreach (StationModel station in data.stations)
    //    {
    //        foreach (Items outputItem in station.factory.outputItems)
    //        {
    //            //double closestDistance = Vector2.Distance(data.stars[station.solarIndex].galacticPosition, model.position) * 1000;
    //            //closestDistance += Vector2.Distance(station.GamePosition(data.date.time), model.GamePosition(data.date.time));
    //            //if (outputItem.name == itemName && closestDistance < distance && outputItem.amount > 0)
    //            //{
    //            //    distance = closestDistance;
    //            //    foundStation = station;

    //            //}
    //        }
    //    }

    //    return foundStation;
    //}
    //private void SetStationLinks()
    //{
    //    StationController thisStation = selectedObj.GetComponent<StationController>();

    //    foreach (StationModel station in data.stations)
    //    {
    //        foreach (Items item in station.factory.inputItems)
    //        {
    //            foreach (Items thisItem in thisStation.GetOutputItems())
    //            {
    //                if (thisItem.name == item.name)
    //                {
    //                    station.lineTarget = thisStation.transform.position;
    //                    station.lineColor = item.color;
    //                    station.NotifyChange();
    //                }
    //            }
                

    //        }
    //        foreach (Items item in station.factory.outputItems)
    //        {
    //            foreach (Items thisItem in thisStation.GetInputItems())
    //            {
    //                if (thisItem.name == item.name)
    //                {
    //                    station.lineTarget = thisStation.transform.position;
    //                    station.lineColor = item.color;
    //                    station.NotifyChange();
    //                }
    //            }

    //        }
    //    }
    //}

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
            stats += string.Format("\n{0}. {1} - {2} | {3}/{4}", i+1, "<color=" + ColorTypeConverter.ToRGBHex(sortedStations[i].color) + ">" + sortedStations[i].name + "</color>", sortedStations[i].money.ToString("0.00"), (sortedStations[i].factory.productionProgress * sortedStations[i].factory.produtionTime).ToString("0.00"), sortedStations[i].factory.produtionTime.ToString("0.00"));
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
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeControl;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Vectrosity;
/// <summary>
/// Manages the main aspects of the game, including player interaction, main ai, game ui, and view changes. Anything else should be moved to a more specific file
/// </summary>
public class GameManager : MonoBehaviour {

    public GameObject galaxyManager;
    public GameObject pathfindingManager;
    public LoadingPanel loadingPanel;
    public GameObject exitPanel;
    public GameObject savePanel;
    public Slider saveSlider;
    public Image saveFill;
    public Text dateInfo;
    public Text nameOfSystem;
    internal float localScaleMod = 1;
    internal bool galaxyView = true;
    public int numShipsPerFrame;
    public int numStationsPerFrame;
    public double marketPriceMod = .0001f;
    internal GameObject selectedObj;
    
    
    internal GameDataModel data;
    
    internal int statsDisplay = 0;
    internal GalaxyManager galaxy;
    internal TradeRouteRequestManager tradeRequestManager;
    internal static GameManager instance;

    private VectorLine line;
    public Texture lineTexture;

    //Used to setup the game and make sure nothing is running while things are being configured
    internal bool gameInitiated = false;
    internal bool setup = false;
    internal bool following = false;

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
        line = new VectorLine("Galaxy line", new List<Vector3>(), 1);

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
                            //if (selectedObj.tag == "station")
                            //{
                            //    OpenInfoPanel(selectedObj.GetComponent<StationController>().Model);
                            //}

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
                if (Input.GetKeyDown(KeyCode.Slash))
                {
                    timeScale = 1;
                }
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (selectedObj != null && !following)
                    {
                        following = true;
                        data.cameraGalaxyPosition += (Vector2d)(Vector2)selectedObj.transform.position * data.cameraGalaxyOrtho / Camera.main.orthographicSize;
                    }
                    else following = false;
                }

                dateInfo.text = data.date.GetDateTime() + "\nTimescale: " + timeScale + "\n";
                CreateLines();

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

    private void CreateLines()
    {

        //if (MapTogglePanel.instance.galaxyConnections.isOn)
        //{
        //    var points = new List<Vector3>();
        //    var colors = new List<Color32>();
        //    ModelRefs<SolarModel> models = new ModelRefs<SolarModel>();
        //    foreach (SolarModel solar in data.stars)
        //    {
        //        models.Add(solar);
        //    }

        //    line.SetWidth((float)(Mathd.Pow((models[0].solar.bodyRadius), .02f) * data.cameraGalCameraScaleMod) * 5);

        //    while (models.Count > 0)
        //    {
        //        var model = models[0];
        //        models.RemoveAt(0);
        //        if (CameraController.CameraOffsetGalaxyPosition(model.galacticPosition).sqrMagnitude < 40000)
        //        {
        //            foreach (SolarModel neighbor in model.nearStars)
        //            {
        //                if (models.Contains(neighbor))
        //                {
        //                    points.Add(CameraController.CameraOffsetGalaxyPosition(model.galacticPosition));
        //                    points.Add(CameraController.CameraOffsetGalaxyPosition(neighbor.galacticPosition));

        //                    if (MapTogglePanel.instance.galaxyTerritory.isOn)
        //                    {
        //                        if (model.government.Model != null)
        //                        {
        //                            var govModel = model.government.Model;
        //                            colors.Add(new Color32((byte)(govModel.spriteColor.r * 255), (byte)(govModel.spriteColor.g * 255), (byte)(govModel.spriteColor.b * 255), 50));
        //                        }
        //                        else
        //                        {
        //                            colors.Add(new Color32((byte)(50), (byte)(50), (byte)(50), 10));
        //                        }

        //                        //if (neighbor.government.Model != null)
        //                        //{
        //                        //    var govModel = neighbor.government.Model;
        //                        //    colors.Add(new Color32((byte)(govModel.spriteColor.r * 255), (byte)(govModel.spriteColor.g * 255), (byte)(govModel.spriteColor.b * 255), 50));
        //                        //}
        //                        //else
        //                        //{
        //                        //    colors.Add(new Color32((byte)(50), (byte)(50), (byte)(50), 10));
        //                        //}

        //                    }
        //                    else
        //                    {
        //                        colors.Add(new Color32((byte)(model.solar.color.r * 255), (byte)(model.solar.color.g * 255), (byte)(model.solar.color.b * 255), 10));
        //                        //colors.Add(new Color32((byte)(neighbor.solar.color.r * 255), (byte)(neighbor.solar.color.g * 255), (byte)(neighbor.solar.color.b * 255), 10));
        //                    }
        //                }
        //            }
        //        }
                
                
        //    }

        //    line.points3 = points;
        //    line.SetColors(colors);
        //    line.Draw3D();

        //}
        //else
        //{
        //    //if (line.lineWidth != 0)
        //    //{
        //    //    line.SetWidth(0);
        //    //    line.Draw3D();
        //    //}
        //}
        
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
        StartCoroutine("UpdateGovernments");
        StartCoroutine("UpdateCompanies");
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

    internal void OpenInfoPanel(int id,  TargetType targetType)
    {
        InfoPanelModel infoModel = new InfoPanelModel(id,targetType);
        Controller.Instantiate<InfoPanelController>("infopanelcanvas", infoModel);
    }

    internal void OpenInfoPanel(Creature creature)
    {
        InfoPanelModel infoModel = new InfoPanelModel(creature);
        Controller.Instantiate<InfoPanelController>("infopanelcanvas", infoModel);
    }
    internal void OpenInfoPanel(IdentityModel model)
    {
        InfoPanelModel infoModel = new InfoPanelModel(model);
        Controller.Instantiate<InfoPanelController>("infopanelcanvas", infoModel);
    }
    internal void OpenInfoPanel(List<int> solarIndex)
    {
        InfoPanelModel infoModel = new InfoPanelModel(solarIndex);
        Controller.Instantiate<InfoPanelController>("infopanelcanvas", infoModel);
    }
    internal void OpenInfoPanel(Structure structure)
    {
        InfoPanelModel infoModel = new InfoPanelModel(structure);
        Controller.Instantiate<InfoPanelController>("infopanelcanvas", infoModel);
    }
    internal void OpenInfoPanel(Item item)
    {
        InfoPanelModel infoModel = new InfoPanelModel(item);
        Controller.Instantiate<InfoPanelController>("infopanelcanvas", infoModel);
    }
    internal void OpenInfoPanel(RawResource resource)
    {
        InfoPanelModel infoModel = new InfoPanelModel(resource);
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
            //if (shipIndex >= data.ships.Count)
            //    shipIndex = 0;
            //for (int i = 0; i < updatesPerFrame; i++)
            //{
            //    //ShipControl(shipIndex);
            //    shipIndex++;
            //}

            yield return null;
        }
    }
    /// <summary>
    /// Updates all stations per frame
    /// </summary>
    /// <param name="updatesPerFrame">how many stations to update per frame.</param>
    /// <returns></returns>
    //IEnumerator UpdateStations(int updatesPerFrame)
    //{
    //    int stationIndex = 0;
    //    while (true)
    //    {
    //        if (stationIndex >= data.stations.Count)
    //        {
    //            stationIndex = 0;

    //        }
                
    //        for (int i = 0; i < updatesPerFrame; i++)
    //        {
    //            StationControl(stationIndex);
    //        }
    //        stationIndex++;
    //        yield return null;
    //    }
    //}
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

            double deltaTime = data.date.time - model.age.time - model.dateCreated.time;
            model.age.AddTime(deltaTime);

            foreach (SolarModel star in model.stars)
            {
                if (star.solar == null)
                {
                    star.Delete();
                }
                else
                {
                    double totalPop = star.solar.totalPopulation;

                    //star.governmentInfluence += (totalPop * .000000001f * (float)data.date.deltaTime) - (star.governmentInfluence * .0000001f * (float)data.date.deltaTime);

                    //foreach (SolarModel nearStar in star.nearStars)
                    //{
                    //    nearStar.governmentInfluence += star.governmentInfluence * .0000001f * (float)data.date.deltaTime;
                    //    star.governmentInfluence -= star.governmentInfluence * .0000001f * (float)data.date.deltaTime;

                    //    if (nearStar.governmentInfluence > 250 && nearStar.government.Model == null)
                    //    {
                    //        nearStar.government.Model = star.government.Model;
                    //        model.stars.Add(nearStar);

                    //        if (galaxy.mapButtonCanvases.Count > 0)
                    //        {
                    //            nearStar.color = star.government.Model.spriteColor;
                    //        }

                    //    }
                        //nearStar.NotifyChange();

                    //}
                    //star.NotifyChange();
                }

            }
            foreach (List<int> solarIndex in model.solarBodiesWithStructures)
            {
                var body = data.getSolarBody(solarIndex);
                bool found = false;
                //if (body.deleteStructure)
                //{

                //}
                foreach (Structure owned in body.spaceStructures)
                {
                    if (owned.owner.Model == model)
                    {
                        found = true;

                        if (owned.structureType == Structure.StructureTypes.SpaceStation)
                        {
                            ((Station)owned).Update(body, deltaTime);
                        }
                        if ((owned).deleteStructure)
                        {
                            body.groundStructures.Remove(owned);
                            break;
                        }
                    }

                }
                foreach (Structure owned in body.groundStructures)
                {
                    if (owned.owner.Model == model)
                    {
                        found = true;

                        if (owned.structureType == Structure.StructureTypes.Driller)
                        {
                            ((Driller)owned).UpdateProduction(body, deltaTime);
                        }
                        if (owned.structureType == Structure.StructureTypes.Factory)
                        {
                            ((Factory)owned).UpdateProduction(body, deltaTime);
                        }
                        if (owned.structureType == Structure.StructureTypes.BuildStructure)
                        {
                            ((BuildStructure)owned).UpdateProduction(body, deltaTime);
                            if (((BuildStructure)owned).deleteStructure)
                            {
                                body.groundStructures.Remove(owned);
                                break;
                            }
                            
                        }
                        if (owned.structureType == Structure.StructureTypes.GroundStorage)
                        {
                            ((GroundStorage)owned).UpdateProduction(body, deltaTime);
                        }
                    }
                    
                }
                if (!found)
                {
                    model.solarBodiesWithStructures.Remove(solarIndex);
                    break;
                }
            }
            
            yield return null;
            govIndex++;
            if (govIndex >= data.governments.Count)
                govIndex = 0;
        }
    }
    /// <summary>
    /// Updates all the government bodies per frame
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateCompanies()
    {
        int compIndex = 0;
        while (true)
        {
            CompanyModel model = data.companies[compIndex];

            double deltaTime = data.date.time - model.age.time - model.dateCreated.time;
            model.age.AddTime(deltaTime);

            foreach (List<int> solarIndex in model.solarBodiesWithStructures)
            {
                var body = data.getSolarBody(solarIndex);
                bool found = false;
                //if (body.deleteStructure)
                //{

                //}
                foreach (Structure owned in body.spaceStructures)
                {
                    if (owned.owner.Model == model)
                    {
                        found = true;

                        if (owned.structureType == Structure.StructureTypes.SpaceStation)
                        {
                            ((Station)owned).Update(body, deltaTime);
                        }
                        if ((owned).deleteStructure)
                        {
                            body.groundStructures.Remove(owned);
                            break;
                        }
                    }

                }
                foreach (Structure owned in body.groundStructures)
                {
                    if (owned.owner.Model == model)
                    {
                        found = true;

                        if (owned.structureType == Structure.StructureTypes.Driller)
                        {
                            ((Driller)owned).UpdateProduction(body, deltaTime);
                        }
                        if (owned.structureType == Structure.StructureTypes.Factory)
                        {
                            ((Factory)owned).UpdateProduction(body, deltaTime);
                        }
                        if (owned.structureType == Structure.StructureTypes.BuildStructure)
                        {
                            ((BuildStructure)owned).UpdateProduction(body, deltaTime);
                            if (((BuildStructure)owned).deleteStructure)
                            {
                                body.groundStructures.Remove(owned);
                                break;
                            }

                        }
                        if (owned.structureType == Structure.StructureTypes.GroundStorage)
                        {
                            ((GroundStorage)owned).UpdateProduction(body, deltaTime);
                        }
                    }

                }
                if (!found)
                {
                    model.solarBodiesWithStructures.Remove(solarIndex);
                    break;
                }
            }

            yield return null;
            compIndex++;
            if (compIndex >= data.companies.Count)
                compIndex = 0;
        }
    }
    /// <summary>
    /// runs commands on a station when called
    /// </summary>
    /// <param name="stationIndex">index of station in the station data to call</param>
    //private void StationControl(int stationIndex)
    //{
    //    Station model = data.stations[stationIndex];
    //    double deltaTime = data.date.time - model.age.time - model.dateCreated.time;
    //    model.age.AddTime(deltaTime);
    //    //int factoryStatus = model.factory.UpdateProduction(deltaTime);

    //    //foreach (ProductionItem item in model.factory.inputItems)
    //    //{
    //    //    if (model.factory.outputItems.Length == 0 && factoryStatus > 0)
    //    //    {
    //    //        model.money += item.productionAmount * factoryStatus * item.price;
    //    //    }
    //    //}

    //    if (model.timeUpdate < model.age.time)
    //    {
    //        model.timeUpdate = model.age.time + Dated.Hour;

    //        //Money Evaluation
    //        model.money -= model.runningCost;
    //        model.money -= 10 * model.workers;

    //        if (model.manager.Model != model.owner.Model)
    //        {
    //            model.manager.Model.money += 85;
    //            model.money -= 85;
    //        }

    //        double moneyEarned = model.money - model.moneyStats.data["Money"][model.moneyStats.data["Money"].Count - 1].y;

    //        if (moneyEarned > 0)
    //        {
    //            model.manager.Model.money += moneyEarned * .1f;
    //            model.money -= moneyEarned * .1f;
    //        }
    //        moneyEarned = model.money - model.moneyStats.data["Money"][model.moneyStats.data["Money"].Count - 1].y;
    //        model.moneyChange = moneyEarned;
    //        model.moneyStats.data["Money Change"].Add(new Stat(model.age.time, moneyEarned));
    //        model.moneyStats.data["Money"].Add(new Stat(model.age.time, model.money));

    //        model.owner.Model.money += model.money;
    //        model.money = 0;


    //    }

    //    if (model.money < 0)
    //    {
    //        foreach (ShipModel ship in model.incomingShips)
    //        {
    //            ship.NotifyChange();
    //        }
    //        print(model.name + " Died");
    //        model.Delete();
    //    }
    //    //else if (model.money > 5000000)
    //    //{
    //    //    int factoryIndex = UnityEngine.Random.Range(0, 10);
    //    //    int starIndex;
    //    //    if (UnityEngine.Random.Range(0, 2) == 1)
    //    //        starIndex = UnityEngine.Random.Range(0, galaxy.starCount);
    //    //    else starIndex = model.solarIndex;
    //    //    int planetIndex = UnityEngine.Random.Range(0, game.data.stars[starIndex].planets.Length);
    //    //    SolarBody parent;
    //    //    if (game.data.stars[starIndex].planets.Length > 0)
    //    //        parent = game.data.stars[starIndex].planets[planetIndex];
    //    //    else
    //    //        parent = game.data.stars[starIndex].sun;

    //    //    Polar2 position = new Polar2(UnityEngine.Random.Range(parent.bodyRadius + 2, parent.SOI), UnityEngine.Random.Range(0, 2 * Mathf.PI));

    //    //    StationModel station = StationCreator.CreateStation((FactoryType)factoryIndex, game.data.stars[starIndex], parent, position, model.owner.Model);
    //    //    data.stations.Add(station);
    //    //    UpdateCreatures(station);
    //    //    model.money -= 1750000;
    //    //    model.owner.Model.money += 1000000;
    //    //    print(name + " Bought " + (FactoryType)factoryIndex);
    //    //}
    //}
    /// <summary>
    /// Runs commands on ships when called
    /// </summary>
    /// <param name="shipIndex">index of ship in the ship data to call</param>
    //private void ShipControl(int shipIndex)
    //{
    //    Ship model = data.ships[shipIndex];
    //    double deltaTime = data.date.time - model.age.time - model.dateCreated.time;
    //    model.age.AddTime(deltaTime);

    //    if (model.timeUpdate < model.age.time)
    //    {
    //        model.timeUpdate += Dated.Hour;

    //        //Money Evaluation
    //        model.money -= model.runningCost;
    //        model.money -= model.workers * 15;

    //        if (model.captain.Model != model.owner.Model)
    //        {
    //            model.captain.Model.money += 35;
    //            model.money -= 35;
    //        }

    //        double moneyEarned = model.money - model.moneyStats.data["Money"][model.moneyStats.data["Money"].Count - 1].y;

    //        if (moneyEarned > 0)
    //        {
    //            model.captain.Model.money += moneyEarned * .1f;
    //            model.money -= moneyEarned * .1f;
    //        }
    //        moneyEarned = model.money - model.moneyStats.data["Money"][model.moneyStats.data["Money"].Count - 1].y;
    //        model.moneyChange = moneyEarned;
    //        model.moneyStats.data["Money Change"].Add(new Stat(model.age.time, model.moneyChange));
    //        model.moneyStats.data["Money"].Add(new Stat(model.age.time, model.money));
    //        model.owner.Model.money += model.money;
    //        model.money = 0;
    //        //if (model.money < 0)
    //        //{
    //        //    if (model.mode == ShipMode.Sell && model.target != null && model.target.Model != null)
    //        //    {
    //        //        ((StationModel) model.target.Model).SellIncomplete(model.item);
    //        //    }
    //        //    print(model.name + " Died");
    //        //    model.Delete();
    //        //    return;
    //        //}

    //    }

    //    if ((float)model.fuel.amount / model.fuelCapacity < .25f && model.target == null)
    //    {
    //        //model.target = new ModelRef<StructureModel>(FindClosestStation( ItemTypes.Fuel.ToString() , model));
    //        //if (model.target != null)
    //        //{
    //        //    Station station = (Station) model.target.Model;
    //        //    Items buyItem = new Items(ItemTypes.Fuel, model.fuelCapacity);
    //        //    //model.items.Add(station.Buy(buyItem, model));
    //        //    model.spriteColor = Color.yellow;
    //        //    model.mode = ShipMode.Buy;
    //        //    station.incomingShips.Add(model);
    //        //    model.NotifyChange();
    //        //    return;
    //        //}
    //        //else if (model.fuel.amount < 0) {  }
    //    }

    //    foreach (ItemsModel item in model.items)
    //    {
    //        if (item.name == ItemTypes.Ship.ToString() && item.amount > 0)
    //        {
    //            for (int i = 0; i < item.amount; i++)
    //            {
    //                CreatureModel captain = new CreatureModel(model.owner.Model.name + " Ship Captain " + model.owner.Model.itemsBought, 1000);
    //                Ship ship = ShipCreator.CreateShip(model.owner.Model.name + "." + model.owner.Model.itemsBought, model.solarIndex, model.parentIndex, model.orbit, model.owner.Model, captain);
    //                data.ships.Add(ship);
    //                data.creatures.Add(captain);
    //                model.owner.Model.itemsBought++;
    //            }

    //            item.amount = 0;
    //        }

    //        if (item.amount == 0 && item.pendingAmount == 0)
    //            model.items.Remove(item);
    //    }


    //    //if (model.money > 10000 && model.target.Model == null)
    //    //{
    //    //    model.target.Model = FindClosestStation("Ship", model);
    //    //    if (model.target.Model != null)
    //    //    {
    //    //        StationModel station = (StationModel) model.target.Model;
    //    //        foreach (Items outputItem in station.factory.outputItems)
    //    //        {
    //    //            if (outputItem.name == "Ship")
    //    //            {
    //    //                if (outputItem.price + 2000 < model.money)
    //    //                {
    //    //                    model.item = new Items("Ship", 1);
    //    //                    model.spriteColor = Color.cyan;
    //    //                    model.mode = ShipMode.Buy;
    //    //                    station.incomingShips.Add(model);
    //    //                    model.NotifyChange();
    //    //                    return;
    //    //                }
    //    //                else
    //    //                {
    //    //                    model.target.Model = null;
    //    //                }
    //    //            }
    //    //        }
    //    //    }
    //    //}
    //    if (model.shipAction == ShipMode.Idle)
    //    {
    //        model.mode = ShipMode.SearchingTradeRoute;
    //        TradeRouteRequestManager.RequestTradeRoute(model, OnFindRouteFinished);
    //    }

    //    ShipTravel(model, deltaTime);
    //}
    /// <summary>
    /// Controls ship movement, taking into the account of when the ship was last called
    /// </summary>
    /// <param name="model">ship model</param>
    /// <param name="deltaTime">time since the ship was last called</param>
    private void ShipTravel(Ship model, double deltaTime)
    {
        if (model.target != null && model.target != null && !model.hyperSpace)
        {
            //Vector3 distance = target.transform.position - transform.position;
            //Polar2 angleOfAttack = new Polar2(distance);
            //float rotateAmount = angleOfAttack.angle * Mathf.Rad2Deg - transform.eulerAngles.z;
            //transform.Rotate(0, 0, rotateAmount * model.rotateSpeed * Time.deltaTime);
            //distance.Normalize();
            //Vector2 targetPosition = model.target.Model.GamePosition(data.date.time);
            //model.orbit.SetWorldPosition( Vector3d.MoveTowards((Vector3d) model.orbit.Radius(data.date.time), (Vector2d) targetPosition, model.speed * deltaTime), data.date.time);
            model.fuel.RemoveAmount((float) (deltaTime / model.fuelEfficiency));

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
            data.cameraGalaxyPosition = data.stars[model.solarIndex[0]].galacticPosition;

        }
    }
    internal void GoToTarget(SolarModel model)
    {
        galaxyView = true;
        galaxy.GalaxyView();
        data.cameraGalaxyPosition = model.galacticPosition;
    }

    internal void GoToTarget(List<int> solarIndex)
    {
        galaxyView = false;
        var solarBody = data.getSolarBody(solarIndex);
        galaxy.solarModel = data.stars[solarIndex[0]];
        galaxy.GoToSolarView();
        data.cameraGalaxyPosition = solarBody.GamePosition(data.date.time) + data.stars[solarIndex[0]].galacticPosition;
        //transform.position = new Vector3(CameraController.CameraOffsetPoistion(model.galacticPosition).x, CameraController.CameraOffsetPoistion(model.galacticPosition).y, -10);
    }

    private void OnFindRouteFinished(Ship model, ItemsModel buyItem, StructureModel[] targets, bool success)
    {
        if (success)
        {
            //model.target. = targets[0];
            //model.sellTarget.Model = targets[1];
            //model.mode = ShipMode.Buy;
            //Station station = (Station)model.target.Model;
            ////model.items.Add(station.Buy(buyItem, model));
            //station.incomingShips.Add(model);
            //model.NotifyChange();
        }
        else
        {
            model.SetShipAction(ShipAction.Idle);
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
        Dictionary<string, ItemsModel> stationData = new Dictionary<string, ItemsModel>();

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
        List<ItemsModel> sortedItems = new List<ItemsModel>();
        foreach (ItemsModel item in stationData.Values)
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
        string stats = "";//"\nTotal Ships: " + data.ships.Count + "\n";

        //List<Ship> sortedShips = new List<Ship>();
        //foreach (Ship shipModel in data.ships)
        //{
        //    sortedShips.Add(shipModel);
        //}
        //sortedShips.Sort(delegate (Ship c1, Ship c2) { return c2.money.CompareTo(c1.money); });
        //for (int i = 0; i < sortedShips.Count; i++)
        //{
        //    stats += string.Format("\n{0}. {1} - {2} | {3}", i+1, sortedShips[i].name, sortedShips[i].money.ToString("0.00"), sortedShips[i].passangerCapacity);
        //}
        return stats;
    }
    //public void SetStationStats()
    //{
    //    selectedObj = null;
    //    statsDisplay = 2;
    //}
    //public string StationStats()
    //{
    //    string stats = "\nTotal Stations: " + data.stations.Count + "\n";
    //    List<Station> sortedStations = new List<Station>();
    //    foreach (Station station in data.stations)
    //    {
    //        sortedStations.Add(station);
    //    }
    //    sortedStations.Sort(delegate (Station c1, Station c2) { return c2.money.CompareTo(c1.money); });
    //    for (int i = 0; i < sortedStations.Count; i++)
    //    {
    //        stats += string.Format("\n{0}. {1} - {2} | {3}/{4}", i+1, "<color=" + ColorTypeConverter.ToRGBHex(sortedStations[i].color) + ">" + sortedStations[i].name + "</color>", sortedStations[i].money.ToString("0.00"), (sortedStations[i].factory.productionProgress * sortedStations[i].factory.produtionTime).ToString("0.00"), sortedStations[i].factory.produtionTime.ToString("0.00"));
    //    }

    //    return stats;
    //}
    public void SetCreatureStats()
    {
        selectedObj = null;
        statsDisplay = 3;
    }
    public string CreatureStats()
    {
        string stats = "\nTotal Creatures:" + data.creatures.Model.creatures.Count;
        //List<CreatureModel> sortedCreatures = new List<CreatureModel>();
        //foreach (CreatureModel creature in data.creatures)
        //{
        //    sortedCreatures.Add(creature);
        //}
        //sortedCreatures.Sort(delegate (CreatureModel c1, CreatureModel c2) { return c2.money.CompareTo(c1.money); });
        //for (int i = 0; i < sortedCreatures.Count; i++)
        //{
        //    stats += string.Format("\n{0}. {1} - {2} | {3} - {4}", i + 1, sortedCreatures[i].name, sortedCreatures[i].money.ToString("0.00"), sortedCreatures[i].ships.Count, sortedCreatures[i].stations.Count);
        //    if (i >= 99)
        //        break;
        //}
        return stats;
    }
    public void SaveGame()
    {
        //StartCoroutine("Save");
        Model.SaveAll("TraderSaves");
    }

    public IEnumerator Save()
    {
        savePanel.SetActive(true);
        int all = Model.GetAll().Count;

        float progress = 0;
        foreach(Model model in Model.GetAll())
        {
            Model.Save("TraderSaves", new List<Model>() { model }, false);
            progress++;
            saveSlider.value = progress / all;
            saveFill.color = Color.HSVToRGB(saveSlider.value * .2f, 1, 1);
            yield return null;
        }
        savePanel.SetActive(false);
        print("Game Saved");
    }
}

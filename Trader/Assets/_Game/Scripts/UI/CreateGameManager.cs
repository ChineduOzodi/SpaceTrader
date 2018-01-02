using CodeControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateGameManager : MonoBehaviour {

    public GameObject loadingPanel;
    public Text loadingText;
    public Slider loadingProgress;
    public Text galaxyNameText;

    //Galaxy Parameters
    public int starCount;
    public Vector2 mapField;
    public Gradient sunSizeColor;
    public Gradient planetSizeColor;

    //Game Parameters
    public string galaxyName;
    public int numGov;
    public int numComp;
    public int numStation;
    public int numShip;

    internal GameManager game;
    internal GalaxyManager galaxy;

    internal static float G = .01f;
    internal NameGen names;

    public void Awake()
    {
        transform.localPosition = Vector3.zero;
    }

    public void CreateGame()
    {
        galaxy = GalaxyManager.instance;
        game = GameManager.instance;
        names = new NameGen();
        GameManager.instance.setup = true;
        loadingPanel.SetActive(true);
        StartCoroutine("CreatingGame"); //Creates a new Game
    }

    IEnumerator CreatingGame()
    {
        loadingText.text = "Creating Game...";
        game.galaxy.gameObject.SetActive(true); //Sets the galaxy manager to active
        game.pathfindingManager.SetActive(true); //Sets pathfinding to active
        loadingProgress.value = 0;
        while (GameManager.instance.setup)
        {
            //CreateStarSystem
            loadingText.text = "Creating Stars...";
            game.data.galaxyName = galaxyName;
            galaxyNameText.text = galaxyName;

            foreach (RawResourceBlueprint raw in game.data.rawResources.Model.rawResources)
            {
                var item = new ItemBlueprint(raw);
                GameManager.instance.data.itemsData.Model.items.Add(item);
            }
            game.data.itemsData.Model.items.Add(
                new ItemBlueprint("Fuel Cell", ItemType.Fuel, "A normal fuel cell.", 60, new List<Item>() {
                    new Item(defaultRawResources.Fuelodite.ToString(), (int)defaultRawResources.Fuelodite, ItemType.RawMaterial, 10,1) }));
            game.data.itemsData.Model.items.Add(
                new ItemBlueprint("Basic AI", ItemType.AI, "A basic AI unit.", Dated.Hour, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), (int)defaultRawResources.Armoroid, ItemType.RawMaterial, 1,1),
                    new Item(defaultRawResources.Coppode.ToString(), (int)defaultRawResources.Coppode, ItemType.RawMaterial, 5,1)
                }));
            game.data.itemsData.Model.items.Add(
                new ItemBlueprint("Basic Machinery", ItemType.FactoryMachinery, "Factory Machinery.", Dated.Hour, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), (int)defaultRawResources.Armoroid, ItemType.RawMaterial, 10,1),
                    new Item(ItemType.AI, 1,1)
                }));
            game.data.itemsData.Model.items.Add(
                new ItemBlueprint("Basic Factory", ItemType.Factory, "A usable factory.",2 * Dated.Day, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), (int)defaultRawResources.Armoroid, ItemType.RawMaterial, 250,1),
                    new Item(ItemType.FactoryMachinery, 1,1)
                }));
            game.data.itemsData.Model.items.Add(
               new ItemBlueprint("Basic Driller", ItemType.Driller, "A usable driller.", Dated.Day, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), (int)defaultRawResources.Armoroid, ItemType.RawMaterial, 200,1),
                    new Item(ItemType.FactoryMachinery, 1,1)
               }));
            game.data.itemsData.Model.items.Add(
               new ItemBlueprint("Basic Ground Storgae", ItemType.GroundStorage, "Used to store items.", .5f * Dated.Day, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), (int)defaultRawResources.Armoroid, ItemType.RawMaterial, 100,1),
               }));
            game.data.itemsData.Model.items.Add(
                new ItemBlueprint("Basic Engine", ItemType.Engine, "A usable engine.", .5f * Dated.Day, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), (int)defaultRawResources.Armoroid, ItemType.RawMaterial, 20,1),
                    new Item(defaultRawResources.Warium.ToString(), (int)defaultRawResources.Warium, ItemType.RawMaterial, 5,1)
                }));
            game.data.itemsData.Model.items.Add(
                new ItemBlueprint("Ship fueltank", ItemType.FuelTank, "Ship fueltank.", 3 * Dated.Hour, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), (int)defaultRawResources.Armoroid, ItemType.RawMaterial, 15,1),
                    new Item(defaultRawResources.Limoite.ToString(), (int)defaultRawResources.Limoite, ItemType.RawMaterial, 1,1)
                }));
            game.data.itemsData.Model.items.Add(
                new ItemBlueprint("Ship sensor", ItemType.Sensor, "Basic ship sensor.", 5 * Dated.Hour, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), (int)defaultRawResources.Armoroid, ItemType.RawMaterial, 5,1),
                    new Item(defaultRawResources.Goldium.ToString(), (int)defaultRawResources.Goldium, ItemType.RawMaterial, 1,1),
                    new Item(ItemType.AI, 1,1)
                }));
            game.data.itemsData.Model.items.Add(
                new ItemBlueprint("Exploration Ship", ItemType.SpaceShip, "Basic ship", 5 * Dated.Day, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), (int)defaultRawResources.Armoroid, ItemType.RawMaterial, 200,1),
                    new Item(defaultRawResources.Glassitum.ToString(), (int)defaultRawResources.Glassitum, ItemType.RawMaterial, 20,1),
                    new Item(ItemType.AI, 1,1),
                    new Item(ItemType.Engine, 1,1),
                    new Item(ItemType.FuelTank, 1,1),
                    new Item(ItemType.Sensor, 1,1)
                }));
            game.data.itemsData.Model.items.Add(
                new ItemBlueprint("Combat Ship", ItemType.SpaceShip, "Basic combat ship", 8 * Dated.Day, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), (int)defaultRawResources.Armoroid, ItemType.RawMaterial, 400,1),
                    new Item(defaultRawResources.Glassitum.ToString(), (int)defaultRawResources.Glassitum, ItemType.RawMaterial, 20,1),
                    new Item(ItemType.AI, 2,1),
                    new Item(ItemType.Engine, 4,1),
                    new Item(ItemType.FuelTank, 2,1),
                    new Item(ItemType.Sensor, 1,1)
                }));
            CreateStars(starCount, game.data.galaxyName);
            LoadStars();
            loadingProgress.value = .5f;

            GameManager.instance.setup = false;
            yield return null;
            for (int a = 0; a < numGov; a++)
            {
                //Create Government
                loadingText.text = "Creating Governments...";
                List<int> leaders = new List<int>();

                
                GovernmentModel gov = new GovernmentModel(names.GenerateWorldName() + " Government", leaders);

                //Location
                SolarBody parent = FindGovernmentStar(gov);

                //Add leader

                Creature leader = new Creature(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 100000, parent.solarIndex,game.data.date, new Dated(30 * Dated.Year), CreatureType.Human);
                leaders.Add(leader.id);


                //Add Government Capital
                Station station = new Station(gov.name + " Station", gov, parent);

                //location
                gov.SetLocation(parent.solarIndex);

                //Add Drills and Factories

                foreach(RawResource raw in parent.rawResources)
                {
                    Driller driller = new Driller(gov, raw.id, parent, 1);
                    GroundStorage storage = new GroundStorage(gov, parent);
                }

                var factoryBlueprint = game.data.itemsData.Model.items.Find(x => x.itemType == ItemType.Factory);

                foreach (ItemBlueprint item in game.data.itemsData.Model.items)
                {
                    if (item.itemType != ItemType.RawMaterial)
                    {
                        var factory = new Factory(gov, factoryBlueprint.id, item.id, parent.solarIndex);
                    }
                }
                
                //BuildStructure build = new BuildStructure(gov, game.data.itemsData.Model.items.Find(x => x.itemType == ItemType.Factory).id, game.data.itemsData.Model.items.Find(x => x.itemType == ItemType.Factory).id, parent);
                BuildStructure build = new BuildStructure(gov, StructureTypes.Ship, game.data.itemsData.Model.items.Find(x => x.name == "Combat Ship").id, parent);

                var solarModel = game.data.stars[parent.solarIndex[0]];

                for (int c = 0; c < numComp; c++)
                {
                    var random = new System.Random((gov.Id + c + gov.name).GetHashCode());
                    var body = solarModel.solar.satelites[random.Next(solarModel.solar.satelites.Count)];

                    Creature owner = new Creature(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 100000, body.solarIndex, game.data.date, new Dated(30 * Dated.Year), CreatureType.Human);
                    CompanyModel comp = new CompanyModel(names.GenerateRegionName() + " Company", body, gov, owner);
                    //if (c == 0)
                    //{
                    //    owner.isPlayer = true;
                    //    GameManager.instance.data.playerCreatureId = owner.id;
                    //}

                    var itemBlueprint = game.data.itemsData.Model.items[random.Next(game.data.itemsData.Model.items.Count)];
                    if (itemBlueprint.itemType != ItemType.RawMaterial)
                    {
                        var factory = new Factory(comp, factoryBlueprint.id, itemBlueprint.id, body.solarIndex);
                    }
                    else if (body.rawResources.Count > 0)
                    {
                        Driller driller = new Driller(comp, body.rawResources[random.Next(body.rawResources.Count)].id, body);
                        GroundStorage storage = new GroundStorage(comp, body);
                    }

                    //--------------Create Ships---------------------------//
                    for (int i = 0; i < numShip; i++)
                    {
                        if (i != 0)
                        {
                            owner = new Creature(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 10000, station, game.data.date, new Dated(30 * Dated.Year), CreatureType.Human);
                        }

                        Ship ship = new Ship(comp.name + " Ship " + (i + 1), comp, owner, -1);
                        //loadingText.text = string.Format("Creating ships: {0} of {1}", i, numShip);

                    }
                    yield return null;
                }
                loadingProgress.value = .5f + a / numGov * .5f;
                yield return null;
            }
            //Sets menu and loading panel to inactive and starts the game
            loadingText.text = "Done";
            loadingPanel.SetActive(false);
            gameObject.SetActive(false);
            game.StartGame();
        }
    }

    public void CreateStars(int count, string seed)
    {
        Random.InitState(seed.GetHashCode());
        var random = new System.Random(seed.GetHashCode());
        for (int i = 0; i < count; i++)
        {

            Vector2d position = new Vector2d((float) random.NextDouble() * mapField.x * 2 - mapField.x * .5f, (float)random.NextDouble() * mapField.y * 2 - mapField.y * .5f);
            position = new Vector2d((float)NormalizedRandom(-mapField.x * .5f, mapField.x * .5f, random.NextDouble()), (float)NormalizedRandom(-mapField.y * .5f, mapField.y * .5f, random.NextDouble()));
            SolarModel star = new SolarModel(names.GenerateWorldName(random.Next().ToString()) + " " + (i + 1),i , position, sunSizeColor);
            game.data.stars.Add(star);
        }

        //for (int i = 0; i < game.data.stars.Count; i++) //Checking the distance to each already generated star and then adding it to a list of near stars if close enough
        //{

        //    for (int c = 0; c < game.data.stars.Count; c++) //Checking the distance to each already generated star and then adding it to a list of near stars if close enough
        //    {
        //        if (c != i)
        //        {
        //            double maxDist = 200;
        //            double actualDist = Vector3d.Distance(game.data.stars[i].galacticPosition, game.data.stars[c].galacticPosition);
        //            if (actualDist < maxDist)
        //            {
        //                game.data.stars[i].nearStars.Add(game.data.stars[c]);
        //                game.data.stars[c].nearStars.Add(game.data.stars[i]);
        //            }
        //        }

        //    }
        //}

       

        for (int i = 0; i < game.data.stars.Count; i++) //Checking the distance to each already generated star and then adding it to a list of near stars if close enough
        {
            SolarModel closestStar1 = new SolarModel();
            double distance1 = 0;
            SolarModel closestStar2 = new SolarModel();
            for (int c = 0; c < game.data.stars.Count; c++) //Checking the distance to each already generated star and then adding it to a list of near stars if close enough
            {
                if (c != i)
                {
                    double actualDist = Vector3d.Distance(game.data.stars[i].galaxyPosition, game.data.stars[c].galaxyPosition);
                    if (distance1 == 0)
                    {
                        closestStar1 = game.data.stars[c];
                        distance1 = actualDist;
                    }
                    else if (actualDist < distance1)
                    {
                        closestStar2 = closestStar1;
                        closestStar1 = game.data.stars[c];
                        distance1 = actualDist;
                    }
                }
            }
            game.data.stars[i].nearStars.Add(closestStar1);
            closestStar1.nearStars.Add(game.data.stars[i]);
            game.data.stars[i].nearStars.Add(closestStar2);
            closestStar2.nearStars.Add(game.data.stars[i]);
        }
        float connectedness = 0;
        for (int i = 0; i < count; i++)
        {
            float closestStarDist = 1000000000000000;
            int closestStarIndex = 0;
            for (int c = 0; c < count; c++)
            {
                if (c != i)
                {
                    float actualDist = (float) Vector3d.Distance(game.data.stars[i].galaxyPosition, game.data.stars[c].galaxyPosition);
                    if (actualDist < closestStarDist)
                    {
                        closestStarDist = actualDist;
                        closestStarIndex = c;
                    }
                }

            }
            if (game.data.stars[i].nearStars.Count == 0)
            {
                game.data.stars[i].nearStars.Add(game.data.stars[closestStarIndex]);
                game.data.stars[closestStarIndex].nearStars.Add(game.data.stars[i]);
                //print("Connected disconnected star");
            }
            connectedness += game.data.stars[i].nearStars.Count;
        }
        connectedness /= count;
        print("Connectedness average: " + connectedness);
    }
    /// <summary>
    /// Loads all stars references in the main data file
    /// </summary>
    public void LoadStars()
    {
        var stars = new GameObject("Stars");
        foreach (SolarModel star in game.data.stars)
        {
            Controller.Instantiate<SolarController>("solar", star,stars.transform);
        }
    }
    /// <summary>
    /// Finds a space in the galaxy for a new government,and then sets the stars as that government's space
    /// </summary>
    /// <param name="model">The government model to set</param>
    /// <returns></returns>
    private SolarBody FindGovernmentStar(GovernmentModel model)
    {
        for (int index = 0; index < GameManager.instance.data.stars.Count; index++)
        {
            if (game.data.stars[index].government.Model == null && game.data.stars[index].solar.satelites.Count > 0)
            {
                bool freeSpace = true;
                foreach (SolarModel star in game.data.stars[index].nearStars)
                {
                    if (star.government.Model != null)
                    {
                        freeSpace = false;
                    }
                }
                if (freeSpace) //Found spot
                {
                    foreach (SolarBody body in game.data.stars[index].solar.satelites)
                    {
                        if (body.solarSubType == SolarSubType.EarthLike)
                        {
                            game.data.stars[index].government.Model = model;
                            model.stars.Add(game.data.stars[index]);
                            game.data.stars[index].isCapital = true;
                            foreach (SolarModel star in game.data.stars[index].nearStars)
                            {
                                star.government.Model = model;
                                model.stars.Add(star);
                            }
                            body.AddPopulation(UnityEngine.Random.Range(1000000, 1000000000));
                            body.companies.Add(model);
                            return body;
                        }

                    }
                }
            }
        }

        return null;
    }

    public static double NextGaussianDouble(double seed)
    {
        double U, u, v, S;
        var random = new System.Random(seed.GetHashCode());
        do
        {
            u = 2.0 * random.NextDouble() - 1.0;
            v = 2.0 * random.NextDouble() - 1.0;
            S = u * u + v * v;
        }
        while (S >= 1.0);

        double fac = Mathd.Sqrt(-2.0 * Mathd.Log(S) / S);
        return u * fac;
    }

    public static double NormalizedRandom(double minValue, double maxValue, double seed)
    {
        var mean = (minValue + maxValue) / 2;
        var sigma = (maxValue - mean) / 3;
        return nextRandom(mean, sigma, seed);
    }

    private static double nextRandom(double mean, double sigma, double seed)
    {
        var standard = NextGaussianDouble(seed) + mean;

        var value = standard * sigma;

        if (value < mean - 3 * sigma)
        {
            return mean - 3 * sigma;
        }
        if (value > mean + 3 * sigma)
        {
            return mean + 3 * sigma;
        }
        return value;
    }
}

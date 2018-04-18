using CodeControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateGameManager : MonoBehaviour {

    public GameObject loadingPanel;
    public Text loadingText;
    public Slider loadingProgress;
    //public Text galaxyNameText;

    //Galaxy Parameters
    public int starCount;
    public Vector3 mapField;
    public Gradient sunSizeColor;
    public Gradient planetSizeColor;

    //Game Parameters
    public string galaxyName;
    public int numGov;
    public int numComp;
    public int numStation;
    public int numShip;
    public bool randomSeed;

    internal GameManager game;
    internal ViewManager galaxy;

    internal static float G = .01f;
    internal NameGen names;

    public void Awake()
    {
        transform.localPosition = Vector3.zero;
    }

    public void CreateGame()
    {
        galaxy = ViewManager.instance;
        game = GameManager.instance;
        names = new NameGen();
        GameManager.instance.setup = true;
        loadingPanel.SetActive(true);
        StartCoroutine("CreatingGame"); //Creates a new Game
    }

    IEnumerator CreatingGame()
    {
        loadingText.text = "Creating Game...";
        loadingProgress.value = 0;
        while (GameManager.instance.setup)
        {
            //CreateStarSystem
            loadingText.text = "Creating Stars...";
            game.data.galaxyName = galaxyName;
            //galaxyNameText.text = galaxyName;

            foreach (RawResourceBlueprint raw in game.data.rawResources.Model.rawResources)
            {
                var item = new ItemBlueprint(raw);
                GameManager.instance.data.itemsData.Model.blueprints.Add(item);
            }
            game.data.itemsData.Model.blueprints.Add(
                new FuelBlueprint("Fuel Cell",1, "A normal fuel cell.", 60, new List<Item>() {
                    new Item(defaultRawResources.Fuelodite.ToString(), 10) }));
            game.data.itemsData.Model.blueprints.Add(
                new ItemBlueprint("Basic AI", ItemType.AI, "A basic AI unit.", Dated.Hour, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), 1),
                    new Item(defaultRawResources.Coppode.ToString(), 5)
                }));
            game.data.itemsData.Model.blueprints.Add(
                new ItemBlueprint("Basic Machinery", ItemType.FactoryMachinery, "Factory Machinery.", Dated.Hour, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), 10),
                    new Item(ItemType.AI, 1)
                }));
            game.data.itemsData.Model.blueprints.Add(
                new ItemBlueprint("Basic Factory", ItemType.Factory, "A usable factory.",2 * Dated.Day, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), 250),
                    new Item(ItemType.FactoryMachinery, 1)
                })
                { workers = 300 });
            game.data.itemsData.Model.blueprints.Add(
               new ItemBlueprint("Basic Driller", ItemType.Driller, "A usable driller.", Dated.Day, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), 200),
                    new Item(ItemType.FactoryMachinery, 1)
               })
               { workers = 200 });
            game.data.itemsData.Model.blueprints.Add(
               new ItemBlueprint("DistributionCenter", ItemType.DistributionCenter, "Distributes things.", .5f * Dated.Day, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), 100),
               }));
            game.data.itemsData.Model.blueprints.Add(
                new ItemBlueprint("Basic Engine", ItemType.Engine, "A usable engine.", .5f * Dated.Day, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), 20),
                    new Item(defaultRawResources.Warium.ToString(), 5)
                }));
            game.data.itemsData.Model.blueprints.Add(
                new ItemBlueprint("Ship fueltank", ItemType.FuelTank, "Ship fueltank.", 3 * Dated.Hour, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), 15),
                    new Item(defaultRawResources.Limoite.ToString(), 1)
                }));
            game.data.itemsData.Model.blueprints.Add(
                new ItemBlueprint("Ship sensor", ItemType.Sensor, "Basic ship sensor.", 5 * Dated.Hour, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), 5),
                    new Item(defaultRawResources.Goldium.ToString(),1),
                    new Item(ItemType.AI, 1)
                }));
            game.data.itemsData.Model.blueprints.Add(
                new ShipBlueprint("Cargo Ship", ShipType.Cargo, "Basic cargo ship", 5 * Dated.Day, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), 200),
                    new Item(defaultRawResources.Glassitum.ToString(), 20),
                    new Item(ItemType.AI, 1),
                    new Item(ItemType.Engine, 2),
                    new Item(ItemType.FuelTank, 1),
                    new Item(ItemType.Sensor, 1)
                }));
            game.data.itemsData.Model.blueprints.Add(
                new ShipBlueprint("Combat Ship", ShipType.Combat , "Basic combat ship", 8 * Dated.Day, new List<Item>() {
                    new Item(defaultRawResources.Armoroid.ToString(), 400),
                    new Item(defaultRawResources.Glassitum.ToString(), 20),
                    new Item(ItemType.AI, 2),
                    new Item(ItemType.Engine, 4),
                    new Item(ItemType.FuelTank, 2),
                    new Item(ItemType.Sensor, 1)
                }));
            if (randomSeed)
            {
                CreateStars(starCount, System.DateTime.Now.ToString());
            }
            else CreateStars(starCount, game.data.galaxyName);
            loadingProgress.value = .5f;

            GameManager.instance.setup = false;
            yield return null;
            for (int a = 0; a < numGov; a++)
            {
                //Create Government
                loadingText.text = "Creating Governments...";

                
                GovernmentModel gov = new GovernmentModel(names.GenerateWorldName() + " Government");

                //Location
                SolarBody parent = FindGovernmentStar(gov);

                //Add leader

                Creature leader = new Creature(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 100000, parent.id,game.data.date, new Dated(30 * Dated.Year), CreatureType.Human);
                gov.leaders.Add(leader.id);


                //Add Government Capital
                Station station = new Station(gov.name + " Station", gov,parent.id);

                DistributionCenter center = new DistributionCenter(gov, parent);

                //location
                gov.SetLocation(parent.id);

                //Add Drills and Factories

                foreach(RawResource raw in parent.rawResources)
                {
                    Driller driller = new Driller(gov, raw.id, parent.id, 1);
                    //DistributionCenter storage = new DistributionCenter(gov, parent);
                }

                var factoryBlueprint = game.data.itemsData.Model.blueprints.Find(x => x.itemType == ItemType.Factory);

                foreach (ItemBlueprint item in game.data.itemsData.Model.blueprints)
                {
                    if (item.itemType != ItemType.RawMaterial)
                    {
                        var factory = new Factory(gov, factoryBlueprint.id, item.id, parent.id);
                    }
                    else if (!parent.rawResources.Exists( x => x.id == item.id))
                    {
                        //Finds missing materials
                        foreach (SolarBody body in parent.Star.GetAllSolarBodies())
                        {
                            if (body.rawResources.Exists(x => x.id == item.id) && !gov.GetStructures().Exists(x =>
                          {
                              Driller structure = x as Driller;
                              if (structure != null)
                              {
                                  return (structure.productionItemId == item.id);
                              }
                              return false;

                          }))
                            {

                                Driller driller = new Driller(gov, item.id, body.id, 1);
                                break;
                            }
                        }
                    }
                }

                //for (int i = 0; i < numShip * 5; i++)
                //{
                //    Creature captain = new Creature(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 10000, station, game.data.date, new Dated(30 * Dated.Year), CreatureType.Human);

                //    if (gov.structureIds.Count > 0)
                //    {
                //        Ship ship = new Ship(gov.name + " Ship " + (i + 1), gov, captain, gov.structureIds[0]);
                //    }

                //    //loadingText.text = string.Format("Creating ships: {0} of {1}", i, numShip);

                //}



                var star = parent.Star;

                for (int c = 0; c < numComp; c++)
                {
                    var random = new System.Random((gov.Id + c + gov.name).GetHashCode());
                    var body = star.satelites[random.Next(star.satelites.Count)];

                    Creature captain = new Creature(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 100000, body.id, game.data.date, new Dated(30 * Dated.Year), CreatureType.Human);
                    CompanyModel comp = new CompanyModel(names.GenerateRegionName() + " Company", body, gov, captain);
                    //if (c == 0)
                    //{
                    //    owner.isPlayer = true;
                    //    GameManager.instance.data.playerCreatureId = owner.id;
                    //}

                    var itemBlueprint = game.data.itemsData.Model.blueprints[random.Next(game.data.itemsData.Model.blueprints.Count)];
                    if (itemBlueprint.itemType != ItemType.RawMaterial)
                    {
                        var factory = new Factory(comp, factoryBlueprint.id, itemBlueprint.id, body.id);
                    }
                    else if (body.rawResources.Count > 0)
                    {
                        Driller driller = new Driller(comp, body.rawResources[random.Next(body.rawResources.Count)].id, body.id);
                        //GroundStorage storage = new GroundStorage(comp, body);
                    }

                    //--------------Create Ships---------------------------//
                    //for (int i = 0; i < numShip; i++)
                    //{
                    //    if (i != 0)
                    //    {
                    //        captain = new Creature(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 10000, station, game.data.date, new Dated(30 * Dated.Year), CreatureType.Human);
                    //    }

                    //    if (comp.structureIds.Count > 0)
                    //    {
                    //        Ship ship = new Ship(comp.name + " Ship " + (i + 1), comp, captain, comp.structureIds[0]);
                    //    }
                        
                    //    //loadingText.text = string.Format("Creating ships: {0} of {1}", i, numShip);

                    //}
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
            // Create stars with distance in lightyears

            // position = new Vector3((float) random.NextDouble() * mapField.x * 2 - mapField.x * .5f, (float)random.NextDouble() * mapField.y * 2 - mapField.y * .5f, (float)random.NextDouble() * mapField.z * 2 - mapField.z * .5f);
            Vector3 position = new Vector3((float)NormalizedRandom(-mapField.x * .5f, mapField.x * .5f, random.NextDouble()), (float)NormalizedRandom(-mapField.y * .5f, mapField.y * .5f, random.NextDouble()), (float)NormalizedRandom(-mapField.z * .5f, mapField.z * .5f, random.NextDouble()));
            SolarModel star = new SolarModel(names.GenerateWorldName(random.Next().ToString()) + " " + (i + 1), position);
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
                    double actualDist = Vector3d.Distance(game.data.stars[i].solar.referencePosition, game.data.stars[c].solar.referencePosition);
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
                    float actualDist = (float) Vector3d.Distance(game.data.stars[i].solar.referencePosition, game.data.stars[c].solar.referencePosition);
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
                            model.referenceId = body.id;
                            foreach (SolarModel star in game.data.stars[index].nearStars)
                            {
                                star.government.Model = model;
                                model.stars.Add(star);
                            }
                            body.Population(1);
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

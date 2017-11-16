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
            CreateStars(starCount, game.data.galaxyName);
            LoadStars();
            loadingProgress.value = .5f;

            GameManager.instance.setup = false;
            yield return null;
            for (int a = 0; a < numGov; a++)
            {
                //Create Government
                loadingText.text = "Creating Governments...";
                ModelRefs<CreatureModel> leaders = new ModelRefs<CreatureModel>();
                CreatureModel leader = new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 100000);
                leaders.Add(leader);
                GovernmentModel gov = new GovernmentModel(names.GenerateWorldName() + " Government", leaders);

                //Location
                int solarIndex = FindGovernmentStar(gov);
                int planetIndex = 0;
                SolarBody parent;
                foreach (SolarBody body in game.data.stars[solarIndex].solar.satelites)
                {
                    if (body.solarSubType != SolarSubType.GasGiant)
                    {
                        parent = game.data.stars[solarIndex].solar.satelites[planetIndex];
                        parent.population = UnityEngine.Random.Range(1000000, 1000000000);
                        break; //TODO: Fix the possible bug where there is only a gas giant in the system and no suitable alternatives
                    }
                    
                }

                

                //    //Add Government Capital

                //    StationModel station = StationCreator.CreateStation(gov.name + " Capital", game.data.stars[solarIndex], parent.index, parent.orbit, gov, leader);
                //    station.population = 5;

                //    //location
                //    gov.location.Model = station;
                //    gov.orbit = station.orbit;
                //    gov.stations.Add(station);
                //    for (int c = 0; c < numComp; c++)
                //    {
                //        CreatureModel owner = new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 1000000);
                //        CompanyModel comp = new CompanyModel(names.GenerateRegionName() + " Company", gov, owner);
                //        gov.companyAcess.Add(comp);
                //        comp.governmentAccess.Add(gov);
                //        game.data.creatures.Add(owner);
                //        game.data.companies.Add(comp);
                //        //Add Company Headquarter Station
                //        solarIndex = UnityEngine.Random.Range(0, gov.stars.Count);
                //        planetIndex = UnityEngine.Random.Range(0, gov.stars[solarIndex].planets.Length);
                //        if (gov.stars[solarIndex].planets.Length > 0)
                //            parent = gov.stars[solarIndex].planets[planetIndex];
                //        else
                //            parent = gov.stars[solarIndex].sun;

                //        position = new Polar2d(UnityEngine.Random.Range( (float) parent.bodyRadius * 1.2f, (float) parent.orbit.soi), UnityEngine.Random.Range(0, 2 * Mathf.PI));
                //        CreatureModel manager = new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 100000);
                //        station = StationCreator.CreateStation(names.GenerateRegionName() + " Station", gov.stars[solarIndex], parent.index, parent.orbit, comp, manager);
                //        comp.stations.Add(station);
                //        game.data.creatures.Add(manager);
                //        game.data.stations.Add(station);

                //        //--------------Create Ships---------------------------//
                //        for (int i = 0; i < numShip; i++)
                //        {
                //            StationModel startStation = station;
                //            manager = new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 10000);
                //            ShipModel ship = ShipCreator.CreateShip(comp.name + " Ship " + i, startStation.solarIndex, startStation.parentIndex, startStation.orbit, comp, manager);
                //            game.data.ships.Add(ship);
                //            game.data.creatures.Add(manager);
                //            //loadingText.text = string.Format("Creating ships: {0} of {1}", i, numShip);

                //        }
                //        yield return null;
                //    }
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
                    double actualDist = Vector3d.Distance(game.data.stars[i].galacticPosition, game.data.stars[c].galacticPosition);
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
                    float actualDist = (float) Vector3d.Distance(game.data.stars[i].galacticPosition, game.data.stars[c].galacticPosition);
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
    private int FindGovernmentStar(GovernmentModel model)
    {
        int outTime = 0;
        while (true)
        {
            int index = UnityEngine.Random.Range(0, game.data.stars.Count);

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

                    game.data.stars[index].government.Model = model;
                    model.stars.Add(game.data.stars[index]);
                    game.data.stars[index].isCapital = true;
                    foreach (SolarModel star in game.data.stars[index].nearStars)
                    {
                        star.government.Model = model;
                        model.stars.Add(star);
                    }
                    return index;
                }
            }
            outTime++;
            if (outTime > 1000)
            {
                throw new System.Exception("Could not find solar sytem to put government");
            }
        }
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

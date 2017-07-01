using CodeControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateGameManager : MonoBehaviour {

    public GameObject loadingPanel;
    public Text loadingText;
    public Slider loadingProgress;

    //Galaxy Parameters
    public int starCount;
    public Vector2 mapField;
    public Gradient sunSizeColor;
    public Gradient planetSizeColor;

    //Game Parameters
    public int numGov;
    public int numComp;
    public int numStation;
    public int numShip;

    internal GameManager game;
    internal GalaxyManager galaxy;

    internal static float G = .01f;

    public void Awake()
    {
        transform.localPosition = Vector3.zero;
    }

    public void CreateGame()
    {
        galaxy = GalaxyManager.instance;
        game = GameManager.instance;

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
        NameGen names = new NameGen();
        while (GameManager.instance.setup)
        {
            //CreateStarSystem
            loadingText.text = "Creating Stars...";
            CreateStars(starCount);
            LoadStars();
            loadingProgress.value = .5f;

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
                int planetIndex = Random.Range(0, game.data.stars[solarIndex].planets.Length);
                SolarBody parent;
                if (game.data.stars[solarIndex].planets.Length > 0)
                    parent = game.data.stars[solarIndex].planets[planetIndex];
                else
                    parent = game.data.stars[solarIndex].sun;
                Polar2d position = new Polar2d(UnityEngine.Random.Range((float)parent.orbit.bodyRadius + 2, (float)parent.orbit.soi), UnityEngine.Random.Range(0, 2 * Mathf.PI));
                if (parent.planetType == PlanetType.Regular)
                {
                    position = new Polar2d(0, 0);
                    parent.population = UnityEngine.Random.Range(1000, 100000);
                }

                //Add Government Capital

                StationModel station = StationCreator.CreateStation(gov.name + " Capital", game.data.stars[solarIndex], parent.index, parent.orbit, gov, leader);
                station.population = 5;

                //location
                gov.location.Model = station;
                gov.orbit = station.orbit;
                gov.stations.Add(station);
                for (int c = 0; c < numComp; c++)
                {
                    CreatureModel owner = new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 1000000);
                    CompanyModel comp = new CompanyModel(names.GenerateRegionName() + " Company", gov, owner);
                    gov.companyAcess.Add(comp);
                    comp.governmentAccess.Add(gov);
                    game.data.creatures.Add(owner);
                    game.data.companies.Add(comp);
                    //Add Company Headquarter Station
                    solarIndex = UnityEngine.Random.Range(0, gov.stars.Count);
                    planetIndex = UnityEngine.Random.Range(0, gov.stars[solarIndex].planets.Length);
                    if (gov.stars[solarIndex].planets.Length > 0)
                        parent = gov.stars[solarIndex].planets[planetIndex];
                    else
                        parent = gov.stars[solarIndex].sun;

                    position = new Polar2d(UnityEngine.Random.Range( (float) parent.orbit.bodyRadius * 1.2f, (float) parent.orbit.soi), UnityEngine.Random.Range(0, 2 * Mathf.PI));
                    CreatureModel manager = new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 100000);
                    station = StationCreator.CreateStation(names.GenerateRegionName() + " Station", gov.stars[solarIndex], parent.index, parent.orbit, comp, manager);
                    comp.stations.Add(station);
                    game.data.creatures.Add(manager);
                    game.data.stations.Add(station);

                    //--------------Create Ships---------------------------//
                    for (int i = 0; i < numShip; i++)
                    {
                        StationModel startStation = station;
                        manager = new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateWorldName(), 10000);
                        ShipModel ship = ShipCreator.CreateShip(comp.name + " Ship " + i, startStation.solarIndex, startStation.parentIndex, startStation.orbit, comp, manager);
                        game.data.ships.Add(ship);
                        game.data.creatures.Add(manager);
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

    public void CreateStars(int count)
    {

        for (int i = 0; i < count; i++)
        {
            Vector2 position = new Vector2(UnityEngine.Random.Range(-mapField.x * .5f, mapField.x * .5f), UnityEngine.Random.Range(-mapField.y * .5f, mapField.y * .5f));
            float sunMass = Random.Range(1f, 1000);
            int numPlanets = Random.Range(0, (int)Mathf.Sqrt(sunMass * .1f));
            SolarModel star = new SolarModel("Solar " + i + 1, i, position, sunMass, G, numPlanets, sunSizeColor, sunMass * .001f);
            game.data.stars.Add(star);
            for (int c = 0; c < i; c++) //Checking the distance to each already generated star and then adding it to a list of near stars if close enough
            {
                double maxDist = Mathd.Pow(sunMass + game.data.stars[c].sun.orbit.Mass, .5f);
                float actualDist = Vector3.Distance(position, game.data.stars[c].galacticPosition);
                if (actualDist < maxDist)
                {
                    star.nearStars.Add(game.data.stars[c]);
                    game.data.stars[c].nearStars.Add(star);
                }
            }

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
                    float actualDist = Vector3.Distance(game.data.stars[i].galacticPosition, game.data.stars[c].galacticPosition);
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
                print("Connected disconnected star");
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
        foreach (SolarModel star in game.data.stars)
        {
            Controller.Instantiate<SolarController>("solar", star);
        }
    }
    /// <summary>
    /// Finds a space in the galazy for a new government,and then sets the stars as that government's space
    /// </summary>
    /// <param name="model">The government model to set</param>
    /// <returns></returns>
    private int FindGovernmentStar(GovernmentModel model)
    {
        int outTime = 0;
        while (true)
        {
            int index = UnityEngine.Random.Range(0, game.data.stars.Count);

            if (game.data.stars[index].government.Model == null)
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
                print("Find Star timed out");
                return -1;
            }
        }
    }
}

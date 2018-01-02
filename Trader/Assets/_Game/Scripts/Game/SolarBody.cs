using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarBody: IPositionEntity
{
    public string name;
    public int totalPopulation { get; private set; }
    public SolarType solarType { get; private set; }
    public SolarSubType solarSubType { get; private set; }
    public Orbit orbit;
    public double mass { get; set; }
    public double bodyRadius { get; set; }
    public Vector2d lastKnownPosition { get; private set; }
    public Vector2d[] approximatePositions {get; private set;}

    //-----------------Star Properties---------------------//
    public double surfaceTemp { get; private set; }
    public double surfacePressure { get; private set; }
    public double luminosity { get; private set; } // in ergs per second

    //----------------Rocky Satellite Properties-----------//
    public double bondAlebo { get; private set; } 
    public double greenhouse { get; private set; }
    public double surfaceGravity { get; private set; }
    public List<PlanetTile> planetTiles { get; private set; }
    public List<RawResource> rawResources { get; private set; }
    public List<IStructure> structures = new List<IStructure>();
    public bool deleteStructure;
    //----------------Commerce-----------------------------//
    public ItemStorage buyList { get; private set; }
    public ItemStorage sellList { get; private set; }

    public ModelRefs<IdentityModel> companies = new ModelRefs<IdentityModel>();


    public int population { get; private set; }
    //public Dictionary<RawResources, double> rawResources;
    public Color color;

    public List<SolarBody> satelites = new List<SolarBody>();
    /// <summary>
    /// Index of solar body in solarsystem.
    /// </summary>
    public List<int> solarIndex { get; set; }
    public int structureId { get; set; }
    public int shipId { get; set; }

    public Vector2d galaxyPosition
    {
        get
        {
            if (solarIndex.Count == 3)
            {
                return GameManager.instance.data.stars[solarIndex[0]].solar.satelites[solarIndex[1]].galaxyPosition
                        + lastKnownPosition;
            }
            else if (solarIndex.Count == 2)
            {
                return GameManager.instance.data.stars[solarIndex[0]].galaxyPosition
                        + lastKnownPosition;
            }
            else
            {
                throw new System.Exception("Solarbody " + name + " solarIndex count incorrect: " + solarIndex.Count);
            }
        }

        set
        {
            throw new System.Exception("Can't set solarbody galaxyPosition, use GamePosition instead");
        }
    }

    /// <summary>
    /// Used for creating stars
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_solarIndex"></param>
    /// <param name="_solarSubType"></param>
    /// <param name="mass"></param>
    /// <param name="radius"></param>
    /// <param name="orbit"></param>
    /// <param name="_color"></param>
    /// <param name="_surfaceTemp"></param>
    public SolarBody(string _name, List<int> _solarIndex, SolarSubType _solarSubType, double mass, double radius, Orbit orbit, Color _color, double _surfaceTemp)
    {
        rawResources = new List<RawResource>();
        planetTiles = new List<PlanetTile>();
        totalPopulation = 0;
        population = 0;
        this.orbit = orbit;
        this.mass = mass;
        this.bodyRadius = radius;
        name = _name;
        this.solarType = solarType;
        solarIndex = _solarIndex;
        structureId = -1;
        shipId = -1;
        color = _color;
        surfaceTemp = _surfaceTemp;
        surfacePressure = 1; // In atm
        luminosity = 3.846e33 / Mathd.Pow(mass / 2e30, 3); // in ergs per sec
        surfaceGravity = GameDataModel.G * mass / Mathd.Pow(Units.convertToMeters * radius, 2) / 9.81;
        solarType = SolarType.Star;
        solarSubType = _solarSubType;
        bondAlebo = .5;
        surfacePressure = 1;
        greenhouse = .0137328 * Mathd.Pow(surfacePressure, 2) + .0986267 * surfacePressure;

        buyList = new ItemStorage();
        sellList = new ItemStorage();
    }

    public SolarBody(string _name, List<int> _solarIndex, SolarType _solarType, SolarSubType _solarSubType, double mass, double radius, Orbit orbit, Color _color, SolarBody star)
    {
        this.orbit = orbit;
        this.mass = mass;
        this.bodyRadius = radius;
        name = _name;
        totalPopulation = 0;
        population = 0;
        this.solarType = _solarType;
        this.solarSubType = _solarSubType;
        solarIndex = _solarIndex;
        structureId = -1;
        shipId = -1;
        color = _color;
        surfaceGravity = GameDataModel.G * mass / Mathd.Pow(Units.convertToMeters * radius, 2) / 9.81;
        surfacePressure = 0; // In atm
        bondAlebo = Random.value;

        buyList = new ItemStorage();
        sellList = new ItemStorage();

        var dist = orbit.sma;
        if (solarIndex.Count == 3)
        {
            dist = star.satelites[solarIndex[1]].orbit.sma;
        }

        if (solarType == SolarType.Moon || solarType == SolarType.DwarfPlanet)
        {
            surfacePressure = Random.Range(.01f, 1);
            greenhouse = .0137328 * Mathd.Pow(surfacePressure, 2) + .0986267 * surfacePressure;
            if (greenhouse < 0)
            {
                greenhouse = 0;
            }
            if (solarType == SolarType.Moon)
            {
                surfaceTemp = Mathd.Pow(((1 - bondAlebo) * star.luminosity) / ((16 * Mathd.PI * 5.6705e-8) * Mathd.Pow(star.satelites[solarIndex[1]].orbit.sma * Units.convertToMeters + orbit.sma * Units.convertToMeters, 2)), .25) * Mathd.Pow((1 + .438 * greenhouse * .9), .25);
            }
            else
            {
                surfaceTemp = Mathd.Pow(((1 - bondAlebo) * star.luminosity) / ((16 * Mathd.PI * 5.6705e-8) * Mathd.Pow(orbit.sma * Units.convertToMeters, 2)), .25) * Mathd.Pow((1 + .438 * greenhouse * .9), .25);
            }

            var rand = Random.value;

            if (surfaceTemp - 273.15 < -50)
            {
                if (rand < .15)
                {
                    solarSubType = SolarSubType.Ice;
                }
                else if (rand < .2)
                {
                    solarSubType = SolarSubType.Volcanic;
                }
            }
            else if (surfaceTemp - 273.15 > 120)
            {
                if (surfacePressure > .1)
                {
                    if (rand < .75)
                    {
                        solarSubType = SolarSubType.Volcanic;
                    }
                }
                else
                {
                    if (rand < .25)
                    {
                        solarSubType = SolarSubType.Desert;
                    }
                }

            }
            else
            {
                if (surfacePressure > .5)
                {
                    if (rand < .55)
                    {
                        solarSubType = SolarSubType.Ocean;
                    }
                    else if (rand < .85)
                    {
                        solarSubType = SolarSubType.EarthLike;
                    }
                    else if (rand < .95)
                    {
                        solarSubType = SolarSubType.Volcanic;
                    }
                }
                else if (surfacePressure > .25)
                {
                    if (rand < .75)
                    {
                        solarSubType = SolarSubType.EarthLike;
                    }
                    else if (rand < .85)
                    {
                        solarSubType = SolarSubType.Ocean;
                    }
                }
            }
        }
        else if (solarSubType == SolarSubType.GasGiant)
        {
            surfacePressure = Random.Range(10, 100);
            greenhouse = .0137328 * Mathd.Pow(surfacePressure, 2) + .0986267 * surfacePressure;
            if (greenhouse < 0)
            {
                greenhouse = 0;
            }
            surfaceTemp = Mathd.Pow(((1 - bondAlebo) * star.luminosity) / ((16 * Mathd.PI * 5.6705e-8) * Mathd.Pow(orbit.sma * Units.convertToMeters, 2)), .25) * Mathd.Pow((1 + .438 * greenhouse * .9), .25);
        }
        else if (solarType == SolarType.Planet && solarSubType == SolarSubType.Rocky)
        {
            var rand = Random.value;
            if (rand < .75)
            {
                surfacePressure = Random.Range(.01f, 2);
            }
            else
            {
                surfacePressure = Random.Range(.01f, 100);
            }

            greenhouse = .0137328 * Mathd.Pow(surfacePressure, 2) + .0986267 * surfacePressure;
            if (greenhouse < 0)
            {
                greenhouse = 0;
            }
            surfaceTemp = Mathd.Pow(((1 - bondAlebo) * star.luminosity) / ((16 * Mathd.PI * 5.6705e-8) * Mathd.Pow(orbit.sma * Units.convertToMeters, 2)), .25) * Mathd.Pow((1 + .438 * greenhouse * .9), .25);

            rand = Random.value;
            if (surfaceTemp - 273.15 < -50)
            {
                if (rand < .25)
                {
                    solarSubType = SolarSubType.Ice;
                }
            }
            else if (surfaceTemp - 273.15 > 120)
            {
                if (surfacePressure > 20)
                {
                    if (rand < .75)
                    {
                        solarSubType = SolarSubType.Volcanic;
                    }
                }
                else
                {
                    if (rand < .25)
                    {
                        solarSubType = SolarSubType.Desert;
                    }
                }
                
            }
            else
            {
                if (surfacePressure > 40)
                {
                    if (rand < .55)
                    {
                        solarSubType = SolarSubType.Volcanic;
                    }
                    else if (rand < .85)
                    {
                        solarSubType = SolarSubType.Ocean;
                    }
                }
                else if (surfacePressure > 20)
                {
                    if (rand < .55)
                    {
                        solarSubType = SolarSubType.Ocean;
                    }
                    else if (rand < .85)
                    {
                        solarSubType = SolarSubType.EarthLike;
                    }
                    else if (rand < .95)
                    {
                        solarSubType = SolarSubType.Volcanic;
                    }
                }
                else if (surfacePressure > .25)
                {
                    if (rand < .75)
                    {
                        solarSubType = SolarSubType.EarthLike;
                    }
                    else if (rand < .85)
                    {
                        solarSubType = SolarSubType.Ocean;
                    }
                }
            }
        }
        
        //------------Assigning Colors----------------//
        if (solarSubType == SolarSubType.Rocky)
        {
            color = Color.gray;
        }
        else if (solarSubType == SolarSubType.EarthLike)
        {
            color = Color.green;
        }
        else if (solarSubType == SolarSubType.Ice)
        {
            color = Color.white;
        }
        else if (solarSubType == SolarSubType.Desert)
        {
            color = Color.yellow;
        }
        else if (solarSubType == SolarSubType.Ocean)
        {
            color = Color.blue;
        }
        else if (solarSubType == SolarSubType.Volcanic)
        {
            color = Color.red;
        }
        else if (solarSubType == SolarSubType.GasGiant)
        {
            color = new Color(1, .5f, 0);
        }

        rawResources = new List<RawResource>();
        planetTiles = new List<PlanetTile>();

        if (solarType != SolarType.Star && solarSubType != SolarSubType.GasGiant)
        {
            GeneratePlanetTiles();
        }
    }

    private void GeneratePlanetTiles()
    {
        int numTiles = (int)(2 * Mathd.PI * bodyRadius * Units.convertToMeters / 4e7 * 15) + 1;
        System.Random rand = new System.Random(GameManager.instance.data.id);
        for(int i = 0; i < numTiles; i++)
        {
            PlanetTile tile = new PlanetTile(solarSubType);
            var index = planetTiles.FindIndex(x =>x.planetTileType == tile.planetTileType);
            if (index >= 0)
            {
                planetTiles[index].count += 1;
            }
            else
                planetTiles.Add(tile);

            foreach (RawResourceBlueprint raw in GameManager.instance.data.rawResources.Model.rawResources)
            {
                RawResourceInfo info = raw.GetInfo(tile.planetTileType);

                if (rand.NextDouble() < info.probability)
                {
                    RawResource resource = new RawResource(raw.name, raw.id, rand.Next(info.maxAmount), Random.Range(info.accessibility.x, info.accessibility.y));

                    index = rawResources.FindIndex(x => x.id == resource.id);

                    if (index >= 0)
                    {
                        rawResources[index].AddAmount(resource.amount);
                    }
                    else
                    {
                        rawResources.Add(resource);
                    }
                }
                
            }
           
            

        }
        
    }

    public SolarBody() { }

    public IStructure GetStructure(int structureId)
    {
        int index = structures.FindIndex(x => x.id == structureId);
        if (index == -1)
            index = structures.FindIndex(x => x.id == structureId);
        else
            return structures[index];
        if (index != -1)
            return structures[index];
        else
            return null;
    }
    #region "Commerce"

    public double GetMarketPrice(int itemId)
    {
        return GameManager.instance.data.stars[solarIndex[0]].GetMarketPrice(itemId);
    }

    public void SetBuying(Item item)
    {

        int itemIndex = -1;
        if (item.structureId == -1)
        {
            itemIndex = buyList.items.FindIndex(x => x.id == item.id);
        }
        else
        {
            itemIndex = buyList.items.FindIndex(x => x.id == item.id && x.structureId == item.structureId);
        }
        if (itemIndex >= 0)
        {
            buyList.items[itemIndex].SetAmount(item.amount, item.price);
        }
        else
        {
            buyList.AddItem(item);
        }

        GameManager.instance.data.stars[solarIndex[0]].SetBuying(item);
    }
    public void RemoveBuying(int itemId, int structureId, double amount)
    {

        int itemIndex = -1;
        if (structureId == -1)
        {
            itemIndex = buyList.items.FindIndex(x => x.id == itemId);
        }
        else
        {
            itemIndex = buyList.items.FindIndex(x => x.id == itemId && x.structureId == structureId);
        }

        if (itemIndex >= 0)
        {
            buyList.items[itemIndex].RemoveAmount(amount);
            if (buyList.items[itemIndex].amount == 0)
                buyList.items.RemoveAt(itemIndex);
        }

        GameManager.instance.data.stars[solarIndex[0]].RemoveBuying(itemId,structureId, amount);
    }

    public void SetSelling(Item item)
    {
        int itemIndex = -1;
        if (item.structureId == -1)
        {
            itemIndex = sellList.items.FindIndex(x => x.id == item.id);
        }
        else
        {
            itemIndex = sellList.items.FindIndex(x => x.id == item.id && x.structureId == item.structureId);
        }
        if (itemIndex >= 0)
        {
            sellList.items[itemIndex].SetAmount(item.amount, item.price);
        }
        else
        {
            sellList.AddItem(item);
        }
        GameManager.instance.data.stars[solarIndex[0]].SetSelling(item);
    }
    
    

    public void RemoveSelling(int itemId, int structureId, double amount)
    {

        int itemIndex = -1;
        if (structureId == -1)
        {
            itemIndex = sellList.items.FindIndex(x => x.id == itemId);
        }
        else
        {
            itemIndex = sellList.items.FindIndex(x => x.id == itemId && x.structureId == structureId);
        }

        if (itemIndex >= 0)
        {
            sellList.items[itemIndex].RemoveAmount(amount);
            if (sellList.items[itemIndex].amount == 0)
                sellList.items.RemoveAt(itemIndex);
        }

        GameManager.instance.data.stars[solarIndex[0]].RemoveSelling(itemId, structureId, amount);
    }

    #endregion

    public void AddPopulation(int people)
    {
        population += people;
        totalPopulation += people;
        if (solarIndex.Count > 1)
        {
            GameManager.instance.data.stars[solarIndex[0]].solar.AddTotalPopulation(people);
        }
        if (solarIndex.Count >2)
        {
            GameManager.instance.data.stars[solarIndex[0]].solar.satelites[solarIndex[1]].AddTotalPopulation(people);
        }

    }

    protected void AddTotalPopulation(int people)
    {
        totalPopulation += people;
    }

    public void SubtractPopulation(int people)
    {
        population -= people;
        totalPopulation -= people;
        if (solarIndex.Count > 1)
        {
            GameManager.instance.data.stars[solarIndex[0]].solar.SuntractTotalPopulation(people);
        }
        if (solarIndex.Count > 2)
        {
            GameManager.instance.data.stars[solarIndex[0]].solar.satelites[solarIndex[1]].SuntractTotalPopulation(people);
        }

    }

    protected void SuntractTotalPopulation(int people)
    {
        totalPopulation -= people;
    }

    //-----------------Orbital Functions-------------------//
    #region "Orbital Functions"
    public Vector2d GamePosition(double time)
    {
        if (orbit.sma == 0)
        {
            return Vector3d.zero;
        }

        double parentMass;
        if (solarIndex.Count == 2)
        {
            parentMass = GameManager.instance.data.stars[solarIndex[0]].solar.mass;
        }
        else if (solarIndex.Count == 3)
        {
            parentMass = GameManager.instance.data.stars[solarIndex[0]].solar.satelites[solarIndex[1]].mass;
        }
        else
        {
            throw new System.Exception("parentMass not found.");
        }

        var ena = ENA(time, parentMass);
        var pos = new Vector3d(orbit.sma * (Mathd.Cos(ena) - orbit.ecc), orbit.sma * Mathd.Sqrt(1 - (orbit.ecc * orbit.ecc)) * Mathd.Sin(ena));
        var pol = new Polar2d(pos);
        pol.angle += orbit.lpe;
        var pos2 = (pol.cartesian / GameDataModel.galaxyDistanceMultiplication); //Used to set the scale of the positioning of the solar systems
        lastKnownPosition = pos2;
        return pos2;
    }

    public Vector2d SetOrbit(double time, double orbitedObjectMass)
    {
        approximatePositions = new Vector2d[361];

        double timeInc = (OrbitalPeriod(orbitedObjectMass) / 360);

        for (var b = 0; b < 361; b++)
        {
            approximatePositions[b] = GamePosition(time);
            time += timeInc;
        }

        return lastKnownPosition;
    }

    private Vector3d Position(double time)
    {
        if (orbit.sma == 0)
        {
            return Vector3d.zero;
        }

        double parentMass;
        if (solarIndex.Count == 2)
        {
            parentMass = GameManager.instance.data.stars[solarIndex[0]].solar.mass;
        }
        else if (solarIndex.Count == 3)
        {
            parentMass = GameManager.instance.data.stars[solarIndex[0]].solar.satelites[solarIndex[1]].mass;
        }
        else
        {
            throw new System.Exception("parentMass not found.");
        }
        var ena = ENA(time, parentMass);
        var pos = new Vector3d(orbit.sma * (Mathd.Cos(ena) - orbit.ecc), orbit.sma * Mathd.Sqrt(1 - (orbit.ecc * orbit.ecc)) * Mathd.Sin(ena));
        var pol = new Polar2d(pos);
        pol.angle += orbit.lpe;
        var pos2 = pol.cartesian;
        return pos2;

    }

    public double temp(double time)
    {
        if (GameManager.instance.data.stars[solarIndex[0]].solar == this)
            return surfaceTemp;

        Vector3d position = Position(time);

        var solar = GameManager.instance.data.stars[solarIndex[0]].solar;

        if (solarIndex.Count == 3)
        {
            position += solar.satelites[solarIndex[1]].Position(time);
        }
        var distance = position.magnitude;
        //(Mathd.Pow(solar.bodyRadius, 2) / Mathd.Pow(position.magnitude, 2)) * solar.surfaceTemp * 2314;
        surfaceTemp = Mathd.Pow(((1 - bondAlebo) * solar.luminosity) / ((16 * Mathd.PI * 5.6705e-8) * Mathd.Pow(distance * Units.convertToMeters, 2)), .25) * Mathd.Pow((1 + .438 * greenhouse * .9), .25);

        return surfaceTemp;
    }
    /// <summary>
    /// Eccentric anomaly at a future time given the current eccentric anomaly
    /// </summary>
    /// <returns></returns>
    private double ENA(double time, double parentMass)
    {
        double M = orbit.mna + (MM(parentMass) * (time - orbit.eph));
        double E = M;
        for (int i = 0; i < 10; i++) // How many times it iterates to solve the equation.
        {
            E = M + orbit.ecc * Mathd.Sin(E);
        }
        return E;
    }
    /// <summary>
    /// Orbital period in seconds.
    /// </summary>
    /// <param name="parentMass"></param>
    /// <returns></returns>
    public double OrbitalPeriod (double parentMass) {  return 2 * Mathf.PI / MM(parentMass);  }

    public double MM(double parentMass) // Mean motion of orbit in radians per second.
    {
        return Mathd.Sqrt((GameDataModel.G * parentMass) / (Mathd.Pow((double) orbit.sma *Units.convertToMeters, 3)));
    }

    public double SOI(double parentMass)
    {
        if (orbit.sma == 0) // Assuming sun SOI.
        {
            return Units.M * Mathd.Pow(81 * mass, .25);
        }
        return orbit.sma * Mathd.Pow(mass / parentMass, .4f);
    }
#endregion
    public string GetInfo(double time)
    {
        double parentMass;
        if (solarIndex.Count == 2)
        {
            parentMass = GameManager.instance.data.stars[solarIndex[0]].solar.mass;
        }
        else if (solarIndex.Count == 3)
        {
            parentMass = GameManager.instance.data.stars[solarIndex[0]].solar.satelites[solarIndex[1]].mass;
        }
        else if (solarIndex.Count == 1)
        {
            parentMass = mass;
            return string.Format("Mass: {0}\nRadius: {1}\nSatelite Count: {2}\nSurfaceTemp: {3} C\nSurface Gravity: {4} g",
           mass.ToString("G4") + " kg",
           Units.ReadDistance(bodyRadius),
           satelites.Count,
           surfaceTemp.ToString("0.00"), 
           (surfaceGravity).ToString("0.00"));
        }
        else
        {
            throw new System.Exception("parentMass not found.");
        }

        var sTemp = temp(time) - 273.15;

        return string.Format("Type: {8}\nSubType: {9}\nMass: {0}\nRadius: {1}\nOrbital Period: {2}\nSatelite Count: {3}\nSemi-Major Axis: {4}\nSurfaceTemp: {5} C\nSurface Gravity: {6} g\nSurfacePressure: {7} atm\n",
            mass.ToString("G4") + " kg",
            Units.ReadDistance(bodyRadius),
            Dated.ReadTime(OrbitalPeriod(parentMass)),
            satelites.Count,
            Units.ReadDistance(orbit.sma),
            (GameManager.instance.data.stars[solarIndex[0]].solar == this)?surfaceTemp.ToString():sTemp.ToString("0.0"),
            (surfaceGravity).ToString("0.00"),
            surfacePressure.ToString("0.00"),
            solarType.ToString(),
            solarSubType.ToString());
    }
}

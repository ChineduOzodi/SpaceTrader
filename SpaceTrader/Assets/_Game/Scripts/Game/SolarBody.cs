using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarBody: PositionEntity
{
    public int totalPopulation { get; private set; }
    public SolarType solarType { get; private set; }
    public SolarSubType solarSubType { get; private set; }
    public Orbit orbit;
    public double mass { get; set; }
    /// <summary>
    /// In km
    /// </summary>
    public double bodyRadius { get; set; } 
    public Vector3 lastKnownPosition { get; private set; }
    public Vector3[] approximatePositions {get; private set;}

    //-----------------Star Properties---------------------//
    public double surfaceTemp { get; private set; }
    public double surfacePressure { get; private set; }
    public double luminosity { get; private set; } // in ergs per second
    public SolarBody Star { get
        {
            if (referenceId == "") return this;
            return ((SolarBody)GameManager.instance.locations[referenceId]).Star;
        } }
    public List<SolarBody> GetAllSolarBodies()
    {
        List<SolarBody> bodies = new List<SolarBody>(satelites);
        foreach(SolarBody body in satelites)
        {
            bodies.AddRange(body.GetAllSolarBodies());
        }
        return bodies;
    }
    //----------------Rocky Satellite Properties-----------//
    public double bondAlebo { get; private set; } 
    public double greenhouse { get; private set; }
    public double surfaceGravity { get; private set; }
    public List<PlanetTile> planetTiles { get; private set; }
    public List<RawResource> rawResources { get; private set; }
    public List<Structure> structures = new List<Structure>();
    public bool deleteStructure;
   

    //----------------GameDisplay Properties---------------//
    public int scaleIndex;
    public bool HasStations
    {
        get
        {
            if (structureIds.Count > 0) return true;
            for (int i = 0; i < satelites.Count; i++)
            {
                if (satelites[i].HasStations) return true;
            }
            return false;
        }
    }
    //----------------Commerce-----------------------------//
    public IItemStorage buyList { get; private set; }
    public IItemStorage sellList { get; private set; }

    public ModelRefs<IdentityModel> companies = new ModelRefs<IdentityModel>();

    public bool inhabited = false;
    public bool sateliteInhabited = false;
    public Population population { get; private set; }
    //public Dictionary<RawResources, double> rawResources;
    public Color color;

    public List<SolarBody> satelites = new List<SolarBody>();
    public List<string> shipIds = new List<string>();
    


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
    public SolarBody(string _name, Vector3d galaxyPosition, SolarSubType _solarSubType, double mass, double radius, Orbit orbit, Color _color, double _surfaceTemp):
        base("", galaxyPosition)
    {
        rawResources = new List<RawResource>();
        planetTiles = new List<PlanetTile>();
        this.orbit = orbit;
        this.mass = mass;
        this.bodyRadius = radius;
        name = _name;
        this.solarType = solarType;
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

        scaleIndex = 1;
    }

    public SolarBody(string _name, string referenceId, SolarType _solarType, SolarSubType _solarSubType, double mass, double radius, Orbit orbit, Color _color, SolarBody star):
        base(referenceId)
    {
        this.orbit = orbit;
        this.mass = mass;
        this.bodyRadius = radius;
        name = _name;
        totalPopulation = 0;
        this.solarType = _solarType;
        this.solarSubType = _solarSubType;
        referencePosition = new Vector3d(0,orbit.sma, 0);
        color = _color;
        surfaceGravity = GameDataModel.G * mass / Mathd.Pow(Units.convertToMeters * radius, 2) / 9.81;
        surfacePressure = 0; // In atm
        bondAlebo = Random.value;
        

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
                surfaceTemp = Mathd.Pow(((1 - bondAlebo) * star.luminosity) / ((16 * Mathd.PI * 5.6705e-8) * Mathd.Pow(SystemPosition.magnitude * Units.convertToMeters, 2)), .25) * Mathd.Pow((1 + .438 * greenhouse * .9), .25);
            }
            else //Assumes solarbody is a planet orbiting the center star
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

        SetScaleIndex();
    }

    private void SetScaleIndex()
    {
        if (this.ReferenceBody.solarType == SolarType.Star)
        {
            scaleIndex = 1; //Gm
        }
        else scaleIndex = 2; //Mm
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

    public Structure GetStructure(string structureId)
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

    public void Population(int people)
    {
        population = new Population(this);
        this.inhabited = true;
    }

    #region "Commerce"

    public double GetMarketPrice(string itemId)
    {
        return 1.00;
        //    return GameManager.instance.data.stars[solarIndex[0]].GetMarketPrice(itemId);
    }

    public void SetBuying(Item item)
    {

        //    int itemIndex = -1;
        //    if (item.structureId == -1)
        //    {
        //        itemIndex = buyList.items.FindIndex(x => x.id == item.id);
        //    }
        //    else
        //    {
        //        itemIndex = buyList.items.FindIndex(x => x.id == item.id && x.structureId == item.structureId);
        //    }
        //    if (itemIndex >= 0)
        //    {
        //        buyList.items[itemIndex].SetAmount(item.amount, item.price);
        //    }
        //    else
        //    {
        //        buyList.AddItem(item);
        //    }

        //    GameManager.instance.data.stars[solarIndex[0]].SetBuying(item);
    }
    public void RemoveBuying(string itemId, string structureId, double amount)
    {

        //    int itemIndex = -1;
        //    if (structureId == -1)
        //    {
        //        itemIndex = buyList.items.FindIndex(x => x.id == itemId);
        //    }
        //    else
        //    {
        //        itemIndex = buyList.items.FindIndex(x => x.id == itemId && x.structureId == structureId);
        //    }

        //    if (itemIndex >= 0)
        //    {
        //        buyList.items[itemIndex].RemoveAmount(amount);
        //        if (buyList.items[itemIndex].amount == 0)
        //            buyList.items.RemoveAt(itemIndex);
        //    }

        //    GameManager.instance.data.stars[solarIndex[0]].RemoveBuying(itemId,structureId, amount);
    }

    public void SetSelling(Item item)
    {
        //    int itemIndex = -1;
        //    if (item.structureId == -1)
        //    {
        //        itemIndex = sellList.items.FindIndex(x => x.id == item.id);
        //    }
        //    else
        //    {
        //        itemIndex = sellList.items.FindIndex(x => x.id == item.id && x.structureId == item.structureId);
        //    }
        //    if (itemIndex >= 0)
        //    {
        //        sellList.items[itemIndex].SetAmount(item.amount, item.price);
        //    }
        //    else
        //    {
        //        sellList.AddItem(item);
        //    }
        //    GameManager.instance.data.stars[solarIndex[0]].SetSelling(item);
        }



    public void RemoveSelling(string itemId, string structureId, double amount)
    {

        //    int itemIndex = -1;
        //    if (structureId == -1)
        //    {
        //        itemIndex = sellList.items.FindIndex(x => x.id == itemId);
        //    }
        //    else
        //    {
        //        itemIndex = sellList.items.FindIndex(x => x.id == itemId && x.structureId == structureId);
        //    }

        //    if (itemIndex >= 0)
        //    {
        //        sellList.items[itemIndex].RemoveAmount(amount);
        //        if (sellList.items[itemIndex].amount == 0)
        //            sellList.items.RemoveAt(itemIndex);
        //    }

        //    GameManager.instance.data.stars[solarIndex[0]].RemoveSelling(itemId, structureId, amount);
    }

        #endregion
        //-----------------Orbital Functions-------------------//
        #region "Orbital Functions"
        /// <summary>
        /// Local position of solar body scaled to the game position
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public Vector3 GamePosition(double time)
    {
        var pos2 = (Position(time) * (KmToLocalScale(scaleIndex)));
        lastKnownPosition = (Vector3) pos2;
        return (Vector3) pos2;
    }

    public Vector3 SetOrbit(double time, double orbitedObjectMass)
    {
        approximatePositions = new Vector3[361];

        double timeInc = (OrbitalPeriod(orbitedObjectMass) / 360);

        for (var b = 0; b < 361; b++)
        {
            approximatePositions[b] = GamePosition(time);
            time += timeInc;
        }

        return lastKnownPosition;
    }
    /// <summary>
    /// Returns local position of solar body in km
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private Vector3d Position(double time)
    {
        if (orbit.sma == 0)
        {
            return Vector3d.zero;
        }

        double parentMass = ((SolarBody)GameManager.instance.locations[referenceId]).mass;
        var ena = ENA(time, parentMass);
        var pos = new Vector3d(orbit.sma * (Mathd.Cos(ena) - orbit.ecc), orbit.sma * Mathd.Sqrt(1 - (orbit.ecc * orbit.ecc)) * Mathd.Sin(ena));
        var pol = new Polar2d(pos);
        pol.angle += orbit.lpe;
        var pos2 = pol.cartesian;

        //Update Reference Position
        referencePosition = pos2;

        return pos2;

    }

    public double Temp(double time)
    {
        if (solarType == SolarType.Star)
            return surfaceTemp;

        Vector3d position = Position(time);

        var solar = Star;

        var distance = SystemPosition.magnitude;
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

    public double SOI()
    {
        if (orbit.sma == 0) // Assuming sun SOI.
        {
            return Units.M * Mathd.Pow(mass, .25);
        }

        var trueSOI = orbit.sma * Mathd.Pow(mass / ReferenceBody.mass, .4f);
        var estSOI = Units.M * Mathd.Pow(mass, .25);
        return (trueSOI < estSOI) ? trueSOI : estSOI;
    }
#endregion
    public string GetInfo(double time)
    {
        double parentMass;
        if (solarType == SolarType.Star)
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
            parentMass = ((SolarBody)GameManager.instance.locations[referenceId]).mass;
        }

        var sTemp = Temp(time) - 273.15;

        return string.Format("Type: {8}\nSubType: {9}\nMass: {0}\nRadius: {1}\nOrbital Period: {2}\nSatelite Count: {3}\nSemi-Major Axis: {4}\nSurfaceTemp: {5} C\nSurface Gravity: {6} g\nSurfacePressure: {7} atm\n",
            mass.ToString("G4") + " kg",
            Units.ReadDistance(bodyRadius),
            Dated.ReadTime(OrbitalPeriod(parentMass)),
            satelites.Count,
            Units.ReadDistance(orbit.sma),
            sTemp.ToString("0.0"),
            (surfaceGravity).ToString("0.00"),
            surfacePressure.ToString("0.00"),
            solarType.ToString(),
            solarSubType.ToString());
    }
}

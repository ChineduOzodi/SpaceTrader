using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarBody
{
    public string name;

    public SolarType solarType { get; private set; }
    public SolarSubType solarSubType { get; private set; }
    public Orbit orbit;
    public double mass { get; set; }
    public double bodyRadius { get; set; }
    
    //-----------------Star Properties---------------------//
    public double surfaceTemp { get; private set; }
    public double surfacePressure { get; private set; }
    public double luminosity { get; private set; } // in ergs per second

    //----------------Rocky Satellite Properties-----------//
    public double bondAlebo { get; private set; } 
    public double greenhouse { get; private set; }
    public double surfaceGravity { get; private set; }
    public List<PlanetTile> planetTiles;
    public List<int[]> groundStructureLocations;


    public double population;
    //public Dictionary<RawResources, double> rawResources;
    public Color color;

    public List<SolarBody> satelites = new List<SolarBody>();
    /// <summary>
    /// Index of solar body in solarsystem.
    /// </summary>
    public List<int> solarIndex { get; set; }
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
        this.orbit = orbit;
        this.mass = mass;
        this.bodyRadius = radius;
        name = _name;
        this.solarType = solarType;
        solarIndex = _solarIndex;
        color = _color;
        surfaceTemp = _surfaceTemp;
        surfacePressure = 1; // In atm
        luminosity = 3.846e33 / Mathd.Pow(mass / 2e30, 3); // in ergs per sec
        surfaceGravity = GameDataModel.G * mass / (radius * radius) / 9.81;
        solarType = SolarType.Star;
        solarSubType = _solarSubType;
    }

    public SolarBody(string _name, List<int> _solarIndex, SolarType _solarType, SolarSubType _solarSubType, double mass, double radius, Orbit orbit, Color _color, SolarBody star)
    {
        this.orbit = orbit;
        this.mass = mass;
        this.bodyRadius = radius;
        name = _name;
        this.solarType = _solarType;
        this.solarSubType = _solarSubType;
        solarIndex = _solarIndex;
        color = _color;
        surfaceGravity = GameDataModel.G * mass / (radius * radius) / 9.81;
        surfacePressure = 0; // In atm
        bondAlebo = Random.value;

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
            surfaceTemp = Mathd.Pow(((1 - bondAlebo) * star.luminosity) / ((16 * Mathd.PI * 5.6705e-8) * Mathd.Pow(orbit.sma, 2)), .25) * Mathd.Pow((1 + .438 * greenhouse * .9), .25);

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
            surfaceTemp = Mathd.Pow(((1 - bondAlebo) * star.luminosity) / ((16 * Mathd.PI * 5.6705e-8) * Mathd.Pow(orbit.sma, 2)), .25) * Mathd.Pow((1 + .438 * greenhouse * .9), .25);
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
            surfaceTemp = Mathd.Pow(((1 - bondAlebo) * star.luminosity) / ((16 * Mathd.PI * 5.6705e-8) * Mathd.Pow(orbit.sma, 2)), .25) * Mathd.Pow((1 + .438 * greenhouse * .9), .25);

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

        if (solarType != SolarType.Star && solarSubType != SolarSubType.GasGiant)
        {
            GeneratePlanetTiles();
        }
    }

    private void GeneratePlanetTiles()
    {
        planetTiles = new List<PlanetTile>();

        int numTiles = (int)(2 * Mathd.PI * bodyRadius / 4e7 * 15) + 1;

        for(int i = 0; i < numTiles; i++)
        {
            planetTiles.Add(new PlanetTile(solarSubType));
        }
        
    }

    public SolarBody() { }

    public Vector3 GamePosition(double time)
    {
        if (orbit.sma == 0)
        {
            return Vector3.zero;
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
        var pos2 = (Vector2)(pol.cartesian / GameDataModel.solarDistanceMultiplication);
        return pos2;

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
        surfaceTemp = Mathd.Pow(((1 - bondAlebo) * solar.luminosity) / ((16 * Mathd.PI * 5.6705e-8) * Mathd.Pow(distance, 2)), .25) * Mathd.Pow((1 + .438 * greenhouse * .9), .25);

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
        return Mathd.Sqrt((GameDataModel.G * parentMass) / (Mathd.Pow((double) orbit.sma, 3)));
    }

    public double SOI(double parentMass)
    {
        if (orbit.sma == 0) // Assuming sun SOI.
        {
            return Units.M * Mathd.Pow(81 * mass, .25);
        }
        return orbit.sma * Mathd.Pow(mass / parentMass, .4f);
    }

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
           surfaceTemp.ToString(), 
           (surfaceGravity).ToString("0.00"));
        }
        else
        {
            throw new System.Exception("parentMass not found.");
        }

        var sTemp = temp(time) - 273.15;

        return string.Format("Type: {9}\nSubType: {10}\nMass: {0}\nRadius: {1}\nOrbital Period: {2}\nSatelite Count: {3}\n{4}\nSurfaceTemp: {5} C\nSurface Gravity: {6} g\nSurfacePressure: {7} atm\nGreenhouse: {8}",
            mass.ToString("G4") + " kg",
            Units.ReadDistance(bodyRadius),
            Dated.ReadTime(OrbitalPeriod(parentMass)),
            satelites.Count,
            orbit.GetOrbitalInfo(),
            (GameManager.instance.data.stars[solarIndex[0]].solar == this)?surfaceTemp.ToString():sTemp.ToString("0.0"),
            (surfaceGravity).ToString("0.00"),
            surfacePressure,
            greenhouse,
            solarType.ToString(),
            solarSubType.ToString());
    }
}

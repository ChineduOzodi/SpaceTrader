using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarBody
{
    public string name;

    public Orbit orbit;
    public Units mass { get; set; }

    public Units bodyRadius { get; set; }
    public SolarType solarType { get; set; }
    public Units population;
    public Dictionary<RawResources, double> rawResources;
    public Color color;

    public Dictionary<int,SolarBody> satelites = new Dictionary<int, SolarBody>();
    /// <summary>
    /// Index of solar body in solarsystem.
    /// </summary>
    public List<int> solarIndex { get; set; }

    public SolarBody(string _name, List<int> _solarIndex, SolarType solarType,Units mass, Units radius, Orbit orbit, Color _color)
    {
        this.orbit = orbit;
        this.mass = mass;
        this.bodyRadius = radius;
        name = _name;
        this.solarType = solarType;
        solarIndex = _solarIndex;
        color = _color;            
    }

    public SolarBody() { }

    public Vector3 GamePosition(double time, Units parentMass)
    {
        if (orbit.sma == 0)
        {
            return Vector3.zero;
        }
        var ena = ENA(time, parentMass);
        var pos = new Vector3d(orbit.sma * (Mathd.Cos(ena) - orbit.ecc), orbit.sma * Mathd.Sqrt(1 - (orbit.ecc * orbit.ecc)) * Mathd.Sin(ena));
        var pol = new Polar2d(pos);
        pol.angle += orbit.lpe;
        return (Vector2)(pol.cartesian / orbit.sma.multiplicationFactor);

    }

    /// <summary>
    /// Eccentric anomaly at a future time given the current eccentric anomaly
    /// </summary>
    /// <returns></returns>
    private double ENA(double time, Units parentMass)
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
    public double OrbitalPeriod (Units parentMass) {  return 2 * Mathf.PI / MM(parentMass);  }

    public double MM(Units parentMass) // Mean motion of orbit in radians per second.
    {
        return Mathd.Sqrt((GameDataModel.G * parentMass) / (Mathd.Pow((double) orbit.sma, 3)));
    }

    public Units SOI(Units parentMass)
    {
        if (orbit.sma == null) // Assuming sun SOI.
        {
            return new Units((Units.G * Mathd.Pow(81 * mass.number, .25)) / GameDataModel.galaxyDistanceMultiplication, GameDataModel.galaxyDistanceMultiplication, GameDataModel.galaxyDistanceUnit);
        }
        return new Units((orbit.sma * Mathd.Pow(mass / parentMass, .4f)) / GameDataModel.solarDistanceMultiplication, GameDataModel.solarDistanceMultiplication, GameDataModel.solarDistanceUnit);
    }
}

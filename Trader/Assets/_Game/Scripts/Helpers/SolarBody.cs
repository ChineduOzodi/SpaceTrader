using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarBody
{
    public Orbit orbit;
    public SolarType solarType { get; set; }
    public string name;
    public int population;
    public Color color;
    public List<SolarBody> children;
    public PlanetType planetType;
    public Dictionary<RawResources, double> rawResources;
    public int solarIndex { get; set; }
    public int index { get; set; }

    public SolarBody(string _name, int _starIndex, SolarType solarType, Orbit orbit, Color _color)
    {
        this.orbit = orbit;
        name = _name;
        this.solarType = solarType;
        color = _color;            
    }
}

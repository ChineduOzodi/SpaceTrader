using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarBody
{
    
    public SolarType solarType
    {
        get { return solType; }
    }
    public float SOI
    {
        get { return soi; }
    }
    public string name;
    public float soi;
    public float mass;
    public int population;
    public Color color;
    public float bodyRadius;
    public SolarBody[] children;
    public SolarType solType;
    public PlanetType planetType;
    public RawResources rawResource;
    public Orbit solar;


    public SolarBody(string _name, int _starIndex, SolarType solarType, Polar2 _position, float _mass, Color _color, float G, SolarBody _parent)
    {
        solar = new Orbit(_starIndex, _parent, _position);
        mass = _mass;
        name = _name;
        solType = solarType;
        bodyRadius = Mathf.Sqrt(mass / Mathf.PI);
        color = _color;       
        if (solarType != SolarType.Star)
        {
            solar.angularSpeed = Mathf.Sqrt((G * solar.parent.mass) / Mathf.Pow(solar.pos.radius, 3));
            //soi = pos.radius * Mathf.Pow(mass / par.mass, 0.4f);
            soi = mass * Mathf.Sqrt(mass + 10);
            if(mass < 3)
            {
                planetType = PlanetType.Dwarf;
            }
            else if (mass < 7)
            {
                planetType = PlanetType.Regular;
                rawResource =(RawResources) Random.Range(0, 3);
            }
            else
            {
                planetType = PlanetType.Gas;
            }

            if (solarType == SolarType.Moon)
            {
                if(Random.value < .25f)
                {
                    rawResource = (RawResources)Random.Range(0, 3);
                }
            }
        }            
        else
        {
            soi = mass * Mathf.Sqrt(mass + 10);
            solar.angularSpeed = 0;
        }
            
    }
}

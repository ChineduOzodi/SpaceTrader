using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarBody:Orbit
{
    
    public SolarType solarType
    {
        get { return solType; }
    }

    public string name;
    public int population;
    public Color color;
    public float bodyRadius;
    public SolarBody[] children;
    public SolarType solType;
    public PlanetType planetType;
    public RawResources rawResource;

    public SolarBody(string _name, int _starIndex, SolarType solarType, Polar2 _position, float _mass, Color _color, float G, SolarBody _parent)
    {
        starIndex = _starIndex;
        name = _name;
        solType = solarType;
        pos = _position;
        mass = _mass;
        bodyRadius = Mathf.Sqrt(mass / Mathf.PI);
        color = _color;
        parent = _parent;       
        if (solarType != SolarType.Star)
        {
            angularSpeed = Mathf.Sqrt((G * parent.mass) / Mathf.Pow(pos.radius, 3));
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
            angularSpeed = 0;
        }
            
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarBody
{
    public float radius
    {
        get { return pos.radius; }
    }
    public SolarBody parent
    {
        get { return par; }
    }
    public SolarType solarType
    {
        get { return solType; }
    }
    public float SOI
    {
        get { return soi; }
    }
    public string name;
    public int starIndex;
    public float mass;
    public Color color;
    public float bodyRadius;
    public SolarBody[] children;
    private float angularSpeed;
    private float soi;
    private Polar2 pos;
    private SolarBody par;
    private SolarType solType;
    

    public SolarBody(string _name, int _starIndex, SolarType solarType, Polar2 _position, float _mass, Color _color, float G, SolarBody _parent)
    {
        starIndex = _starIndex;
        name = _name;
        solType = solarType;
        pos = _position;
        mass = _mass;
        bodyRadius = Mathf.Sqrt(mass / Mathf.PI);
        color = _color;
        par = _parent;       
        if (solarType != SolarType.Star)
        {
            angularSpeed = Mathf.Sqrt((G * par.mass) / pos.radius) / pos.radius;
            //soi = pos.radius * Mathf.Pow(mass / par.mass, 0.4f);
            soi = mass * Mathf.Sqrt(mass + 10);
        }            
        else
        {
            soi = mass * Mathf.Sqrt(mass + 10);
            angularSpeed = 0;
        }
            
    }

    public Polar2 GetLocalPosition(float time)
    {
        return new Polar2(pos.radius, pos.angle + time * angularSpeed);
    }
    public Vector2 GetWorldPosition(float time)
    {
        if (par == null)
        {
            return GetLocalPosition(time).cartesian;
        }
        else
        {
            return par.GetWorldPosition(time) + GetLocalPosition(time).cartesian;
        }
        
    }
}

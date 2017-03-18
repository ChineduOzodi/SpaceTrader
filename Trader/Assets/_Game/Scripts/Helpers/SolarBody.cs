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
    public string name;
    public float mass;
    public Color color;

    private float angularSpeed;
    private Polar2 pos;
    private SolarBody par;

    public SolarBody(string _name, Polar2 _position, float _mass, Color _color, float sunMass, float G, SolarBody _parent)
    {
        name = _name;
        pos = _position;
        mass = _mass;
        color = _color;
        par = parent;
        angularSpeed = Mathf.Sqrt((G * sunMass) / pos.radius) / pos.radius;
    }

    public Polar2 GetPosition(float time)
    {
        return new Polar2(pos.radius, pos.angle + time * angularSpeed);
    }
}

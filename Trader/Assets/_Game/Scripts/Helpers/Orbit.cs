using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit {

    protected float angularSpeed;
    protected float soi;
    protected Polar2 pos;
    public SolarBody parent;
    public float mass;
    public int starIndex;

    public float radius
    {
        get { return pos.radius; }
    }
    public float SOI
    {
        get { return soi; }
    }

    public Polar2 GetLocalPosition(float time)
    {
        return new Polar2(pos.radius, pos.angle + time * angularSpeed);
    }

    public Vector2 GetWorldPosition(float time)
    {
        if (parent == null)
        {
            return GetLocalPosition(time).cartesian;
        }
        else
        {
            return parent.GetWorldPosition(time) + GetLocalPosition(time).cartesian;
        }

    }
}

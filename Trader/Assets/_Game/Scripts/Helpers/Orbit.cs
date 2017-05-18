using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit {

    public float angularSpeed;
    
    public Polar2 pos;
    public SolarBody parent;
    
    public int starIndex;
    internal static float G = .01f;

    public Orbit (int _starIndex, SolarBody _parent, Polar2 _pos)
    {
        starIndex = _starIndex;
        parent = _parent;
        pos = _pos;
    }
    public float radius
    {
        get { return pos.radius; }
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
            return parent.solar.GetWorldPosition(time) + GetLocalPosition(time).cartesian;
        }

    }
    public void SetWorldPosition( Vector2 position, float time)
    {
        if (parent == null)
        {
            pos = new Polar2(position);
        }
        else
        {
            pos = new Polar2(position - parent.solar.GetWorldPosition(time));
        }
        
        angularSpeed = Mathf.Sqrt((G * parent.mass) / Mathf.Pow(pos.radius, 3));
        pos.angle = pos.angle - time * angularSpeed;
    }
}

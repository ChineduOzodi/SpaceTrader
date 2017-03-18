using UnityEngine;
using System.Collections;

public struct Polar2
{

    public float radius;
    public float angle;

    public Vector2 cartesian
    {
        get
        {
            return PolarToCartesian(radius, angle);
        }
    }

    /// <summary>
    /// Creates polar coordinates, with angle in radians
    /// </summary>
    /// <param name="_radius">radius</param>
    /// <param name="_angle">angle in radians</param>
	public Polar2(float _radius, float _angle)
    {
        radius = _radius;
        angle = _angle;
    }
    public Polar2(Vector2 point)
    {
        Polar2 polar = CartesianToPolar(point);

        angle = polar.angle;
        radius = polar.radius;
    }

    //--------------Static Fields-------------------//
    public static Polar2 zero
    {
        get { return new Polar2(0, 0); }
    }
    //--------------Static Functions----------------//
    /// <summary>
    /// the angle difference from angle1 to angle2 (in radians)
    /// </summary>
    /// <param name="angle1"></param>
    /// <param name="angle2"></param>
    /// <returns></returns>
    public static float Angle(float angle1, float angle2)
    {
        Polar2 angle1Pol = new Polar2(1, angle1);
        Polar2 angle2Pol = new Polar2(1, angle2);

        return Vector2.Angle(angle1Pol.cartesian, angle2Pol.cartesian) * Mathf.Deg2Rad;
    }
    /// <summary>
    /// Creates x and y variable from polar coords
    /// </summary>
    /// <param name="polar">Polar coords</param>
    /// <returns></returns>
    public static Vector2 PolarToCartesian(Polar2 polar)
    {
        return new Vector2(polar.radius * Mathf.Cos(polar.angle), polar.radius * Mathf.Sin(polar.angle));
    }

    private static Vector2 PolarToCartesian(float radius, float angle)
    {
        return new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
    }
    /// <summary>
    /// Converts Cartesian coords to polar coords with angle in radians
    /// </summary>
    /// <param name="point">Cartesian coordinate</param>
    /// <returns></returns>
    public static Polar2 CartesianToPolar(Vector2 point)
    {
        Polar2 polar;

        float angle = Mathf.Atan(point.y / point.x);

        if (point.x < 0)
        {
            angle += Mathf.PI;
        }
        else if (point.y < 0)
        {
            angle += Mathf.PI * 2;
        }

        polar = new Polar2(point.magnitude, angle);
        return polar;
    }
}

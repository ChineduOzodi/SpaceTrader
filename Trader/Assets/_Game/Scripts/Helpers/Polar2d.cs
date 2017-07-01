using UnityEngine;
using System.Collections;

public struct Polar2d
{

    public double radius;
    public double angle;

    public Vector2d cartesian
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
	public Polar2d(double _radius, double _angle)
    {
        radius = _radius;
        angle = _angle;
    }
    public Polar2d(Vector2d point)
    {
        Polar2d polar = CartesianToPolar(point);

        angle = polar.angle;
        radius = polar.radius;
    }

    //--------------Static Fields-------------------//
    public static Polar2d zero
    {
        get { return new Polar2d(0, 0); }
    }
    //--------------Static Functions----------------//
    /// <summary>
    /// the angle difference from angle1 to angle2 (in radians)
    /// </summary>
    /// <param name="angle1"></param>
    /// <param name="angle2"></param>
    /// <returns></returns>
    public static double Angle(double angle1, double angle2)
    {
        Polar2d angle1Pol = new Polar2d(1, angle1);
        Polar2d angle2Pol = new Polar2d(1, angle2);

        return Vector2d.Angle(angle1Pol.cartesian, angle2Pol.cartesian) * Mathd.Deg2Rad;
    }
    /// <summary>
    /// Creates x and y variable from polar coords
    /// </summary>
    /// <param name="polar">Polar coords</param>
    /// <returns></returns>
    public static Vector2d PolarToCartesian(Polar2d polar)
    {
        return new Vector2d(polar.radius * Mathd.Cos(polar.angle), polar.radius * Mathd.Sin(polar.angle));
    }

    private static Vector2d PolarToCartesian(double radius, double angle)
    {
        if (radius == 0)
            return Vector2d.zero;
        return new Vector2d(radius * Mathd.Cos(angle), radius * Mathd.Sin(angle));
    }
    /// <summary>
    /// Converts Cartesian coords to polar coords with angle in radians
    /// </summary>
    /// <param name="point">Cartesian coordinate</param>
    /// <returns></returns>
    public static Polar2d CartesianToPolar(Vector2d point)
    {
        Polar2d polar;

        double angle = Mathd.Atan(point.y / point.x);

        if (point.x < 0)
        {
            angle += Mathd.PI;
        }
        else if (point.y < 0)
        {
            angle += Mathd.PI * 2;
        }

        polar = new Polar2d(point.magnitude, angle);
        return polar;
    }
}

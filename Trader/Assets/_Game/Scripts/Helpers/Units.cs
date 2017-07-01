using UnityEngine;
using System.Collections;

public static class Units
{
    /// <summary>
    /// 10 m
    /// </summary>
    public static int dam = 10;
    /// <summary>
    /// 100 m
    /// </summary>
    public static int hm = 100;
    public static int km = 1000;
    /// <summary>
    /// 1000 km
    /// </summary>
    public static int Mm = 1000000;
    /// <summary>
    /// 1 mill km
    /// </summary>
    public static int Gm = 1000000000;

    public static string ReadDistance(double distance)
    {
        if (distance < 500)
        {
            return distance.ToString("0.0") + " m";
        }
        else if (distance < Gm * .5d)
        {
            return (distance / km).ToString("0.0") + " km";
        }
        else
        {
            return (distance / Gm).ToString("0.00") + " Gm";
        }
    }
}
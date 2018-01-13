using UnityEngine;
using System.Collections;

public class Units
{
    /// <summary>
    /// Converts base unit to meters
    /// </summary>
    public static int convertToMeters = 1000;
    /// <summary>
    /// 10
    /// </summary>
    public static double da = .01;
    /// <summary>
    /// 100
    /// </summary>
    public static double h = .1;
    public static int k = 1;
    /// <summary>
    /// 1000 k or 1 mil
    /// </summary>
    public static int M = 1000;
    /// <summary>
    /// 1 mill k or 10^9 or 1 bil
    /// </summary>
    public static int G = 1000000;

    public static double ly = 9.461e+12;

    /// <summary>
    /// Used to allow compatibility with other numbers of the same measurement
    /// </summary>
    public double multiplicationFactor { get; set; }
    public string unit { get; set; }


    public static string ReadDistance(double number)
    {
        if (number < .5)
        {
            return (number * 1000).ToString("0.0") + " m";
        }
        else if (number < G * .5d)
        {
            return (number).ToString("0.0") + " km";
        }
        else if (number < ly * .5)
        {
            return (number / G).ToString("0.00") + " Gm";
        }
        else
        {
            return (number / ly).ToString("0.00") + " ly";
        }
    }

    public static string ReadItem(double number)
    {
        if (number < 10000)
        {
            if (Mathd.Floor(number) == number)
            {
                return ((int)number).ToString() + " ";
            }
            return (number).ToString("0.0")+" ";
        }
        else if (number < 1000000)
        {
            return (number/ 1000).ToString("0.00") + " k";
        }
        else if (number < 1000000000)
        {
            return (number / 1000000).ToString("0.000") + " M";
        }
        else
        {
            return (number /1000000000).ToString("0.000") + " B";
        }
    }

}
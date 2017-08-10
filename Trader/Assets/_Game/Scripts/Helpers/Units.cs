using UnityEngine;
using System.Collections;

public class Units
{
    /// <summary>
    /// 10
    /// </summary>
    public static int da = 10;
    /// <summary>
    /// 100
    /// </summary>
    public static int h = 100;
    public static int k = 1000;
    /// <summary>
    /// 1000 k or 1 mil
    /// </summary>
    public static int M = 1000000;
    /// <summary>
    /// 1 mill k or 10^9 or 1 bil
    /// </summary>
    public static int G = 1000000000;

    /// <summary>
    /// Used to allow compatibility with other numbers of the same measurement
    /// </summary>
    public double multiplicationFactor { get; set; }
    public string unit { get; set; }


    public static string ReadDistance(double number)
    {
        if (number < 500)
        {
            return number.ToString("0.0") + " m";
        }
        else if (number < G * .5d)
        {
            return (number / k).ToString("0.0") + " km";
        }
        else
        {
            return (number / G).ToString("0.00") + " Gm";
        }
    }

}
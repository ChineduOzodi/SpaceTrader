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

    public double number { get; set; }
    /// <summary>
    /// Used to allow compatibility with other numbers of the same measurement
    /// </summary>
    public double multiplicationFactor { get; set; }
    public string unit { get; set; }


    public string ReadMetric()
    {
        if (number < 500)
        {
            return number.ToString("0.0") + " " + unit;
        }
        else if (number < G * .5d)
        {
            return (number / k).ToString("0.0") + " k" + unit;
        }
        else
        {
            return (number / G).ToString("0.00") + " G" + unit;
        }
    }

    public Units() { multiplicationFactor = 1; }

    public Units(double number, double multiplicationFactor, string unit)
    {
        this.number = number;
        this.multiplicationFactor = multiplicationFactor;
        this.unit = unit;
    }

    public Units(double number)
    {
        this.number = number;
        multiplicationFactor = 1;
    }

    public static double operator *(Units unit, Units num)
    {
        return num * unit.number * unit.multiplicationFactor;
    }

    public static double operator *(Units unit, double num)
    {
        return num * unit.number * unit.multiplicationFactor;
    }

    public static double operator *( double num, Units unit)
    {
        return num * unit.number * unit.multiplicationFactor;
    }
    public static double operator *(Units unit, float num)
    {
        return num * unit.number * unit.multiplicationFactor;
    }
    public static double operator *(float num, Units unit)
    {
        return num * unit.number * unit.multiplicationFactor;
    }
    public static double operator *(Units unit, int num)
    {
        return num * unit.number * unit.multiplicationFactor;
    }

    public static double operator /(Units unit, Units num)
    {
        return (unit.number * unit.multiplicationFactor) / num ;
    }

    public static double operator /(double num, Units unit)
    {
        return num / (unit.number * unit.multiplicationFactor);
    }

    public static double operator /(Units unit, double num)
    {
        return  (unit.number * unit.multiplicationFactor) / num;
    }
    public static long operator +(Units unit, long num)
    {
        return num + (long) (unit.number * unit.multiplicationFactor);
    }
    public static long operator +(Units unit, Units num)
    {
        return num + (long)(unit.number * unit.multiplicationFactor);
    }
    public static bool operator ==(Units unit, int num)
    {
        return num == (unit.number * unit.multiplicationFactor);
    }

    public static bool operator !=(Units unit, int num)
    {
        return num != (unit.number * unit.multiplicationFactor);
    }

    public static bool operator >(Units unit, double num)
    {
        return num < (unit.number * unit.multiplicationFactor);
    }

    public static bool operator <(Units unit, double num)
    {
        return num > (unit.number * unit.multiplicationFactor);
    }
    public static explicit operator double(Units unit)
    {
        return unit.number * unit.multiplicationFactor;
    }

}
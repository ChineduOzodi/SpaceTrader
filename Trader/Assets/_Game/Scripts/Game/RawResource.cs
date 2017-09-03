using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RawResource
{
    public RawResourceType rawResourceType;

    public double amount;
    public double accesibility;

    public RawResource(RawResourceType type, double amount, double accesibility)
    {
        rawResourceType = type;
        this.amount = amount;
        this.accesibility = accesibility;
    }

    public RawResource(RawResourceType type, double amount)
    {
        rawResourceType = type;
        this.amount = amount;
        this.accesibility = 1;
    }

    public string GetInf()
    {
        return string.Format("Resource Type: {0}\nResource Amount: {1} kg\nResource:Accesibility: {2}",
            rawResourceType.ToString(),
            amount,
            accesibility);
    }
}

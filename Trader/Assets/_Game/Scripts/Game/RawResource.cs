using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawResources
{
    private List<RawResource> rawResources;
    public RawResources() { }

    public RawResources(List<RawResource> rawResources)
    {
        this.rawResources = rawResources;
    }
    /// <summary>
    /// Returns a dictionary of the resource types and their amounts.
    /// </summary>
    /// <returns></returns>
    public Dictionary<RawResourceType,float> GetResources()
    {
        Dictionary<RawResourceType, float> resources = new Dictionary<RawResourceType, float>();
        foreach (RawResource resource in rawResources)
        {
            resources[resource.rawResourceType] = resource.amount;
        }
        return resources;
    }
    /// <summary>
    /// Finds amount of raw resouce in list. Returns 0 if not found or amount = 0.
    /// </summary>
    /// <param name="raw"></param>
    /// <returns></returns>
    public float FindAmount(RawResourceType raw)
    {
        foreach (RawResource resource in rawResources)
        {
            if (resource.rawResourceType == raw)
            {
                return resource.amount;
            }
        }
        return 0;
    }
    /// <summary>
    /// Removes an amount of an item from the list. Error if item not in list or amount to remove too much.
    /// </summary>
    /// <param name="raw"></param>
    /// <param name="amoun"></param>
    public void RemoveAmount(RawResourceType raw, float amount)
    {
        for (int i = 0; i < rawResources.Count; i++)
        {
            if (rawResources[i].rawResourceType == raw)
            {
                rawResources[i].RemoveAmount(amount);
                return;
            }
        }
        throw new System.Exception("Item not found.");
    }
}

public struct RawResource
{
    public RawResourceType rawResourceType;

    public float amount;
    public double accesibility;

    public RawResource(RawResourceType type, float amount, double accesibility)
    {
        rawResourceType = type;
        this.amount = amount;
        this.accesibility = accesibility;
        if (amount < 0)
        {
            throw new System.Exception("Amount bellow 0");
        }
    }

    public RawResource(RawResourceType type, float amount)
    {
        rawResourceType = type;
        this.amount = amount;
        this.accesibility = 1;
    }

    public void RemoveAmount(float removeAmount)
    {
        amount -= removeAmount;
        if (amount < 0)
        {
            throw new System.Exception("Amount bellow 0");
        }
    }

    public string GetInf()
    {
        return string.Format("Resource Type: {0}\nResource Amount: {1} kg\nResource:Accesibility: {2}",
            rawResourceType.ToString(),
            amount,
            accesibility);
    }
}
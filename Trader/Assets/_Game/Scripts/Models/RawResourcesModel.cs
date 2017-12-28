using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawResourcesModel: Model
{
    public List<RawResourceBlueprint> rawResources { get; private set; }

    public RawResourcesModel() {

        rawResources = new List<RawResourceBlueprint>();

        int rare = 1000;
        int scarce = 10000;
        int common = 100000;
        int abundant = 1000000;
        rawResources.Add(new RawResourceBlueprint(defaultRawResources.Warium.ToString(), (int)defaultRawResources.Warium,
            "Gaseous material mainly used for weapons", 1000 * Dated.Day / common,
            new List<RawResourceInfo>()
        {
            new RawResourceInfo(PlanetTileType.Desert, .1f, Vector2.up, common),
            new RawResourceInfo(PlanetTileType.Grasslands, .5f, Vector2.up,common),
            new RawResourceInfo(PlanetTileType.Ice, .1f, Vector2.up,common),
            new RawResourceInfo(PlanetTileType.Ocean, .5f, Vector2.up,common),
            new RawResourceInfo(PlanetTileType.Rocky, .01f, Vector2.up,common),
            new RawResourceInfo(PlanetTileType.Volcanic, .75f, Vector2.up,common),
        }));

        rawResources.Add(new RawResourceBlueprint(defaultRawResources.Armoroid.ToString(), (int)defaultRawResources.Armoroid,
            "Main material used to build structures", 1000 * Dated.Day / abundant,
            new List<RawResourceInfo>()
        {
            new RawResourceInfo(PlanetTileType.Desert, .1f, Vector2.up, abundant),
            new RawResourceInfo(PlanetTileType.Grasslands, .25f, Vector2.up,abundant),
            new RawResourceInfo(PlanetTileType.Ice, .1f, Vector2.up,common),
            new RawResourceInfo(PlanetTileType.Ocean, .05f, Vector2.up,scarce),
            new RawResourceInfo(PlanetTileType.Rocky, .5f, Vector2.up,abundant),
            new RawResourceInfo(PlanetTileType.Volcanic, .25f, Vector2.up,abundant),
        }));

        rawResources.Add(new RawResourceBlueprint(defaultRawResources.Fuelodite.ToString(), (int)defaultRawResources.Fuelodite,
            "Used to create ship fuel", 1000 * Dated.Day / abundant,
            new List<RawResourceInfo>()
        {
            new RawResourceInfo(PlanetTileType.Desert, .1f, Vector2.up, scarce),
            new RawResourceInfo(PlanetTileType.Grasslands, .1f, Vector2.up,scarce),
            new RawResourceInfo(PlanetTileType.Ice, .1f, Vector2.up,scarce),
            new RawResourceInfo(PlanetTileType.Ocean, .75f, Vector2.up,abundant),
            new RawResourceInfo(PlanetTileType.Rocky, .1f, Vector2.up,scarce),
            new RawResourceInfo(PlanetTileType.Volcanic, .5f, Vector2.up,common),
        }));

        rawResources.Add(new RawResourceBlueprint(defaultRawResources.Glassitum.ToString(), (int)defaultRawResources.Glassitum,
            "Used to create glass like structure", 1000 * Dated.Day / common,
            new List<RawResourceInfo>()
        {
            new RawResourceInfo(PlanetTileType.Desert, .95f, Vector2.up, abundant),
            new RawResourceInfo(PlanetTileType.Grasslands, .25f, Vector2.up,common),
            new RawResourceInfo(PlanetTileType.Ice, .25f, Vector2.up,abundant),
            new RawResourceInfo(PlanetTileType.Ocean, .05f, Vector2.up,scarce),
            new RawResourceInfo(PlanetTileType.Rocky, .15f, Vector2.up,common),
            new RawResourceInfo(PlanetTileType.Volcanic, .35f, Vector2.up,common),
        }));

        rawResources.Add(new RawResourceBlueprint(defaultRawResources.Goldium.ToString(), (int)defaultRawResources.Goldium,
            "Rare gold like material", 1000 * Dated.Day / rare,
            new List<RawResourceInfo>()
        {
            new RawResourceInfo(PlanetTileType.Desert, .02f, Vector2.up, rare),
            new RawResourceInfo(PlanetTileType.Grasslands, .05f, Vector2.up,rare),
            new RawResourceInfo(PlanetTileType.Ice, .01f, Vector2.up,rare),
            new RawResourceInfo(PlanetTileType.Ocean, .01f, Vector2.up,rare),
            new RawResourceInfo(PlanetTileType.Rocky, .1f, Vector2.up,rare),
            new RawResourceInfo(PlanetTileType.Volcanic, .05f, Vector2.up,rare),
        }));

        rawResources.Add(new RawResourceBlueprint(defaultRawResources.Coppode.ToString(), (int)defaultRawResources.Coppode,
            "Used to make intricate machinery", 1000 * Dated.Day / scarce,
            new List<RawResourceInfo>()
        {
            new RawResourceInfo(PlanetTileType.Desert, .1f, Vector2.up, scarce),
            new RawResourceInfo(PlanetTileType.Grasslands, .25f, Vector2.up,scarce),
            new RawResourceInfo(PlanetTileType.Ice, .05f, Vector2.up,rare),
            new RawResourceInfo(PlanetTileType.Ocean, .05f, Vector2.up,scarce),
            new RawResourceInfo(PlanetTileType.Rocky, .5f, Vector2.up,common),
            new RawResourceInfo(PlanetTileType.Volcanic, .75f, Vector2.up,common),
        }));

        rawResources.Add(new RawResourceBlueprint(defaultRawResources.Limoite.ToString(), (int)defaultRawResources.Limoite,
            "Used to make things more heat resistant", 1000 * Dated.Day / scarce,
            new List<RawResourceInfo>()
        {
            new RawResourceInfo(PlanetTileType.Desert, .1f, Vector2.up, common),
            new RawResourceInfo(PlanetTileType.Grasslands, .25f, Vector2.up,scarce),
            new RawResourceInfo(PlanetTileType.Ice, .05f, Vector2.up,rare),
            new RawResourceInfo(PlanetTileType.Ocean, .05f, Vector2.up,rare),
            new RawResourceInfo(PlanetTileType.Rocky, .5f, Vector2.up,scarce),
            new RawResourceInfo(PlanetTileType.Volcanic, .75f, Vector2.up,abundant),
        }));


    }

    public RawResourcesModel(List<RawResourceBlueprint> rawResources)
    {
        this.rawResources = rawResources;
    }

    public RawResourceBlueprint GetResource(int id)
    {
        return rawResources.Find(x => x.id == id);
    }
    /// <summary>
    /// Finds amount of raw resouce in list. Returns 0 if not found or amount = 0.
    /// </summary>
    /// <param name="raw"></param>
    /// <returns></returns>
    //public float FindAmount(defaultRawResources raw)
    //{
    //    foreach (RawResourceBlueprint resource in rawResources)
    //    {
    //        if (resource.rawResourceType == raw)
    //        {
    //            return resource.amount;
    //        }
    //    }
    //    return 0;
    //}
    /// <summary>
    /// Removes an amount of an item from the list. Error if item not in list or amount to remove too much.
    /// </summary>
    /// <param name="raw"></param>
    /// <param name="amoun"></param>
    //public void RemoveAmount(defaultRawResources raw, float amount)
    //{
    //    for (int i = 0; i < rawResources.Count; i++)
    //    {
    //        if (rawResources[i].rawResourceType == raw)
    //        {
    //            rawResources[i].RemoveAmount(amount);
    //            return;
    //        }
    //    }
    //    throw new System.Exception("Item not found.");
    //}
}

public struct RawResource
{
    public string name { get; private set; }
    public int id { get; private set; }
    public double amount { get; private set; }
    public double accessibility { get; private set; }

    public RawResource(string _name, int _resourceId, int _amount, float accessibility)
    {
        name = _name;
        id = _resourceId;
        amount = _amount;
        //Vector2 accessRange = GameManager.instance.data.rawResources.Model.GetResource(_resourceId).rawResourceInfos.Find(x => x.planetTileType == tileType).accessibility;
        this.accessibility = accessibility;
    }

    public double RemoveAmount(double removeAmount)
    {        
        if (removeAmount < amount)
        {
            amount -= removeAmount;
        }
        else
        {
            removeAmount = amount;
            amount = 0;
        }

        return removeAmount;
    }

    public void AddAmount(double amount)
    {
        this.amount += amount;
    }

    public string GetInf()
    {
        return string.Format("Resource Name: {0}\nResource Amount: {1} kg\nResource:Accesibility: {2}",
            name,
            amount,
            accessibility);
    }

    //Operators
    public static RawResource operator *(RawResource item, int num)
    {
        item.amount *= num;
        return item;
    }
    public static RawResource operator /(RawResource item, int num)
    {
        item.amount /= num;
        return item;
    }
    public static RawResource operator +(RawResource item, int num)
    {
        item.amount += num;
        return item;
    }
    public static RawResource operator -(RawResource item, int num)
    {
        item.amount -= num;
        if (item.amount < 0)
        {
            throw new System.Exception("Amount bellow 0");
        }
        return item;
    }
}

public struct RawResourceBlueprint
{
    public string name;
    public string description;
    public List<RawResourceInfo> rawResourceInfos { get; private set; }
    public int id { get; private set; }
    public double miningTime;

    public RawResourceBlueprint(string _name, string _description, double _miningTime, List<RawResourceInfo> resourcesInfo)
    {
        name = _name;
        description = _description;
        rawResourceInfos = resourcesInfo;
        id = GameManager.instance.data.id++;
        miningTime = _miningTime;
    }
    public RawResourceBlueprint(string _name, int _id, string _description, double _miningTime, List<RawResourceInfo> resourcesInfo)
    {
        name = _name;
        description = _description;
        rawResourceInfos = resourcesInfo;
        id = _id;
        miningTime = _miningTime;
    }

    public RawResourceInfo GetInfo(PlanetTileType planetTile)
    {
        return rawResourceInfos.Find(x => x.planetTileType == planetTile);
    }
}

public struct RawResourceInfo
{
    public PlanetTileType planetTileType;
    public float probability;
    public Vector2 accessibility;
    public int maxAmount;

    public RawResourceInfo(PlanetTileType tile, float _probability, Vector2 _accessibility, int _maxAmount)
    {
        planetTileType = tile;
        probability = _probability;
        accessibility = _accessibility;
        maxAmount = _maxAmount;
    }
}

public enum defaultRawResources
{
    Goldium,
    Armoroid,
    Coppode,
    Fuelodite,
    Warium,
    Limoite,
    Glassitum
}
using UnityEngine;
using System.Collections;
using CodeControl;
using System.Collections.Generic;
using System;

public class ItemsModel: Model{

    public List<ItemBlueprint> blueprints { get; set; }
    

    public ItemsModel() { blueprints = new List<ItemBlueprint>(); }

    public ItemBlueprint GetItem(string id)
    {
        return blueprints.Find(x => x.id == id);
    }
    
}
public class Item
{
    public string name
    {
        get { return GameManager.instance.data.itemsData.Model.GetItem(id).name; }
    }
    public ItemType itemType
    {
        get { return GameManager.instance.data.itemsData.Model.GetItem(id).itemType; }
    }
    public double EstimatedValue
    {
        get
        {
            return GameManager.instance.data.itemsData.Model.GetItem(id).estimatedValue;
        }
    }
    public float Size
    {
        get
        {
            return GameManager.instance.data.itemsData.Model.GetItem(id).size;
        }
    }
    /// <summary>
    /// Item size in meters squared.
    /// </summary>
    public SizeClass ItemSize
    {
        get
        {
            if (Size < 10) return SizeClass.S;
            if (Size < 100) return SizeClass.M;
            if (Size < 1000) return SizeClass.L;
            if (Size < 10000) return SizeClass.XL;
            else return SizeClass.T;
        }
    }

    public string id;
    public double amount;
    public string referenceId;
    public string destinationId;

    public Item() { }

    public Item(string _itemId, double _amount, string referenceId, string _destinationId)
        : this(_itemId, _amount,referenceId)
    {
        this.destinationId = _destinationId;
    }

    public Item(string _itemId, double _amount, string referenceId)
        :this(_itemId, _amount)
    {
        this.referenceId = referenceId;
    }
    public Item(string _itemId, double _amount)
    {
        id = _itemId;
        amount = _amount;
    }
    public Item(ItemType type, int _amount)
    {
        var itemBlueprint = GameManager.instance.data.itemsData.Model.blueprints.Find(x => x.itemType == type);
        id = itemBlueprint.id;
        amount = _amount;
    }
    public void SetAmount(double _amount)
    {
        amount = _amount;
    }

    public void AddAmount(double _amount)
    {
        if (_amount <= 0)
            return;
        amount += _amount;
    }

    public double RemoveAmount(double _amount)
    {
        
        if (amount - _amount < 0)
        {
            double removedAmount = amount;
            amount = 0;
            return removedAmount;
        }
        amount -= _amount;
        return _amount;
    }

    public float GetBaseArmor()
    {
        return GameManager.instance.data.itemsData.Model.GetItem(id).baseArmor;
    }

    public int GetWorkers()
    {
        return GameManager.instance.data.itemsData.Model.GetItem(id).workers;
    }
}

public class ItemBlueprint: IWorkers
{
    public string name { get; private set; }
    public string id { get; private set; }
    public string description { get; private set; }

    public int count;

    public Color color;
    public string coloredName;

    public ItemType itemType;
    /// <summary>
    /// Work amount needed to create. On average, a worker does one work a day.
    /// </summary>
    public double workAmount { get; set; }
    public double estimatedValue { get; private set; }
    /// <summary>
    /// Required resources to create one component.
    /// </summary>
    public List<Item> contstructionParts { get; private set; }

    /// <summary>
    /// Item Size in meters squared;
    /// </summary>
    public float size = 1;
    public float baseArmor { get; private set; }

    /// <summary>
    /// workers to operate 1 unit
    /// </summary>
    public int workers { get; set; }

    public double workerPayRate { get; set; }

    //Production Stats
    public double unitsCreated = 0;
    public double supply = 0;
    public double demand = 0;

    public ItemBlueprint() { }

    public ItemBlueprint(string _name, ItemType _itemType, string _desctiption, float workAmount, List<Item> parts)
    {
        name = _name;
        description = "This is the " + name;
        id = GetType().ToString()+ GameManager.instance.data.id++.ToString();
        itemType = _itemType;
        this.workAmount = workAmount;
        contstructionParts = parts;
        float armor = 0;
        int workers = 0;
        double cost = 0;
        parts.ForEach(x => { armor += x.GetBaseArmor(); workers += x.GetWorkers(); cost += x.EstimatedValue * x.amount; } );
        baseArmor = armor;
        this.workers = workers;
        cost += (workers + 15) * .00116f * this.workAmount; //worker pay rate
        estimatedValue = cost;
        System.Random rand = new System.Random(id.GetHashCode());
        float a = (float) rand.NextDouble();
        float b = (float)rand.NextDouble();
        float c = (float)rand.NextDouble();
        color = new Color(a, b, c);
        coloredName = "<color=" + ColorTypeConverter.ToRGBHex(color) + ">" + name + "</color>";
    }

    public ItemBlueprint(RawResourceBlueprint raw)
    {
        name = raw.name;
        description = raw.description;
        id = raw.id;
        itemType = ItemType.RawMaterial;
        workAmount = raw.miningTime;
        contstructionParts = new List<Item>();
        float armor = .1f;
        int workers = 0;
        baseArmor = armor;
        this.workers = workers;
        estimatedValue = (workers + 10) * workAmount * .00116f;
        System.Random rand = new System.Random(id.GetHashCode());
        float a = (float)rand.NextDouble();
        float b = (float)rand.NextDouble();
        float c = (float)rand.NextDouble();
        color = new Color(a, b, c);
        coloredName = "<color=" + ColorTypeConverter.ToRGBHex(color) + ">" + name + "</color>";
    }
}

public interface IItemStorage
{
    double itemCapacity { get; set; }

    List<Item> itemsStorage { get; set; }

    bool AddItem(Item item);

    bool UseItem(string itemId, double amount);

    /// <summary>
    /// Uses as much of the item amount as it can from the storage, returns the left over item amount that was not used.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    double UseAsMuchItem(string itemId, double amount);

    /// <summary>
    /// Finds and returns whether the itemstorage contains a certain item at a certain amount
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool ContainsItem(string itemId, double amount);
    /// <summary>
    /// Returns whether storage contains a certain item.
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    bool ContainsItem(string itemId);
    /// <summary>
    /// Finds item with the same id stored in ItemStorage.
    /// </summary>
    /// <param name="item">item to look for</param>
    /// <returns>item that was found, or null</returns>
    Item Find(Item item);
    /// <summary>
    /// Finds item with the same id stored in ItemStorage.
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    Item Find(string itemId);
}

public enum ItemType
{
    BuildingMaterial,
    Fuel,
    Engine,
    Shield,
    Weapon,
    Container,
    FuelTank,
    CommandCapsule,
    Sensor,
    FactoryMachinery,
    AI,
    RawMaterial,
    Storage,
    Driller,
    Factory,
    SpaceStation,
    SpaceShip,
    DistributionCenter,
    StorageContainer
}

public enum SizeClass
{
    S,
    M,
    L,
    XL,
    T
}

public class FuelBlueprint : ItemBlueprint
{
    /// <summary>
    /// Percent modification of ship fuel range.
    /// </summary>
    public double fuelRangeMod;

    public FuelBlueprint() { }

    public FuelBlueprint(string _name, double fuelRangeMod, string _desctiption, float workAmount, List<Item> parts) :
        base(_name, ItemType.Fuel, _desctiption, workAmount, parts)
    {
        this.fuelRangeMod = fuelRangeMod;
    }
}

public class ShipBlueprint : ItemBlueprint
{
    public ShipType shipType;

    public int passangerCapacity;

    /// <summary>
    /// Sub light speed in km/s
    /// </summary>
    public float subLightSpeed;
    /// <summary>
    /// Rotate speed in degrees per second;
    /// </summary>
    public float rotateSpeed;

    public string fuelBlueprintId;
    public double fuelRange;
    public int fuelCapacity = 100;

    public double cargoCapacity = 100;

    //Ship Properties
    public double ApproxFuelCostPerKm
    {
        get { return Fuel.estimatedValue / fuelRange; }
    }

    public FuelBlueprint Fuel { get { return GameManager.instance.data.itemsData.Model.GetItem(fuelBlueprintId) as FuelBlueprint; } }

    public ShipBlueprint() { }

    public ShipBlueprint(string name, ShipType _shipType, string _desctiption, float workAmount, List<Item> parts) :
        base(name, ItemType.SpaceShip, _desctiption,workAmount,parts)
    {
        shipType = _shipType;

        this.workers = 10;
        workerPayRate = 15 / Dated.Hour;

        this.passangerCapacity = 10;
        cargoCapacity = 100;

        this.subLightSpeed = 1000;

        this.fuelRange = 100 * Units.G;
        this.fuelCapacity = 100;
        FuelBlueprint fuelBlueprint = GameManager.instance.data.itemsData.Model.blueprints.Find(x => x.itemType == ItemType.Fuel) as FuelBlueprint;
        fuelBlueprintId = fuelBlueprint.id;
        fuelRange *= fuelBlueprint.fuelRangeMod;
    }
}

public enum ShipType
{
    Cargo,
    Explorer,
    Combat
}

public class Fuel: Item
{
    /// <summary>
    /// Percent modification of ship fuel range.
    /// </summary>
    public double fuelRangeMod;

    public Fuel() { }
    public Fuel(string _itemId, double _amount, string _structureId):
        base(_itemId,_amount, _structureId)
    {
        fuelRangeMod = ((FuelBlueprint) GameManager.instance.data.itemsData.Model.GetItem(_itemId)).fuelRangeMod;
    }
}


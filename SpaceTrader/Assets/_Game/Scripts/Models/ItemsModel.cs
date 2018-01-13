using UnityEngine;
using System.Collections;
using CodeControl;
using System.Collections.Generic;
using System;

public class ItemsModel: Model{

    public List<ItemBlueprint> items { get; private set; }
    

    public ItemsModel() { items = new List<ItemBlueprint>(); }

    public ItemBlueprint GetItem(int id)
    {
        return items.Find(x => x.id == id);
    }
    
}
public class Item: IPositionEntity
{
    public string name { get; private set; }
    public ItemType itemType { get; private set; }
    public int id { get; private set; }
    public double amount { get; private set; }
    public double price;
    public double estimatedValue { get; private set; }

    public Vector2d galaxyPosition
    {
        get
        {
            if (shipId == -1)
            {
                if (solarIndex.Count == 3)
                {
                    return GameManager.instance.data.stars[solarIndex[0]].solar.satelites[solarIndex[1]].satelites[solarIndex[2]].galaxyPosition;
                }
                else if (solarIndex.Count == 2)
                {
                    return GameManager.instance.data.stars[solarIndex[0]].solar.satelites[solarIndex[1]].galaxyPosition;
                }
                else
                {
                    throw new System.Exception("BuildStructure " + name + " solarIndex count incorrect: " + solarIndex.Count);
                }
            }
            else if (shipId > -1)
            {
                return GameManager.instance.data.ships.Model.ships.Find(x => x.id == shipId).galaxyPosition;
            }
            else
            {
                throw new Exception("Unknown error with retriviing galaxy position");
            }
            
        }

        set
        {
            throw new System.Exception("Can't set galaxyPosition of item, move item instead");
        }
    }

    public List<int> solarIndex { get; set; }
    public int structureId { get; set; }
    public int shipId { get; set; }

    public Item(int _itemId, double _amount, double _price, List<int> _solarIndex, int _structureId = -1)
    {

        id = _itemId;
        name = GameManager.instance.data.itemsData.Model.GetItem(id).name;
        estimatedValue = GameManager.instance.data.itemsData.Model.GetItem(id).estimatedValue;
        amount = _amount;
        structureId = _structureId;
        shipId = -1;
        solarIndex = _solarIndex;
        price = _price;
        itemType = ItemType.AI;
        itemType = GetItemType();

    }
    public Item(int _itemId, double _amount, double _price)
    {

        id = _itemId;
        name = GameManager.instance.data.itemsData.Model.GetItem(id).name;
        estimatedValue = GameManager.instance.data.itemsData.Model.GetItem(id).estimatedValue;
        amount = _amount;
        shipId = -1;
        structureId = -1;
        price = _price;
        itemType = ItemType.AI;
        itemType = GetItemType();

    }

    public Item(string _name, int _itemId, ItemType type, int _amount, double _price)
    {

        id = _itemId;
        name = _name;
        amount = _amount;
        estimatedValue = GameManager.instance.data.itemsData.Model.GetItem(id).estimatedValue;
        price = _price;
        itemType = ItemType.AI;
        itemType = type;

        shipId = -1;
        structureId = -1;
    }

    public Item(ItemType type, int _amount, double _price)
    {
        var itemBlueprint = GameManager.instance.data.itemsData.Model.items.Find(x => x.itemType == type);
        id = itemBlueprint.id;
        name = itemBlueprint.name;
        estimatedValue = GameManager.instance.data.itemsData.Model.GetItem(id).estimatedValue;
        amount = _amount;
        price = _price;
        itemType = type;

        shipId = -1;
        structureId = -1;
    }

    public void SetAmount(double _amount, double _price)
    {
        price = _price;
        amount = _amount;
    }

    public void AddAmount(double _amount, double _price)
    {
        if (_amount <= 0)
            return;
        price = amount * price + _amount * _price;
        amount = amount + _amount;
        price /= amount;
    }

    public double RemoveAmount(double _amount)
    {
        if (amount - _amount < 0)
        {
            amount = 0;
            return amount;
        }
        amount -= _amount;
        return amount;
    }

    public float GetBaseArmor()
    {
        return GameManager.instance.data.itemsData.Model.GetItem(id).baseArmor;
    }

    public int GetWorkers()
    {
        return GameManager.instance.data.itemsData.Model.GetItem(id).workers;
    }

    private ItemType GetItemType()
    {
        return GameManager.instance.data.itemsData.Model.GetItem(id).itemType;
    }

    //Operators
    //public static Item operator *(Item item, int num)
    //{
    //    item.amount *= num;
    //    return item;
    //}
    //public static Item operator /(Item item, int num)
    //{
    //    item.amount /= num;
    //    return item;
    //}
    //public static Item operator +(Item item, int num)
    //{
    //    item.amount += num;
    //    return item;
    //}
    //public static Item operator -(Item item, float num)
    //{
    //    item.amount -= num;
    //    return item;
    //}
    //public static Item operator *(Item item, float num)
    //{
    //    item.amount *= num;
    //    return item;
    //}
    //public static Item operator /(Item item, float num)
    //{
    //    item.amount /= num;
    //    return item;
    //}
    //public static Item operator +(Item item, float num)
    //{
    //    item.amount += num;
    //    return item;
    //}
    //public static Item operator -(Item item, int num)
    //{
    //    item.amount -= num;
    //    return item;
    //}
}

public struct ItemBlueprint
{
    public string name { get; private set; }
    public int id { get; private set; }
    public string description { get; private set; }

    public Color color;
    public string coloredName;

    public ItemType itemType { get; private set; }
    public double productionTime { get; private set; }
    public double estimatedValue { get; private set; }
    /// <summary>
    /// Required resources to create one component.
    /// </summary>
    public List<Item> contstructionParts { get; private set; }

    public float baseArmor { get; private set; }

    /// <summary>
    /// workers to operate 1 unit
    /// </summary>
    public int workers { get; private set; }

    public ItemBlueprint(string _name, ItemType _itemType, string _desctiption, float _productionTime, List<Item> parts)
    {
        name = _name;
        description = "This is the " + name;
        id = GameManager.instance.data.id++;
        itemType = _itemType;
        productionTime = _productionTime;
        contstructionParts = parts;
        float armor = 0;
        int workers = 0;
        double cost = 0;
        parts.ForEach(x => { armor += x.GetBaseArmor(); workers += x.GetWorkers(); cost += x.estimatedValue * x.amount; } );
        baseArmor = armor;
        this.workers = workers;
        cost += (workers + 15) * .00116f * productionTime; //worker pay rate
        estimatedValue = cost;
        System.Random rand = new System.Random(id);
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
        productionTime = raw.miningTime;
        contstructionParts = new List<Item>();
        float armor = .1f;
        int workers = 0;
        baseArmor = armor;
        this.workers = workers;
        estimatedValue = (workers + 10) * productionTime * .00116f;
        System.Random rand = new System.Random(id);
        float a = (float)rand.NextDouble();
        float b = (float)rand.NextDouble();
        float c = (float)rand.NextDouble();
        color = new Color(a, b, c);
        coloredName = "<color=" + ColorTypeConverter.ToRGBHex(color) + ">" + name + "</color>";
    }
}

public class ItemStorage
{
    public List<Item> items = new List<Item>();
    public int structureId = -1;

    public ItemStorage() {    }
    public ItemStorage(int _structureId)
    {
        structureId = _structureId;
    }
    public bool SetAmount(Item item)
    {
        int itemIndex = -1;
        itemIndex = items.FindIndex(x => x.id == item.id);

        if (itemIndex >= 0)
        {
            items[itemIndex].SetAmount(item.amount, item.price);
        }
        else
        {
            item.structureId = structureId;
            items.Add(item);
        }
        return true;
    }
    public bool AddItem(Item item)
    {
        int itemIndex = -1;
        itemIndex = items.FindIndex(x => x.id == item.id);

        if (itemIndex >= 0)
        {
            items[itemIndex].AddAmount(item.amount, item.price);
        }
        else
        {
            item.structureId = structureId;
            items.Add(item);
        }
        return true;
    }

    public bool UseItem(Item item)
    {
        int itemIndex = -1;
        itemIndex = items.FindIndex(x => x.id == item.id);

        //if (item.structureId == -1)
        //{
        //    itemIndex = items.FindIndex(x => x.id == item.id && x.structureId == item.structureId);
        //}
        //else
        //    itemIndex = items.FindIndex(x => x.id == item.id);

        if (itemIndex >= 0)
        {
            if (items[itemIndex].amount < item.amount)
            {
                return false;
            }
            items[itemIndex].RemoveAmount(item.amount);
            if (items[itemIndex].amount == 0)
            {
                items.RemoveAt(itemIndex);
            }
            return true;
        }
        return false;
    }
    /// <summary>
    /// Uses as much of the item amount as it can from the storage, returns the left over item amount that was not used.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public Item UseAsMuchItem(Item item)
    {
        int itemIndex = -1;
        itemIndex = items.FindIndex(x => x.id == item.id);

        if (itemIndex >= 0)
        {
            if (items[itemIndex].amount <= item.amount)
            {
                item.RemoveAmount(items[itemIndex].amount);
                items[itemIndex].RemoveAmount(items[itemIndex].amount);
                items.RemoveAt(itemIndex);
                return item;
            }
            else
            {
                items[itemIndex].RemoveAmount(item.amount);
                item.RemoveAmount(item.amount);
                return item;
            }
        }
        throw new Exception("Could not find item " + item.name);
    }

    public bool ContainsItem(Item item)
    {
        int itemIndex = -1;
        itemIndex = items.FindIndex(x => x.id == item.id);

        return itemIndex != -1;
    }

    public bool ContainsItem(int itemId)
    {
        return items.Find(x => x.id == itemId) != null;
    }
    /// <summary>
    /// Finds item with the same id stored in ItemStorage.
    /// </summary>
    /// <param name="item">item to look for</param>
    /// <returns>item that was found, or null</returns>
    public Item Find(Item item)
    {
        return items.Find(x => x.id == item.id);
    }
    public Item Find(int itemId)
    {
        return items.Find(x => x.id == itemId);
    }
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
    GroundStorage,
    StorageContainer
}

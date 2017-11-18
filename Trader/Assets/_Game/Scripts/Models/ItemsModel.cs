using UnityEngine;
using System.Collections;
using CodeControl;
using System.Collections.Generic;

public class ItemsModel: Model{

    public List<ItemBluePrint> items { get; private set; }
    

    public ItemsModel() { items = new List<ItemBluePrint>(); }

    public ItemBluePrint GetItem(int id)
    {
        return items.Find(x => x.id == id);
    }
    
}
public struct Item
{
    public string name { get; private set; }
    public ItemType itemType { get; private set; }
    public int id { get; private set; }
    public float amount { get; private set; }

    public Item(int _itemId, int _amount)
    {
        
        id = _itemId;
        name = GameManager.instance.data.items.Model.GetItem(id).name;
        amount = _amount;
        itemType = ItemType.AI;
        itemType = GetItemType();
    }
    public Item(string _name, int _itemId, ItemType type, int _amount)
    {

        id = _itemId;
        name = _name;
        amount = _amount;
        itemType = ItemType.AI;
        itemType = type;
    }
    //Operators
    public static Item operator *(Item item, int num)
    {
        item.amount *= num;
        return item;
    }
    public static Item operator /(Item item, int num)
    {
        item.amount /= num;
        return item;
    }
    public static Item operator +(Item item, int num)
    {
        item.amount += num;
        return item;
    }
    public static Item operator -(Item item, float num)
    {
        item.amount -= num;
        return item;
    }
    public static Item operator *(Item item, float num)
    {
        item.amount *= num;
        return item;
    }
    public static Item operator /(Item item, float num)
    {
        item.amount /= num;
        return item;
    }
    public static Item operator +(Item item, float num)
    {
        item.amount += num;
        return item;
    }
    public static Item operator -(Item item, int num)
    {
        item.amount -= num;
        return item;
    }

    public bool RemoveAmount(float _amount)
    {
        if (amount - _amount < 0)
        {
            return false;
        }
        amount -= _amount;
        return true;
    }

    public float GetBaseArmor()
    {
        return GameManager.instance.data.items.Model.GetItem(id).baseArmor;
    }

    public int GetWorkers()
    {
        return GameManager.instance.data.items.Model.GetItem(id).workers;
    }

    private ItemType GetItemType()
    {
        return GameManager.instance.data.items.Model.GetItem(id).itemType;
    }
}

public struct ItemBluePrint
{
    public string name { get; private set; }
    public int id { get; private set; }
    public string description { get; private set; }

    public Color color;
    public string coloredName;

    public ItemType itemType { get; private set; }
    public float productionTime { get; private set; }
    /// <summary>
    /// Required resources to create one component.
    /// </summary>
    public List<Item> contstructionParts { get; private set; }

    public float baseArmor { get; private set; }

    /// <summary>
    /// workers to operate 1 unit
    /// </summary>
    public int workers { get; private set; }

    public ItemBluePrint(string _name, ItemType _itemType, string _desctiption, float _productionTime, List<Item> parts)
    {
        name = _name;
        description = "This is the " + name;
        id = GameManager.instance.data.id++;
        itemType = _itemType;
        productionTime = _productionTime;
        contstructionParts = parts;
        float armor = 0;
        int workers = 0;
        parts.ForEach(x => { armor += x.GetBaseArmor(); workers += x.GetWorkers(); } );
        baseArmor = armor;
        this.workers = workers;
        
        System.Random rand = new System.Random(id);
        float a = (float) rand.NextDouble();
        float b = (float)rand.NextDouble();
        float c = (float)rand.NextDouble();
        color = new Color(a, b, c);
        coloredName = "<color=" + ColorTypeConverter.ToRGBHex(color) + ">" + name + "</color>";
    }

    public ItemBluePrint(RawResourceBlueprint raw)
    {
        name = raw.name;
        description = raw.description;
        id = raw.id;
        itemType = ItemType.RawMaterial;
        productionTime = .1f;
        contstructionParts = new List<Item>();
        float armor = .1f;
        int workers = 0;
        baseArmor = armor;
        this.workers = workers;

        System.Random rand = new System.Random(id);
        float a = (float)rand.NextDouble();
        float b = (float)rand.NextDouble();
        float c = (float)rand.NextDouble();
        color = new Color(a, b, c);
        coloredName = "<color=" + ColorTypeConverter.ToRGBHex(color) + ">" + name + "</color>";
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
    RawMaterial
}

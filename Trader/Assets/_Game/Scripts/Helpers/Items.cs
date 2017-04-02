using UnityEngine;
using System.Collections;

public class Items {
    public string name;
    public ItemTypes itemType;
    public int pendingAmount;
    public int amount;
    public Color color;
    public string coloredName;

    public Items() { }
    public Items(ItemTypes _itemType, int _amount)
    {
        name = _itemType.ToString();
        itemType = _itemType;
        amount = _amount;
        pendingAmount = amount;

        System.Random rand = new System.Random(name.GetHashCode());
        float a = rand.Next(1000) / 1000f;
        float b = rand.Next(1000) / 1000f;
        float c = rand.Next(1000) / 1000f;
        color = new Color(a, b, c);
        coloredName = "<color=" + ColorTypeConverter.ToRGBHex(color) + ">" + name + "</color>";
    }
    public Items(string _name, ItemTypes _itemType, int _amount)
    {
        name = _name;
        itemType = _itemType;
        amount = _amount;
        pendingAmount = amount;

        System.Random rand = new System.Random(name.GetHashCode());
        float a = rand.Next(1000) / 1000f;
        float b = rand.Next(1000) / 1000f;
        float c = rand.Next(1000) / 1000f;
        color = new Color(a, b, c);
        coloredName = "<color=" + ColorTypeConverter.ToRGBHex(color) + ">" + name + "</color>";
    }

    public static Items CoalOre
    {
        get { return new Items(ItemTypes.Coal, 1); }
    }
    public static Items Rock
    {
        get { return new Items(ItemTypes.Rock, 1); }
    }
    public static Items IronOre
    {
        get { return new Items(ItemTypes.Iron, 1); }
    }
    public static Items Fuel
    {
        get { return new Items(ItemTypes.Fuel, 1); }
    }
    public static Items Steel
    {
        get { return new Items(ItemTypes.Steel, 1); }
    }
    public static Items Wheat
    {
        get { return new Items(ItemTypes.Wheat, 1); }
    }
    public static Items Food
    {
        get { return new Items(ItemTypes.Food, 1); }
    }
    public static Items Glass
    {
        get { return new Items(ItemTypes.Glass, 1); }
    }
    public static Items Sculpture
    {
        get { return new Items(ItemTypes.Sculpture, 1); }
    }
    public static Items Ship
    {
        get { return new Items(ItemTypes.Ship, 1); }
    }

    //Operators
    public static Items operator *(Items item, int num)
    {
        item.amount *= num;
        return item;
    }
    public static Items operator /(Items item, int num)
    {
        item.amount /= num;
        return item;
    }
    public static Items operator +(Items item, int num)
    {
        item.amount += num;
        return item;
    }
    public static Items operator -(Items item, int num)
    {
        item.amount -= num;
        return item;
    }
}

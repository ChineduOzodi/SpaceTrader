using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionItem : Items {

    public int productionAmount;
    public float price;
    public float totalPrice;

    public ProductionItem() { }
    public ProductionItem(ItemTypes _type, int amount)
    {
        name = _type.ToString();
        itemType = _type;
        productionAmount = amount;
    }

    public ProductionItem(string _name, ItemTypes _type, int amount)
    {
        name = _name;
        itemType = _type;
        productionAmount = amount;
    }

    public static new ProductionItem CoalOre
    {
        get { return new ProductionItem(ItemTypes.Coal, 1); }
    }
    public static new ProductionItem Rock
    {
        get { return new ProductionItem(ItemTypes.Rock, 1); }
    }
    public static new ProductionItem IronOre
    {
        get { return new ProductionItem(ItemTypes.Iron, 1); }
    }
    public static new ProductionItem Fuel
    {
        get { return new ProductionItem(ItemTypes.Fuel, 1); }
    }
    public static new ProductionItem Steel
    {
        get { return new ProductionItem(ItemTypes.Steel, 1); }
    }
    public static new ProductionItem Wheat
    {
        get { return new ProductionItem(ItemTypes.Wheat, 1); }
    }
    public static new ProductionItem Food
    {
        get { return new ProductionItem(ItemTypes.Food, 1); }
    }
    public static new ProductionItem Glass
    {
        get { return new ProductionItem(ItemTypes.Glass, 1); }
    }
    public static new ProductionItem Sculpture
    {
        get { return new ProductionItem(ItemTypes.Sculpture, 1); }
    }
    public static new ProductionItem Ship
    {
        get { return new ProductionItem(ItemTypes.Ship, 1); }
    }

    //Operators
    public static ProductionItem operator *(ProductionItem item, int num)
    {
        item.amount *= num;
        return item;
    }
    public static ProductionItem operator /(ProductionItem item, int num)
    {
        item.amount /= num;
        return item;
    }
    public static ProductionItem operator +(ProductionItem item, int num)
    {
        item.amount += num;
        return item;
    }
    public static ProductionItem operator -(ProductionItem item, int num)
    {
        item.amount -= num;
        return item;
    }


}

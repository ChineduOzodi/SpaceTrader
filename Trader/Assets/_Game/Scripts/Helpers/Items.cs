using UnityEngine;
using System.Collections;

public class Items {

    public string name;

    public StructureType type = StructureType.Item;

    public int pendingAmount;
    public int amount;

    public float price;
    public float basePrice;
    public float processTime;
    public float totalPrice;

    public int maxAmount;
    public bool selfProducing = false;
    public int ratio;

    public static Items coalOre
    {
        get
        {
            return new Items("Coal Ore", 0);
        }
    }

    public Items(string _name, int _amount, int _ratio = 1, int _maxAmount = 1000)
    {
        name = _name;
        amount = _amount;
        pendingAmount = amount;
        
        ratio = _ratio;
        maxAmount = _maxAmount;

        if (_name == "Coal Ore")
        {
            basePrice = 12;
            processTime = .5f;            
        }
        else if (_name == "Coal Rock")
        {
            basePrice = 7;
            processTime = 1.5f;
        }
        else if (_name == "Iron Ore")
        {
            basePrice = 15;
            processTime = 2f;
        }
        else if (_name == "Iron Rock")
        {
            basePrice = 9;
            processTime = 1.75f;
        }
        else if (_name == "Fuel")
        {
            basePrice = 5;
            processTime = 1f;
        }
        else if (_name == "Rock")
        {
            basePrice = 1;
            processTime = 1f;
        }
        else if (_name == "Unrefined Fuel")
        {
            basePrice = 3;
            processTime = 1.2f;
        }
        else if (_name == "Steel")
        {
            basePrice = 30;
            processTime = 4f;
        }
        else if (_name == "Glass")
        {
            basePrice = 10;
            processTime = 2f;
        }
        else if (_name == "Ship")
        {
            basePrice = 5000;
            processTime = 120f;
        }
        else if (_name == "Seed")
        {
            basePrice = 5;
            processTime = 10f;
        }
        else if (_name == "Wheat")
        {
            basePrice = 5;
            processTime = 15f;
        }
        else if (_name == "Food")
        {
            basePrice = 8;
            processTime = 5f;
        }
        else if (_name == "Sculpture")
        {
            basePrice = 50;
            processTime = 30f;
        }
        price = basePrice;
    }
}

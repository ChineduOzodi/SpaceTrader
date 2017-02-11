using UnityEngine;
using System.Collections;

public class Items {

    public string name;

    public StructureTypes type = StructureTypes.Item;

    public int amount;

    public float price;
    public float basePrice;

    public int maxAmount;
    public bool selfProducing = false;
    public int ratio;

    public Items(string _name, int _amount, float _basePrice, int _ratio = 1, int _maxAmount = 1000)
    {
        name = _name;
        amount = _amount;
        basePrice = _basePrice;
        price = _basePrice;
        ratio = _ratio;
        maxAmount = _maxAmount;
    }
}

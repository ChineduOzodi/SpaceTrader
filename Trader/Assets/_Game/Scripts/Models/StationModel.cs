using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeControl;

public class StationModel: StructureModel {


    StructureType type = StructureType.Station;

    public ModelRefs<ShipModel> incomingShips = new ModelRefs<ShipModel>();
    
    public float capacity;
    public Color color;
    public Color backgroundColor;

    public float runningCost = 10f;

    public Factory factory;

    public StationModel() { }

    internal Items Buy(string itemName, int itemAmount, ShipModel buyer = null)
    {
        foreach (Items item in factory.outputItems)
        {
            if (item.name == itemName)
            {
                if (itemAmount > item.amount)
                {
                    itemAmount = item.amount;
                }
                Items soldItem = new Items(itemName, itemAmount);
                soldItem.price = item.price;
                soldItem.totalPrice = soldItem.price * soldItem.amount;

                money += soldItem.totalPrice;
                item.amount -= itemAmount;
                factory.SetPrices();

                return soldItem;
            }

        }

        return null;
    }

    internal float Sell(Items sellItem)
    {
        foreach (Items item in factory.inputItems)
        {
            if (item.name == sellItem.name)
            {
                item.pendingAmount += sellItem.amount;

                money -= item.price * sellItem.amount;
                float price = item.price * sellItem.amount;
                factory.SetPrices();
                return price;
            }
        }

        return -1;
    }
    internal void SellComplete(Items sellItem)
    {
        foreach (Items item in factory.inputItems)
        {
            if (item.name == sellItem.name)
            {
                item.amount += sellItem.amount;
            }
        }
    }
    internal void SellIncomplete(Items sellItem)
    {
        foreach (Items item in factory.inputItems)
        {
            if (item.name == sellItem.name)
            {
                item.pendingAmount -= sellItem.amount;
                money += item.price * sellItem.amount;
                factory.SetPrices();
            }
        }
    }
}

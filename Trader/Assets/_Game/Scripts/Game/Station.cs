using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeControl;
using System.Xml.Serialization;

public class Station: Structure {

    //public ModelRef<CreatureModel> manager = new ModelRef<CreatureModel>();
    //public ModelRefs<ShipModel> incomingShips = new ModelRefs<ShipModel>();
    
    public float totalDocks { get; private set; }
    public float usedDocks { get; private set; }

    public float runningCost = 10f;

    public Station() { }

    public Station(string name, IdentityModel owner, SolarBody body)
    {

        this.owner = new ModelRef<IdentityModel>(owner);
        structureType = StructureTypes.SpaceStation;
        id = GameManager.instance.data.id++;
        solarIndex = body.solarIndex;
        this.name = name;
        workers = 250;
        totalDocks = 50;
        usedDocks = 0;

        body.spaceStructures.Add(this);

        //Money Setup
        owner.money -= 1000000;
        owner.AddSolarBodyWithStructure(body);
    }

    /// <summary>
    /// Item is Being Baught from station
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="itemType"></param>
    /// <param name="itemAmount"></param>
    /// <param name="buyer"></param>
    /// <returns></returns>
    //internal Items Buy(Items buyItem, IdentityModel buyer)
    //{
    //    foreach (ProductionItem item in factory.outputItems)
    //    {
    //        if (item.name == buyItem.name)
    //        {
    //            if (buyItem.amount > item.amount)
    //            {
    //                buyItem.amount = item.amount;
    //            }
    //            ProductionItem soldItem = (ProductionItem) buyItem;
    //            soldItem.price = item.price;
    //            soldItem.totalPrice = soldItem.price * soldItem.amount;

    //            money += soldItem.totalPrice;
    //            item.amount -= buyItem.amount;
    //            buyer.money -= soldItem.totalPrice;
    //            buyItem.amount = 0;
    //            buyItem.pendingAmount = soldItem.amount;
    //            return buyItem;
    //        }

    //    }

    //    return buyItem * 0;
    //}
    ///// <summary>
    ///// Item is being sold to station
    ///// </summary>
    ///// <param name="sellItem"></param>
    ///// <returns>Return much item that was actually sold to station</returns>
    //internal Items Sell(Items sellItem, IdentityModel seller)
    //{
    //    foreach (ProductionItem item in factory.inputItems)
    //    {
    //        if (item.name == sellItem.name)
    //        {
    //            item.pendingAmount += sellItem.amount;
    //            float price = item.price * sellItem.amount;
    //            money -= price;
    //            return sellItem;
    //        }
    //    }

    //    return sellItem * 0;
    //}
    //internal void SellIncomplete(Items sellItem)
    //{
    //    foreach (ProductionItem item in factory.inputItems)
    //    {
    //        if (item.name == sellItem.name)
    //        {
    //            item.pendingAmount -= sellItem.amount;
    //            money += item.price * sellItem.amount;
    //        }
    //    }
    //}
}

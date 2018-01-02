using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeControl;
using System.Xml.Serialization;

public class Station: ProductionStructure, IWorkers {

    //public ModelRef<CreatureModel> manager = new ModelRef<CreatureModel>();
    //public ModelRefs<ShipModel> incomingShips = new ModelRefs<ShipModel>();
    
    public float totalDocks { get; private set; }
    public float usedDocks { get; private set; }

    public List<Item> planetItems = new List<Item>();


    public bool isProducing = true;
    public double timePassed = 0;

    public float runningCost = 10f;

    public int workers { get; set; }

    public double workerPayRate { get; set; }

    public Station() { }

    public Station(string name, IdentityModel owner, SolarBody body, int _count = 1)
    {

        this.owner = new ModelRef<IdentityModel>(owner);
        structureType = StructureTypes.SpaceStation;
        id = GameManager.instance.data.id++;
        count = _count;
        solarIndex = body.solarIndex;
        structureId = -1;
        shipId = -1;
        this.name = name;
        workers = 250;
        workerPayRate = .00116;
        totalDocks = 50;
        usedDocks = 0;
        planetItems = new List<Item>();
        //GameManager.instance.data.itemsData.Model.items.ForEach(x =>
        //{
        //    if (x.itemType != ItemType.Fuel)
        //        planetItems.Add(new Item(x.id, Mathd.Ceil(Mathd.Pow(body.population,.5) / (Mathd.Pow(x.productionTime, 1.25))), 1,owner, solarIndex));
        //});
        var fuel = GameManager.instance.data.itemsData.Model.items.Find(x => x.itemType == ItemType.Fuel);
        requiredItems = new List<Item>() { new Item(fuel.id, 100, fuel.estimatedValue, solarIndex)};

        requiredItems.AddRange(planetItems);
        requiredItems.ForEach(x => {
            x.price = body.GetMarketPrice(x.id);
        });
        body.structures.Add(this);
        owner.AddSolarBodyWithStructure(body);

        productionTime = 1;
    }

    public void Update(SolarBody parentBody, double deltaTime)
    {
        if (deleteStructure)
            return;
        info = "Is producing: " + isProducing + "\nTime Completed: " + (timePassed / Dated.Day).ToString("0.000") + "%";
        if (isProducing)
        {
            timePassed += deltaTime;
            SearchRequiredItems(parentBody, deltaTime);
            if (timePassed > Dated.Day)
            {
                timePassed = 0;
                UsePlanetItems();
            }
        }
        
    }

    private void UsePlanetItems()
    {
        foreach (Item item in planetItems)
        {
            Item use = storage.Find(item.id);
            if (use != null)
            {
                var found = storage.UseItem(use);
                if (!found)
                    throw new System.Exception("Item is not found in correct amount");
            }
        }
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeControl;
using System.Xml.Serialization;

public class Station: ProductionStructure {

    //public ModelRef<CreatureModel> manager = new ModelRef<CreatureModel>();
    //public ModelRefs<ShipModel> incomingShips = new ModelRefs<ShipModel>();
    
    /// <summary>
    /// List of the ids of ships that are docked. If 0 then dock is empty.
    /// </summary>
    public List<int> docks { get; private set; }

    public List<Item> planetItems = new List<Item>(); //TODO: replace planet items with proper suply demand for planets


    public bool isProducing = true;
    public double timePassed = 0;

    public float runningCost = 10f;

    public Station() { }

    public Station(string name, IdentityModel owner, string referenceId, int _count = 1):
        base( owner, referenceId, (Vector3d)Random.onUnitSphere * (((SolarBody)GameManager.instance.locations[referenceId]).bodyRadius * .5 + 2500))
    {
        //blueprintId = GameManager.instance.data.itemsData.Model.blueprints.Find(x => x.itemType == ItemType.SpaceStation).id;
        structureType = StructureTypes.SpaceStation;
        count = _count;
        this.name = name;
        workers = 250 * count;
        workerPayRate = .00116;
        docks = new List<int>(4);
        planetItems = new List<Item>();
        //GameManager.instance.data.itemsData.Model.items.ForEach(x =>
        //{
        //    if (x.itemType != ItemType.Fuel)
        //        planetItems.Add(new Item(x.id, Mathd.Ceil(Mathd.Pow(body.population,.5) / (Mathd.Pow(x.productionTime, 1.25))), 1,owner, solarIndex));
        //});
        var fuel = GameManager.instance.data.itemsData.Model.blueprints.Find(x => x.itemType == ItemType.Fuel);
        requiredItems = new List<Item>() { new Item(fuel.id, 100)};

        SolarBody body = ((SolarBody)GameManager.instance.locations[referenceId]);

        requiredItems.AddRange(planetItems);
        //requiredItems.ForEach(x => {
        //    x.price = body.GetMarketPrice(x.id);
        //});
        body.structures.Add(this);
        workAmount = 1;
    }

    public override void Update()
    {
        deltaTime = GameManager.instance.data.date.time - lastUpdated.time;
        lastUpdated.AddTime(deltaTime);

        if (deleteStructure)
            return;

        Info = "Is producing: " + isProducing + "\nTime Completed: " + (timePassed / Dated.Day).ToString("0.000") + "%";
        if (isProducing)
        {
            timePassed += deltaTime;
            SearchRequiredItems();
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
            Item use = Find(item.id);
            if (use != null)
            {
                var found = UseItem(use.id,use.amount);
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
    internal Item Buy(Item buyItem, IdentityModel buyer)
    {
        return null;
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
    }
    ///// <summary>
    ///// Item is being sold to station
    ///// </summary>
    ///// <param name="sellItem"></param>
    ///// <returns>Return much item that was actually sold to station</returns>
    internal Item Sell(Item sellItem, IdentityModel seller)
    {
        return null;
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
    }
    }

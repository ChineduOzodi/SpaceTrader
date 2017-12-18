using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeControl;
using System.Xml.Serialization;

public class Station: IStructure, IWorkers {

    //public ModelRef<CreatureModel> manager = new ModelRef<CreatureModel>();
    //public ModelRefs<ShipModel> incomingShips = new ModelRefs<ShipModel>();
    
    public float totalDocks { get; private set; }
    public float usedDocks { get; private set; }

    public List<Item> requiredItems { get; private set; }
    public bool isProducing = true;
    public ItemsList storage = new ItemsList();

    public float runningCost = 10f;

    public StructureTypes structureType { get; set; }

    public string name { get; set; }
    public string info { get; set; }
    public ModelRef<IdentityModel> owner { get; set; }
    public int managerId { get; set; }
    public float maxArmor { get; set; }
    public float currentArmor { get; set; }
    public int id { get; set; }
    public Dated dateCreated { get; set; }
    public Dated lastUpdated { get; set; }
    public bool deleteStructure { get; set; }

    public Vector2d galaxyPosition
    {
        get
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

        set
        {
            throw new System.Exception("Can't set galaxyPosition, set solarIndex instead");
        }
    }

    public List<int> solarIndex { get; set; }
    public int structureId { get; set; }
    public int shipId { get; set; }

    public int workers { get; set; }

    public double workerPayRate { get; set; }

    public Station() { }

    public Station(string name, IdentityModel owner, SolarBody body)
    {

        this.owner = new ModelRef<IdentityModel>(owner);
        structureType = StructureTypes.SpaceStation;
        id = GameManager.instance.data.id++;
        solarIndex = body.solarIndex;
        structureId = -1;
        shipId = -1;
        this.name = name;
        workers = 250;
        workerPayRate = .00116;
        totalDocks = 50;
        usedDocks = 0;
        var fuel = GameManager.instance.data.itemsData.Model.items.Find(x => x.itemType == ItemType.Fuel);
        requiredItems = new List<Item>() { new Item(fuel.id, 100, fuel.estimatedValue,owner)};
        requiredItems.ForEach(x => {
            x.price = GameManager.instance.data.getSolarBody(solarIndex).GetMarketPrice(x.id);
            x.owner.Model = owner;
        });
        body.spaceStructures.Add(this);
        owner.AddSolarBodyWithStructure(body);
    }

    public void Update(SolarBody parentBody, double deltaTime)
    {
        if (deleteStructure)
            return;
        if (isProducing)
        {
            SearchRequiredItems(parentBody, deltaTime);
        }
        
    }

    private bool SearchRequiredItems(SolarBody parentBody, double deltaTime)
    {
        int neededItemCount = requiredItems.Count;
        int itemCount = 0;
        foreach (Item item in requiredItems)
        {
            double neededAmount = item.amount;
            if (storage.ContainsItem(item))
            {
                neededAmount -= storage.Find(item).amount;
            }
            if (neededAmount <= 0)
            {
                item.price = GameManager.instance.data.getSolarBody(solarIndex).GetMarketPrice(item.id);
                parentBody.RemoveBuying(item.id, owner.Model, id, item.amount);
                itemCount++;
            }
            else
            {
                var neededItem = new Item(item.id, neededAmount, item.price, owner.Model, id);
                var found = false;
                foreach (IStructure structure in parentBody.groundStructures)
                {
                    if (structure.structureType == StructureTypes.GroundStorage)
                    {
                        GroundStorage groundStruct = (GroundStorage)structure;
                        if (groundStruct.owner.Model == owner.Model && groundStruct.RemoveItem(neededItem))
                        {
                            storage.AddItem(neededItem);
                            parentBody.RemoveBuying(item.id, owner.Model, id, neededItem.amount);
                            parentBody.RemoveSelling(item.id, owner.Model, id, neededItem.amount);
                            neededAmount = item.amount - neededItem.amount;
                            if (neededAmount <= 0)
                            {
                                itemCount++;
                                found = true;
                                break;
                            }
                            neededItem = new Item(item.id, neededAmount, item.price);
                        }
                    }
                }
                //Runs if there are still needed Items
                if (!found)
                {
                    parentBody.SetBuying(neededItem);
                    item.price += item.price * GameManager.instance.marketPriceMod * deltaTime;
                    if (item.price < .1)
                    {
                        item.price = .1f;
                    }
                }
            }

        }
        return (neededItemCount == itemCount);
    }
    private void useRequiredItems(SolarBody parentBody)
    {
        foreach (Item item in requiredItems)
        {
            var found = storage.RemoveItem(item);
            if (!found)
                throw new System.Exception("Item is not found in correct amount");
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

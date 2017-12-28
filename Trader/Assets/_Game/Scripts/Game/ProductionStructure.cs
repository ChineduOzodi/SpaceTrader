using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionStructure: IStructure {

    public List<Item> requiredItems { get; protected set; }

    public string productionItemName;
    public int productionItemId { get; protected set; }
    /// <summary>
    /// Time to complete one cycle in days.
    /// </summary>
    public double productionTime { get; protected set; }

    public ItemsList storage = new ItemsList();

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

    public int count { get; set; }

    public ProductionStructure() { }

    public ProductionStructure(IdentityModel owner, int _productionItemId, SolarBody body, int _count = 1)
    {
        storage = new ItemsList();

        //Look to see if similar structure already exists
        foreach (IStructure structure in body.structures)
        {
            if (GetType() == structure.GetType())
            {
                //If the same structure is found, check ownership, and productionItemId
                ProductionStructure prodStruct = (ProductionStructure)structure;
                if (prodStruct.owner.Model == owner && prodStruct.productionItemId == _productionItemId)
                {
                    //If found, increase count
                    prodStruct.count++;
                    return;
                }
            }
        }

        this.owner = new ModelRef<IdentityModel>(owner);
        owner.AddSolarBodyWithStructure(body);
        body.structures.Add(this);

        id = GameManager.instance.data.id++;
        count = _count;
        solarIndex = body.solarIndex;
        structureId = -1;
        shipId = -1;

        productionItemId = _productionItemId;
        var product = GameManager.instance.data.itemsData.Model.GetItem(productionItemId);
        name = product.name + " " + structureType.ToString() + " " + id;
        productionItemName = product.name;
        
        requiredItems = product.contstructionParts;
        requiredItems.ForEach(x => {
            x.price = GameManager.instance.data.getSolarBody(solarIndex).GetMarketPrice(x.id);
            x.owner.Model = owner;
        });

        maxArmor = 1000;
        currentArmor = maxArmor;
        productionTime = product.productionTime;

        dateCreated = new Dated(GameManager.instance.data.date.time);
        lastUpdated = new Dated(GameManager.instance.data.date.time);

    }

    protected bool SearchRequiredItems(SolarBody parentBody, double deltaTime)
    {
        int neededItemCount = requiredItems.Count;
        int itemCount = 0;
        foreach (Item item in requiredItems)
        {
            double neededAmount = item.amount * count;
            if (storage.ContainsItem(item, false))
            {
                neededAmount -= storage.Find(item, false).amount;
            }
            if (neededAmount <= 0)
            {
                item.price = GameManager.instance.data.getSolarBody(solarIndex).GetMarketPrice(item.id);
                parentBody.RemoveBuying(item.id, owner.Model, id, item.amount);
                itemCount++;
            }
            else
            {
                var neededItem = new Item(item.id, neededAmount, item.price, owner.Model, solarIndex, id);
                var found = false;
                foreach (IStructure structure in parentBody.structures)
                {
                    if (structure.structureType == StructureTypes.GroundStorage)
                    {
                        GroundStorage groundStruct = (GroundStorage)structure;
                        if (groundStruct.owner.Model == owner.Model && groundStruct.ContainsItem(neededItem))
                        {
                            double itemAmount = neededItem.amount;
                            Item unusedItem = groundStruct.UseAsMuchItem(neededItem);
                            neededItem.SetAmount(itemAmount - unusedItem.amount, item.price);
                            storage.AddItem(neededItem);


                            parentBody.RemoveBuying(item.id, owner.Model, id, neededItem.amount);
                            parentBody.RemoveSelling(item.id, owner.Model, id, neededItem.amount);
                            neededAmount = itemAmount - neededItem.amount;
                            if (neededAmount <= 0)
                            {
                                itemCount++;
                                found = true;
                                break;
                            }
                            neededItem = new Item(item.id, neededAmount, item.price, owner.Model, solarIndex, id);
                        }
                    }
                }
                //Runs if there are still needed Items
                if (!found)
                {
                    parentBody.SetBuying(neededItem);
                    item.price += item.price * GameManager.instance.marketPriceMod * deltaTime * productionTime;
                    if (item.price < .1)
                    {
                        item.price = .1f;
                    }
                }
            }

        }
        return (neededItemCount == itemCount);
    }

    protected void UseRequiredItems(SolarBody parentBody)
    {
        foreach (Item item in requiredItems)
        {
            Item use = new Item(item.id, item.amount * count, item.price, item.owner.Model, item.solarIndex, item.structureId);
            var found = storage.UseItem(use, false);
            if (!found)
                throw new System.Exception("Item is not found in correct amount");
        }
    }

    protected bool StoreCreatedItem(SolarBody parentBody, double amount)
    {
        var found = false;
        Item item = new Item(productionItemId, amount, parentBody.GetMarketPrice(productionItemId), owner.Model, solarIndex, id);
        foreach (IStructure structure in parentBody.structures)
        {
            if (structure.structureType == StructureTypes.GroundStorage)
            {
                GroundStorage groundStruct = (GroundStorage)structure;
                if (groundStruct.owner.Model == owner.Model &&
                groundStruct.CanAddItem(item))
                {
                    groundStruct.AddItem(item);
                    //parentBody.SetSelling(item);
                    found = true;
                    break;
                }
            }

        }
        if (!found)
            throw new System.Exception("Component storage not found or nearly full");
        return true;
    }
}

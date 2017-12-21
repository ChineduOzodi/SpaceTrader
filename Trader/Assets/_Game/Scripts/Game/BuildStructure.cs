using UnityEngine;
using System.Collections.Generic;
using System;
using CodeControl;

public class BuildStructure : IStructure, IWorkers {

    /// <summary>
    /// For use with factories
    /// </summary>
    public int structureItemId { get; private set; }
    /// <summary>
    /// Time to complete one cycle in days.
    /// </summary>
    public double produtionTime { get; private set; }
    public float productionProgress { get; private set; }
    public List<Item> requiredItems { get; private set; }
    public ItemsList storage { get; private set; }
    public int factoryProductionId;
    public StructureTypes buildStructureType { get; private set; }

    public bool isProducing { get; private set; }

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

    public BuildStructure() { }

    public BuildStructure(IdentityModel owner, int itemId, int _productionItemId, SolarBody body)
    {
        this.owner = new ModelRef<IdentityModel>(owner);
        owner.AddSolarBodyWithStructure(body);
        structureType = StructureTypes.BuildStructure;
        id = GameManager.instance.data.id++;
        var product = GameManager.instance.data.itemsData.Model.GetItem(itemId);
        name = "Building ---> " + product.name + " | " + id;
        solarIndex = body.solarIndex;
        structureId = -1;
        shipId = -1;
        body.structures.Add(this);
        buildStructureType = StructureTypes.Factory;

        factoryProductionId = _productionItemId;
        requiredItems = new List<Item>() { new Item(itemId, 1, 1)};
        storage = new ItemsList();

        maxArmor = product.baseArmor * .5f;
        currentArmor = maxArmor;
        dateCreated = new Dated(GameManager.instance.data.date.time);
        lastUpdated = new Dated(GameManager.instance.data.date.time);
        produtionTime = GameManager.instance.data.itemsData.Model.GetItem(itemId).productionTime * .25f;
        workers = product.workers;
        workerPayRate = .00116;
    }

    public BuildStructure(IdentityModel owner, StructureTypes _structureType, int structureItemId, SolarBody body)
    {
        this.owner = new ModelRef<IdentityModel>(owner);
        owner.AddSolarBodyWithStructure(body);
        structureType = StructureTypes.BuildStructure;
        id = GameManager.instance.data.id++;
        var product = GameManager.instance.data.itemsData.Model.GetItem(structureItemId);
        name = "Building ---> " + product.name + " | " + id;
        solarIndex = body.solarIndex;
        structureId = -1;
        shipId = -1;
        body.structures.Add(this);
        buildStructureType = _structureType;
        
        this.structureItemId = structureItemId;
        requiredItems = new List<Item>() { new Item(product.id, 1, 1, owner, solarIndex) };
        requiredItems.ForEach(x => { x.price = GameManager.instance.data.getSolarBody(solarIndex).GetMarketPrice(x.id);
            x.owner.Model = owner;
        });
        storage = new ItemsList();

        maxArmor = product.baseArmor * .5f;
        currentArmor = maxArmor;
        dateCreated = new Dated(GameManager.instance.data.date.time);
        lastUpdated = new Dated(GameManager.instance.data.date.time);
        produtionTime = GameManager.instance.data.itemsData.Model.GetItem(structureItemId).productionTime * .25f;
        workers = product.workers;
        workerPayRate = .00116;
    }

    /// <summary>
    /// Creates items and uses items based on the elapsed time
    /// </summary>
    /// <param name="elapsedTime">time elapsed (in seconds)</param>
    public void UpdateProduction(SolarBody parentBody, double deltaTime)
    {
        if (deleteStructure)
            return;
        while (true)
        {
            var progress = (float)(deltaTime / produtionTime);
            info = "Active: " + isProducing.ToString();

            if (isProducing)
            {
                productionProgress += progress;
                if (productionProgress > 1)
                {
                    if (structureType == StructureTypes.Factory)
                    {
                        var structure = new Factory(owner.Model, structureItemId, factoryProductionId, solarIndex);
                        
                    }
                    deleteStructure = true;
                    GameManager.instance.data.getSolarBody(solarIndex).deleteStructure = true;
                    productionProgress = 1;
                    break;
                }
                else
                {
                    break;
                }
            }
            else if (SearchRequiredItems(parentBody, progress))
            {
                isProducing = true;
                useRequiredItems(parentBody);
            }
            else
            {
                break;
            }
        }
        
    }

    private bool SearchRequiredItems(SolarBody parentBody, float progress)
    {
        int neededItemCount = requiredItems.Count;
        int itemCount = 0;
        foreach ( Item item in requiredItems)
        {
            double neededAmount = item.amount;
            if (storage.ContainsItem(item))
            {
                neededAmount -= storage.Find(item).amount;
            }
            if (neededAmount <= 0)
            {
                parentBody.RemoveBuying(item.id, owner.Model, id, item.amount * progress);
                itemCount++;
                break;
            }
            var neededItem = new Item(item.id, neededAmount, item.price, owner.Model, solarIndex, id);
            var found = false;
            foreach (IStructure structure in parentBody.structures)
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
                            parentBody.RemoveBuying(item.id, owner.Model, id, item.amount);
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
                item.price += item.price * GameManager.instance.marketPriceMod * progress * produtionTime;
                if (item.price < .1)
                {
                    item.price = .1f;
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
                throw new Exception("Item is not found in correct amount");
        }
    }
}

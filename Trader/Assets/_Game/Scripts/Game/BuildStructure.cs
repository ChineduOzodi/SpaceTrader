using UnityEngine;
using System.Collections.Generic;
using System;
using CodeControl;

public class BuildStructure : Structure {

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
    public bool done { get; private set; }

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
        body.groundStructures.Add(this);
        buildStructureType = StructureTypes.Factory;

        factoryProductionId = _productionItemId;
        requiredItems = new List<Item>() { new Item(itemId, 1, 1)};
        storage = new ItemsList();

        maxArmor = product.baseArmor * .5f;
        currentArmor = maxArmor;
        produtionTime = GameManager.instance.data.itemsData.Model.GetItem(itemId).productionTime * .25f;
        workers = product.workers;
    }

    public BuildStructure(IdentityModel owner, Structure.StructureTypes _structureType, int structureItemId, SolarBody body)
    {
        this.owner = new ModelRef<IdentityModel>(owner);
        owner.AddSolarBodyWithStructure(body);
        structureType = StructureTypes.BuildStructure;
        id = GameManager.instance.data.id++;
        var product = GameManager.instance.data.itemsData.Model.GetItem(structureItemId);
        name = "Building ---> " + product.name + " | " + id;
        solarIndex = body.solarIndex;
        body.groundStructures.Add(this);
        buildStructureType = _structureType;
        
        this.structureItemId = structureItemId;
        requiredItems = new List<Item>() { new Item(product.id, 1, 1) };
        storage = new ItemsList();

        maxArmor = product.baseArmor * .5f;
        currentArmor = maxArmor;
        produtionTime = GameManager.instance.data.itemsData.Model.GetItem(structureItemId).productionTime * .25f;
        workers = product.workers;
    }

    /// <summary>
    /// Creates items and uses items based on the elapsed time
    /// </summary>
    /// <param name="elapsedTime">time elapsed (in seconds)</param>
    public void UpdateProduction(SolarBody parentBody, double deltaTime)
    {
        if (done)
            return;
        while (true)
        {
            var progress = (float)(deltaTime / produtionTime);
            if (isProducing)
            {
                productionProgress += progress;
                if (productionProgress > 1)
                {
                    if (structureType == StructureTypes.Factory)
                    {
                        var structure = new Factory(owner.Model, structureItemId, factoryProductionId, solarIndex);
                        
                    }
                    done = true;
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
            var neededItem = new Item(item.id, neededAmount, item.price);
            var found = false;
            foreach (Structure structure in parentBody.groundStructures)
            {
                if (structure.structureType == StructureTypes.GroundStorage)
                {
                    GroundStorage groundStruct = (GroundStorage)structure;
                    if (groundStruct.owner.Model == owner.Model && groundStruct.storage.RemoveItem(neededItem))
                    {
                        storage.AddItem(neededItem);
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
                parentBody.SetBuying(new Item(item.id, neededAmount * 2, parentBody.GetMarketPrice(item.id), owner.Model,id), neededAmount * progress);
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

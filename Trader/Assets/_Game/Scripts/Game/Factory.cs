using UnityEngine;
using System.Collections.Generic;
using System;
using CodeControl;

public class Factory : Structure {

    public string productionItemName;
    public int productionItemId { get; private set; }
    /// <summary>
    /// Time to complete one cycle in days.
    /// </summary>
    public double produtionTime { get; private set; }
    public float productionProgress { get; private set; }
    public List<Item> requiredItems { get; private set; }
    public ItemsList storage { get; private set; }
    

    public bool isProducing;

    public Factory() { }

    public Factory(IdentityModel owner, int _productionItemId, SolarBody body)
    {
        this.owner = new ModelRef<IdentityModel>(owner);
        owner.AddSolarBodyWithStructure(body);
        structureType = StructureTypes.Factory;
        id = GameManager.instance.data.id++;
        
        solarIndex = body.solarIndex;
        body.groundStructures.Add(this);
        var product = GameManager.instance.data.itemsData.Model.GetItem(productionItemId);
        name = product.name + " " + structureType.ToString() + " " + id;
        productionItemName = product.name;
        productionItemId = _productionItemId;
        requiredItems =product.contstructionParts;
        storage = new ItemsList();

        maxArmor = 1000;
        currentArmor = maxArmor;
        produtionTime = product.productionTime;
        workers = 30;

        owner.money -= 10000;
    }

    public Factory(IdentityModel owner, int structureItemId, int productionItemId, List<int> solarIndex)
    {
        this.productionItemId = productionItemId;
        this.solarIndex = solarIndex;
        var body = GameManager.instance.data.getSolarBody(solarIndex);
        this.owner = new ModelRef<IdentityModel>(owner);
        owner.AddSolarBodyWithStructure(body);
        structureType = StructureTypes.Factory;
        id = GameManager.instance.data.id++;
        
        body.groundStructures.Add(this);

        var product = GameManager.instance.data.itemsData.Model.GetItem(productionItemId);
        var blueprint = GameManager.instance.data.itemsData.Model.GetItem(structureItemId);
        name = product.name + " " + blueprint.name + " " + id;
        productionItemName = product.name;
        requiredItems = product.contstructionParts;
        storage = new ItemsList();

        maxArmor = blueprint.baseArmor;
        currentArmor = maxArmor;

        int machineryAmount = (int)blueprint.contstructionParts.Find(x => x.itemType == ItemType.FactoryMachinery).amount;
        produtionTime = product.productionTime / machineryAmount;
        workers = 15 + blueprint.workers;
    }

    /// <summary>
    /// Creates items and uses items based on the elapsed time
    /// </summary>
    /// <param name="elapsedTime">time elapsed (in seconds)</param>
    public void UpdateProduction(SolarBody parentBody, double deltaTime)
    {
        var price = parentBody.GetMarketPrice(productionItemId) / produtionTime;
        var cost = workers * workerPayRate;
        if ( price < cost)
        {
            return;
        }
            
        var progress = (float)(deltaTime / produtionTime);
        if (isProducing)
        {
            productionProgress += progress;
            owner.Model.money -= workerPayRate * workers * deltaTime;
            int loopCount = 0;
            while (productionProgress > 1)
            {
                loopCount++;
                if (loopCount > 1000)
                {
                    throw new Exception("Looping Update Production");
                }
                if (StoreCreatedItem(parentBody))
                {
                    productionProgress--;
                    
                    isProducing = false;
                    if (SearchRequiredItems(parentBody, progress))
                    {
                        isProducing = true;
                        useRequiredItems(parentBody);
                    }
                    else
                    {
                        productionProgress = 0;
                        break;
                    }

                }
            }
        }
        else if (SearchRequiredItems(parentBody, progress))
        {
            isProducing = true;
            useRequiredItems(parentBody);
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
            }
            else
            {
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
                    parentBody.SetBuying(new Item(item.id, neededAmount * 2, parentBody.GetMarketPrice(item.id), owner.Model, id), neededAmount * progress);
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
    private bool StoreCreatedItem(SolarBody parentBody)
    {
        var found = false;
        Item item = new Item(productionItemId, 1,parentBody.GetMarketPrice(productionItemId), owner.Model);
        foreach (Structure structure in parentBody.groundStructures)
        {
            if (structure.structureType == StructureTypes.GroundStorage)
            {
                GroundStorage groundStruct = (GroundStorage)structure;
                if (groundStruct.owner.Model == owner.Model && groundStruct.CanAddItem(item))
                {
                    groundStruct.AddItem(item);
                    parentBody.SetSelling(item);
                    found = true;
                    break;
                }
            }
        }
        return found;
    }
}

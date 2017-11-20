﻿using UnityEngine;
using System.Collections.Generic;
using System;
using CodeControl;

public class Driller : Structure {

    public string productionItemName;
    public int productionItemId { get; private set; }
    /// <summary>
    /// Time to complete one cycle in days.
    /// </summary>
    public double produtionTime { get; private set; }
    public double productionProgress { get; private set; }


    public bool isProducing = true;

    public Driller() { }

    /// <summary>
    /// Creates an itemFactory, the factory blueprint must have ai and machinery
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="drillerBlueprint"></param>
    /// <param name="_productionItemId"></param>
    public Driller(IdentityModel owner, ItemBluePrint drillerBlueprint, int _productionItemId, SolarBody body)
    {
        this.owner = new ModelRef<IdentityModel>(owner);
        solarIndex = body.solarIndex;
        body.groundStructures.Add(this);
        structureType = StructureTypes.Driller;
        id = GameManager.instance.data.id++;
        productionItemId = _productionItemId;
        productionItemName = name = GameManager.instance.data.itemsData.Model.GetItem(productionItemId).name;
        name = productionItemName + " Driller";
        maxArmor = drillerBlueprint.baseArmor;
        currentArmor = maxArmor;
        int aiAmount = (int) drillerBlueprint.contstructionParts.Find(x => x.itemType == ItemType.AI).amount;
        int machineryAmount = (int) drillerBlueprint.contstructionParts.Find(x => x.itemType == ItemType.FactoryMachinery).amount;
        produtionTime = GameManager.instance.data.itemsData.Model.GetItem(productionItemId).productionTime / machineryAmount;

        var productionEfficiency = (produtionTime * .9f + aiAmount * 1.1f) / (produtionTime + aiAmount);
        produtionTime /= productionEfficiency;

        workers = drillerBlueprint.workers + 10;
    }

    public Driller(IdentityModel owner, int _productionItemId, SolarBody body)
    {
        this.owner = new ModelRef<IdentityModel>(owner);
        owner.AddSolarBodyWithStructure(body);
        solarIndex = body.solarIndex;
        body.groundStructures.Add(this);
        structureType = StructureTypes.Driller;
        id = GameManager.instance.data.id++;
        productionItemId = _productionItemId;
        name = GameManager.instance.data.itemsData.Model.GetItem(productionItemId).name + " Driller " + id;
        maxArmor = 1000;
        currentArmor = maxArmor;
        produtionTime = GameManager.instance.data.itemsData.Model.GetItem(productionItemId).productionTime;

        workers = 25;
        owner.money -= 10000;
    }

    /// <summary>
    /// Creates items and uses items based on the elapsed time
    /// </summary>
    /// <param name="elapsedTime">time elapsed (in seconds)</param>
    public void UpdateProduction(SolarBody parentBody, double deltaTime)
    {
        if (isProducing && parentBody.GetMarketPrice(productionItemId) / produtionTime > workers * workerPayRate)
        {
            productionProgress = deltaTime / produtionTime;
            DrillResource(parentBody, productionProgress);
            owner.Model.money -= workerPayRate * workers * deltaTime;
        }
    }

    private bool RequiredComponentsAvailable(SolarBody parentBody)
    {
        var resource = GameManager.instance.data.rawResources.Model.GetResource(productionItemId);
        var found = false;
        foreach (RawResource raw in parentBody.rawResources)
        {
            if (raw.id == resource.id)
            {
                found = true;
                break;
            }
        }
        return found;
    }
    private void DrillResource(SolarBody parentBody, double amount)
    {
        var resource = GameManager.instance.data.rawResources.Model.GetResource(productionItemId);

        foreach (RawResource raw in parentBody.rawResources)
        {
            if (raw.id == resource.id)
            {

                double amountRemoved = raw.RemoveAmount(amount);
                storeCreatedItem(parentBody, amountRemoved);
                if (amount > amountRemoved)
                    isProducing = false;
                break;
            }
        }
    }
    private void storeCreatedItem(SolarBody parentBody, double amount)
    {
        var found = false;
        Item item = new Item(productionItemId, amount, parentBody.GetMarketPrice(productionItemId), owner.Model);
        foreach (Structure structure in parentBody.groundStructures)
        {
            if (structure.structureType == StructureTypes.GroundStorage) {
                GroundStorage groundStruct = (GroundStorage)structure;
                if (groundStruct.owner.Model == owner.Model &&
                groundStruct.CanAddItem(item))
                {
                    groundStruct.AddItem(item);
                    parentBody.SetSelling(item);
                    found = true;
                    break;
                }
            }
            
        }
        if (!found)
            throw new Exception("Component storage not found or nearly full");
    }
}
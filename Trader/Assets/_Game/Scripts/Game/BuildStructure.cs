using UnityEngine;
using System.Collections.Generic;
using System;
using CodeControl;

public class BuildStructure : ProductionStructure, IWorkers {

    /// <summary>
    /// For use with factories
    /// </summary>
    public int structureItemId { get; private set; }
    public float productionProgress { get; private set; }
    public int factoryProductionId;
    public StructureTypes buildStructureType { get; private set; }

    public bool isProducing { get; private set; }

    public int workers { get; set; }

    public double workerPayRate { get; set; }

    public BuildStructure() { }

    public BuildStructure(IdentityModel owner, int itemId, int _productionItemId, SolarBody body, int _count = 1):
        base(owner, _productionItemId, body, _count)
    {
        var product = GameManager.instance.data.itemsData.Model.GetItem(itemId);
        name = "Building ---> " + product.name + " | " + id;
        buildStructureType = StructureTypes.Factory;

        factoryProductionId = _productionItemId;
        requiredItems = new List<Item>() { new Item(itemId, 1, 1)};

        maxArmor = product.baseArmor * .5f;
        currentArmor = maxArmor;
        
        productionTime = GameManager.instance.data.itemsData.Model.GetItem(itemId).productionTime * .25f;
        workers = product.workers;
        workerPayRate = .00116;
    }

    public BuildStructure(IdentityModel owner, StructureTypes _structureType, int structureItemId, SolarBody body, int _count = 1)
    {
        this.owner = new ModelRef<IdentityModel>(owner);
        owner.AddSolarBodyWithStructure(body);
        structureType = StructureTypes.BuildStructure;
        id = GameManager.instance.data.id++;
        count = _count;
        var product = GameManager.instance.data.itemsData.Model.GetItem(structureItemId);
        name = "Building ---> " + product.name + " | " + id;
        solarIndex = body.solarIndex;
        structureId = -1;
        shipId = -1;
        body.structures.Add(this);
        buildStructureType = _structureType;
        
        this.structureItemId = structureItemId;
        requiredItems = new List<Item>() { new Item(product.id, 1, 1, solarIndex) };
        requiredItems.ForEach(x => { x.price = GameManager.instance.data.getSolarBody(solarIndex).GetMarketPrice(x.id);});
        storage = new ItemStorage();

        maxArmor = product.baseArmor * .5f;
        currentArmor = maxArmor;
        dateCreated = new Dated(GameManager.instance.data.date.time);
        lastUpdated = new Dated(GameManager.instance.data.date.time);
        productionTime = GameManager.instance.data.itemsData.Model.GetItem(structureItemId).productionTime * .25f;
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
            var progress = (float)(deltaTime / productionTime);
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
                UseRequiredItems(parentBody);
            }
            else
            {
                break;
            }
        }
        
    }
}

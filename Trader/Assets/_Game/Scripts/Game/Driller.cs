using UnityEngine;
using System.Collections.Generic;
using System;
using CodeControl;

public class Driller : ProductionStructure, IWorkers {

    public double productionProgress { get; private set; }

    public int workers { get; set; }

    public double workerPayRate { get; set; }

    public bool isProducing = true;

    public Driller() { }

    /// <summary>
    /// Creates an itemFactory, the factory blueprint must have ai and machinery
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="drillerBlueprint"></param>
    /// <param name="_productionItemId"></param>
    public Driller(IdentityModel owner, ItemBluePrint drillerBlueprint, int _productionItemId, SolarBody body, int _count = 1):
        base(owner, _productionItemId,body, _count)
    {
        
        structureType = StructureTypes.Driller;
        
        name = productionItemName + " Driller " + id;
        
        maxArmor = drillerBlueprint.baseArmor;
        currentArmor = maxArmor;
        int aiAmount = (int) drillerBlueprint.contstructionParts.Find(x => x.itemType == ItemType.AI).amount;
        int machineryAmount = (int) drillerBlueprint.contstructionParts.Find(x => x.itemType == ItemType.FactoryMachinery).amount;
        productionTime /= machineryAmount;

        var productionEfficiency = (productionTime * .9f + aiAmount * 1.1f) / (productionTime + aiAmount);
        productionTime /= productionEfficiency;

        workers = drillerBlueprint.workers + 10;
        workerPayRate = .00116;
    }

    public Driller(IdentityModel owner, int _productionItemId, SolarBody body, int _count = 1):
        base(owner, _productionItemId,body,_count)
    {
             
        structureType = StructureTypes.Driller;
        
        name = productionItemName + " Driller " + id;

        workers = 25;
        workerPayRate = .00116;
        owner.money -= 10000;
    }

    /// <summary>
    /// Creates items and uses items based on the elapsed time
    /// </summary>
    /// <param name="elapsedTime">time elapsed (in seconds)</param>
    public void UpdateProduction(SolarBody parentBody, double deltaTime)
    {
        if (deleteStructure)
            return;
        var price = parentBody.GetMarketPrice(productionItemId) / productionTime;
        var cost = workers * workerPayRate;
        info = "Count: " + count + "\nOn: " + isProducing.ToString()+ "\nPrice:Cost " + price + " - " + cost;
        if (isProducing &&  price > cost)
        {
            if (GameManager.instance.timeScale > 2000)
            {
                GameManager.instance.timeScale = 1;
                GameManager.instance.OpenInfoPanel(this);
            }
            
            productionProgress = deltaTime / productionTime;
            DrillResource(parentBody, productionProgress * count);
            owner.Model.money -= workerPayRate * workers * deltaTime * count;
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
                StoreCreatedItem(parentBody, amountRemoved);
                if (amount > amountRemoved)
                    isProducing = false;
                break;
            }
        }
    }
    
}

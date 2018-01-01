using UnityEngine;
using System.Collections.Generic;
using System;
using CodeControl;

public class Driller : ProductionStructure, IWorkers {

    public double productionProgress { get; private set; }

    public int workers { get; set; }

    public double workerPayRate { get; set; }

    public bool on = true;
    public bool isProducing = false;

    public Driller() { }

    /// <summary>
    /// Creates an itemFactory, the factory blueprint must have ai and machinery
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="drillerBlueprint"></param>
    /// <param name="_productionItemId"></param>
    public Driller(IdentityModel owner, ItemBlueprint drillerBlueprint, int _productionItemId, SolarBody body, int _count = 1):
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
        info = "Count: " + count + "\nOn: " + on.ToString()
            + "\nIs Producing: " + isProducing.ToString()
            + "\nPrice/Cost " + (price/cost).ToString("0.0000");
        UpdateConnectionItems();
        if (on &&  (price > cost ||
            structureConnectionIdsOut.Exists(x => !((ProductionStructure)parentBody.GetStructure(x)).storage.ContainsItem(productionItemId) ||
            (((ProductionStructure)parentBody.GetStructure(x)).storage.ContainsItem(productionItemId) &&
                ((ProductionStructure)parentBody.GetStructure(x)).storage.items.Find(b => b.id == productionItemId).amount <
                ((ProductionStructure)parentBody.GetStructure(x)).requiredItems.Find(b => b.id == productionItemId).amount * 3 *
                ((ProductionStructure)parentBody.GetStructure(x)).count))
            ))
        {
            //if (GameManager.instance.timeScale > 2000)
            //{
            //    GameManager.instance.timeScale = 1;
            //    GameManager.instance.OpenInfoPanel(this);
            //}
            
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
                    on = false;
                break;
            }
        }
    }
    
}

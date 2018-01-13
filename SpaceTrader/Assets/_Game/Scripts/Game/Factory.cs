using UnityEngine;
using System.Collections.Generic;
using System;
using CodeControl;

public class Factory : ProductionStructure,IWorkers {

    
    
    public float productionProgress { get; private set; }

    public int workers { get; set; }

    public double workerPayRate { get; set; }

    public bool isProducing;
    public bool on = true;

    public Factory() { }

    public Factory(IdentityModel owner, int _productionItemId, int count, SolarBody body): 
        base(owner,_productionItemId,body,count)
    {
        
        structureType = StructureTypes.Factory;

        workers = 30;
        workerPayRate = .00116;
        owner.money -= 10000;
    }

    public Factory(IdentityModel owner, int structureItemId, int productionItemId, List<int> solarIndex, int _count = 1):
        base(owner,productionItemId, GameManager.instance.data.getSolarBody(solarIndex), _count)
    {
        structureType = StructureTypes.Factory;

        var blueprint = GameManager.instance.data.itemsData.Model.GetItem(structureItemId);
        if (blueprint.itemType == ItemType.RawMaterial)
            deleteStructure = true;
        name = productionItemName + " " + blueprint.name + " " + id;

        maxArmor = blueprint.baseArmor;
        currentArmor = maxArmor;

        int machineryAmount = (int)blueprint.contstructionParts.Find(x => x.itemType == ItemType.FactoryMachinery).amount;
        productionTime /= machineryAmount;

        workers = 15 + blueprint.workers;
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
        var price = parentBody.GetMarketPrice(productionItemId) / productionTime;
        var cost = workers * workerPayRate;

        requiredItems.ForEach(x => cost += x.amount * x.price / GameManager.instance.data.itemsData.Model.GetItem(x.id).productionTime);
        info = "Count: " + count + "\nOn: " + on.ToString()
            + "\nIs Producing: " + isProducing.ToString()
            + "\nPrice/Cost " + (price / cost).ToString("0.0000");
        UpdateConnectionItems();
        if (on && (price > cost ||
            structureConnectionIdsOut.Exists(x => !((ProductionStructure)parentBody.GetStructure(x)).storage.ContainsItem(productionItemId) ||
            (((ProductionStructure)parentBody.GetStructure(x)).storage.ContainsItem(productionItemId) &&
                ((ProductionStructure)parentBody.GetStructure(x)).storage.items.Find(b => b.id == productionItemId).amount <
                ((ProductionStructure)parentBody.GetStructure(x)).requiredItems.Find(b => b.id == productionItemId).amount * 3 *
                ((ProductionStructure)parentBody.GetStructure(x)).count))
            ))
        {
            requiredItems.ForEach(x => {
                parentBody.RemoveBuying(x.id, id, x.amount * count);
                x.price = parentBody.GetMarketPrice(x.id);
            }
            );
            return;
        }
            
        var progress = (float)(deltaTime / productionTime);
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
                if (StoreCreatedItem(parentBody, count))
                {
                    productionProgress--;
                    
                    isProducing = false;
                    if (SearchRequiredItems(parentBody, progress))
                    {
                        isProducing = true;
                        UseRequiredItems(parentBody);
                    }
                    else
                    {
                        productionProgress = 0;
                        requiredItems.ForEach(x => x.price = GameManager.instance.data.getSolarBody(solarIndex).GetMarketPrice(x.id));
                        break;
                    }

                }
            }
        }
        else if (SearchRequiredItems(parentBody, progress))
        {
            isProducing = true;
            UseRequiredItems(parentBody);
        }     
    }
}

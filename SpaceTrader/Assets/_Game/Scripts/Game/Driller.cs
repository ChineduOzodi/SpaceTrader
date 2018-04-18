using UnityEngine;
using System.Collections.Generic;
using CodeControl;

public class Driller : ProductionStructure {

    public Driller() { }

    /// <summary>
    /// Creates an itemFactory, the factory blueprint must have ai and machinery
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="drillerBlueprintId"></param>
    /// <param name="_productionItemId"></param>
    public Driller(IdentityModel owner, string drillerBlueprintId, string _productionItemId, string referenceId, int _count = 1):
        base(owner, _productionItemId,referenceId, (Vector3d)Random.onUnitSphere * ((SolarBody)GameManager.instance.locations[referenceId]).bodyRadius * .5, _count)
    {
        
        structureType = StructureTypes.Driller;
        ItemBlueprint drillerBlueprint = GameManager.instance.data.itemsData.Model.GetItem(drillerBlueprintId);
        name = productionItemName + " Driller " + id;
        
        maxArmor = drillerBlueprint.baseArmor;
        currentArmor = maxArmor;
        int aiAmount = (int) drillerBlueprint.contstructionParts.Find(x => x.itemType == ItemType.AI).amount;
        int machineryAmount = (int) drillerBlueprint.contstructionParts.Find(x => x.itemType == ItemType.FactoryMachinery).amount;
        workAmount /= machineryAmount;

        var productionEfficiency = (workAmount * .9f + aiAmount * 1.1f) / (workAmount + aiAmount);
        workAmount /= productionEfficiency;
        blueprintId = drillerBlueprintId;
        workers = drillerBlueprint.workers * count + 10;
        workerPayRate = .00116;
    }

    public Driller(IdentityModel owner, string _productionItemId, string referenceId, int _count = 1):
        base(owner, _productionItemId, referenceId, (Vector3d)Random.onUnitSphere * ((SolarBody)GameManager.instance.locations[referenceId]).bodyRadius * .5, _count)
    {
             
        structureType = StructureTypes.Driller;
        blueprintId = GameManager.instance.data.itemsData.Model.blueprints.Find(x => x.itemType == ItemType.Driller).id;
        name = productionItemName + " Driller " + id;

        workers = 250 * count;
        workerPayRate = .00116;

        owner.PayConstruction(100000);
    }

    /// <summary>
    /// Creates items and uses items based on the elapsed time
    /// </summary>
    /// <param name="elapsedTime">time elapsed (in seconds)</param>
    public override void Update()
    {
        base.Update();

        if (deleteStructure)
            return;

        //var price = parentBody.GetMarketPrice(productionItemId) / productionTime;
        //var cost = workers * workerPayRate;
        Info = "Count: " + count + "\nOn: " + on.ToString()
            + "\nIs Producing: " + isProducing.ToString()
            + "\nResources Mined: " + Units.ReadItem(productionProgress)+ "u";
        
        if (on)
        {
            isProducing = true;
            //if (GameManager.instance.timeScale > 2000)
            //{
            //    GameManager.instance.timeScale = 1;
            //    GameManager.instance.OpenInfoPanel(this);
            //}
            var productionDone = (float)(deltaTime * ProductionRateActual);
            productionProgress += productionDone;

            DrillResource(productionDone);
            owner.Model.PayWorkers(workerPayRate * workers * deltaTime);
        }
        else
        {
            isProducing = false;
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
    private void DrillResource(double amount)
    {
        var resource = GameManager.instance.data.rawResources.Model.GetResource(productionItemId);

        foreach (RawResource raw in ReferenceBody.rawResources)
        {
            if (raw.id == resource.id)
            {

                double amountRemoved = raw.RemoveAmount(amount);
                StoreCreatedItem(amountRemoved);
                if (amount > amountRemoved)
                    on = false;
                break;
            }
        }
    }
    
}

using UnityEngine;
using System.Collections.Generic;
using CodeControl;

public class Factory : ProductionStructure {

    

    public Factory() { }

    public Factory(IdentityModel owner, string _productionItemId, int count, string referenceId): 
        base(owner,_productionItemId,referenceId, (Vector3d)Random.onUnitSphere * ((SolarBody)GameManager.instance.locations[referenceId]).bodyRadius * .5,count)
    {
        
        structureType = StructureTypes.Factory;

        workers = 300 * count;
        workerPayRate = .00116;
        owner.PayConstruction(100000);
    }

    public Factory(IdentityModel owner, string structureItemId, string productionItemId, string _referenceId, int _count = 1):
        base(owner,productionItemId, _referenceId, (Vector3d)Random.onUnitSphere * ((SolarBody)GameManager.instance.locations[_referenceId]).bodyRadius * .5, _count)
    {
        structureType = StructureTypes.Factory;
        blueprintId = GameManager.instance.data.itemsData.Model.blueprints.Find(x => x.itemType == ItemType.Factory).id;
        var blueprint = GameManager.instance.data.itemsData.Model.GetItem(structureItemId);
        if (blueprint.itemType == ItemType.RawMaterial)
            deleteStructure = true;
        name = productionItemName + " " + blueprint.name + " " + id;

        maxArmor = blueprint.baseArmor;
        currentArmor = maxArmor;

        int machineryAmount = (int)blueprint.contstructionParts.Find(x => x.itemType == ItemType.FactoryMachinery).amount;
        workAmount /= machineryAmount;

        workers = 15 + blueprint.workers * count;
        workerPayRate = .00116;
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

        //requiredItems.ForEach(x => cost += x.amount * x.price / GameManager.instance.data.itemsData.Model.GetItem(x.id).productionTime);
        Info = "Count: " + count + "\nOn: " + on.ToString()
            + "\nIs Producing: " + isProducing.ToString();
            //+ "\nPrice/Cost " + (price / cost).ToString("0.0000");

        var progress = (float)(deltaTime / ProductionTime);
        if (isProducing)
        {
            productionProgress += progress;
            owner.Model.PayWorkers(workerPayRate * workers * deltaTime);
            while (productionProgress > 1)
            {

                
                //productionProgress -= Mathd.Floor(productionProgress);

                if (SearchRequiredItems())
                {
                    isProducing = true;
                    UseRequiredItems();
                    productionProgress--;
                }
                else
                {
                    productionProgress = 0;
                    isProducing = false;
                    //requiredItems.ForEach(x => x.price = ReferenceBody.GetMarketPrice(x.id));
                }
            }
        }
        else if (SearchRequiredItems())
        {
            isProducing = true;
        }
    }
    }

using UnityEngine;
using System.Collections.Generic;
using System;
using CodeControl;

public class BuildStructure : ProductionStructure {

    public StructureTypes buildStructureType { get; set; }

    public string targetStructureBlueprintId;

    public BuildStructure() { }
    public BuildStructure(IdentityModel owner, string referenceId, Vector3d localPosisiton) :
        base(owner, referenceId, localPosisiton)
    {
    }

    public void BuildFactory( string factoryBlueprintId, string _productionItemId, int _count = 1)  
    {
        count = _count;

        ItemBlueprint product = GameManager.instance.data.itemsData.Model.GetItem(factoryBlueprintId);
        name = "Building ---> " + product.name + " | " + id;
        buildStructureType = StructureTypes.Factory;
        targetStructureBlueprintId = factoryBlueprintId;
        productionItemId = _productionItemId;
        requiredItems = product.contstructionParts;

        maxArmor = product.baseArmor;
        currentArmor = 0;

        workAmount = product.workAmount;
        workers = product.workers * count;
        workerPayRate = .00116;
    }

    public void BuildDriller(string drillerBlueprintId, string resourceItemId, int _count = 1)
    {
        count = _count;

        ItemBlueprint product = GameManager.instance.data.itemsData.Model.GetItem(drillerBlueprintId);
        name = "Building ---> " + product.name + " | " + id;
        buildStructureType = StructureTypes.Driller;
        targetStructureBlueprintId = drillerBlueprintId;
        productionItemId = resourceItemId;
        requiredItems = product.contstructionParts;

        maxArmor = product.baseArmor;
        currentArmor = 0;

        workAmount = product.workAmount;
        workers = product.workers * count;
        workerPayRate = .00116;
    }

    public override void Update()
    {
        base.Update();
        
        if (deleteStructure)
            return;

        while (true)
        {
            if (productionState == ProductionState.Active)
            {
                var productionDone = (float)(deltaTime * ProductionRateActual);
                productionProgress += productionDone;
                if (productionProgress > 1)
                {
                    if (structureType == StructureTypes.Factory)
                    {
                        new Factory(owner.Model, targetStructureBlueprintId,productionItemId, referenceId, count);
                    }
                    else if (structureType == StructureTypes.Driller)
                    {
                        new Driller(owner.Model, targetStructureBlueprintId, productionItemId, referenceId, count);
                    }
                    deleteStructure = true; //TODO: Create proper structure deletion
                    //GameManager.instance.data.getSolarBody(solarIndex).deleteStructure = true;
                    productionProgress = 1;
                    break;
                }
                else
                {
                    break;
                }
            }
            else if (productionState == ProductionState.LackMaterials)
            {
                //SearchContracts();
                //Check to see if has all 
            }
            else
            {
                break;
            }
        }

    }

    /// <summary>
    /// Makes sure that the factory is at the maximum production rate by finding contracts with known companies for needed materials
    /// </summary>
    public override void SearchContracts(string itemId, double itemAmount)
    {
        //Find a contract
        string contractId = "";

        contractId = owner.Model.Government.GetCheapestContractId(itemId, id, itemAmount);

        if (contractId != "" && contractId != null)
        {
            supplierContractIds.Add(contractId);
            Contract contract = GameManager.instance.contracts[contractId];

            //Set wanted conditions;
            contract.destinationId = id;
            contract.contractState = ContractState.Sent;
            contract.reknewable = false;
            contract.itemAmount = itemAmount;
        }
    }
}

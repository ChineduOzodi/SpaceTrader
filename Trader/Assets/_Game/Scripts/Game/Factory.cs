using UnityEngine;
using System.Collections.Generic;
using System;
using CodeControl;

public class Factory : Structure {

    public int productionItemId { get; private set; }
    /// <summary>
    /// Time to complete one cycle in days.
    /// </summary>
    public float produtionTime { get; private set; }
    public float productionProgress { get; private set; }

    

    public bool isProducing;

    public Factory() { structureType = StructureTypes.Factory; }

    /// <summary>
    /// Creates an itemFactory, the factory blueprint must have ai and machinery
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="factoryBlueprint"></param>
    /// <param name="_productionItemId"></param>
    public Factory(IdentityModel owner, ItemBluePrint factoryBlueprint, int _productionItemId)
    {
        this.owner = new ModelRef<IdentityModel>(owner);

        structureType = StructureTypes.Factory;
        
        productionItemId = _productionItemId;

        maxArmor = factoryBlueprint.baseArmor;
        currentArmor = maxArmor;
        int aiAmount = (int) factoryBlueprint.contstructionParts.Find(x => x.itemType == ItemType.AI).amount;
        int machineryAmount = (int) factoryBlueprint.contstructionParts.Find(x => x.itemType == ItemType.FactoryMachinery).amount;
        produtionTime = factoryBlueprint.productionTime / machineryAmount;

        var productionEfficiency = (produtionTime * .9f + aiAmount * 1.1f) / (produtionTime + aiAmount);
        produtionTime /= productionEfficiency;

        workers = factoryBlueprint.workers;
    }

    /// <summary>
    /// Creates items and uses items based on the elapsed time
    /// </summary>
    /// <param name="elapsedTime">time elapsed (in seconds)</param>
    public void UpdateProduction(SolarBody parentBody, float daysPassed)
    {
        if (isProducing)
        {
            productionProgress += daysPassed / produtionTime;
            if (productionProgress > 1)
            {
                
                
            }
        }
        else if (RequiredComponentsAvailable(parentBody))
        {
            isProducing = true;
            useRequiredItems(parentBody);
        }

    }

    private bool RequiredComponentsAvailable(SolarBody parentBody)
    {
        var requiredItems = GameManager.instance.data.items.Model.GetItem(productionItemId).contstructionParts;

        foreach ( Item item in requiredItems)
        {
            var found = false;
            foreach(GroundStorage groundStruct in parentBody.groundStructures)
            {
                if (groundStruct.structureType == StructureTypes.RawResourceStorage &&
                    groundStruct.owner == owner &&
                    groundStruct.items.Find(x => x.itemType == item.itemType && x.amount >= item.amount).name != "")
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                return false;
        }
        return true;
    }
    private void useRequiredItems(SolarBody parentBody)
    {
        var requiredItems = GameManager.instance.data.items.Model.GetItem(productionItemId).contstructionParts;

        foreach (Item item in requiredItems)
        {
            var found = false;
            foreach (GroundStorage groundStruct in parentBody.groundStructures)
            {
                if (groundStruct.structureType == StructureTypes.ComponentStorage &&
                    groundStruct.owner == owner &&
                    groundStruct.items.Contains(item))
                {
                    groundStruct.RemoveItem(item);
                    found = true;
                    break;
                }
            }
            if (!found)
                throw new Exception("Item is not found in correct amount");
        }
    }
    private void StoreCreatedItem(SolarBody parentBody)
    {
        var found = false;
        Item item = new Item(productionItemId, 1);
        foreach (GroundStorage groundStruct in parentBody.groundStructures)
        {
            if (groundStruct.structureType == StructureTypes.ComponentStorage &&
                groundStruct.owner == owner &&
                groundStruct.CanAddItem(item))
            {
                groundStruct.AddItem(item);
                found = true;
                break;
            }
        }
        if (!found)
            throw new Exception("Component storage not found or nearly full");
    }
}

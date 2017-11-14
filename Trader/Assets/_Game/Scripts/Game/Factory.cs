using UnityEngine;
using System.Collections.Generic;
using System;
using CodeControl;

public class ComponentFactory : GroundStructure {

    public ConstructionComponent productionItem;
    /// <summary>
    /// Time to complete one cycle in days.
    /// </summary>
    public float produtionTime { get; private set; }
    public float productionProgress { get; private set; }

    public int workers;

    public bool isProducing;

    public ComponentFactory() { }

    public ComponentFactory(CompanyModel owner, FactoryBlueprintModel blueprint, ConstructionComponent item)
    {
        this.owner = new ModelRef<CompanyModel>(owner);

        groundStructureType = GroundStructureType.Factory;

        productionItem = item;

        maxArmor = blueprint.structureComponent.baseArmor * blueprint.structureComponent.amount;
        currentArmor = maxArmor;

        var productionEfficiency = (produtionTime * .9f + blueprint.aiComponent.amount * 1.1f) / (produtionTime + blueprint.aiComponent.amount);
        produtionTime = 100/blueprint.machinaryComponent.amount * productionEfficiency;
        

        workers = (int) (blueprint.machinaryComponent.workers * produtionTime + blueprint.aiComponent.amount * blueprint.aiComponent.workers);
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
            UseRawResources(parentBody);
        }

    }

    private bool RequiredComponentsAvailable(SolarBody parentBody)
    {
        var requiredResouces = productionItem.rawResources.GetResources();
        foreach ( KeyValuePair<RawResourceType,float> raw in requiredResouces)
        {
            var found = false;
            foreach(RawResourcesGroundStorage groundStruct in parentBody.groundStructures)
            {
                if (groundStruct.groundStructureType == GroundStructureType.RawResourceStorage &&
                    groundStruct.owner == owner &&
                    groundStruct.rawResources.FindAmount(raw.Key) >= raw.Value)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                return found;
        }
        return true;
    }
    private void UseRawResources(SolarBody parentBody)
    {
        var requiredResouces = productionItem.rawResources.GetResources();
        foreach (KeyValuePair<RawResourceType, float> raw in requiredResouces)
        {
            var found = false;
            foreach (RawResourcesGroundStorage groundStruct in parentBody.groundStructures)
            {
                if (groundStruct.groundStructureType == GroundStructureType.ComponentStorage &&
                    groundStruct.owner == owner &&
                    groundStruct.rawResources.FindAmount(raw.Key) >= raw.Value)
                {
                    groundStruct.rawResources.RemoveAmount(raw.Key, raw.Value);
                    found = true;
                    break;
                }
            }
            if (!found)
                throw new Exception("Raw resource not found in correct amount");
        }
    }
    private void StoreComponents(SolarBody parentBody)
    {
        var found = false;
        foreach (ComponentsGroundStorage groundStruct in parentBody.groundStructures)
        {
            if (groundStruct.groundStructureType == GroundStructureType.ComponentStorage &&
                groundStruct.owner == owner &&
                groundStruct.EnoughSpace(productionItem.componentType, productionItem.amount))
            {
                groundStruct.constructionComponents.AddAmount(productionItem.componentType, productionItem.amount);
                found = true;
                break;
            }
        }
        if (!found)
            throw new Exception("Component storage not found or nearly full");
    }
}

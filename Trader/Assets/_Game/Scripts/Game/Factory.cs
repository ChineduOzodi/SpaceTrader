using UnityEngine;
using System.Collections;
using System;
using CodeControl;

public class Factory : GroundStructure {

    public ConstructionComponent productionItem;
    public double productionAmount { get; private set; }
    public double productionEfficiency { get; private set; }

    public double workers;

    public bool isProducing;

    public Factory() { }

    public Factory(CompanyModel owner, FactoryBlueprintModel blueprint, ConstructionComponent item)
    {
        this.owner = new ModelRef<CompanyModel>(owner);

        groundStructureType = GroundStructureType.Factory;

        productionItem = item;

        maxArmor = blueprint.structureComponent.GetArmor();
        currentArmor = maxArmor;

        productionAmount = blueprint.machinaryComponent.amount;
        productionEfficiency = (productionAmount * .9 + blueprint.aiComponent.amount * 1.1) / (productionAmount + blueprint.aiComponent.amount);

        workers = blueprint.machinaryComponent.workers * productionAmount + blueprint.aiComponent.amount * blueprint.aiComponent.workers;
    }

    /// <summary>
    /// Creates items and uses items based on the elapsed time
    /// </summary>
    /// <param name="elapsedTime">time elapsed (in seconds)</param>
    public void UpdateProduction(SolarBody parentBody)
    {
        if (isProducing)
        {
            if (RequiredComponentsAvailable(parentBody))
            {

            }
        }

    }

    private bool RequiredComponentsAvailable(SolarBody parentBody)
    {
        foreach (RawResource raw in productionItem.rawResources)
        {
            foreach(int[] location in parentBody.groundStructureLocations)
            {
                var groundStruct = parentBody.planetTiles[location[0]].structures[location[1]];
                if (groundStruct.groundStructureType == GroundStructureType.Storage)
                {
                    if (groundStruct.)
                }
            }
        }
    }
}

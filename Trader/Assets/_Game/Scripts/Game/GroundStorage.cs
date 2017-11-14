using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawResourcesGroundStorage : GroundStructure {

    public float totalStorageAmount;

    public RawResources rawResources;

    public RawResourcesGroundStorage() { }

    public RawResourcesGroundStorage(StorageBlueprintModel blueprint)
    {

    }

    public RawResourcesGroundStorage(StorageBlueprintModel blueprint, List<RawResource> rawResources, List<ConstructionComponent> components)
    {

    }
}

public class ComponentsGroundStorage : GroundStructure
{

    public float totalStorageAmount;
    public float currentStorageAmount;

    public ConstructionComponents constructionComponents;

    public ComponentsGroundStorage() { }

    public ComponentsGroundStorage(StorageBlueprintModel blueprint)
    {

    }

    public ComponentsGroundStorage(StorageBlueprintModel blueprint, List<RawResource> rawResources, List<ConstructionComponent> components)
    {

    }

    public bool AddComponent(ConstructionComponentType comp, int amount)
    {
        if (currentStorageAmount + amount > totalStorageAmount)
            return false;
        constructionComponents.AddAmount(comp, amount);
        currentStorageAmount += amount;
        return true;
    }

    public bool EnoughSpace(ConstructionComponentType comp, int amount)
    {
        return currentStorageAmount + amount < totalStorageAmount;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundStorage : GroundStructure {

    public float totalStorageAmount;

    public List<RawResource> rawResources;

    public List<ConstructionComponent> components;

    public GroundStorage() { }

    public GroundStorage(StorageBlueprintModel blueprint)
    {

    }

    public GroundStorage(StorageBlueprintModel blueprint, List<RawResource> rawResources, List<ConstructionComponent> components)
    {

    }
}

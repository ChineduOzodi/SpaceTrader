using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistributionCenter : ProductionStructure {

    public DistributionCenter() { }

    public DistributionCenter(IdentityModel owner, ItemBlueprint distributionCenterBlueprint, SolarBody body, int _count = 1):
        this(owner,body,_count)
    {
        structureType = StructureTypes.DistributionCenter;
    }

    public DistributionCenter(IdentityModel owner, SolarBody body, int _count = 1):
        base(owner,"None", body.id, (Vector3d)Random.onUnitSphere * ((SolarBody)GameManager.instance.locations[body.id]).bodyRadius * .5)
    {
        structureType = StructureTypes.DistributionCenter;
        //Look to see if similar structure already exists
        foreach (Structure structure in body.structures)
        {
            if (structure.StructureType == StructureTypes.DistributionCenter)
            {
                //If the same structure is found, check ownership, and productionItemId
                if (structure.owner.Model == owner)
                {
                    //If found, increase count
                    structure.Count++;
                    return;
                }
            }
        }
        blueprintId = GameManager.instance.data.itemsData.Model.blueprints.Find(x => x.itemType == ItemType.DistributionCenter).id;

        //Add required items
        requiredItems = new List<Item>();
        foreach (ItemBlueprint item in GameManager.instance.data.itemsData.Model.blueprints)
        {
            requiredItems.Add(new Item(item.id, 1));
        }

        workers = 1;
        workAmount = 1 * 24 * 30; //Optimal Rate of 1 a month

        owner.AddStructure(id, body.id);
        //solarIndex = body.solarIndex; TODO: Fix solar index
        count = _count;
        body.structures.Add(this);

        name = structureType.ToString() + " " + id;

        //Set IStructure Properties
        maxArmor = 100;
        currentArmor = maxArmor;

        UpdateConnectionItems();
    }

    public override void Update()
    {
        base.Update();

        if (deleteStructure)
            return;

        Info = "Count: " + count + "\nItemCount: " + itemsStorage.Count;

        foreach (Item item in itemsStorage)
        {
            //Delete items?
        }

    }
}

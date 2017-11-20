using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundStorage : Structure {

    public double totalStorageAmount;
    public double currentStorageAmount;

    public ItemsList storage = new ItemsList();

    public GroundStorage() { }

    public GroundStorage(ItemBluePrint blueprint, SolarBody body)
    {
        id = GameManager.instance.data.id++;
        solarIndex = body.solarIndex;
        body.groundStructures.Add(this);
    }

    public GroundStorage(ItemBluePrint blueprint, ItemsList _items, SolarBody body)
    {
        storage = _items;
        id = GameManager.instance.data.id++;
        solarIndex = body.solarIndex;
        body.groundStructures.Add(this);
    }

    public GroundStorage(IdentityModel owner, SolarBody body)
    {
        this.owner = new ModelRef<IdentityModel>(owner);
        owner.AddSolarBodyWithStructure(body);
        solarIndex = body.solarIndex;
        body.groundStructures.Add(this);
        structureType = StructureTypes.GroundStorage;
        id = GameManager.instance.data.id++;
        name = structureType.ToString() + " " + id;
        maxArmor = 1000;
        currentArmor = maxArmor;
        totalStorageAmount = 10000;

        workers = 5;
        owner.money -= 1000;
    }

    public bool AddItem(Item item)
    {
        if (currentStorageAmount + item.amount > totalStorageAmount)
            return false;
        storage.AddItem(item);
        currentStorageAmount += item.amount;
        return true;
    }

    public bool RemoveItem(Item item)
    {
        if (storage.RemoveItem(item))
        {
            currentStorageAmount -= item.amount;
            return true;
        }
        return false;
    }

    public bool ContainsItem(Item item)
    {
        return storage.ContainsItem(item);
    }

    public bool CanAddItem(Item item)
    {
        return currentStorageAmount + item.amount < totalStorageAmount;
    }
}

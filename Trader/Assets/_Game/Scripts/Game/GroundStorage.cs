using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundStorage : IStructure {

    public double totalStorageAmount;
    public double currentStorageAmount;

    public ItemsList storage = new ItemsList();

    public StructureTypes structureType { get; set; }

    public string name { get; set; }
    public string info { get; set; }
    public ModelRef<IdentityModel> owner { get; set; }
    public int managerId { get; set; }
    public float maxArmor { get; set; }
    public float currentArmor { get; set; }
    public int id { get; set; }
    public Dated dateCreated { get; set; }
    public Dated lastUpdated { get; set; }
    public bool deleteStructure { get; set; }
    public int count { get; set; }

    public Vector2d galaxyPosition
    {
        get
        {
            if (solarIndex.Count == 3)
            {
                return GameManager.instance.data.stars[solarIndex[0]].solar.satelites[solarIndex[1]].satelites[solarIndex[2]].galaxyPosition;
            }
            else if (solarIndex.Count == 2)
            {
                return GameManager.instance.data.stars[solarIndex[0]].solar.satelites[solarIndex[1]].galaxyPosition;
            }
            else
            {
                throw new System.Exception("BuildStructure " + name + " solarIndex count incorrect: " + solarIndex.Count);
            }
        }

        set
        {
            throw new System.Exception("Can't set galaxyPosition, set solarIndex instead");
        }
    }

    public List<int> solarIndex { get; set; }
    public int structureId { get; set; }
    public int shipId { get; set; }

    public GroundStorage() { }

    public GroundStorage(IdentityModel owner, ItemBlueprint blueprint, SolarBody body, int _count = 1):
        this(owner,body,_count)
    {
        
    }

    public GroundStorage(IdentityModel owner, ItemBlueprint blueprint, ItemsList _items, SolarBody body, int _count = 1):
        this(owner, blueprint,body,_count)
    {
        storage = _items;
    }

    public GroundStorage(IdentityModel owner, SolarBody body, int _count = 1)
    {
        //Look to see if similar structure already exists
        foreach (IStructure structure in body.structures)
        {
            if (structure.structureType == StructureTypes.GroundStorage)
            {
                //If the same structure is found, check ownership, and productionItemId
                if (structure.owner.Model == owner)
                {
                    //If found, increase count
                    structure.count++;
                    return;
                }
            }
        }

        this.owner = new ModelRef<IdentityModel>(owner);
        owner.AddSolarBodyWithStructure(body);
        solarIndex = body.solarIndex;
        count = _count;
        body.structures.Add(this);
        structureType = StructureTypes.GroundStorage;
        id = GameManager.instance.data.id++;
        name = structureType.ToString() + " " + id;

        //Set IStructure Properties
        maxArmor = 100;
        currentArmor = maxArmor;
        dateCreated = new Dated(GameManager.instance.data.date.time);
        lastUpdated = new Dated(GameManager.instance.data.date.time);

        totalStorageAmount = 10000;
        owner.money -= 1000;
    }

    public void UpdateProduction(SolarBody parentBody, double deltaTime)
    {
        if (deleteStructure)
            return;
        info = "Count: " + count + "\nItemCount: " + storage.items.Count;
        foreach (Item item in storage.items)
        {
            item.price -= item.price * GameManager.instance.marketPriceMod * deltaTime;
            if (item.price < .1)
            {
                item.price = .1f;
            }
            parentBody.SetSelling(item);
        }

    }


    public bool AddItem(Item item)
    {
        if (currentStorageAmount + item.amount > totalStorageAmount * count)
            return false;
        item.owner.Model = owner.Model;
        storage.AddItem(item);
        currentStorageAmount += item.amount;
        return true;
    }

    public bool RemoveItem(Item item)
    {
        if (storage.UseItem(item))
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

    public Item UseAsMuchItem(Item item)
    {
        double itemAmount = item.amount;
        Item unusedItem = storage.UseAsMuchItem(item);
        currentStorageAmount -= (itemAmount - unusedItem.amount);
        if (currentStorageAmount < 0)
        {
            currentStorageAmount = 0;
            storage.items.ForEach(x => currentStorageAmount += x.amount);
            if (currentStorageAmount < 0)
            {
                Debug.Log("current storage below 0: " + currentStorageAmount + " + " + (itemAmount - unusedItem.amount));
                currentStorageAmount = 0;
            }
                
        }
            
        return unusedItem;
    }

    public bool CanAddItem(Item item)
    {
        return currentStorageAmount + item.amount < totalStorageAmount * count;
    }
}

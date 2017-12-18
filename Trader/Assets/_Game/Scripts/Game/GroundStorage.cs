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

    public GroundStorage(ItemBluePrint blueprint, SolarBody body)
    {
        id = GameManager.instance.data.id++;
        solarIndex = body.solarIndex;
        structureId = -1;
        shipId = -1;
        body.groundStructures.Add(this);
        //Implement IStructure
        maxArmor = 1000;
        currentArmor = maxArmor;
        dateCreated = new Dated(GameManager.instance.data.date.time);
        lastUpdated = new Dated(GameManager.instance.data.date.time);
    }

    public GroundStorage(ItemBluePrint blueprint, ItemsList _items, SolarBody body)
    {
        storage = _items;
        id = GameManager.instance.data.id++;
        solarIndex = body.solarIndex;
        structureId = -1;
        shipId = -1;
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
        if (currentStorageAmount + item.amount > totalStorageAmount)
            return false;
        item.owner.Model = owner.Model;
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

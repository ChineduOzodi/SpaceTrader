using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using System.Xml.Serialization;

public class Structure: PositionEntity, IItemStorage {

    public StructureTypes StructureType { get; set; }

    public string Info { get; set; }
    public int Count { get; set; }

    public string blueprintId;

    public ModelRef<IdentityModel> owner;
    public string managerId { get; set; }

    public float maxArmor { get; set; }
    public float currentArmor { get; set; }

    public Dated dateCreated { get; set; }//new Dated(GameManager.instance.data.date.time);
    public Dated lastUpdated { get; set; }//new Dated(GameManager.instance.data.date.time);

    public List<string> incomingShips = new List<string>();
    public bool deleteStructure { get; set; }

    public List<Item> itemsStorage { get; set; }
    public double itemCapacity { get; set; }

    public double deltaTime = 1;

    public double SOI = 10;

    public Structure() { }

    public Structure(IdentityModel _owner, string _referenceId, Vector3d _localPosition) :
        base(_referenceId, _localPosition)
    {
        if (_referenceId != "")
        {
            GameManager.instance.locations[_referenceId].structureIds.Add(id);
        }
        owner = new ModelRef<IdentityModel>(_owner);
        _owner.AddKnownSolarBodyId(referenceId);
        dateCreated = new Dated(GameManager.instance.data.date.time);
        lastUpdated = new Dated(GameManager.instance.data.date.time);
        itemsStorage = new List<Item>();
    }

    public virtual void Update()
    {
        deltaTime = GameManager.instance.data.date.time - lastUpdated.time;
        lastUpdated.AddTime(deltaTime);
    }
    public bool AddItem(string itemId, string destinationId, double amount)
    {

        int itemIndex = -1;

        itemIndex = itemsStorage.FindIndex(x => x.id == itemId && x.destinationId == destinationId);

        if (itemIndex >= 0)
        {
            itemsStorage[itemIndex].AddAmount(amount);
        }
        else
        {
            itemsStorage.Add(new Item(itemId,amount,id,destinationId));
        }
        return true;
    }
    public bool AddItem(Item item)
    {
        return AddItem(item.id, item.destinationId, item.amount);
    }

    public bool UseItem(string itemId, double amount)
    {
        int itemIndex = -1;
        itemIndex = itemsStorage.FindIndex(x => x.id == itemId);

        if (itemIndex >= 0)
        {
            if (itemsStorage[itemIndex].amount < amount)
            {
                return false;
            }
            itemsStorage[itemIndex].RemoveAmount(amount);
            if (itemsStorage[itemIndex].amount == 0)
            {
                itemsStorage.RemoveAt(itemIndex);
            }
            return true;
        }
        return false;
    }
    /// <summary>
    /// Uses as much of the item amount as it can from the storage, returns the left over item amount that was not used.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public double UseAsMuchItem(string itemId, double amount)
    {
        int itemIndex = -1;
        itemIndex = itemsStorage.FindIndex(x => x.id == itemId);

        if (itemIndex >= 0)
        {
            if (itemsStorage[itemIndex].amount <= amount)
            {
                double remaining = amount - itemsStorage[itemIndex].amount;
                itemsStorage[itemIndex].RemoveAmount(itemsStorage[itemIndex].amount);
                itemsStorage.RemoveAt(itemIndex);
                return remaining;
            }
            else
            {
                itemsStorage[itemIndex].RemoveAmount(amount);
                return 0;
            }
        }
        throw new System.Exception("Could not find item " + itemId);
    }

    public bool ContainsItem(string itemId, double amount)
    {
        int itemIndex = -1;
        itemIndex = itemsStorage.FindIndex(x => x.id == itemId && x.amount >= amount);

        return itemIndex != -1;
    }

    public bool ContainsItem(string itemId)
    {
        return itemsStorage.Find(x => x.id == itemId) != null;
    }
    /// <summary>
    /// Finds item with the same id stored in ItemStorage.
    /// </summary>
    /// <param name="item">item to look for</param>
    /// <returns>item that was found, or null</returns>
    public Item Find(Item item)
    {
        return itemsStorage.Find(x => x.id == item.id);
    }
    public Item Find(string itemId)
    {
        return itemsStorage.Find(x => x.id == itemId);
    }
}

public enum StructureTypes
{
    Factory,
    Driller,
    DistributionCenter,
    LivingQuarters,
    SpaceStation,
    Ship,
    BuildStructure
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundStorage : Structure {

    public float totalStorageAmount;
    public float currentStorageAmount;

    public List<Item> items;

    public GroundStorage() { }

    public GroundStorage(ItemBluePrint blueprint)
    {

    }

    public GroundStorage(ItemBluePrint blueprint, List<Item> _items)
    {
        items = _items;
    }

    public bool AddItem(Item item)
    {
        if (currentStorageAmount + item.amount > totalStorageAmount)
            return false;
        int itemIndex = items.FindIndex(x => x.id == item.id);
        if (itemIndex >= 0)
        {
            items[itemIndex] += item.amount;
        }
        else
        {
            items.Add(item);
        }
        currentStorageAmount += item.amount;
        return true;
    }

    public bool RemoveItem(Item item)
    {
        int itemIndex = items.FindIndex(x => x.id == item.id);
        if (itemIndex >= 0)
        {
            if (items[itemIndex].amount < item.amount)
            {
                return false;
            }
            items[itemIndex] -= item.amount;
            if (items[itemIndex].amount == 0)
            {
                items.RemoveAt(itemIndex);
            }
            currentStorageAmount -= item.amount;
            return true;
        }
        return false;
    }

    public bool ContainsItem(Item item)
    {
        return items.Find(x => x.id == item.id && x.amount >= item.amount).name != "";
    }

    public bool CanAddItem(Item item)
    {
        return currentStorageAmount + item.amount < totalStorageAmount;
    }
}

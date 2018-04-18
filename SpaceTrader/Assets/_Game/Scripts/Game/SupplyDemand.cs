using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyDemand {

    public string name
    {
        get { return GameManager.instance.data.itemsData.Model.GetItem(id).name; }
    }
    public ItemType itemType
    {
        get { return GameManager.instance.data.itemsData.Model.GetItem(id).itemType; }
    }
    public string id;
    public double itemSupply;
    public double totalItemAmount;
    public double totalItemPrice;
    public double itemDemand;
    public int contractsCount;
    public int factoryCount;
    public double marketPrice
    {
        get
        {
            if (totalItemAmount == 0)
            {
                return 0;
            }
            else
            {
                return totalItemPrice / totalItemAmount;
            }
        }
    }

    public SupplyDemand() { }

    public SupplyDemand(string itemId, double supply, double demand)
    {
        this.id = itemId;
        itemSupply = supply;
        itemDemand = demand;
    }

    public SupplyDemand(string itemId, double supply, double demand, double itemAmount, double totalItemPrice):
        this(itemId,supply,demand)
    {
        this.totalItemAmount += itemAmount;
        this.totalItemPrice += totalItemPrice;
    }

    public void Reset()
    {
        itemSupply = 0;
        itemDemand = 0;
        this.totalItemAmount = 0;
        this.totalItemPrice = 0;
        contractsCount = 0;
        factoryCount = 0;
    }
}

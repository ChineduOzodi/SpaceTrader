using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyDemand {

    public string itemName { get; set; }
	public int itemId { get; set; }
    public double itemSupply { get; set; }
    public double averageItemBuyPrice { get; set; }
    public double itemDemand { get; set; }
    public double averageItemSellPrice { get; set; }
    public double marketPrice = 0;

    public SupplyDemand() { }

    public SupplyDemand(string name, int itemId, double estimatedValue, double supply, double demand)
    {
        itemName = name;
        this.itemId = itemId;
        itemSupply = supply;
        itemDemand = demand;
        marketPrice = estimatedValue;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyItemTradeAction : GoapAction {

    private bool itemBought = false;
    private IPositionEntity item; // where the item is located
    private IdentityModel owner;
    private Ship ship;

    private float startTime = 0;
    public float workDuration = 30; // seconds

    public BuyItemTradeAction(Ship _ship)
    {
        addPrecondition("hasStorage", true); // we need storage for items
        addPrecondition("hasTradeItems", false); // if we have items we don't want more
        addEffect("hasTradeItems", true);

        ship = _ship;
        owner = _ship.owner.Model;
        
    }


    public override void reset()
    {
        itemBought = false;
        item = null;
        startTime = 0;
    }

    public override bool isDone()
    {
        return itemBought;
    }

    public override bool requiresInRange()
    {
        return true; // yes we need to be near a chopping block
    }

    public override bool checkProceduralPrecondition(IPositionEntity agent)
    {
        // find the nearest station to buy item to be sold at a profit
      

        IPositionEntity profitableItem = null;

        foreach (SolarModel solar in owner.knownSolars)
        {
            foreach (Item item in solar.sellList.items)
            {
                if (profitableItem == null && CalculateProfit(item, ship) > 0)
                {
                    // first one, so choose it for now
                    profitableItem = item;
                }
                else if (CalculateProfit(item, ship) > 0)
                {
                    profitableItem = item;
                }
            }
            
        }
        if (profitableItem == null)
            return false;

        item = profitableItem;
        target = item;

        return profitableItem != null;
    }

    public override bool perform(IPositionEntity agent)
    {
        if (startTime == 0)
            startTime = Time.time;

        if (Time.time - startTime > workDuration)
        {
            // finished chopping
            //BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));
            //backpack.numFirewood += 5;
            //chopped = true;
            //ToolComponent tool = backpack.tool.GetComponent(typeof(ToolComponent)) as ToolComponent;
            //tool.use(0.34f);
            //if (tool.destroyed()) {
            //	Destroy(backpack.tool);
            //	backpack.tool = null;
            //}
        }
        return true;
    }

    private double CalculateProfit(Item item, Ship ship)
    {
        double fuelMarketPrice = owner.knownSolars[0].GetMarketPrice(ship.fuel.id);
        double itemAmount = item.amount;
        if (ship.itemCapacity < itemAmount)
            itemAmount = ship.itemCapacity;

        return item.price * itemAmount - (item.galaxyPosition - ship.galaxyPosition).magnitude *  fuelMarketPrice / (ship.speed * ship.fuelEfficiency);
    }
}

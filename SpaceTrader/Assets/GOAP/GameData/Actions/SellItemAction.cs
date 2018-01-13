using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellItemAction : GoapAction {

    private bool itemSold = false;
    private IPositionEntity item; // where the item is located
    private IdentityModel owner;
    private Ship ship;

    private float startTime = 0;
    public float workDuration = 30; // seconds

    public SellItemAction(Ship _ship)
    {
        addPrecondition("hasTradeItems", true); // if we have items we don't want more
        addEffect("hasTradeItems", false);
        addEffect("itemsSold", true);
        owner = _ship.owner.Model;
        ship = _ship;
    }


    public override void reset()
    {
        itemSold = false;
        item = null;
        startTime = 0;
    }

    public override bool isDone()
    {
        return itemSold;
    }

    public override bool requiresInRange()
    {
        return true; // yes we need to be near a chopping block
    }

    public override bool checkProceduralPrecondition(IPositionEntity agent)
    {
        // find the nearest station to buy item to be sold at a profit


        IPositionEntity profitableSell = null;

        foreach (SolarModel solar in owner.knownSolars)
        {
            foreach (Item item in solar.buyList.items)
            {
                if (profitableSell == null && CalculateProfit(item, ship) > 0)
                {
                    // first one, so choose it for now
                    profitableSell = item;
                }
                else if (CalculateProfit(item, ship) > 0)
                {
                    profitableSell = item;
                }
            }

        }
        if (profitableSell == null)
            return false;

        item = profitableSell;
        target = item;

        return profitableSell != null;
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

        return item.price * itemAmount - (item.galaxyPosition - ship.galaxyPosition).magnitude * fuelMarketPrice / (ship.speed * ship.fuelEfficiency); //- ship.items[0].amount * ship.items[0].price;
    }
}

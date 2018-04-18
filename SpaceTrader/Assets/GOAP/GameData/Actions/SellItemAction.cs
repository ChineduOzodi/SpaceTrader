using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellItemAction : GoapAction {

    private bool itemUnloaded = false;
    private IdentityModel owner;
    private Ship ship;

    private double startTime = 0;
    public double workDuration = 30; // seconds

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
        itemUnloaded = false;
        startTime = 0;
    }

    public override bool isDone()
    {
        return itemUnloaded;
    }

    public override bool requiresInRange()
    {
        return true; // yes
    }

    public override bool checkProceduralPrecondition(PositionEntity agent)
    {
        // Check to make sure contract has destinationId


        Ship ship = agent as Ship;
        if (ship.contractId != null)
        {
            Contract contract = GameManager.instance.contracts[ship.contractId];

            if (contract.contractState == ContractState.Active)
            {
                if (contract.destinationId != null && contract.destinationId != "")
                {
                    target = GameManager.instance.locations[contract.destinationId];
                    return true;
                }
            }
        }

        return false;
    }

    public override bool perform(PositionEntity agent)
    {
        if (startTime == 0)
        {
            startTime = GameManager.instance.data.date.time;

            Ship ship = agent as Ship;
            if (ship.contractId == null)
            {
                Debug.Log("No contract, it ended");
                ship.itemsStorage.RemoveAll(x => true);
                return false;
            }
            Contract contract = GameManager.instance.contracts[ship.contractId];
            Item item = ship.itemsStorage.Find(x => x.id == contract.itemId && x.destinationId == contract.destinationId);

            if (item == null)
            {
                Debug.Log("Could not find item " + contract.itemId + " in " + ship.name + " with destination " + contract.destinationId);
                ship.itemsStorage.RemoveAll(x => true);
                return false;
            }
            workDuration = Dated.Minute * item.amount;
        }


        if (GameManager.instance.data.date.time - startTime > workDuration)
        {
            // Finish item unload
            Ship ship = agent as Ship;
            ProductionStructure structure = GameManager.instance.locations[ship.referenceId] as ProductionStructure;
            
            if (ship.contractId == null)
            {
                Debug.Log("No contract, it ended");
                ship.itemsStorage.RemoveAll(x => true);
                return false;
            }
            Contract contract = GameManager.instance.contracts[ship.contractId];
            Item item = ship.itemsStorage.Find(x => x.id == contract.itemId && x.destinationId == contract.destinationId);
            if (item.id == structure.blueprintId)
            {
                //Construct structure
                structure.count += (int)item.amount;
                structure.workers = structure.GetBlueprint().workers * structure.count;
                contract.itemAmount -= item.amount;
                ship.itemsStorage.Remove(item);
                Debug.Log(item.name + "Construction at " + structure.name);

                //Pay
                double cost = contract.unitPrice * item.amount + contract.distance * contract.PricePerKm;
                contract.client.Model.PayContract(cost);
                owner.EarnMoney(cost);

                if (((ProductionStructure)GameManager.instance.locations[contract.destinationId]).GetType() == typeof(DistributionCenter))
                {
                    //Undo payment if going to DC
                    contract.client.Model.PayContract(-cost);
                }
            }
            else
            {
                //Add to storage
                structure.AddItem(item);
                ship.itemsStorage.Remove(item);
                contract.itemAmount -= item.amount;
                //Pay
                double cost = contract.unitPrice * item.amount + contract.distance * contract.PricePerKm;
                contract.client.Model.PayContract(cost);
                owner.EarnMoney(cost);
                if (((ProductionStructure)GameManager.instance.locations[contract.destinationId]).GetType() == typeof(DistributionCenter))
                {
                    contract.client.Model.PayContract(-cost);
                }
            }
            
            itemUnloaded = true;
            
        }
        return true;
    }

    private double CalculateProfit(Item item, Ship ship)
    {
        //double fuelMarketPrice = owner.KnownStarIds[0].GetMarketPrice(ship.fuel.id);
        //double itemAmount = item.amount;
        //if (ship.itemCapacity < itemAmount)
        //    itemAmount = ship.itemCapacity;

        //return item.price * itemAmount - (item.position[0] - ship.position[0]).magnitude * fuelMarketPrice / (ship.speed * ship.fuelEfficiency); //- ship.items[0].amount * ship.items[0].price;
        return 1;
    }
}

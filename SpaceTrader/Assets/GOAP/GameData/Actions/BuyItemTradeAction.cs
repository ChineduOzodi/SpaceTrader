using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyItemTradeAction : GoapAction {

    private bool itemLoaded = false;
    private IdentityModel owner;
    private Ship ship;

    private double startTime = 0;
    public double workDuration = Dated.Hour; // seconds

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
        itemLoaded = false;
        startTime = 0;
    }

    public override bool isDone()
    {
        return itemLoaded;
    }

    public override bool requiresInRange()
    {
        return true; // yes
    }

    public override bool checkProceduralPrecondition(PositionEntity agent)
    {
        // Check to make sure contract exists, is active, and has an origin

        Ship ship = agent as Ship;
        if (ship.contractId != null)
        {
            try
            {
                Contract contract = GameManager.instance.contracts[ship.contractId];

                if (contract.contractState == ContractState.Active)
                {
                    if (contract.originId != null && contract.originId != "")
                    {
                        target = GameManager.instance.locations[contract.originId];
                        var structure = target as ProductionStructure;
                        Item item = structure.itemsStorage.Find(x => x.id == contract.itemId && x.destinationId == contract.destinationId);

                        if (item == null)
                        {
                            //Debug.Log("Could not find item " + contract.itemId + " in " + structure.name + " with destination " + contract.destinationId);
                            return false;
                        }
                        else
                        {
                            if (item.amount < 1)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
            }
            catch(KeyNotFoundException e)
            {
                return false;
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
            ProductionStructure structure = GameManager.instance.locations[ship.referenceId] as ProductionStructure;
            Contract contract = GameManager.instance.contracts[ship.contractId];
            Item item = structure.itemsStorage.Find(x => x.id == contract.itemId && x.destinationId == contract.destinationId);

            if (item == null)
            {
                //Debug.Log("Could not find item " + contract.itemId + " in " + structure.name + " with destination " + contract.destinationId);
                return false;
            }
            if (item.amount > ship.itemCapacity)
            {
                workDuration = Dated.Minute * ship.itemCapacity;
            }
            else
                workDuration = Dated.Minute * item.amount;

            ship.Info = "Loading " + ship.name + ", duration: " + Dated.ReadTime(workDuration);
            if (GameManager.instance.debugLog)
                GameManager.instance.timeScale = 1;
        }
            

        if (GameManager.instance.data.date.time - startTime > workDuration)
        {
            // Finish item load
            Ship ship = agent as Ship;
            ProductionStructure structure = GameManager.instance.locations[ship.referenceId] as ProductionStructure;
            Contract contract = GameManager.instance.contracts[ship.contractId];
            Item item = structure.itemsStorage.Find(x => x.id == contract.itemId && x.destinationId == contract.destinationId);
            if (item == null)
            {
                //Debug.Log("Could not find item " + contract.itemId + " in " + structure.name + " with destination " + contract.destinationId);
                return false;
            }
            if (item.amount > ship.itemCapacity)
            {
                ship.AddItem(item.id, item.destinationId, ship.itemCapacity);
                structure.itemsStorage.Find(x => x.id == item.id && x.destinationId == item.destinationId).RemoveAmount(ship.itemCapacity);
            }
            else
            {
                ship.AddItem(item);
                structure.itemsStorage.Remove(item);
            }
            itemLoaded = true;
            ship.Info = ("Loading " + ship.name + " Finished. Destination: " + GameManager.instance.locations[contract.destinationId].name);

        }
        return true;
    }
}

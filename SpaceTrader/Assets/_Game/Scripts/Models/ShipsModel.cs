using UnityEngine;
using System.Collections;
using CodeControl;
using System.Collections.Generic;

public class ShipsModel: Model
{
    public List<Ship> ships { get; private set; }
    public ShipsModel() { ships = new List<Ship>(); }
}

public class Ship: Structure, IWorkers, IGoap {

    public ShipAction shipAction = ShipAction.Idle;
    public bool hyperSpace;

    public string shipTargetId;

    public string contractId;

    public StructureTypes structureType = StructureTypes.Ship;
    public ShipType shipType { get { return GetShipBlueprint().shipType; } }

    //IWorkers


    public GoapAgent agent;

    public int workers { get; set; }

    public double workerPayRate { get; set; }

    //Ship Properties

    public int PassangerCapacity { get { return GetShipBlueprint().passangerCapacity; } }
    public List<int> passengerIds;

    /// <summary>
    /// Sub light speed in km/s
    /// </summary>
    public float SubLightSpeed { get { return GetShipBlueprint().subLightSpeed; } }
    /// <summary>
    /// Rotate speed in degrees per second;
    /// </summary>
    public float rotateSpeed { get { return GetShipBlueprint().rotateSpeed; } }

    public Fuel fuel;
    /// <summary>
    /// Number of km for one unit of fuel;
    /// </summary>
    public double fuelRange { get { return GetShipBlueprint().fuelRange; } }
    public int fuelCapacity = 100;


    public double idleTime;

    //Ship Properties
    public double ApproxFuelCostPerKm
    {
        get { return fuel.EstimatedValue / fuelRange; }
    }

    public Ship() { }

    public Ship(string shipBlueprintId, IdentityModel owner, Creature captain, string _referenceId, int _count = 1) :
        base(owner, _referenceId, Vector3d.zero)
    {
        this.managerId = captain.id;
        referenceId = _referenceId;
        this.Count = _count;
        //this.captain.Model.location = this;
        this.owner.Model.ownedShipIds.Add(id);
        GameManager.instance.data.ships.Model.ships.Add(this);

        //Get Blueprint
        blueprintId = shipBlueprintId;

        ShipBlueprint ship = GetShipBlueprint();
        name = ship.name + " " + ship.count++;


        //workers
        this.workers = ship.workers;
        workerPayRate = ship.workerPayRate;

        //Cargo
        itemCapacity = ship.cargoCapacity;

        //Fuel
        this.fuelCapacity = 100;
        this.fuel = new Fuel(GetShipBlueprint().fuelBlueprintId,fuelCapacity, id);

        //Agent
        agent = new GoapAgent(this, new GoapAction[] {
            new BuyItemTradeAction(this),
            new SellItemAction(this)
        });
    }

    public ShipBlueprint GetShipBlueprint()
    { return GameManager.instance.data.itemsData.Model.GetItem(blueprintId) as ShipBlueprint; }

    public void SetShipAction(ShipAction action)
    {
        shipAction = action;
    }

    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        worldData.Add(new KeyValuePair<string, object>("position", referenceId));
        worldData.Add(new KeyValuePair<string, object>("hasStorage", true));
        worldData.Add(new KeyValuePair<string, object>("hasTradeItems", (itemsStorage.Count > 0)));

        return worldData;
    }

    public HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        goal.Add(new KeyValuePair<string, object>("itemsSold", true));
        return goal;
    }

    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {
        // Not handling this here since we are making sure our goals will always succeed.
        // But normally you want to make sure the world state has changed before running
        // the same goal again, or else it will just fail.
    }

    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions)
    {
        // Yay we found a plan for our goal
        Debug.Log("<color=green>Plan found</color> " + GoapAgent.prettyPrint(actions));
    }

    public void actionsFinished()
    {
        // Everything is done, we completed our actions for this gool. Hooray!
        Debug.Log("<color=blue>Actions completed</color>");
    }

    public void planAborted(GoapAction aborter)
    {
       // An action bailed out of the plan.State has been reset to plan again.
       // Take note of what happened and make sure if you run the same goal again
       // that it can succeed.
       Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.prettyPrint(aborter));
    }

    public bool moveAgent(GoapAction nextAction)
    {
        // move towards the NextAction's target
        double step = SubLightSpeed * deltaTime;
        SystemPosition = Vector3d.MoveTowards(SystemPosition, nextAction.target.SystemPosition, step);

        //Remove Fuel
        fuel.RemoveAmount(step / fuelRange);

        //Set State and Target
        shipTargetId = nextAction.target.id;
        shipAction = ShipAction.Moving;

        //Exiting SOI

        var location = GameManager.instance.locations[referenceId];

        if (location.GetType()== typeof(SolarBody))
        {
            SolarBody solar = (SolarBody)location;

            if (referencePosition.sqrMagnitude > Mathd.Pow(solar.SOI(),2))
            {
                if (GameManager.instance.debugLog)
                    Debug.Log("Exiting " + solar.name);
                solar.structureIds.Remove(id);
                referenceId = location.referenceId;

                referencePosition += location.referencePosition;

                location = GameManager.instance.locations[referenceId];
                ((SolarBody)location).structureIds.Add(id);
            }
        }
        else
        {
            Structure structure = (Structure) location;

            if (referencePosition.sqrMagnitude > Mathd.Pow(structure.SOI, 2))
            {
                if (GameManager.instance.debugLog)
                    Debug.Log("Exiting " + structure.name);
                referenceId = location.referenceId;
                structure.structureIds.Remove(id);
                referencePosition += location.referencePosition;

                location = GameManager.instance.locations[referenceId];
                ((SolarBody)location).structureIds.Add(id);
            }
        }

        //Entering SOI SolarBody

        if (location.GetType() == typeof(SolarBody))
        {

            SolarBody solar = location as SolarBody;

            foreach (SolarBody body in solar.satelites)
            {
                
                if ((referencePosition - body.referencePosition).sqrMagnitude < Mathd.Pow(body.SOI(),2))
                {
                    solar.structureIds.Remove(id);
                    if (GameManager.instance.debugLog)
                        Debug.Log("Entering " + body.name);
                    //move into an SOI

                    referenceId = body.id;
                    location = GameManager.instance.locations[referenceId];
                    referencePosition -= body.referencePosition;
                    ((SolarBody)location).structureIds.Add(id);
                    break;

                }

            }

        }

        //Entering SOI Structure

        if (location.GetType() == typeof(SolarBody))
        {

            SolarBody solar = location as SolarBody;
            foreach (Structure structure in solar.structures)
            {
                if ((referencePosition - structure.referencePosition).sqrMagnitude < Mathd.Pow(structure.SOI, 2))
                {
                    if (GameManager.instance.debugLog)
                        Debug.Log("Entering " + structure.name);
                    solar.structureIds.Remove(id);
                    //move into an SOI

                    referenceId = structure.id;
                    location = GameManager.instance.locations[referenceId];
                    referencePosition -= structure.referencePosition;
                    ((Structure)location).structureIds.Add(id);
                    break;

                }

            }
        }

        //Reached Location

        if ((SystemToGalaxyPosition() - nextAction.target.SystemToGalaxyPosition()).sqrMagnitude < 1/Position.SystemConversion[0] && referenceId == nextAction.target.id)
        {
            // we are at the target location, we are done
            if (GameManager.instance.debugLog)
                Debug.Log("Location Reached");
            shipAction = ShipAction.Docked;
            nextAction.setInRange(true);
            return true;
        }
        else
            return false;
    }
}

public enum ShipAction
{
    Moving,
    Docked,
    Refueling,
    Idle
}

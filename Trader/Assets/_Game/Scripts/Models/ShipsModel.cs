using UnityEngine;
using System.Collections;
using CodeControl;
using System.Collections.Generic;

public class ShipsModel: Model
{
    public List<Ship> ships { get; private set; }
    public ShipsModel() { ships = new List<Ship>(); }
}

public class Ship: IStructure, IWorkers, IGoap {

    public ShipAction shipAction { get; private set; }
    public bool hyperSpace { get; private set; }

    public IPositionEntity target { get; private set; }
    public int shipTargetId { get; private set; }
    public int structureTargetId { get; private set; }

    public List<int> passengerIds { get; private set; }

    //IStructure and IWorkers
    public StructureTypes structureType { get; set; }

    public string name { get; set; }
    public string info { get; set; }
    public ModelRef<IdentityModel> owner { get; set; }
    public GoapAgent agent;
    public int managerId { get; set; }
    public float maxArmor { get; set; }
    public float currentArmor { get; set; }
    public int id { get; set; }
    public Dated dateCreated { get; set; }
    public Dated lastUpdated { get; set; }
    public bool deleteStructure { get; set; }
    public int count { get; set; }

    public Vector2d galaxyPosition { get; set; }

    public List<int> solarIndex { get; set; }
    public int structureId { get; set; }
    public int shipId { get; set; }

    public int workers { get; set; }

    public double workerPayRate { get; set; }

    //Ship Properties

    public int passangerCapacity { get; private set; }
    
    public float speed { get; private set; }
    public float rotateSpeed { get; private set; }
    public float jumpHullDamage { get; private set; }


    public List<Item> items = new List<Item>();
    public int itemCapacity = 100;

    public Item fuel { get; private set; }
    public float fuelEfficiency = 5;
    public int fuelCapacity = 100;

    public Ship() { }

    public Ship(string name, IdentityModel owner, Creature captain, int _structureId, int _count = 1)
    {
        this.owner = new ModelRef<IdentityModel>(owner);
        this.managerId = captain.id;
        structureId = _structureId;
        structureType = StructureTypes.Ship;
        this.count = _count;
        //this.captain.Model.location = this;
        id = GameManager.instance.data.id++;
        this.owner.Model.ships.Add(id);
        GameManager.instance.data.ships.Model.ships.Add(this);
        shipAction = ShipAction.Idle;
        solarIndex = captain.solarIndex;
        galaxyPosition = GameManager.instance.data.stars[solarIndex[0]].galaxyPosition + GameManager.instance.data.getSolarBody(solarIndex).lastKnownPosition;
        structureId = -1;
        shipId = -1;

        this.workers = 10;
        this.name = name;
        this.dateCreated = new Dated(GameManager.instance.data.date.time);
        this.lastUpdated = new Dated(GameManager.instance.data.date.time);
        this.passangerCapacity = 10;

        this.speed = Random.Range(2f, 5f) * (1 - this.passangerCapacity / 200f + .5f);
        this.fuelEfficiency = Random.Range(5000f, 1000f) * (1 - this.passangerCapacity / 200f + .5f);
        this.fuelCapacity = (int)(Random.Range(50, 200) * (this.passangerCapacity / 200f + .5f));
        var fuelBluePrint = GameManager.instance.data.itemsData.Model.items.Find(x => x.itemType == ItemType.Fuel);
        this.fuel = new Item(fuelBluePrint.id,fuelCapacity, 1, owner, this.solarIndex);
        this.fuel.shipId = this.id;

        agent = new GoapAgent(this, new GoapAction[] {
            new BuyItemTradeAction(this),
            new SellItemAction(this)
        });
    }

    public void SetShipAction(ShipAction action)
    {
        shipAction = action;
    }

    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        worldData.Add(new KeyValuePair<string, object>("position", solarIndex));
        worldData.Add(new KeyValuePair<string, object>("hasStorage", true));
        worldData.Add(new KeyValuePair<string, object>("hasTradeItems", (items.Count > 0)));

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
        double step = speed * GameManager.instance.data.date.deltaTime;
        galaxyPosition = Vector2d.MoveTowards(galaxyPosition, nextAction.target.galaxyPosition, step);

        if ((galaxyPosition - nextAction.target.galaxyPosition).sqrMagnitude < 5)
        {
            // we are at the target location, we are done
            nextAction.setInRange(true);
            return true;
        }
        else
            return false;
    }
}

public enum ShipAction
{
    Buy,
    Sell,
    SearchingTradeRoute,
    Idle
}

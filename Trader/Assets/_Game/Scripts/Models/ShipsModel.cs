using UnityEngine;
using System.Collections;
using CodeControl;
using System.Collections.Generic;

public class ShipsModel: Model
{
    public List<Ship> ships { get; private set; }
    public ShipsModel() { ships = new List<Ship>(); }
}

public class Ship: Structure {

    public ShipAction shipAction { get; private set; }
    public bool hyperSpace { get; private set; }
    public Vector3d galaxyPosition;
    public int shipId { get; private set; }
    public int structureId { get; private set; }

    public List<int> target { get; private set; }
    public List<int> sellTarget { get; private set; }
    public int shipTargetId { get; private set; }
    public int structureTargetId { get; private set; }

    public List<int> passengerIds { get; private set; }

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

    public void SetShipAction(ShipAction action)
    {
        shipAction = action;
    }

    public Ship(string name, IdentityModel owner, Creature captain)
    {
        this.owner = new ModelRef<IdentityModel>(owner);
        this.managerId = captain.id;
        //this.captain.Model.location = this;
        shipId = GameManager.instance.data.id++;
        this.owner.Model.ships.Add(shipId);
        GameManager.instance.data.ships.Model.ships.Add(this);

        solarIndex = captain.solarIndex;
        galaxyPosition = GameManager.instance.data.stars[solarIndex[0]].galacticPosition + GameManager.instance.data.getSolarBody(solarIndex).lastKnownPosition;

        this.workers = 10;
        this.name = name;
        this.dateCreated = new Dated(GameManager.instance.data.date.time);
        this.lastUpdated = new Dated(GameManager.instance.data.date.time);
        this.passangerCapacity = 10;

        this.speed = Random.Range(2f, 5f) * (1 - this.passangerCapacity / 200f + .5f);
        this.fuelEfficiency = Random.Range(5000f, 1000f) * (1 - this.passangerCapacity / 200f + .5f);
        this.fuelCapacity = (int)(Random.Range(50, 200) * (this.passangerCapacity / 200f + .5f));
        this.fuel = new Item(GameManager.instance.data.items.Model.items.Find(x => x.itemType == ItemType.Fuel).id,fuelCapacity);

        //Money Setup
    }

}

public enum ShipAction
{
    Buy,
    Sell,
    SearchingTradeRoute,
    Idle
}

using UnityEngine;
using System.Collections;
using CodeControl;
using System.Collections.Generic;

public class ShipsModel: Model
{
    public List<Ship> ships { get; private set; }
    public ShipsModel() { ships = new List<Ship>(); }
}

public class Ship: IStructure, IWorkers {

    public ShipAction shipAction { get; private set; }
    public bool hyperSpace { get; private set; }

    public List<int> target { get; private set; }
    public List<int> sellTarget { get; private set; }
    public int shipTargetId { get; private set; }
    public int structureTargetId { get; private set; }

    public List<int> passengerIds { get; private set; }

    //IStructure and IWorkers
    public StructureTypes structureType { get; set; }

    public string name { get; set; }
    public string info { get; set; }
    public ModelRef<IdentityModel> owner { get; set; }
    public int managerId { get; set; }
    public float maxArmor { get; set; }
    public float currentArmor { get; set; }
    public int id { get; set; }
    public Dated dateCreated { get; set; }
    public Dated lastUpdated { get; set; }
    public bool deleteStructure { get; set; }

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

    public Ship(string name, IdentityModel owner, Creature captain, int _structureId)
    {
        this.owner = new ModelRef<IdentityModel>(owner);
        this.managerId = captain.id;
        structureId = _structureId;
        structureType = StructureTypes.Ship;
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
        this.fuel = new Item(fuelBluePrint.id,fuelCapacity, 1, owner);
    }

    public void SetShipAction(ShipAction action)
    {
        shipAction = action;
    }

}

public enum ShipAction
{
    Buy,
    Sell,
    SearchingTradeRoute,
    Idle
}

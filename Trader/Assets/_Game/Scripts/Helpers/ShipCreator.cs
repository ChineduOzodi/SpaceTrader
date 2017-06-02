using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class ShipCreator{

    public static ShipModel CreateShip(string name, int starIndex, SolarBody parent, Polar2 position, IdentityModel owner, CreatureModel captain)
    {
        ShipModel shipModel = new ShipModel();
        shipModel.owner = new ModelRef<IdentityModel>(owner);
        shipModel.captain = new ModelRef<CreatureModel>(captain);
        shipModel.captain.Model.location.Model = shipModel;
        shipModel.owner.Model.ships.Add(shipModel);
        shipModel.workers = 0;

        shipModel.name = name;
        shipModel.dateCreated = new Date(GameManager.instance.data.date.time);
        shipModel.lastUpdated = new Date(GameManager.instance.data.date.time);
        shipModel.capacity = Random.Range(10, 200);
        shipModel.workerCapacity = shipModel.capacity / 10;
        shipModel.speed = Random.Range(2f,5f) * ( 1 - shipModel.capacity/200f + .5f);
        shipModel.fuelEfficiency = Random.Range(5000f, 1000f) * (1 - shipModel.capacity / 200f + .5f);
        shipModel.fuelCapacity = (int) (Random.Range(50, 200) * (shipModel.capacity / 200f + .5f));
        shipModel.fuel = new Items( ItemTypes.Fuel, shipModel.fuelCapacity);
        shipModel.solar = new Orbit(starIndex, parent, position);
        GameManager.instance.data.stars[starIndex].ships.Add(shipModel);

        //Money Setup
        shipModel.money = 1000f;
        shipModel.owner.Model.money -= 1000;

        shipModel.moneyStats.data.Add("Money", new List<Stat>() { new Stat(shipModel.age.hour, shipModel.money) });
        shipModel.moneyStats.data.Add("Money Change", new List<Stat>());

        return shipModel;
    }
}

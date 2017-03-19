using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class ShipCreator{

    public static ShipModel CreateShip(string name, int starIndex, SolarBody parent, Polar2 position, CreatureModel owner = null)
    {
        ShipModel shipModel = new ShipModel();
        NameGen names = new NameGen();
        if (owner == null)
        {
            shipModel.owner = new ModelRef<CreatureModel>( new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateRegionName(), 1000));
            shipModel.captain = shipModel.owner;
        }
        else
        {
            shipModel.owner = new ModelRef<CreatureModel>(owner);
            shipModel.captain = new ModelRef<CreatureModel>(new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateRegionName()));
        }
        shipModel.owner.Model.ships.Add(shipModel);
        shipModel.workers = new ModelRefs<CreatureModel>();
        shipModel.workers.Add(new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateRegionName()));

        shipModel.name = name;
        
        shipModel.capacity = Random.Range(10, 200);
        shipModel.workerCapacity = shipModel.capacity / 10;
        shipModel.speed = Random.Range(2f,5f) * ( 1 - shipModel.capacity/200f + .5f);
        shipModel.fuelEfficiency = Random.Range(175f, 300f) * (1 - shipModel.capacity / 200f + .5f);
        shipModel.fuelCapacity = (int) (Random.Range(50, 200) * (shipModel.capacity / 200f + .5f));
        shipModel.fuel = new Items("Fuel", shipModel.fuelCapacity);
        shipModel.solar = new SolarBody(shipModel.name, starIndex, SolarType.Structure, position, .0001f, Color.black, CreateGalaxy.G, parent);
        //Money Setup
        shipModel.money = 1000f;
        shipModel.owner.Model.money -= 1000;

        shipModel.moneyStats.data.Add("Money", new List<Stat>() { new Stat(shipModel.age.hour, shipModel.money) });
        shipModel.moneyStats.data.Add("Money Change", new List<Stat>());

        ShipController ship = Controller.Instantiate<ShipController>("ship", shipModel);

        return shipModel;
    }
}

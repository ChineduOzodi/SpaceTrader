using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class ShipCreator{

	public static ShipModel CreateShip(string name, Vector3 location, CreatureModel owner = null)
    {
        ShipModel shipModel = new ShipModel();
        NameGen names = new NameGen();
        if (owner == null)
        {
            owner = new CreatureModel(names.GenerateMaleFirstName() + " " + names.GenerateRegionName());
        }

        
        shipModel.name = name;
        shipModel.owner = new ModelRef<CreatureModel>(owner);
        shipModel.money = 1000f;
        shipModel.position = location;
        shipModel.capacity = Random.Range(10, 200);
        shipModel.speed = Random.Range(2f,5f) * ( 1 - shipModel.capacity/200f + .5f);
        shipModel.fuelEfficiency = Random.Range(15f, 20f) * (1 - shipModel.capacity / 200f + .5f);
        shipModel.fuelCapacity = (int) (Random.Range(50, 200) * (shipModel.capacity / 200f + .5f));

        ShipController ship = Controller.Instantiate<ShipController>("ship", shipModel);

        return shipModel;
    }
}

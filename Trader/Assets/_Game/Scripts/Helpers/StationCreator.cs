using CodeControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationCreator {

    public static StationModel CreateStation(string name, SolarModel star, SolarBody parent, Polar2 position, IdentityModel owner, CreatureModel captain)
    {
        StationModel model = new StationModel();
        
        model.owner = new ModelRef<IdentityModel>(owner);
        model.manager = new ModelRef<CreatureModel>(captain);
        model.manager.Model.location.Model = model;

        model.workers = 10;
        model.workerCapacity = 50;

        model.name = name;
        model.dateCreated = new Date(GameManager.instance.data.date.time);
        model.lastUpdated = new Date(GameManager.instance.data.date.time);
        model.capacity = 10000;
        System.Random rand = new System.Random(model.name.GetHashCode());
        float a = rand.Next(1000)/1000f;
        float b = rand.Next(1000)/1000f;
        float c = rand.Next(1000)/1000f;
        model.color = new Color(a,b,c);
        a = rand.Next(1000) / 1000f;
        b = rand.Next(1000) / 1000f;
        c = rand.Next(1000) / 1000f;
        model.backgroundColor = new Color(a, b, c);
        model.solar = new Orbit(star.index, parent, position);
        star.stations.Add(model);
        if (position.radius == 0 && parent.rawResource != RawResources.None)
        {
            model.factory = new Factory(parent.rawResource,model);
        }
        else
        {
            model.factory = new Factory(true, model);
        }
        

        foreach (Items item in model.factory.inputItems)
        {
            item.pendingAmount = item.amount;
            
        }
        foreach (Items item in model.factory.outputItems)
        {
            item.pendingAmount = item.amount;
        }

        //Money Setup
        model.money = 1000000f;
        model.owner.Model.money -= 1000000;
        model.moneyStats = new DataGraph("Money Over Time", "Time (hours)", "Money");
        model.moneyStats.data.Add("Money", new List<Stat>() { new Stat(model.age.hour, model.money) });
        model.moneyStats.data.Add("Money Change", new List<Stat>());

        return model;
    }
}

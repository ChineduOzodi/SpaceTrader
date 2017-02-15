using CodeControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationCreator {

    public static StationModel CreateStation(FactoryType name, Vector3 location, Transform parent = null)
    {
        StationModel model = new StationModel();

        model.name = name + " Station";
        model.position = location;
        model.capacity = 10000;
        System.Random rand = new System.Random(model.name.GetHashCode());
        float a = rand.Next(1000)/1000f;
        float b = rand.Next(1000)/1000f;
        float c = rand.Next(1000)/1000f;
        model.color = new Color(a,b,c);

        model.factory = new Factory(name);

        StationController station = Controller.Instantiate<StationController>("station", model, parent);

        return model;
    }
}

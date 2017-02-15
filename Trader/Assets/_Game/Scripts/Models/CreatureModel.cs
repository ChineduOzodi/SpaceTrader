using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class CreatureModel : Model {

    public string name;
    public float money = 1000;
    public ModelRefs<StationModel> stations;
    public ModelRefs<ShipModel> ships;

    public CreatureModel(string name)
    {
        this.name = name;
        stations = new ModelRefs<StationModel>();
        ships = new ModelRefs<ShipModel>();
    }
    
}

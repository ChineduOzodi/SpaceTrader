using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeControl;

public class StationModel: StructureModel {


    StructureType type = StructureType.Station;

    public ModelRefs<ShipModel> incomingShips = new ModelRefs<ShipModel>();
    
    public float capacity;
    public Color color;
    public Color backgroundColor;

    public float runningCost = 10f;

    public Factory factory;

    public StationModel() { }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeControl;

public class StationModel: Model {

    public string name;

    StructureType type = StructureType.Station;

    public Vector3 position;

    public ModelRefs<ShipModel> incomingShips = new ModelRefs<ShipModel>();

    public float capacity;
    public Color color;
    public float money = 50000;
    public float runningCost = 1f;

    public Factory factory;

}

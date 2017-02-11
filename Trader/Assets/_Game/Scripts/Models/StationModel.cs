using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeControl;

public class StationModel: Model {

    public string name;

    StructureTypes type = StructureTypes.Station;

    public Vector3 position;

    public float capacity;

    public float money = 1000;

    public Factory factory;

}

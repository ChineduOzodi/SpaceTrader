using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StationModel {

    public string name;

    StructureTypes type = StructureTypes.Station;

    public Vector3 position;

    public float capacity;

    public float spaceLeft;

    public List<Items> items;

}

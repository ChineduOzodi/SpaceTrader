using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class StructureModel : Model {

    public ModelRef<CreatureModel> owner;
    public ModelRef<CreatureModel> captain;
    public ModelRefs<CreatureModel> workers;

    public int workerCapacity;
    public float money = 0;

    public string name;
    public Vector3 position;
    public Vector3 lineTarget;
    public Color lineColor;
    public Date age = new Date(0);
    public Date timeCreated = new Date(0);

    public DataGraph moneyStats = new DataGraph("Money Over Time", "Time (hours)", "Money");
    public float moneyChange; 
}


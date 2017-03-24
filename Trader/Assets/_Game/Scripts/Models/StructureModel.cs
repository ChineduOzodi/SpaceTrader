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
    public SolarBody solar;
    public Vector3 lineTarget;
    public Color lineColor;
    public Date lastUpdated;
    public Date age = new Date(0);
    public Date dateCreated = new Date(0);
    /// <summary>
    /// Used to update money at a given interval;
    /// </summary>
    public float timeUpdate;
    public DataGraph moneyStats = new DataGraph("Money Over Time", "Time (hours)", "Money");
    public float moneyChange; 
}


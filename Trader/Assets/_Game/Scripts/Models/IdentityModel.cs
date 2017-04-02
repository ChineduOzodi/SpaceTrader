using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class IdentityModel : Model {

    public string name;
    public IdentityType identityType;
    public Vector3 lineTarget;
    public Color lineColor;
    /// <summary>
    /// Used to Track how many ships it has bought. Will change later
    /// </summary>
    internal int itemsBought = 0;
    public Date lastUpdated;

    public ModelRef<StructureModel> location = new ModelRef<StructureModel>();
    public Orbit solar;
    public ModelRefs<StationModel> stations = new ModelRefs<StationModel>();
    public ModelRefs<ShipModel> ships = new ModelRefs<ShipModel>();

    public Date age = new Date(0);
    public Date dateCreated = new Date(0);

    /// <summary>
    /// Used to update money at a given interval;
    /// </summary>
    public float timeUpdate;
    public float money = 0;

    public DataGraph moneyStats = new DataGraph("Money Over Time", "Time (hours)", "Money");
    public float moneyChange;
}

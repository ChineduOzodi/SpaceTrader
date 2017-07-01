using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class IdentityModel : Model {

    public string name;
    public IdentityType identityType;
    public Vector3 lineTarget;
    public Color lineColor;
    public Color spriteColor;
    /// <summary>
    /// Used to Track how many ships it has bought. Will change later
    /// </summary>
    internal int itemsBought = 0;
    public Dated lastUpdated;

    public ModelRef<StructureModel> location = new ModelRef<StructureModel>();
    public Orbit orbit;
    public int solarIndex { get; set; }
    public int parentIndex { get; set; }
    public ModelRefs<StationModel> stations = new ModelRefs<StationModel>();
    public ModelRefs<ShipModel> ships = new ModelRefs<ShipModel>();

    public Dated age = new Dated(0);
    public Dated dateCreated = new Dated(0);

    /// <summary>
    /// Used to update money at a given interval;
    /// </summary>
    public double timeUpdate;
    public double money = 0;

    public DataGraph moneyStats = new DataGraph("Money Over Time", "Time (hours)", "Money");
    public double moneyChange;

    public IdentityModel()
    {
        dateCreated = new Dated(GameManager.instance.data.date.time);
        lastUpdated = new Dated(GameManager.instance.data.date.time);
        spriteColor = UnityEngine.Random.ColorHSV(.5f, 1f, .5f, 1f);
        spriteColor.a = 1;
    }
}

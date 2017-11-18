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

    public List<int> solarIndex { get; protected set; }
    public List<List<int>> stations = new List<List<int>>();
    public List<int> ships = new List<int>();

    public Dated age { get; private set; }
    public Dated dateCreated { get; private set; }

    /// <summary>
    /// Used to update money at a given interval;
    /// </summary>
    public double timeUpdate;
    public double money = 0;

    public DataGraph moneyStats = new DataGraph("Money Over Time", "Time (hours)", "Money");
    public double moneyChange;

    public IdentityModel()
    {
        age = new Dated(0);
        dateCreated = new Dated(GameManager.instance.data.date.time);
        lastUpdated = new Dated(GameManager.instance.data.date.time);
        spriteColor = UnityEngine.Random.ColorHSV(.5f, 1f, .5f, 1f);
        spriteColor.a = 1;
    }

    public void SetLocation(List<int> _solarIndex)
    {
        solarIndex = _solarIndex;
    }
}

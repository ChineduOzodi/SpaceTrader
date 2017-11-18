﻿using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;

public class ShipController : Controller<ShipsModel>
{
    internal int mapMask;
    internal int solarMask;
    internal float travelCounter = 0;
    internal float updateCount = 10;
    internal float waitTime = 0;
    internal GameManager game;
    internal SpriteRenderer sprite;
    internal LineRenderer line;
    internal double timeUpdate = 0;
    private GalaxyManager galaxy;
    private Unit unit;

    //internal int solarIndex
    //{
    //    get
    //    {
    //        return model.solarIndex;
    //    }
    //}
    //internal bool hyperspace
    //{
    //    get { return model.hyperSpace; }
    //}

    protected override void OnInitialize()
    {
        //game = GameManager.instance;
        //galaxy = game.galaxy;
        //unit = GetComponent<Unit>();
        //sprite = GetComponent<SpriteRenderer>();
        //line = GetComponent<LineRenderer>();
        //solarMask = gameObject.layer;
        //mapMask = game.gameObject.layer;        

        //name = model.name;

        //model.moneyStats = new DataGraph("Money Over Time", "Time (hours)", "Money");
        //model.moneyStats.data.Add("Money", new List<Stat>() { new Stat(model.age.hour, model.money) });
        //model.moneyStats.data.Add("Money Change", new List<Stat>());

        //transform.localScale = Vector3.one * (model.passangerCapacity / 200f + .5f);
        //timeUpdate = model.age.time + Dated.Hour;
    }
    protected override void OnModelChanged()
    {
        //if (model.solarIndex != model.target.Model.solarIndex)
        //{
        //    HyperSpaceTravel(model.target.Model.solarIndex);
        //}
        

        
    }
    // Update is called once per frame
    void Update()
    {
        //if (model.hyperSpace)
        //{
        //    line.enabled = true;
        //    sprite.enabled = true;
        //    transform.localScale = Vector3.one * (model.passangerCapacity / 200f + .5f) * 2;
        //    line.startWidth = transform.localScale.x * .3f;
        //    line.endWidth = transform.localScale.x * .3f;
        //}
        //else
        //{
        //    gameObject.layer = solarMask;
        //    if (!game.data.stars[model.solarIndex].isActive)
        //    {
        //        sprite.enabled = false;
        //        line.enabled = false;
        //    }
        //    else
        //    {
        //        sprite.enabled = true;
        //        line.enabled = true;

        //        transform.localScale = Vector3.one *(float)( (model.passangerCapacity / 200f + .5f) * Mathd.Pow(game.data.cameraGalCameraScaleMod, 1.7f) * .1f);
        //        //Set orbit outline
        //        //line.startWidth = transform.localScale.x * .3f;
        //        //line.endWidth = transform.localScale.x * .3f;
        //        //Vector3 targetPosition = (Vector2) model.target.Model.GamePosition(game.data.date.time);
        //        //line.SetPositions(new Vector3[] { transform.position, targetPosition });
        //        //line.startColor = sprite.color;
        //        //line.endColor = ((StationModel)model.target.Model).color;
        //    }
        //}
    }

    //private void HyperSpaceTravel(int solarIndex)
    //{
    //    StopCoroutine("SolarTravel");
    //    if (model.solarIndex == solarIndex)
    //    {
    //        HyperSpaceDone();
    //    }
    //    else
    //    {
    //        model.hyperSpace = true;
    //        gameObject.layer = mapMask;
    //        transform.position = CameraController.CameraOffsetGalaxyPosition(game.data.stars[model.solarIndex].galacticPosition);
    //        unit.HyperSpaceTravel(model.solarIndex, solarIndex, model.speed);

    //        SolarBody parent = game.data.stars[solarIndex].solar;
    //        //Polar2d position = new Polar2d(UnityEngine.Random.Range((float) parent.bodyRadius + 2, (float) parent.orbit.soi), UnityEngine.Random.Range(0, 2 * Mathf.PI));
    //        //model.solarIndex = solarIndex;
    //        //model.orbit.parentMass = parent.orbit.mass;
    //        //model.position = game.data.stars[solarIndex].galacticPosition;
    //    }
        
        
    //}

    //public void HyperSpaceDone()
    //{
    //    model.hyperSpace = false;
    //    //transform.position = (Vector2) model.GamePosition(game.data.date.time);
    //    StartCoroutine("SolarTravel");
    //}

    //public string GetInfo()
    //{
    //    string info = "";
    //    string targetName = "---";
    //    //if (model.target != null && model.target != null)
    //    //    targetName = model.target.name;

    //    //info += string.Format("Ship Name: {0}\nMoney: {3}\n\nOwner: {11}\nCaptain: {12}\n Number Workers: {13}/{14}\n\nMode: {1}\nCargo: {4} - {8}/{5}\nTarget: {2}\nSpeed: {9}\nFuel: {6}/{7}\nFuel Efficeincy: {10}\n\n",
    //    //    model.name, model.mode, targetName, model.money, model.item.coloredName, model.capacity, model.fuel.amount, model.fuelCapacity, model.item.amount, model.speed, model.fuelEfficiency, model.owner.Model.name, model.captain.Model.name, model.workers.Count, model.workerCapacity);

    //    List<Stat> moneyStats = new List<Stat>();
    //    moneyStats.AddRange(model.moneyStats.data["Money"]);
    //    moneyStats.Reverse();
    //    foreach (Stat stat in moneyStats)
    //    {
    //        if (stat.x > (model.age.time - Dated.Day))
    //            info += string.Format("\n{0}. {1}", (stat.x / Dated.Hour).ToString("0"), stat.y.ToString("0.00"));
    //    }

    //    return info;
    //}
}

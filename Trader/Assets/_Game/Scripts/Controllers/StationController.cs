using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class StationController : Controller<StationModel> {

    public SpriteRenderer background;
    internal GameManager game;
    internal GalaxyManager galaxy;
    internal SpriteRenderer sprite;
    internal float timeUpdate;
    internal LineRenderer line;
    internal StationModel Model;
    internal float money
    {
        get
        {
            return model.money;
        }
    }
    protected override void OnInitialize()
    {
        Model = model;
        game = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();
        galaxy = game.galaxy;
        sprite = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();
        name = model.name;
        timeUpdate = model.age.time + Date.Hour;
        sprite.color = model.color;

        background.color = model.backgroundColor;
        transform.position = model.solar.GetWorldPosition(game.data.date.time);


        if (!game.data.stars[model.solar.starIndex].isActive)
        {
            sprite.enabled = false;
            background.enabled = false;
        }
        else
        {
            sprite.enabled = true;
            background.enabled = true;
        }
    }
    protected override void OnModelChanged()
    {
        //line.SetPositions(new Vector3[] { transform.position, model.lineTarget });
        //model.lineColor.a = 1.1f - Mathf.Pow(.1f, 100 / (model.lineTarget - transform.position).magnitude);
        //line.startColor = model.lineColor;
        //line.endColor = model.lineColor;
        //line.startWidth = model.lineColor.a;
        //line.endWidth = model.lineColor.a;
    }
    // Update is called once per frame
    void Update() {

        
        if (!game.data.stars[model.solar.starIndex].isActive)
        {
            sprite.enabled = false;
            background.enabled = false;
            line.enabled = false;
        }
        else
        {

            sprite.enabled = true;
            background.enabled = true;
            line.enabled = true;

            //Set orbit outline
            line.startWidth = transform.localScale.x * .3f;
            line.endWidth = transform.localScale.x * .3f;

            //int numPoints = (int) (body.radius * .1f);
            int numPoints = 360;
            float angleStep = (2 * Mathf.PI / numPoints);

            //Creates the line rendering for the orbit of planets

            Vector3[] orbitPos = new Vector3[numPoints + 1];

            for (int b = 0; b < numPoints + 1; b++)
            {
                orbitPos[b] = model.solar.parent.solar.GetWorldPosition(game.data.date.time) + new Polar2(model.solar.radius, angleStep * b).cartesian;
            }
            line.positionCount = numPoints;
            line.SetPositions(orbitPos);
        }
        transform.position = model.solar.GetWorldPosition(game.data.date.time);
        transform.localScale = Vector3.one * .1f * (model.money / 1000000f + .5f) * Mathf.Pow(game.localScaleMod, 1.7f);
    }

    public string GetInfo()
    {
        string info = "";
        info += "Factory Name: <color=" + ColorTypeConverter.ToRGBHex(model.color) + ">" + model.factory.name + "</color>\nMoney: " + model.money + "\n";
        info += string.Format("Owner: {0}\nCaptain: {1}\n Number Workers: {2}/{3}\n", model.owner.Model.name, model.manager.Model.name, model.workers, model.workerCapacity);
        info += "Progress: " + (model.factory.productionTime / model.factory.unitTime).ToString("0.00") + " - " + model.factory.unitTime + "\n\n";

        info += "\n\n";
        List<Stat> moneyStats = new List<Stat>();
        moneyStats.AddRange(model.moneyStats.data["Money"]);
        moneyStats.Reverse();
        foreach (Stat stat in moneyStats)
        {
            if (stat.x > (model.age.time - Date.Day))
                info += string.Format("\n{0}. {1}", (stat.x / Date.Hour).ToString("0"), stat.y.ToString("0.00"));
        }

        return info;
    }

    

    internal Items[] GetInputItems()
    {
        return model.factory.inputItems;
    }

    internal Items[] GetOutputItems()
    {
        return model.factory.outputItems;
    }

}

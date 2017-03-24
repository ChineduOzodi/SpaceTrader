using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class StationController : Controller<StationModel> {

    public SpriteRenderer background;
    internal GameManager game;
    internal CreateGalaxy galaxy;
    internal SpriteRenderer sprite;
    internal float timeUpdate;
    internal LineRenderer line;
    internal float money
    {
        get
        {
            return model.money;
        }
    }
    protected override void OnInitialize()
    {
        game = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();
        galaxy = game.galaxy;
        sprite = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();
        name = model.name;
        timeUpdate = model.age.time + Date.Hour;
        sprite.color = model.color;

        background.color = model.backgroundColor;
        transform.position = model.solar.GetWorldPosition(game.data.date.time);


        if (!galaxy.stars[model.solar.starIndex].isActive)
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

        
        if (!galaxy.stars[model.solar.starIndex].isActive)
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
                orbitPos[b] = model.solar.parent.GetWorldPosition(game.data.date.time) + new Polar2(model.solar.radius, angleStep * b).cartesian;
            }
            line.numPositions = numPoints;
            line.SetPositions(orbitPos);
        }
        transform.position = model.solar.GetWorldPosition(game.data.date.time);
        transform.localScale = Vector3.one * .1f * (model.money / 1000000f + .5f) * Mathf.Pow(game.localScaleMod, 1.7f);
    }

    public string GetInfo()
    {
        string info = "";
        info += "Factory Name: <color=" + ColorTypeConverter.ToRGBHex(model.color) + ">" + model.factory.name + "</color>\nMoney: " + model.money + "\n";
        info += string.Format("Owner: {0}\nCaptain: {1}\n Number Workers: {2}/{3}\n", model.owner.Model.name, model.captain.Model.name, model.workers.Count, model.workerCapacity);
        info += "Progress: " + (model.factory.productionTime / model.factory.unitTime).ToString("0.00") + " - " + model.factory.unitTime + "\n\n";

        foreach (Items item in model.factory.inputItems)
        {
            if (item.selfProducing == false)
                info += "Input " + item.coloredName + ": " + item.amount + " (" + (item.pendingAmount - item.amount) + ")/" + item.maxAmount + "| Price: " + item.price.ToString("0.00") + " - " +item.basePrice +  "\n";
            else
                info += "Input " + item.coloredName + ": ---\n";
        }
        foreach (Items item in model.factory.outputItems)
        {
            info += "Output " + item.coloredName + ": " + item.amount + "/" + item.maxAmount + "| Price: " + item.price.ToString("0.00") + " - " + item.basePrice + "\n";
        }

        List<ShipModel> sortedShips = new List<ShipModel>();
        foreach (ShipModel shipModel in model.incomingShips)
        {
            sortedShips.Add(shipModel);
        }
        sortedShips.Sort(delegate (ShipModel c1, ShipModel c2) { return c2.item.amount.CompareTo(c1.item.amount); });

        for (int i = 0; i < sortedShips.Count; i++)
        {
            info += string.Format("\n{0}. {1} - {2} | {3} - {4}", i + 1, sortedShips[i].name, sortedShips[i].money.ToString("0.00"), sortedShips[i].item.coloredName, sortedShips[i].item.amount);
        }
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

    internal Items Buy(string itemName, int itemAmount, ShipModel buyer = null)
    {
        foreach (Items item in model.factory.outputItems)
        {
            if (item.name == itemName)
            {
                if (itemAmount > item.amount)
                {
                    itemAmount = item.amount;
                }
                Items soldItem = new Items(itemName, itemAmount);
                soldItem.price = item.price;
                soldItem.totalPrice = soldItem.price * soldItem.amount;

                model.money += soldItem.totalPrice;
                item.amount -= itemAmount;
                model.factory.SetPrices();

                if (item.name == "Ship" && itemAmount > 0)
                {
                    item.amount += itemAmount;
                    if (buyer != null)
                    {
                        StationModel startStation = model;
                        Vector3 randomLocation = new Vector3(UnityEngine.Random.Range(-1000, 1000), UnityEngine.Random.Range(-1000, 1000));
                        ShipModel ship = ShipCreator.CreateShip(buyer.name + "." + buyer.index, startStation.solar.starIndex, startStation.solar.parent, startStation.solar.GetLocalPosition(game.data.date.time), buyer.owner.Model);
                        game.data.ships.Add(ship);
                        game.UpdateCreatures(ship);
                        buyer.index++;
                        buyer.item.totalPrice = soldItem.totalPrice;
                    }
                    else
                    {
                        print("No buyer for Ship purchase in Station: " + model.name);
                    }
                }

                return soldItem;
            }

        }

        return null;
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

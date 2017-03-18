using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;

public class ShipController : Controller<ShipModel>
{

    internal string profitableRoutes;
    internal GameObject target;
    internal string mode;
    internal GameObject sellTarget;
    internal float travelCounter = 0;
    internal float updateCount = 10;
    internal float waitTime = 0;
    internal GameManager game;
    internal SpriteRenderer sprite;
    internal LineRenderer line;
    internal float timeUpdate = 0;

    protected override void OnInitialize()
    {
        game = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();
        sprite = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();
        transform.position = model.position;
        name = model.name;

        model.moneyStats = new DataGraph("Money Over Time", "Time (hours)", "Money");
        model.moneyStats.data.Add("Money", new List<Stat>() { new Stat(model.age.hour, model.money) });
        model.moneyStats.data.Add("Money Change", new List<Stat>());

        transform.localScale = Vector3.one * (model.capacity / 200f + .5f);
        timeUpdate = model.age.time + Date.Hour;
        FindTradeRoute();

    }
    protected override void OnModelChanged()
    {
        target = null;
    }
    // Update is called once per frame
    void Update()
    {
        model.age.AddTime(Time.deltaTime);
        if (timeUpdate < model.age.time)
        {
            timeUpdate = model.age.time + Date.Hour;

            //Money Evaluation
            model.money -= model.runningCost;

            foreach (CreatureModel worker in model.workers)
            {
                worker.money += 10;
                model.money -= 10;
            }

            if (model.captain.Model != model.owner.Model)
            {
                model.captain.Model.money += 15;
                model.money -= 15;
            }

            float moneyEarned = model.money - model.moneyStats.data["Money"][model.moneyStats.data["Money"].Count - 1].y;

            if (moneyEarned > 0)
            {
                model.owner.Model.money += moneyEarned * .25f;
                model.money -= moneyEarned * .25f;

                model.captain.Model.money += moneyEarned * .1f;
                model.money -= moneyEarned * .1f;
            }
            moneyEarned = model.money - model.moneyStats.data["Money"][model.moneyStats.data["Money"].Count - 1].y;
            model.moneyChange = moneyEarned;
            model.moneyStats.data["Money Change"].Add(new Stat(model.age.time, model.moneyChange));
            model.moneyStats.data["Money"].Add(new Stat(model.age.time, model.money));


        }

        if (target != null)
        {

            Vector3 distance = target.transform.position - transform.position;
            Polar2 angleOfAttack = new Polar2(distance);
            //float rotateAmount = angleOfAttack.angle * Mathf.Rad2Deg - transform.eulerAngles.z;
            //transform.Rotate(0, 0, rotateAmount * model.rotateSpeed * Time.deltaTime);
            distance.Normalize();
            transform.Translate(distance * model.speed * Time.deltaTime);
            travelCounter += Time.deltaTime;
            if (travelCounter > model.fuelEfficiency)
            {
                model.fuel.amount--;
                travelCounter = 0;
            }

            distance = target.transform.position - transform.position;

            line.SetPositions(new Vector3[] { transform.position, target.transform.position });
            line.startColor = sprite.color;
            line.endColor = target.GetComponent<SpriteRenderer>().color;

            if (distance.sqrMagnitude < 1)
            {
                StationController station = target.GetComponent<StationController>();
                target = null;
                if (mode == "buy")
                {
                    station.RemoveIncoming(model);

                    if (model.item.name == "Fuel")
                    {
                        model.fuel.amount += model.item.amount;
                        FindTradeRoute();
                    }
                    else if (model.item.name == "Ship")
                    {
                        model.item = station.Buy(model.item.name, model.item.amount, model);
                        model.money -= model.item.totalPrice - 1000;
                        model.owner.Model.money += 1000;
                        FindTradeRoute();
                    }
                    else
                    {
                        if (model.item.amount <= 0)
                        {
                            FindTradeRoute();
                        }

                        else
                        {
                            mode = "sell";
                            sprite.color = Color.green;
                            target = sellTarget;
                            if (sellTarget == null)
                                FindTradeRoute();
                            else
                            {
                                station = target.GetComponent<StationController>();
                                station.AddIncoming(model);
                            }

                        }
                    }

                }
                else if (mode == "sell")
                {
                    station.SellComplete(model.item);
                    station.RemoveIncoming(model);
                    FindTradeRoute();
                }
            }


        }
        else if (waitTime > updateCount)
        {
            waitTime = 0;
            FindTradeRoute();
        }
        else
        {
            waitTime += Time.deltaTime;
        }

        if (model.money < 0)
        {
            if (mode == "sell" && target != null)
            {
                StationController station = target.GetComponent<StationController>();
                station.SellIncomplete(model.item);
            }
            print(model.name + " Died");
            model.Delete();
        }

    }

    private GameObject FindClosestStation(string itemName)
    {
        GameObject[] stations = GameObject.FindGameObjectsWithTag("station");
        float distance = 100000000;
        GameObject foundStation = null;
        foreach (GameObject station in stations)
        {
            StationController stationA = station.GetComponent<StationController>();

            foreach (Items outputItem in stationA.GetOutputItems())
            {
                if (outputItem.name == itemName && (station.transform.position - transform.position).sqrMagnitude < distance && outputItem.amount > 0)
                {
                    distance = (station.transform.position - transform.position).sqrMagnitude;
                    foundStation = station;

                }
            }
        }

        return foundStation;
    }

    private void FindTradeRoute()
    {
        mode = "sell";
        if ((float)model.fuel.amount / model.fuelCapacity < .1)
        {
            target = FindClosestStation("Fuel");
            if (target != null)
            {
                StationController station = target.GetComponent<StationController>();
                model.item = new Items("Fuel", model.fuelCapacity);
                sprite.color = Color.yellow;
                mode = "buy";
                model.item = station.Buy(model.item.name, model.item.amount);
                model.money -= model.item.totalPrice;
                station.AddIncoming(model);
                return;
            }
            else if (model.fuel.amount < 0) { return; }
        }

        if (model.money > 4000)
        {
            GameObject pTarget = FindClosestStation("Ship");
            if (pTarget != null)
            {
                StationController station = pTarget.GetComponent<StationController>();
                foreach (Items outputItem in station.GetOutputItems())
                {
                    if (outputItem.name == "Ship")
                    {
                        if (outputItem.price + 2000 < model.money)
                        {
                            target = station.gameObject;
                            model.item = new Items("Ship", 1);
                            sprite.color = Color.cyan;
                            mode = "buy";
                            station.AddIncoming(model);
                            return;
                        }
                    }
                }
            }
        }

        GameObject[] stations = GameObject.FindGameObjectsWithTag("station");
        float profitability = 0;
        foreach (GameObject station in stations)
        {
            StationController stationA = station.GetComponent<StationController>();

            foreach (Items inputItem in stationA.GetInputItems())
            {
                foreach (GameObject otherStation in stations)
                {
                    StationController stationB = otherStation.GetComponent<StationController>();

                    foreach (Items outputItem in stationB.GetOutputItems())
                    {
                        int amountToBuy = model.capacity;
                        if (outputItem.amount < amountToBuy)
                            amountToBuy = outputItem.amount;
                        if (amountToBuy * inputItem.price > stationA.money - 1000)
                        {
                            amountToBuy = (int)((stationA.money - 1000) / inputItem.price);
                        }
                        if (amountToBuy < 0)
                            amountToBuy = 0;
                        if (amountToBuy * outputItem.price > model.money)
                        {
                            amountToBuy = (int) (model.money / outputItem.price);
                        }
                        float distanceToTargetCost = (otherStation.transform.position - transform.position).magnitude / model.speed / model.fuelEfficiency * 5;
                        float routeDistanceCost = (station.transform.position - otherStation.transform.position).magnitude / model.speed / model.fuelEfficiency * 5;

                        if (inputItem.name == outputItem.name && ((inputItem.price - outputItem.price) * amountToBuy - distanceToTargetCost - routeDistanceCost > profitability))
                        {
                            profitability = (inputItem.price - outputItem.price) * amountToBuy - distanceToTargetCost - routeDistanceCost;

                            target = otherStation;
                            sellTarget = station;

                            model.item = new Items(inputItem.name, model.capacity);
                            sprite.color = Color.blue;
                            mode = "buy";
                        }
                    }
                }
            }


        }

        if (mode == "buy")
        {
            StationController station = target.GetComponent<StationController>();

            model.item = station.Buy(model.item.name, model.item.amount);
            model.money -= model.item.totalPrice;

            station.AddIncoming(model);

            station = sellTarget.GetComponent<StationController>();
            model.money += station.Sell(model.item);
        }
    }

    public string GetInfo()
    {
        string info = "";
        string targetName = "---";
        if (target != null)
            targetName = target.name;

        info += string.Format("Ship Name: {0}\nMoney: {3}\n\nOwner: {11}\nCaptain: {12}\n Number Workers: {13}/{14}\n\nMode: {1}\nCargo: {4} - {8}/{5}\nTarget: {2}\nSpeed: {9}\nFuel: {6}/{7}\nFuel Efficeincy: {10}\n\n",
            model.name, mode, targetName, model.money, model.item.coloredName, model.capacity, model.fuel.amount, model.fuelCapacity, model.item.amount, model.speed, model.fuelEfficiency, model.owner.Model.name, model.captain.Model.name, model.workers.Count, model.workerCapacity);

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
}

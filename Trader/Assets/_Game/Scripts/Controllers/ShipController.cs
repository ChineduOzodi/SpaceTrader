using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class ShipController : Controller<ShipModel> {

    internal string info;
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
    
    protected override void OnInitialize()
    {
        game = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();
        sprite = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();
        transform.position = model.position;
        name = model.name;
        transform.localScale = Vector3.one * (model.capacity / 200f + .5f);
        FindTradeRoute();
    }
    protected override void OnModelChanged()
    {
        target = null;
    }
    // Update is called once per frame
    void Update () {
        model.money -= model.runningCost * Time.deltaTime;
        UpdateInfo();
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
                        model.money -= model.item.totalPrice;
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
            StationController station = target.GetComponent<StationController>();
            model.item = new Items("Fuel", model.fuelCapacity);
            sprite.color = Color.yellow;
            mode = "buy";
            model.item = station.Buy(model.item.name, model.item.amount);
            model.money -= model.item.totalPrice;
            station.AddIncoming(model);
            return;
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
                        if (outputItem.price + 1000 < model.money)
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
                        float distanceToTargetCost = (otherStation.transform.position - transform.position).magnitude/model.speed/model.fuelEfficiency * 5;
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

    private void UpdateInfo()
    {
        info = "";
        string targetName = "---";
        if (target != null)
            targetName = target.name;
        
        info += string.Format("Ship Name: {0}\nMoney: {3}\nMode: {1}\nCargo: {4} - {8}/{5}\nTarget: {2}\nSpeed: {9}\nFuel: {6}/{7}\nFuel Efficeincy: {10}", model.name, mode, targetName, model.money, model.item.name, model.capacity, model.fuel.amount, model.fuelCapacity, model.item.amount, model.speed, model.fuelEfficiency);
    }
}

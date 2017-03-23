using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;

public class ShipController : Controller<ShipModel>
{
    internal int mapMask;
    internal int solarMask;
    internal float travelCounter = 0;
    internal float updateCount = 10;
    internal float waitTime = 0;
    internal GameManager game;
    internal SpriteRenderer sprite;
    internal LineRenderer line;
    internal float timeUpdate = 0;
    private CreateGalaxy galaxy;
    private Unit unit;

    internal int starIndex
    {
        get
        {
            return model.solar.starIndex;
        }
    }
    internal bool hyperspace
    {
        get { return model.hyperSpace; }
    }

    protected override void OnInitialize()
    {
        game = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();
        galaxy = game.galaxy;
        unit = GetComponent<Unit>();
        sprite = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();
        solarMask = gameObject.layer;
        mapMask = game.gameObject.layer;        

        name = model.name;

        model.moneyStats = new DataGraph("Money Over Time", "Time (hours)", "Money");
        model.moneyStats.data.Add("Money", new List<Stat>() { new Stat(model.age.hour, model.money) });
        model.moneyStats.data.Add("Money Change", new List<Stat>());

        transform.localScale = Vector3.one * (model.capacity / 200f + .5f);
        timeUpdate = model.age.time + Date.Hour;
    }
    protected override void OnModelChanged()
    {
        if (model.solar.starIndex != model.target.Model.solar.starIndex)
        {
            HyperSpaceTravel(model.target.Model.solar.starIndex);
        }
        

        
    }
    // Update is called once per frame
    void Update()
    {
        if (model.hyperSpace)
        {
            line.enabled = true;
            sprite.enabled = true;
            transform.localScale = Vector3.one * (model.capacity / 200f + .5f) * 2;
            line.startWidth = transform.localScale.x * .3f;
            line.endWidth = transform.localScale.x * .3f;
        }
        else
        {
            gameObject.layer = solarMask;
            if (!galaxy.stars[model.solar.starIndex].isActive)
            {
                sprite.enabled = false;
                line.enabled = false;
            }
            else
            {
                sprite.enabled = true;
                line.enabled = true;

                transform.localScale = Vector3.one * (model.capacity / 200f + .5f) * Mathf.Pow(game.localScaleMod, 1.7f) * .1f;
                //Set orbit outline
                line.startWidth = transform.localScale.x * .3f;
                line.endWidth = transform.localScale.x * .3f;
            }
        }
    }

    IEnumerator SolarTravel()
    {
        while (model.target != null && model.target.Model != null && !model.hyperSpace)
        {
            //Vector3 distance = target.transform.position - transform.position;
            //Polar2 angleOfAttack = new Polar2(distance);
            //float rotateAmount = angleOfAttack.angle * Mathf.Rad2Deg - transform.eulerAngles.z;
            //transform.Rotate(0, 0, rotateAmount * model.rotateSpeed * Time.deltaTime);
            //distance.Normalize();
            Vector3 targetPosition = model.target.Model.solar.GetWorldPosition(game.data.date.time);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, model.speed * Time.deltaTime);
            travelCounter += Time.deltaTime;
            if (travelCounter > model.fuelEfficiency)
            {
                model.fuel.amount--;
                travelCounter = 0;
            }

            Vector2 distance = targetPosition - transform.position;

            line.SetPositions(new Vector3[] { transform.position, targetPosition });
            line.startColor = sprite.color;
            line.endColor = ((StationModel)model.target.Model).color;

            if (distance.sqrMagnitude < 1)
            {
                new WaitForSeconds(model.item.amount * .1f);
                StationModel station = (StationModel) model.target.Model;
                model.target = null;
                if (model.mode == ShipMode.Buy)
                {
                    station.incomingShips.Remove(model);

                    if (model.item.name == "Fuel")
                    {
                        model.fuel.amount += model.item.amount;
                        model.mode = ShipMode.Idle;
                    }
                    else if (model.item.name == "Ship")
                    {
                        model.item = station.Buy(model.item.name, model.item.amount, model);
                        model.money -= model.item.totalPrice - 1000;
                        model.owner.Model.money += 1000;
                        model.mode = ShipMode.Idle;
                    }
                    else
                    {
                        if (model.item.amount <= 0)
                        {
                            model.mode = ShipMode.Idle;
                        }
                        else
                        {
                            model.mode = ShipMode.Sell;
                            model.target = model.sellTarget;
                            sprite.color = Color.green;
                        }
                    }

                }
                else if (model.mode == ShipMode.Sell)
                {
                    station.SellComplete(model.item);
                    station.incomingShips.Remove(model);
                    model.mode = ShipMode.Idle;
                }

                yield break;
            }

            yield return null;
        }

        
    }

    private void HyperSpaceTravel(int starIndex)
    {
        StopCoroutine("SolarTravel");
        if (model.solar.starIndex == starIndex)
        {
            HyperSpaceDone();
        }
        else
        {
            model.hyperSpace = true;
            gameObject.layer = mapMask;
            transform.position = galaxy.stars[model.solar.starIndex].position;
            unit.HyperSpaceTravel(model.solar.starIndex, starIndex, model.speed);

            SolarBody parent = galaxy.stars[starIndex].sun;
            Polar2 position = new Polar2(UnityEngine.Random.Range(parent.bodyRadius + 2, parent.SOI), UnityEngine.Random.Range(0, 2 * Mathf.PI));
            model.solar = new SolarBody(model.name, starIndex, SolarType.Structure, position, .0001f, Color.black, CreateGalaxy.G, parent);
            model.hyperSpacePosition = galaxy.stars[starIndex].position;
        }
        
        
    }

    public void HyperSpaceDone()
    {
        model.hyperSpace = false;
        transform.position = model.solar.GetWorldPosition(game.data.date.time);
        StartCoroutine("SolarTravel");
    }

    public string GetInfo()
    {
        string info = "";
        string targetName = "---";
        if (model.target != null && model.target.Model != null)
            targetName = model.target.Model.name;

        info += string.Format("Ship Name: {0}\nMoney: {3}\n\nOwner: {11}\nCaptain: {12}\n Number Workers: {13}/{14}\n\nMode: {1}\nCargo: {4} - {8}/{5}\nTarget: {2}\nSpeed: {9}\nFuel: {6}/{7}\nFuel Efficeincy: {10}\n\n",
            model.name, model.mode, targetName, model.money, model.item.coloredName, model.capacity, model.fuel.amount, model.fuelCapacity, model.item.amount, model.speed, model.fuelEfficiency, model.owner.Model.name, model.captain.Model.name, model.workers.Count, model.workerCapacity);

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

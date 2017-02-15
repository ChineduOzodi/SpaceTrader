using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class StationController : Controller<StationModel> {


    internal string info;
    internal GameManager game;
    internal SpriteRenderer sprite;
    internal float money
    {
        get
        {
            return model.money;
        }
    }
    protected override void OnInitialize()
    {
        sprite = GetComponent<SpriteRenderer>();
        name = model.name;
        sprite.color = model.color;
        transform.position = model.position;
        game = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update() {

        int factoryStatus = model.factory.UpdateProduction(Time.deltaTime * Time.timeScale);
        model.money -= model.runningCost * Time.deltaTime;
        transform.localScale = Vector3.one * (model.money / 100000f + .5f);
        foreach (Items item in model.factory.outputItems)
        {
            if (item.selfProducing)
            {
                model.money += item.basePrice * item.amount;
                item.amount = 0;
            }
        }

        UpdateInfo();
        if (money < 0)
        {
            foreach (ShipModel ship in model.incomingShips)
            {
                ship.NotifyChange();
            }
            print(model.name + " Died");
            model.Delete();
        }
        else if (money > 150000)
        {
            int index = UnityEngine.Random.Range(0, 10);
            Vector3 randomLocation = new Vector3(UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-20, 20));
            StationCreator.CreateStation((FactoryType)index, transform.position + randomLocation, transform);
            model.money -= 75000;
            print(name + " Bought " + (FactoryType)index);
        }
    }

    private void UpdateInfo()
    {
        info = "";
        info += "Factory Name: " + model.factory.name + "\nMoney: " + model.money + "\n";
        info += "Progress: " + (model.factory.productionTime / model.factory.unitTime).ToString("0.00") + " - " + model.factory.unitTime + "\n\n";

        foreach (Items item in model.factory.inputItems)
        {
            if (item.selfProducing == false)
                info += "Input " + item.name + ": " + item.amount + " (" + (item.pendingAmount - item.amount) + ")/" + item.maxAmount + "| Price: " + item.price.ToString("0.00") + " - " +item.basePrice +  "\n";
            else
                info += "Input " + item.name + ": ---\n";
        }
        foreach (Items item in model.factory.outputItems)
        {
            info += "Output " + item.name + ": " + item.amount + "/" + item.maxAmount + "| Price: " + item.price.ToString("0.00") + " - " + item.basePrice + "\n";
        }

        List<ShipModel> sortedShips = new List<ShipModel>();
        foreach (ShipModel shipModel in model.incomingShips)
        {
            sortedShips.Add(shipModel);
        }
        sortedShips.Sort(delegate (ShipModel c1, ShipModel c2) { return c2.item.amount.CompareTo(c1.item.amount); });

        for (int i = 0; i < sortedShips.Count; i++)
        {
            info += string.Format("\n{0}. {1} - {2} | {3} - {4}", i + 1, sortedShips[i].name, sortedShips[i].money.ToString("0.00"), sortedShips[i].item.name, sortedShips[i].item.amount);
        }
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
                    if (buyer != null )
                    {
                        game.ships.Add(ShipCreator.CreateShip(buyer.name + "." + buyer.index, transform.position));
                        buyer.index++;
                        buyer.item.totalPrice = soldItem.totalPrice;
                    }
                    else game.ships.Add(ShipCreator.CreateShip("Freight Ship", transform.position));
                }

                return soldItem;
            }

        }

        return null;
    }

    internal float Sell(Items sellItem)
    {
        foreach (Items item in model.factory.inputItems)
        {
            if (item.name == sellItem.name)
            {
                item.pendingAmount += sellItem.amount;

                model.money -= item.price * sellItem.amount;
                float price = item.price * sellItem.amount;
                model.factory.SetPrices();
                return price;
            }
        }

        return -1;
    }
    internal void SellComplete(Items sellItem)
    {
        foreach (Items item in model.factory.inputItems)
        {
            if (item.name == sellItem.name)
            {
                item.amount += sellItem.amount;
            }
        }
    }
    internal void SellIncomplete(Items sellItem)
    {
        foreach (Items item in model.factory.inputItems)
        {
            if (item.name == sellItem.name)
            {
                item.pendingAmount -= sellItem.amount;
                model.money += item.price * sellItem.amount;
                model.factory.SetPrices();
            }
        }
    }

    internal Items[] GetInputItems()
    {
        return model.factory.inputItems;
    }

    internal Items[] GetOutputItems()
    {
        return model.factory.outputItems;
    }

    internal void AddIncoming(ShipModel ship)
    {
        model.incomingShips.Add(ship);
    }
    internal void RemoveIncoming(ShipModel ship)
    {
        model.incomingShips.Remove(ship);
    }
}

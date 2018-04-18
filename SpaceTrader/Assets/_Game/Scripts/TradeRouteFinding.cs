﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class TradeRouteFinding : MonoBehaviour
{

    public bool displayGizmos = false;
    TradeRouteRequestManager requestManager;

    private GameManager game;
    private ViewManager galaxy;
    // Use this for initialization
    void Awake()
    {
        game = GetComponent<GameManager>();
        galaxy = GetComponent<ViewManager>();
        requestManager = GetComponent<TradeRouteRequestManager>();

    }

    internal void StartTradeRouteSearch(Ship model)
    {
        StartCoroutine(FindTradeRoute(model));
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
        //Gizmos.DrawCube(transform.position, new Vector3(10, 10));
    }
    IEnumerator FindTradeRoute(Ship model)
    {
        ItemsModel item = new ItemsModel();
        bool success = false;
        Station[] targets = new Station[2];
        while (true)
        {
            model.SetShipAction(ShipAction.Refueling);
            double profitability = 0;
            //foreach (Station sellStation in game.data.stations)
            //{

            //    //foreach (ProductionItem inputItem in sellStation.factory.inputItems)
            //    //{
            //    //    foreach (StationModel buyStation in game.data.stations)
            //    //    {
            //    //        foreach (ProductionItem outputItem in buyStation.factory.outputItems)
            //    //        {
            //    //            float amountToBuy = model.capacity;
            //    //            if (outputItem.amount < amountToBuy)
            //    //                amountToBuy = outputItem.amount;
            //    //            if (amountToBuy * inputItem.price > sellStation.money - 1000)
            //    //            {
            //    //                amountToBuy = (int)((sellStation.money - 1000) / inputItem.price);
            //    //            }
            //    //            if (amountToBuy < 0)
            //    //                amountToBuy = 0;
            //    //            if (amountToBuy * outputItem.price > model.money)
            //    //            {
            //    //                amountToBuy = (int)(model.money / outputItem.price);
            //    //            }

            //    //            //double stationBDistance = (game.data.stars[buyStation.solarIndex].galacticPosition - model.galaxyPosition).magnitude;
            //    //            //stationBDistance += (buyStation.GamePosition(game.data.date.time) - model.GamePosition(game.data.date.time)).magnitude;
            //    //            //double routeDistance = (game.data.stars[sellStation.solarIndex].galacticPosition - game.data.stars[buyStation.solarIndex].galacticPosition).magnitude;
            //    //            //routeDistance += (sellStation.GamePosition(game.data.date.time) - buyStation.GamePosition(game.data.date.time)).magnitude;

            //    //            //double distanceToTargetCost = stationBDistance / model.speed / model.fuelEfficiency;
            //    //            //double routeDistanceCost = routeDistance / model.speed / model.fuelEfficiency;
            //    //            ////print("will make: " + (inputItem.price - outputItem.price) * amountToBuy);
            //    //            ////print("will lose: " + (distanceToTargetCost + routeDistanceCost));
            //    //            //if (inputItem.name == outputItem.name && ((inputItem.price - outputItem.price) * amountToBuy - distanceToTargetCost - routeDistanceCost > profitability))
            //    //            //{
            //    //            //    profitability = (inputItem.price - outputItem.price) * amountToBuy - distanceToTargetCost - routeDistanceCost;
            //    //            //    success = true;
            //    //            //    targets[0] = buyStation;
            //    //            //    targets[1] = sellStation;
            //    //            //    item = new Items(inputItem.name, inputItem.itemType, model.capacity);
            //    //            //    model.spriteColor = Color.blue;
            //    //            //}
            //    //        }
            //    //    }
            //    //}

            //    yield return null;
            //}
            //requestManager.FinishedProcessingRoute(model, item, targets, success);
            
            yield break;
        }

    }
   
}

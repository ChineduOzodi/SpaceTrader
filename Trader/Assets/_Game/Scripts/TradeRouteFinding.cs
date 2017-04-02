using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class TradeRouteFinding : MonoBehaviour
{

    public bool displayGizmos = false;
    TradeRouteRequestManager requestManager;

    private int maxLocalSize = 100 * 100;
    private GameManager game;
    private CreateGalaxy galaxy;
    // Use this for initialization
    void Awake()
    {
        game = GetComponent<GameManager>();
        galaxy = GetComponent<CreateGalaxy>();
        requestManager = GetComponent<TradeRouteRequestManager>();

    }

    internal void StartTradeRouteSearch(ShipModel model)
    {
        StartCoroutine(FindTradeRoute(model));
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
        //Gizmos.DrawCube(transform.position, new Vector3(10, 10));
    }
    IEnumerator FindTradeRoute(ShipModel model)
    {
        Items item = new Items();
        bool success = false;
        StationModel[] targets = new StationModel[2];
        while (true)
        {
            model.mode = ShipMode.SearchingTradeRoute;
            float profitability = 0;
            foreach (StationModel sellStation in game.data.stations)
            {

                foreach (ProductionItem inputItem in sellStation.factory.inputItems)
                {
                    foreach (StationModel buyStation in game.data.stations)
                    {
                        foreach (ProductionItem outputItem in buyStation.factory.outputItems)
                        {
                            int amountToBuy = model.capacity;
                            if (outputItem.amount < amountToBuy)
                                amountToBuy = outputItem.amount;
                            if (amountToBuy * inputItem.price > sellStation.money - 1000)
                            {
                                amountToBuy = (int)((sellStation.money - 1000) / inputItem.price);
                            }
                            if (amountToBuy < 0)
                                amountToBuy = 0;
                            if (amountToBuy * outputItem.price > model.money)
                            {
                                amountToBuy = (int)(model.money / outputItem.price);
                            }

                            float stationBDistance = (galaxy.stars[buyStation.solar.starIndex].position - model.hyperSpacePosition).magnitude;
                            stationBDistance += (buyStation.solar.GetWorldPosition(game.data.date.time) - model.solar.GetWorldPosition(game.data.date.time)).magnitude;
                            float routeDistance = (galaxy.stars[sellStation.solar.starIndex].position - galaxy.stars[buyStation.solar.starIndex].position).magnitude;
                            routeDistance += (sellStation.solar.GetWorldPosition(game.data.date.time) - buyStation.solar.GetWorldPosition(game.data.date.time)).magnitude;

                            float distanceToTargetCost = stationBDistance / model.speed / model.fuelEfficiency;
                            float routeDistanceCost = routeDistance / model.speed / model.fuelEfficiency;
                            //print("will make: " + (inputItem.price - outputItem.price) * amountToBuy);
                            //print("will lose: " + (distanceToTargetCost + routeDistanceCost));
                            if (inputItem.name == outputItem.name && ((inputItem.price - outputItem.price) * amountToBuy - distanceToTargetCost - routeDistanceCost > profitability))
                            {
                                profitability = (inputItem.price - outputItem.price) * amountToBuy - distanceToTargetCost - routeDistanceCost;
                                success = true;
                                targets[0] = buyStation;
                                targets[1] = sellStation;
                                item = new Items(inputItem.name, inputItem.itemType, model.capacity);
                                model.spriteColor = Color.blue;
                            }
                        }
                    }
                }

                yield return null;
            }
            requestManager.FinishedProcessingRoute(model, item, targets, success);
            
            yield break;
        }

    }
   
}

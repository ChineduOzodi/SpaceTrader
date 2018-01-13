using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class TradeRouteRequestManager : MonoBehaviour {

    Queue<TradeRequest> tradeRequestQueue = new Queue<TradeRequest>();
    TradeRequest currentTradeRequest;

    internal static TradeRouteRequestManager instance;
    TradeRouteFinding tradeRoute;

    bool isProcessingPath;

    void Awake()
    {
        instance = this;
        tradeRoute = GetComponent<TradeRouteFinding>();
    }

    public static void RequestTradeRoute(Ship model, Action<Ship, ItemsModel, StructureModel[], bool> callback)
    {
        TradeRequest newRequest = new TradeRequest(model, callback);

        instance.tradeRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && tradeRequestQueue.Count > 0)
        {
            currentTradeRequest = tradeRequestQueue.Dequeue();
            isProcessingPath = true;
            tradeRoute.StartTradeRouteSearch(currentTradeRequest.model);
        }
    }

    public void FinishedProcessingRoute(Ship model, ItemsModel tradeItem, StructureModel[] targets, bool success)
    {
        currentTradeRequest.callback(model, tradeItem, targets, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct TradeRequest
    {
        public Ship model;
        public Action<Ship, ItemsModel, StructureModel[], bool> callback;

        public TradeRequest(Ship _model, Action<Ship, ItemsModel, StructureModel[],bool> _callback)
        {
            model = _model;
            callback = _callback;
        }
    }
}

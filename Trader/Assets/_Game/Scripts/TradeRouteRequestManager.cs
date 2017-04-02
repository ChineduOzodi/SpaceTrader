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

    public static void RequestTradeRoute(ShipModel model, Action<ShipModel, Items, StructureModel[], bool> callback)
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

    public void FinishedProcessingRoute(ShipModel model, Items tradeItem, StructureModel[] targets, bool success)
    {
        currentTradeRequest.callback(model, tradeItem, targets, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct TradeRequest
    {
        public ShipModel model;
        public Action<ShipModel, Items, StructureModel[], bool> callback;

        public TradeRequest(ShipModel _model, Action<ShipModel, Items, StructureModel[],bool> _callback)
        {
            model = _model;
            callback = _callback;
        }
    }
}

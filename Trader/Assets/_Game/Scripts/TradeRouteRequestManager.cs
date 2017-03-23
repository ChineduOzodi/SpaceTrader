using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class TradeRouteRequestManager : MonoBehaviour {

    Queue<TradeRequest> tradeRequestQueue = new Queue<TradeRequest>();
    TradeRequest currentTradeRequest;

    static TradeRouteRequestManager instance;
    TradeRouteFinding tradeRoute;

    bool isProcessingPath;

    void Awake()
    {
        instance = this;
        tradeRoute = GetComponent<TradeRouteFinding>();
    }

    public static void RequestTradeRoute(ShipModel model, Action<StructureModel[], bool> callback)
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

    public void FinishedProcessingRoute(StructureModel[] targets, bool success)
    {
        currentTradeRequest.callback(targets, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct TradeRequest
    {
        public ShipModel model;
        public Action<StructureModel[], bool> callback;

        public TradeRequest(ShipModel _model, Action<StructureModel[],bool> _callback)
        {
            model = _model;
            callback = _callback;
        }
    }
}

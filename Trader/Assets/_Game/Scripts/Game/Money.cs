using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Money {

    public double credits { get; private set; }

    public bool trackCurrency { get; private set; }

    public List<Vector2> creditsStatsMonthly { get; private set; }
    public List<Vector2> creditsStatsYearly { get; private set; }

    public Money(double _credits, bool _trackCurrency)
    {
        credits = _credits;
        trackCurrency = _trackCurrency;

        creditsStatsMonthly = new List<Vector2>();
        creditsStatsYearly = new List<Vector2>();
    }
}

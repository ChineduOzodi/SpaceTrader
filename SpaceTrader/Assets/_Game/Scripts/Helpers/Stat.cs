using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Stat {

    public string groupName;
    public double x;
    public double y;

    public Stat(double x, double y)
    {
        groupName = "";
        this.x = x;
        this.y = y;
    }
}

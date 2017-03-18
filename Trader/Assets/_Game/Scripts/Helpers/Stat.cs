using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Stat {

    public string groupName;
    public float x;
    public float y;

    public Stat(float x, float y)
    {
        groupName = "";
        this.x = x;
        this.y = y;
    }
}

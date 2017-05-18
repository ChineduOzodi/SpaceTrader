using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class CreatureModel : IdentityModel {

    public CreatureModel() { }

    public CreatureModel(string name)
    {
        this.name = name;
        GameManager.instance.data.creatures.Add(this);
    }
    public CreatureModel(string name, float money)
    {
        this.name = name;
        this.money = money;
        GameManager.instance.data.creatures.Add(this);
    }
}

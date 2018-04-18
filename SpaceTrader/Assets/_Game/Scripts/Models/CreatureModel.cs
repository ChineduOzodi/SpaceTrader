using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using System;

public class CreatureModel : Model {


    public List<Creature> creatures { get; private set; }
    

    public CreatureModel() { creatures = new List<Creature>(); }

    public CreatureModel(List<Creature> _creatures)
    {
        creatures = _creatures;
    }
    public void AddCreature(Creature creature)
    {
        creatures.Add(creature);
    }

    public Creature GetCreature(string id)
    {
        return creatures.Find(x => x.id == id);
    }

    

    
}

public class Creature: PositionEntity
{
    public CreatureType creatureType;

    public Dated age { get; private set; }
    public Dated born { get; private set; }

    public bool isPlayer;

    public Money money;

    public Creature(string _name, double credits, string referenceId, Dated currentDate, Dated _age, CreatureType type):
        base(referenceId)
    {
        name = _name;
        isPlayer = false;
        creatureType = type;
        money = new Money(credits, false);
        //this.solarIndex = solarIndex; TODO: fix solarIndex
        age = _age;
        born = currentDate - age;
        GameManager.instance.data.creatures.Model.AddCreature(this);
    }

    public Creature(string _name, double credits, Structure structure, Dated currentDate, Dated _age, CreatureType type)
    {
        name = _name;
        isPlayer = false;
        creatureType = type;
        money = new Money(credits, false);
        //solarIndex = structure.solarIndex; TODO: Fix solar index
        age = _age;
        born = currentDate - age;
        this.referenceId = structure.id;
        GameManager.instance.data.creatures.Model.AddCreature(this);
    }

    public Creature(string _name, double credits, Ship ship, Dated currentDate, Dated _age, CreatureType type)
    {
        name = _name;
        isPlayer = false;
        creatureType = type;
        money = new Money(credits, false);
        //solarIndex = ship.solarIndex; TODO: fix solar index
        age = _age;
        born = currentDate - age;

        referenceId = ship.id;
        GameManager.instance.data.creatures.Model.AddCreature(this);
    }

}

public enum CreatureType
{
    Human,
    Liid
}


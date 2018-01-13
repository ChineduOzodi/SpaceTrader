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

    public List<Creature> GetCreature(string name)
    {
        return creatures.FindAll(x => x.name == name);
    }

    public Creature GetCreature(int id)
    {
        return creatures.Find(x => x.id == id);
    }

    

    
}

public struct Creature: IPositionEntity
{
    public string name;
    public CreatureType creatureType;
    public List<int> solarIndex { get; set; }
    public int structureId { get; set; }
    public int shipId { get; set; }
    public int id;
    public Dated age { get; private set; }
    public Dated born { get; private set; }

    public Vector2d galaxyPosition
    {
        get
        {
            if (solarIndex.Count == 3)
            {
                return GameManager.instance.data.stars[solarIndex[0]].solar.satelites[solarIndex[1]].satelites[solarIndex[2]].galaxyPosition;
            }
            else if (solarIndex.Count == 2)
            {
                return GameManager.instance.data.stars[solarIndex[0]].solar.satelites[solarIndex[1]].galaxyPosition;
            }
            else
            {
                throw new System.Exception("BuildStructure " + name + " solarIndex count incorrect: " + solarIndex.Count);
            }
        }

        set
        {
            throw new System.Exception("Can't set galaxyPosition, set solarIndex instead");
        }
    }

    public bool isPlayer;

    public Money money;

    public Creature(string _name, double credits, List<int> solarIndex, Dated currentDate, Dated _age, CreatureType type)
    {
        name = _name;
        isPlayer = false;
        creatureType = type;
        money = new Money(credits, false);
        this.solarIndex = solarIndex;
        age = _age;
        born = currentDate - age;
        id = GameManager.instance.data.id++;
        structureId = -1;
        shipId = -1;
        GameManager.instance.data.creatures.Model.AddCreature(this);
    }

    public Creature(string _name, double credits, IStructure structure, Dated currentDate, Dated _age, CreatureType type)
    {
        name = _name;
        isPlayer = false;
        creatureType = type;
        money = new Money(credits, false);
        solarIndex = structure.solarIndex;
        age = _age;
        born = currentDate - age;
        id = GameManager.instance.data.id++;
        this.structureId = structure.id;
        shipId = -1;
        GameManager.instance.data.creatures.Model.AddCreature(this);
    }

    public Creature(string _name, double credits, Ship ship, Dated currentDate, Dated _age, CreatureType type)
    {
        name = _name;
        isPlayer = false;
        creatureType = type;
        money = new Money(credits, false);
        solarIndex = ship.solarIndex;
        age = _age;
        born = currentDate - age;
        id = GameManager.instance.data.id++;
        this.structureId = -1;
        shipId = ship.id;
        GameManager.instance.data.creatures.Model.AddCreature(this);
    }

}

public enum CreatureType
{
    Human,
    Liid
}


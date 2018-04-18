using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using UnityEngine.UI;

public class InfoPanelModel : Model {

    public ModelRef<IdentityModel> target;
    public string id;
    public TargetType targetType;
    public Structure structure;
    public Creature creature;

    public InfoPanelModel() { }
    public InfoPanelModel(IdentityModel _target)
    {
        target = new ModelRef<IdentityModel>(_target);
        targetType = TargetType.Identity;
    }
    public InfoPanelModel(TargetType _targetType)
    {
        targetType = _targetType;
    }
    public InfoPanelModel(string _targetId, TargetType _targetType)
    {

        id = _targetId;
        targetType = _targetType;
    }
    public InfoPanelModel(string locationId)
    {
        this.id = locationId;
        var target = GameManager.instance.locations[locationId];
        if (target.GetType() == typeof(SolarBody))
        {
            targetType = TargetType.SolarBody;
        }
        else if (target.GetType() == typeof(Ship))
        {
            targetType = TargetType.Ship;
        }
        else
        {
            targetType = TargetType.Structure;
        }
        
    }
    public InfoPanelModel(Structure structure)
    {
        this.id = structure.id;
        targetType = TargetType.Structure;
        this.structure = structure;
    }
    public InfoPanelModel(Creature creature)
    {
        this.id = creature.id;
        targetType = TargetType.Creature;
        this.creature = creature;
    }
    public InfoPanelModel(Item item)
    {
        this.id = item.id;
        targetType = TargetType.Item;
    }
    public InfoPanelModel(RawResource raw)
    {
        this.id = raw.id;
        targetType = TargetType.RawResource;
    }
}

public enum TargetType
{
    Identity,
    Creature,
    SolarBody,
    Ship,
    Structure,
    Item,
    RawResource,
    Contract,
    Governments
}
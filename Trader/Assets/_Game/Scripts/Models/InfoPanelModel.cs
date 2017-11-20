using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using UnityEngine.UI;

public class InfoPanelModel : Model {

    public ModelRef<IdentityModel> target;
    public int targetId;
    public TargetType targetType; 
    public List<int> solarIndex;
    public Structure structure;
    public Creature creature;

    public InfoPanelModel() { }
    public InfoPanelModel(IdentityModel _target)
    {
        target = new ModelRef<IdentityModel>(_target);
        targetType = TargetType.Identity;
    }
    public InfoPanelModel(int _targetId, TargetType _targetType)
    {

        targetId = _targetId;
        targetType = _targetType;
    }
    public InfoPanelModel(List<int> solarIndex)
    {
        this.solarIndex = solarIndex;
        targetType = TargetType.SolarBody;
    }
    public InfoPanelModel(Structure structure)
    {
        this.solarIndex = structure.solarIndex;
        targetType = TargetType.Structure;
        this.structure = structure;
    }
    public InfoPanelModel(Creature creature)
    {
        this.solarIndex = creature.solarIndex;
        targetType = TargetType.Creature;
        this.creature = creature;
    }
    public InfoPanelModel(Item item)
    {
        this.targetId = item.id;
        targetType = TargetType.Item;
    }
    public InfoPanelModel(RawResource raw)
    {
        this.targetId = raw.id;
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
    RawResource
}
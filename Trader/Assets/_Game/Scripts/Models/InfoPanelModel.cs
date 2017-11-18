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
}

public enum TargetType
{
    Identity,
    Creature,
    SolarBody,
    Ship
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionEntity:Position {

    public string name { get; set; }
    public string id { get; set; }

    /// <summary>
    /// Is the entity docked?
    /// </summary>
    public bool docked { get; set; }
    /// <summary>
    /// Use to set the dock id of the entity.
    /// </summary>
    public int dockId { get; set; }
    public List<string> structureIds = new List<string>();

    public PositionEntity()
    {
        id = GetType().ToString() + GameManager.instance.data.id++.ToString();
        GameManager.instance.locations[id] = this;
    }
    public PositionEntity(string _referenceId) :
        base(_referenceId)
    {
        id = GetType().ToString() + GameManager.instance.data.id++.ToString();
        GameManager.instance.locations[id] = this;
    }
    public PositionEntity(string _referenceId, Vector3d _localPosition) :
        base(_referenceId, _localPosition)
    {
        id = GetType().ToString() + GameManager.instance.data.id++.ToString();
        GameManager.instance.locations[id] = this;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPositionEntity {

    Vector2d galaxyPosition { get; set; }
    /// <summary>
    /// The Position of the body it is/is orbiting/is on
    /// </summary>
    List<int> solarIndex { get; set; }
    /// <summary>
    /// Used to set weather the entity is inside a structure.
    /// </summary>
    int structureId { get; set; }
    /// <summary>
    /// Used to set whether the entity is inside a ship.
    /// </summary>
    int shipId { get; set; }
}

using UnityEngine;
using System.Collections;

public class RocketModel : Model {

    public float Age;
    public Color Color;
    public Vector3 StartPosition;
    public ModelRef<TurretModel> TargetTurret;
    public float Altitude;

}

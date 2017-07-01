using UnityEngine;
using System.Collections;

public class TurretModel : Model {

    public Color Color;
    public Vector3 Position;
    public ModelRef<TurretModel> TargetTurret;
    public float TimeSinceLastShot;

}

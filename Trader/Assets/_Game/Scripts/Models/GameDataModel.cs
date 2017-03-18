using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class GameDataModel : Model {

    public Date date;

    public ModelRefs<ShipModel> ships;
    public ModelRefs<StationModel> stations;
    public ModelRefs<CreatureModel> creatures;
}

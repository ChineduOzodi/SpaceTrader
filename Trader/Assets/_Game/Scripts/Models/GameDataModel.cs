using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class GameDataModel : Model {

    public Date date;

    public ModelRefs<ShipModel> ships = new ModelRefs<ShipModel>();
    public ModelRefs<StationModel> stations = new ModelRefs<StationModel>();
    public ModelRefs<CreatureModel> creatures = new ModelRefs<CreatureModel>();
    public ModelRefs<GovernmentModel> governments = new ModelRefs<GovernmentModel>();
    public ModelRefs<CompanyModel> companies = new ModelRefs<CompanyModel>();
}

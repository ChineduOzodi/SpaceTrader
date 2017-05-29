using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
/// <summary>
/// Holds all the game data to be saved in a save file
/// </summary>
public class GameDataModel : Model {

    public Date date = new Date();

    public ModelRefs<ShipModel> ships = new ModelRefs<ShipModel>();
    public ModelRefs<StationModel> stations = new ModelRefs<StationModel>();
    public ModelRefs<CreatureModel> creatures = new ModelRefs<CreatureModel>();
    public ModelRefs<GovernmentModel> governments = new ModelRefs<GovernmentModel>();
    public ModelRefs<CompanyModel> companies = new ModelRefs<CompanyModel>();
}

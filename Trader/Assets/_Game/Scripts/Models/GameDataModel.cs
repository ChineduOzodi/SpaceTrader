using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
/// <summary>
/// Holds all the game data to be saved in a save file
/// </summary>
public class GameDataModel : Model {

    public Date date = new Date();
    internal ModelRefs<SolarModel> stars = new ModelRefs<SolarModel>();
    public ModelRefs<ShipModel> ships = new ModelRefs<ShipModel>();
    public ModelRefs<StationModel> stations = new ModelRefs<StationModel>();
    public ModelRefs<CreatureModel> creatures = new ModelRefs<CreatureModel>();
    public ModelRefs<GovernmentModel> governments = new ModelRefs<GovernmentModel>();
    public ModelRefs<CompanyModel> companies = new ModelRefs<CompanyModel>();
    public Research[] research = new Research[] {new Research("Hyperdrive","Allows ships to travel at faster than light speeds", 1000, new string[] { }),
        new Research("Jumpdrive","Allows ships to jump to other star systems", 10000, new string[] {"Hyperdrive"}),
        new Research("Quarium Material","More durable ship building material", 2500, new string[] { }),
        new Research("Micro-Quarium Material","Allows ships to travel at faster than light speeds", 5000, new string[] {"Quarium Material" })
    };
}

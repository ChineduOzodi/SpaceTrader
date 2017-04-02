using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class GovernmentModel : StructureModel {

    public ModelRef<StationModel> capital = new ModelRef<StationModel>();
    public ModelRefs<CreatureModel> leaders = new ModelRefs<CreatureModel>();
    public ModelRefs<SolarModel> stars = new ModelRefs<SolarModel>();
    public ModelRefs<CompanyModel> companyAcess = new ModelRefs<CompanyModel>();
    public ModelRefs<GovernmentModel> enemyGovernments = new ModelRefs<GovernmentModel>();

    public GovernmentModel() { }

    public GovernmentModel(string _name, CreatureModel[] leaders = null)
    {
        name = _name;
    }
}

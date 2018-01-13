using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class GovernmentModel : StructureModel {

    public List<int> capital = new List<int>();
    public List<int> leaders = new List<int>();
    public ModelRefs<SolarModel> stars = new ModelRefs<SolarModel>();
    public ModelRefs<CompanyModel> trustedCompanies = new ModelRefs<CompanyModel>();
    public ModelRefs<CompanyModel> knownCompanies = new ModelRefs<CompanyModel>();
    public ModelRefs<CompanyModel> bannedCompanies = new ModelRefs<CompanyModel>();
    public ModelRefs<GovernmentModel> enemyGovernments = new ModelRefs<GovernmentModel>();
    public ModelRefs<GovernmentModel> alliedGovernments = new ModelRefs<GovernmentModel>();
    public ModelRefs<GovernmentModel> knownGovernments = new ModelRefs<GovernmentModel>();

    public GovernmentModel() {
        identityType = IdentityType.Government;
    }

    public GovernmentModel(string _name, List<int> _leaders)
    {
        name = _name;
        leaders = _leaders;
        identityType = IdentityType.Government;
        money = 100000000;
        GameManager.instance.data.governments.Add(this);
    }
}

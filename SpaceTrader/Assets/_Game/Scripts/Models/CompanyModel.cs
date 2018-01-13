using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class CompanyModel : StructureModel {

    public int ceo;

    public ModelRefs<GovernmentModel> governmentAccess = new ModelRefs<GovernmentModel>();

    

    public CompanyModel() { }
    public CompanyModel(string _name, SolarBody home, GovernmentModel[] govs, Creature _ceo)
    {
        name = _name;
        ceo = _ceo.id;
        solarIndex = home.solarIndex;
        home.companies.Add(this);
        foreach (GovernmentModel gov in govs)
        {
            governmentAccess.Add(gov);
        }
    }
    public CompanyModel(string _name, SolarBody home, GovernmentModel gov, Creature _ceo)
    {
        name = _name;
        ceo = _ceo.id;
        solarIndex = home.solarIndex;
        AddKnownSolar(GameManager.instance.data.stars[solarIndex[0]]);
        home.companies.Add(this);
        gov.trustedCompanies.Add(this);
        governmentAccess.Add(gov);
        money = 1000000;
        GameManager.instance.data.companies.Add(this);
    }
}

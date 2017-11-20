using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class CompanyModel : StructureModel {

    public int ceo;

    public ModelRefs<GovernmentModel> governmentAccess = new ModelRefs<GovernmentModel>();

    

    public CompanyModel() { }
    public CompanyModel(string _name, GovernmentModel[] govs, Creature _ceo)
    {
        name = _name;
        ceo = _ceo.id;
        solarIndex = _ceo.solarIndex;
        foreach (GovernmentModel gov in govs)
        {
            governmentAccess.Add(gov);
        }
    }
    public CompanyModel(string _name, GovernmentModel gov, Creature _ceo)
    {
        name = _name;
        ceo = _ceo.id;
        solarIndex = _ceo.solarIndex;
        gov.trustedCompanies.Add(this);
        governmentAccess.Add(gov);
        GameManager.instance.data.companies.Add(this);
    }
}

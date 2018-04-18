using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class CompanyModel : StructureModel {

    public string ceoId;

    public ModelRefs<GovernmentModel> governmentAccess = new ModelRefs<GovernmentModel>();

    

    public CompanyModel() { }
    public CompanyModel(string _name, SolarBody home, GovernmentModel[] govs, Creature _ceo)
    {
        name = _name;
        ceoId = _ceo.id;
        referenceId = home.id;
        home.companies.Add(this);
        foreach (GovernmentModel gov in govs)
        {
            governmentAccess.Add(gov);
            gov.licensedCompanies.Add(this);
        }
    }
    public CompanyModel(string _name, SolarBody home, GovernmentModel gov, Creature _ceo)
    {
        name = _name;
        ceoId = _ceo.id;
        referenceId = home.id;
        AddKnownSolarBodyId(referenceId);
        home.companies.Add(this);
        gov.localCompanies.Add(this);
        governmentAccess.Add(gov);
        gov.licensedCompanies.Add(this);
        StartingBalance(1000000);
        GameManager.instance.data.companies.Add(this);
    }

    public virtual void Update()
    {

    }
}

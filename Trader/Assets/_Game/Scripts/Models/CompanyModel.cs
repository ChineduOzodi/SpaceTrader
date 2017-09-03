using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class CompanyModel : StructureModel {

    public ModelRef<CreatureModel> ceo = new ModelRef<CreatureModel>();

    public ModelRefs<GovernmentModel> governmentAccess = new ModelRefs<GovernmentModel>();

    public List<List<int>> solarBodiesWithStructures;

    public CompanyModel() { }
    public CompanyModel(string _name, GovernmentModel[] govs, CreatureModel _ceo)
    {
        name = _name;
        ceo.Model = _ceo;
        foreach (GovernmentModel gov in govs)
        {
            governmentAccess.Add(gov);
        }
    }
    public CompanyModel(string _name, GovernmentModel gov, CreatureModel _ceo)
    {
        name = _name;
        ceo.Model = _ceo;
        governmentAccess.Add(gov);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class FuelCompany : CompanyModel {

    public FuelCompany() { }
    public FuelCompany(string _name, SolarBody home, GovernmentModel[] govs, Creature _ceo)
        :base(_name,home,govs,_ceo)
    {

    }
    public FuelCompany(string _name, SolarBody home, GovernmentModel gov, Creature _ceo)
        : base(_name, home, gov, _ceo)
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class DrillerCompany : CompanyModel {

    public string resourceId;


    public DrillerCompany() { }
    public DrillerCompany(string _name, SolarBody home, GovernmentModel[] govs, Creature _ceo)
        :base(_name,home,govs,_ceo)
    {

    }
    public DrillerCompany(string _name, SolarBody home, GovernmentModel gov, Creature _ceo)
        : base(_name, home, gov, _ceo)
    {

    }

    public override void Update()
    {
        base.Update();
        if (identityState == IdentityState.Initial)
        {
            if (structureIds.Count == 0)
            {
                BuildDriller();
            }
        }
        lastUpdated = GameManager.instance.data.date;
    }

    void BuildDriller()
    {
        for( int i = 0; i < knownSolarBodyIds.Count; i++)
        {
            //Find a location with the resource
            if (((SolarBody) GameManager.instance.locations[knownSolarBodyIds[i]]).rawResources.Find( x => x.id == resourceId) != null)
            {
                //BuildStructure build = new BuildStructure(gov, game.data.itemsData.Model.items.Find(x => x.itemType == ItemType.Factory).id, game.data.itemsData.Model.items.Find(x => x.itemType == ItemType.Factory).id, parent);
                BuildStructure build = new BuildStructure(this, knownSolarBodyIds[i],
                    (Vector3d)Random.onUnitSphere * ((SolarBody)GameManager.instance.locations[knownSolarBodyIds[i]]).bodyRadius);
                build.BuildDriller(GameManager.instance.data.itemsData.Model.blueprints.Find(x => x.itemType == ItemType.Driller).id, resourceId);
                return;
            }
        }
        
    }
}

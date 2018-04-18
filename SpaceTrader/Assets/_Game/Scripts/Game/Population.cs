using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population
{

    public int totalPopulation { get; private set; }
    public int young { get; private set; }
    public int adult { get; private set; }
    public int old { get; private set; }
    public List<Item> requiredItems { get; private set; }

    public Population() { }

    public Population(SolarBody body)
    {
        young = Random.Range(1000, 1000000);
        adult = Random.Range(1000, 10000000);
        old = Random.Range(1000, 1000000);
        totalPopulation = young + adult + old;
        //requiredItems = new List<Item>() { new Item(defaultRawResources.Goldium.ToString(), (int)defaultRawResources.Goldium, ItemType.RawMaterial, (int)(adult * .0001f), body.GetMarketPrice((int)defaultRawResources.Goldium)) };
        requiredItems = new List<Item>();
    }

    // Update is called once per frame
    public void Update(SolarBody body, double deltaTime)
    {

        var toAdult = young * deltaTime / Dated.Day * .000137;
        var toOld = adult * deltaTime / Dated.Day * .000061;
        var toDeath = old * deltaTime / Dated.Day * .00011;
        var accidentYoung = young * deltaTime / Dated.Day * .00001;
        var accidentAdult = adult * deltaTime / Dated.Day * .00002;
        var accidentOld = old * deltaTime / Dated.Day * .0001;
        var birth = adult * deltaTime / Dated.Day * .00015;

        young = (int)(young - toAdult - accidentYoung + birth);
        adult = (int)(adult + toAdult - toOld - accidentAdult);
        old = (int)(old + toOld - toDeath - accidentOld);

        totalPopulation = young + adult + old;
        //SolarModel solar = GameManager.instance.data.stars[body.solarIndex[0]];
        //foreach (Item item in requiredItems)
        //{
        //    var found = false;
        //    foreach (Structure structure in body.groundStructures)
        //    {
        //        if (structure.structureType == Structure.StructureTypes.GroundStorage)
        //        {
        //            GroundStorage groundStruct = (GroundStorage)structure;
        //            if (groundStruct.owner.Model == solar.government.Model && groundStruct.RemoveItem((Item)item))
        //            {
        //                body.RemoveBuying((int)item.id, solar.government.Model, -1, (double)item.amount);
        //                body.RemoveSelling((int)item.id, solar.government.Model, -1, (double)item.amount);
        //                found = true;
        //            }
        //        }
        //    }
        //    //Runs if there are still needed Items
        //    if (!found)
        //    {
        //        body.SetBuying(item);
        //        item.price += item.price * GameManager.instance.marketPriceMod * deltaTime * 68;
        //        if (item.price < .1)
        //        {
        //            item.price = .1f;
        //        }
        //    }
        //    else
        //    {
        //        body.SetBuying(item);
        //        item.price = body.GetMarketPrice(item.id);
        //        if (item.price < .1)
        //        {
        //            item.price = .1f;
        //        }
        //    }
        //}

    }
}

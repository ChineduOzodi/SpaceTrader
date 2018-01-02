using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

/// <summary>
/// Inherited by all center solar objects.
/// </summary>
public class SolarModel : Model, IPositionEntity {

    public string name;

    public bool isActive = false;
    /// <summary>
    /// position in galaxy in light years
    /// </summary>
    public Vector2d galaxyPosition { get; set; }
    public Color color;
    public SolarBody solar;
    public List<Ship> ships { get; private set; }
    public ModelRefs<SolarModel> nearStars = new ModelRefs<SolarModel>();
    public ModelRef<GovernmentModel> government = new ModelRef<GovernmentModel>();
    public double governmentInfluence;
    public bool isCapital;
    public int index;
    internal float localScale;

    //----------------Commerce-----------------------------//
    public ItemStorage buyList { get; private set; }
    public ItemStorage sellList { get; private set; }
    public List<SupplyDemand> supplyDemand { get; private set; }

    public List<int> solarIndex { get; set; }
    public int structureId { get; set; }
    public int shipId { get; set; }

    public SolarModel() { }

    public SolarModel(string _name, int _index, Vector2d _position, Gradient sunSizeColor)
    {
        name = _name;
        galaxyPosition = _position * Units.ly / GameDataModel.galaxyDistanceMultiplication;
        index = _index;
        structureId = -1;
        shipId = -1;
        this.solarIndex = new List<int>() { index };
        supplyDemand = new List<SupplyDemand>();
        buyList = new ItemStorage();
        sellList = new ItemStorage();

        // Create star

        SolarSubType starSubType;
        double sunMass;
        double temp;
        float sunDensity;

        float star = Random.value;
        if (star < .5f)
        {
            starSubType = SolarSubType.MainSequence;
            sunMass = Random.Range(.1f, 60);
            temp = (42000f / 599f) * sunMass + (1755000 / 599);
            sunMass *= GameDataModel.sunMassMultiplication; // Sun Mass in solar masses
            sunDensity = Random.Range(.5f, 4) * Units.k;
        }
        else if (star < .75f)
        {
            starSubType = SolarSubType.GasGiant;
            sunMass = Random.Range(60f, 100);
            temp = Random.Range(4000, 45000);
            sunMass *= GameDataModel.sunMassMultiplication; // Sun Mass in solar masses
            sunDensity = Random.Range(.1f, 2) * Units.k;
        }
        else
        {
            starSubType = SolarSubType.WhiteDwarf;
            sunMass = Random.Range(.01f, 1);
            temp = Random.Range(6000, 30000);
            sunMass *= GameDataModel.sunMassMultiplication; // Sun Mass in solar masses
            sunDensity = Random.Range(10f, 2000) * Units.k;
        }
        //float sunColorValue = (float) (sunMass / 100 / GameDataModel.sunMassMultiplication);
        //Color color = sunSizeColor.Evaluate(sunColorValue);
        Color color = ColorTempToRGB.TempToRGB((float)temp);
        double sunRadius = Mathd.Pow(((sunMass / sunDensity) * 3) / (4 * Mathd.PI), 1 / 3d) / Units.convertToMeters; // In km
        var solarIndex = new List<int>();
        solarIndex.Add(_index);

        solar = new SolarBody(_name + " " + starSubType.ToString(), solarIndex, starSubType, sunMass , sunRadius, new Orbit(), color, temp);

        int numPlanets = (int)Random.Range(0, (float)Mathd.Pow(sunMass / GameDataModel.sunMassMultiplication, .25f) * 20);
        double sunSoi = solar.SOI(sunMass);

        for (int i = 0; i < numPlanets; i++)
        {
            double sma = Random.Range((float) (solar.bodyRadius) * 1.1f, (float) (sunSoi * .1));
            float lpe = Random.Range(0, 2 * Mathf.PI);

            float satType = Random.value;
            double planetMass;
            float planetDensity;
            SolarType planetType = SolarType.Planet;
            SolarSubType planetSubType = SolarSubType.Rocky;
            float ecc = Random.Range(0, .01f);

            if (satType < .3f) //Rock planets
            {
                
                planetMass = Random.Range(1f, 100f) * 1e+23;
                planetDensity = Random.Range(3.5f, 7) * Units.k * Units.convertToMeters;
            }
            else if (satType < .4f) //Gas Giants
            {
                planetDensity = Random.Range(.5f, 3) * Units.k * Units.convertToMeters;
                planetSubType = SolarSubType.GasGiant;
                planetMass = Random.Range(1, 400) * 1e+25;
            }
            else if (satType < .6f) //Dwarf Planets
            {
                planetType = SolarType.DwarfPlanet;
                planetDensity = Random.Range(3.5f, 7) * Units.k * Units.convertToMeters;
                planetMass = Random.Range(1, 40) * 1e+21;
                ecc = Random.Range(0.01f, .5f);
            }
            else //Comets
            {
                planetDensity = Random.Range(3.5f, 7) * Units.k * Units.convertToMeters;
                planetType = SolarType.Comet;
                planetMass = Random.Range(1, 100) * 1e+13;
                ecc = Random.Range(0.5f, 1);
            }

            double planetRadius = Mathd.Pow((((planetMass) / planetDensity) * 3) / (4 * Mathd.PI), 1 / 3d) / Units.convertToMeters; // In meters
            
            

            var planetIndex = new List<int>();
            planetIndex.Add(solarIndex[0]);
            planetIndex.Add(i);

            color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

            solar.satelites.Add(new SolarBody(name + " " + planetType.ToString() + " " + (i + 1), planetIndex, planetType, planetSubType, planetMass, planetRadius, new Orbit(sma,ecc,lpe,0,lpe),color, solar));

            int numMoonRange = 0;
            float moonChance = 0;

            if (planetSubType == SolarSubType.GasGiant)
            {
                numMoonRange = 30;
                moonChance = 1;
            }
            else if (planetType == SolarType.DwarfPlanet)
            {
                numMoonRange = 2;
                moonChance = .1f;
            }
            else if (planetSubType == SolarSubType.Rocky)
            {
                numMoonRange = 5;
                moonChance = .5f;
            }
            int numMoon = Random.Range(0, numMoonRange);

            double soi = solar.satelites[i].SOI(solar.mass);

            if (Random.value < moonChance)
            {
                for (int m = 0; m < numMoon; m++)
                {
                    double moonMass = Random.Range(.0001f, 50) * 1e+22;

                    sma = Random.Range((float)solar.satelites[i].bodyRadius * 1.2f, (float)(soi * .75f));
                    lpe = Random.Range(0, 2 * Mathf.PI);
                    color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
                    float moonDensity = Random.Range(3.5f, 7) * Units.k * Units.convertToMeters;
                    double moonRadius = Mathd.Pow((((moonMass) / moonDensity) * 3) / (4 * Mathd.PI), 1 / 3d) / Units.convertToMeters; // In meters
                    SolarType moonType = SolarType.Moon;
                    SolarSubType moonSubType = SolarSubType.Rocky;
                    ecc = Random.Range(0, .02f);
                    if (moonMass < 1e+16)
                    {
                        moonType = SolarType.Asteroid;
                        ecc = Random.Range(0.02f, .5f);
                    }

                    var moonIndex = new List<int>();
                    moonIndex.AddRange(planetIndex);
                    moonIndex.Add(m);

                    solar.satelites[i].satelites.Add(new SolarBody(name + "Moon " + (i + 1), moonIndex, moonType, moonSubType, moonMass, moonRadius, new Orbit(sma, ecc, lpe, 0, lpe), color, solar));
                }
            }
            
        }
    }
    public double GetModifiedRadius(float scale)
    {
        return (Mathd.Pow((solar.bodyRadius),scale) * Camera.main.orthographicSize / GameManager.instance.data.cameraGalaxyOrtho);
    }

#region "Commerce"

    public double GetMarketPrice(int itemId)
    {
        int index = supplyDemand.FindIndex(x => x.itemId == itemId);

        if (index >= 0)
        {
            return supplyDemand[index].marketPrice;
        }
        else
        {
            return GameManager.instance.data.itemsData.Model.GetItem(itemId).estimatedValue;
        }
    }

    public void SetBuying(Item item)
    {
        int itemIndex = -1;
        if (item.structureId == -1)
        {
            itemIndex = buyList.items.FindIndex(x => x.id == item.id);
        }
        else
        {
            itemIndex = buyList.items.FindIndex(x => x.id == item.id && x.structureId == item.structureId);
        }
        if (itemIndex >= 0)
        {
            buyList.items[itemIndex].SetAmount(item.amount, item.price);
        }
        else
        {
            buyList.AddItem(item);
        }
    }

    public void RemoveBuying(int itemId, int structureId, double amount)
    {

        int itemIndex = -1;
        if (structureId == -1)
        {
            itemIndex = buyList.items.FindIndex(x => x.id == itemId);
        }
        else
        {
            itemIndex = buyList.items.FindIndex(x => x.id == itemId && x.structureId == structureId);
        }

        if (itemIndex >= 0)
        {
            buyList.items[itemIndex].RemoveAmount(amount);
            if (buyList.items[itemIndex].amount == 0)
                buyList.items.RemoveAt(itemIndex);
        }
    }

    public void SetSelling(Item item)
    {
        int itemIndex = -1;
        if (item.structureId == -1)
        {
            itemIndex = sellList.items.FindIndex(x => x.id == item.id);
        }
        else
        {
            itemIndex = sellList.items.FindIndex(x => x.id == item.id && x.structureId == item.structureId);
        }
        if (itemIndex >= 0)
        {
            sellList.items[itemIndex].SetAmount(item.amount, item.price);
        }
        else
        {
            sellList.AddItem(item);
        }
    }
    public void SetSellingPrice(Item item)
    {
        int itemIndex = -1;
        if (item.structureId == -1)
        {
            itemIndex = sellList.items.FindIndex(x => x.id == item.id);
        }
        else
        {
            itemIndex = sellList.items.FindIndex(x => x.id == item.id && x.structureId == item.structureId);
        }
        if (itemIndex >= 0)
        {
            sellList.items[itemIndex].price = item.price;
        }
    }

    public void RemoveSelling(int itemId, int structureId, double amount)
    {

        int itemIndex = -1;
        if (structureId == -1)
        {
            itemIndex = sellList.items.FindIndex(x => x.id == itemId);
        }
        else
        {
            itemIndex = sellList.items.FindIndex(x => x.id == itemId && x.structureId == structureId);
        }

        if (itemIndex >= 0)
        {
            sellList.items[itemIndex].RemoveAmount(amount);
            if (sellList.items[itemIndex].amount == 0)
                sellList.items.RemoveAt(itemIndex);
        }
    }

    #endregion
}

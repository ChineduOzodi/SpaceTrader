using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

/// <summary>
/// Inherited by all center solar objects.
/// </summary>
public class SolarModel : Model {

    public string name;

    public bool isActive = false;
    /// <summary>
    /// position in galaxy in light years
    /// </summary>
    public Vector3 galacticPosition;
    public Color color;
    public SolarBody solar;
    public ModelRefs<StationModel> stations = new ModelRefs<StationModel>();
    public ModelRefs<ShipModel> ships = new ModelRefs<ShipModel>();
    public ModelRefs<SolarModel> nearStars = new ModelRefs<SolarModel>();
    public ModelRef<GovernmentModel> government = new ModelRef<GovernmentModel>();
    public float governmentInfluence;
    public bool isCapital;
    public int index;
    internal float localScale;

    public SolarModel() { }

    public SolarModel(string _name, int _index, Vector3 _position, Gradient sunSizeColor)
    {
        name = _name;
        galacticPosition = _position;
        index = _index;
        
        // Create star
        Units sunMass = new Units(Random.Range(.1f, 100), GameDataModel.sunMassMultiplication, GameDataModel.sunMassUnit);  ; // Sun Mass in solar masses
        float sunColorValue = (float) sunMass.number / 100;
        Color color = sunSizeColor.Evaluate(sunColorValue);
        float sunDensity = Random.Range(.5f, 4) * Units.k;
        double sunRadius = Mathd.Pow(((sunMass / sunDensity) * 3) / (4 * Mathd.PI), 1 / 3d); // In meters
        var solarIndex = new List<int>();
        solarIndex.Add(_index);

        

        solar = new SolarBody(_name + " Sun", solarIndex, SolarType.Star, sunMass , new Units(sunRadius,1,"m"), new Orbit(), color);

        int numPlanets = (int)Random.Range(0, (float)Mathd.Pow(sunMass.number, .25f) * 20);

        for (int i = 0; i < numPlanets; i++)
        {
            Units sunSoi = solar.SOI(sunMass);
            Units sma = new Units(Random.Range((float) (solar.bodyRadius / GameDataModel.solarDistanceMultiplication) * 1.2f, (float) ( sunSoi/ GameDataModel.solarDistanceMultiplication)),
                GameDataModel.solarDistanceMultiplication, GameDataModel.solarDistanceUnit);
            float lpe = Random.Range(0, 2 * Mathf.PI);
            Units planetMass = new Units(Random.Range(.0001f, 500f) * 5.972e+24, 1, "kg");
            float planetDensity = Random.Range(3.5f, 7) * Units.k;
            double planetRadius = Mathd.Pow((((planetMass) / planetDensity) * 3) / (4 * Mathd.PI), 1 / 3d); // In meters

            SolarType planetType = SolarType.RockyPlanet;
            float ecc = Random.Range(0, .01f);
            if (planetMass > 50e+24)
            {
                planetDensity = Random.Range(.5f, 4) * Units.k;
                planetType = SolarType.GasGiant;
            }
            if (planetMass < 1e+23)
            {
                planetType = SolarType.DwarfPlanet;
                ecc = Random.Range(0.01f, .5f);
            }
            if (planetMass < 5e+21)
            {
                planetType = SolarType.Meteor;
                ecc = Random.Range(0.5f, 1);
            }

            var planetIndex = new List<int>();
            planetIndex.Add(solarIndex[0]);
            planetIndex.Add(i);

            color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

            solar.satelites.Add(i, new SolarBody(name + " Planet " + (i + 1), planetIndex, planetType, planetMass,new Units(planetRadius, 1, "m"), new Orbit(sma,ecc,lpe,0,lpe),color));

            int numMoonRange = (int)(solar.satelites[i].SOI(solar.mass) / Units.G);
            numMoonRange = 3;
            int numMoon = Random.Range(0, numMoonRange);

            for (int m = 0; m < numMoon; m++)
            {
                Units moonMass = new Units(Random.Range(.0001f, 50) * 1e+22, 1, "kg");
                Units soi = solar.satelites[i].SOI(solar.mass);
                sma = new Units(Random.Range((float)solar.satelites[i].bodyRadius * 1.2f, (float)(soi * .75f)) / GameDataModel.solarDistanceMultiplication, GameDataModel.solarDistanceMultiplication, GameDataModel.solarDistanceUnit);
                lpe = Random.Range(0, 2 * Mathf.PI);
                color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
                float moonDensity = Random.Range(3.5f, 7) * Units.k;
                double moonRadius = Mathd.Pow((((moonMass) / moonDensity) * 3) / (4 * Mathd.PI), 1 / 3d); // In meters
                SolarType moonType = SolarType.Moon;
                ecc = Random.Range(0, .02f);
                if (planetMass < 1e+16)
                {
                    moonType = SolarType.Asteroid;
                    ecc = Random.Range(0.02f, .5f);
                }

                var moonIndex = new List<int>();
                moonIndex.AddRange(planetIndex);
                moonIndex.Add(m);

                solar.satelites[i].satelites.Add(m,new SolarBody(name + "Moon " + (i + 1), moonIndex, moonType, moonMass, new Units(moonRadius,1,"m"),  new Orbit(sma,ecc,lpe,0,lpe), color));
            }
        }
    }
}

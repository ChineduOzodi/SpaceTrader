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
    public SolarBody sun;
    public SolarBody[] planets;
    public ModelRefs<StationModel> stations = new ModelRefs<StationModel>();
    public ModelRefs<ShipModel> ships = new ModelRefs<ShipModel>();
    public ModelRefs<SolarModel> nearStars = new ModelRefs<SolarModel>();
    public ModelRef<GovernmentModel> government = new ModelRef<GovernmentModel>();
    public float governmentInfluence;
    public bool isCapital;
    public int index;
    internal float localScale;

    public SolarModel() { }

    public SolarModel(string _name, int _index, Vector3 _position, float sunMass, float G, int numPlanets, Gradient sunSizeColor, float sunColorValue)
    {
        name = _name;
        galacticPosition = _position;
        index = _index;
        Color color = sunSizeColor.Evaluate(sunColorValue);

        sun = new SolarBody(_name + " Sun", _index, SolarType.Star,new Orbit(sunMass), color);

        planets = new SolarBody[numPlanets];

        for (int i = 0; i < numPlanets; i++)
        {

            double radius = Random.Range((float) sun.orbit.bodyRadius * 1.2f, (float) sun.orbit.soi);
            float angle = Random.Range(0, 2 * Mathf.PI);
            float planetMass = Random.Range(1f, 10f);
            color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

            planets[i] = new SolarBody(name + " Planet " + i + 1, _index, SolarType.Planet, new Orbit(planetMass,radius,0,0,0,0,sun.orbit.Mass),color);
            planets[i].index = i;
            int numMoon = Random.Range(0, 5);
            planets[i].children = new List<SolarBody>();
            for (int m = 0; m < numMoon; m++)
            {
                float moonMass = Random.Range(.1f, 1f);
                radius = Random.Range((float)planets[i].orbit.bodyRadius * 1.2f, (float)planets[i].orbit.soi);
                angle = Random.Range(0, 2 * Mathf.PI);
                color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
                planets[i].children.Add(new SolarBody(name + "Moon " + i, _index, SolarType.Moon, new Orbit(moonMass, radius, 0, 0, 0, 0, sun.orbit.Mass), color));
                planets[i].children[m].index = m;
            }
        }
    }
}

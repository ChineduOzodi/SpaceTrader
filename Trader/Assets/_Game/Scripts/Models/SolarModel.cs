using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class SolarModel : Model {

    public string name;

    public bool isActive = false;

    public Vector3 position;
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

    public SolarModel(string _name, int _index, Vector2 _position, float sunMass, float G, int numPlanets, Gradient sunSizeColor, float sunColorValue)
    {
        name = _name;
        position = _position;
        index = _index;
        Color color = sunSizeColor.Evaluate(sunColorValue);

        sun = new SolarBody(_name + " Sun", _index, SolarType.Star, Polar2.zero, sunMass, color, G, null);

        planets = new SolarBody[numPlanets];

        for (int i = 0; i < numPlanets; i++)
        {

            float radius = Random.Range(sun.bodyRadius + 15,sun.SOI);
            float angle = Random.Range(0, 2 * Mathf.PI);
            float planetMass = Random.Range(1f, 10f);
            color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

            planets[i] = new SolarBody(name + " Planet " + i + 1, _index, SolarType.Planet, new Polar2(radius,angle),planetMass,color, G, sun);

            int numMoon = Random.Range(0, 5);
            planets[i].children = new SolarBody[numMoon];
            for (int m = 0; m < numMoon; m++)
            {
                float moonMass = Random.Range(.1f, 1f);
                radius = Random.Range(planets[i].bodyRadius + 2, planets[i].SOI);
                angle = Random.Range(0, 2 * Mathf.PI);
                color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
                planets[i].children[m] = new SolarBody(name + "Moon " + i, _index, SolarType.Moon, new Polar2(radius, angle), moonMass, color, G, planets[i]);
            }
        }
    }
}

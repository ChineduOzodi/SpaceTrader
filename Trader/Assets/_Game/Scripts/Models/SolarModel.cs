using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class SolarModel : Model {

    public string name;

    public bool isActive = false;

    public Vector3 position;

    public SolarBody sun;

    public SolarBody[] planets;

    public SolarModel() { }

    public SolarModel(string _name, Vector2 _position, float sunMass, float G, int numPlanets, Gradient sunSizeColor, float sunColorValue)
    {
        name = _name;
        position = _position;
        Color color = sunSizeColor.Evaluate(sunColorValue);

        sun = new SolarBody(_name + " Sun", Polar2.zero, sunMass, color, sunMass, G, null);

        planets = new SolarBody[numPlanets];

        for (int i = 0; i < numPlanets; i++)
        {

            float radius = Random.Range(sunMass, sunMass * Mathf.Sqrt(sunMass + 10));
            float angle = Random.Range(0, 2 * Mathf.PI);
            float planetMass = Random.Range(1f, 10f);
            color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

            planets[i] = new SolarBody(name + " Planet " + i + 1, new Polar2(radius,angle),planetMass,color, sun.mass, G, sun);
        }
    }
}

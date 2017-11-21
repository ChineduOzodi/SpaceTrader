using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapTogglePanel : MonoBehaviour {

    public static MapTogglePanel instance;
    public Toggle planet;
    public Toggle moons;
    public Toggle desert;
    public Toggle earthLike;
    public Toggle ice;
    public Toggle volcanic;
    public Toggle rocky;
    public Toggle dwarfPlanet;
    public Toggle gasGiant;
    public Toggle comet;
    public Toggle asteroid;
    public Toggle ocean;
    public Toggle solarNormalVisual;
    public Toggle temperatureVisual;
    public Toggle solarOrbits;
    public Toggle populations;
    public Toggle compines;

    //Toggle Options for Galaxy
    public Toggle galaxyNormalVisual;
    public Toggle galaxyTerritory;
    public Toggle galaxyShips;
    public Toggle galaxyConnections;


    internal Dictionary<SolarSubType, Toggle> subtypes;
    internal Dictionary<VisualDisplay, Toggle> visualDisplay;

	// Use this for initialization
	void Start () {
        //if (instance != null)
        //{
        //    Destroy(gameObject);
        //}
        instance = this;

        subtypes = new Dictionary<SolarSubType, Toggle>();
        subtypes.Add(SolarSubType.Desert, desert);
        subtypes.Add(SolarSubType.EarthLike, earthLike);
        subtypes.Add(SolarSubType.Ice, ice);
        subtypes.Add(SolarSubType.Volcanic, volcanic);
        subtypes.Add(SolarSubType.Rocky, rocky);
        subtypes.Add(SolarSubType.GasGiant, gasGiant);
        subtypes.Add(SolarSubType.Ocean, ocean);

        visualDisplay = new Dictionary<VisualDisplay, Toggle>();
        visualDisplay.Add(VisualDisplay.Normal, solarNormalVisual);
        visualDisplay.Add(VisualDisplay.Temperature, temperatureVisual);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

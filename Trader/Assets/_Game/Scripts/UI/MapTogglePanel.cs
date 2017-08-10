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

    internal Dictionary<SolarSubType, Toggle> subtypes;

	// Use this for initialization
	void Start () {
        instance = this;

        subtypes = new Dictionary<SolarSubType, Toggle>();
        subtypes.Add(SolarSubType.Desert, desert);
        subtypes.Add(SolarSubType.EarthLike, earthLike);
        subtypes.Add(SolarSubType.Ice, ice);
        subtypes.Add(SolarSubType.Volcanic, volcanic);
        subtypes.Add(SolarSubType.Rocky, rocky);
        subtypes.Add(SolarSubType.GasGiant, gasGiant);
        subtypes.Add(SolarSubType.Ocean, ocean);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

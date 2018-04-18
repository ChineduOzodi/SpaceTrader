using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetCreator : MonoBehaviour {

    public GameObject planet;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetPlanetSize(float size)
    {
        planet.transform.localScale = Vector3.one * size;
    }
}

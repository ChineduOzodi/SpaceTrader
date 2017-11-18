using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetGridPanel : MonoBehaviour {

    public GameObject waterButton;
    public GameObject iceButton;
    public GameObject volcanicButton;
    public GameObject grassButton;
    public GameObject rockyButton;
    public GameObject desertButton;

	// Use this for initialization
	void Start () {
	}

    public void SelectPlanet(SolarBody body)
    {
        if (body.planetTiles == null)
            return;

        foreach (PlanetTile tile in body.planetTiles)
        {
            GameObject prefab;

            if (tile.planetTileType == PlanetTileType.Desert)
            {
                prefab = desertButton;
            }
            else if (tile.planetTileType == PlanetTileType.Grasslands)
            {
                prefab = grassButton;
            }
            else if (tile.planetTileType == PlanetTileType.Ice)
            {
                prefab = iceButton;
            }
            else if (tile.planetTileType == PlanetTileType.Ocean)
            {
                prefab = waterButton;
            }
            else if (tile.planetTileType == PlanetTileType.Rocky)
            {
                prefab = rockyButton;
            }
            else if (tile.planetTileType == PlanetTileType.Volcanic)
            {
                prefab = volcanicButton;
            }
            else
            {
                prefab = volcanicButton;
                throw new System.Exception("Unknown planet tile type");
            }

            for(int i = 0; i < tile.count; i++)
            {
                GameObject button = Instantiate(prefab, transform);
            }
            
        }
    }
}

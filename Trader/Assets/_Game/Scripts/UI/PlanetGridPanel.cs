using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetGridPanel : MonoBehaviour {

    public Text tileInfo;
    public Text title;
    internal PlanetTile tile;

    public GameObject waterButton;
    public GameObject iceButton;
    public GameObject volcanicButton;
    public GameObject grassButton;
    public GameObject rockyButton;
    public GameObject desertButton;

    public List<GameObject> tileButtons;

	// Use this for initialization
	void Start () {
        tileButtons = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if (tile != null)
        {
            tileInfo.text = tile.GetInfo();
        }
	}

    public void GetInfo(PlanetTile _tile)
    {
        tile = _tile;
    }

    public void SelectPlanet(SolarBody body)
    {
        DestroyTiles();
        tileInfo.text = "";
        title.text = body.name;

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

            GameObject button = Instantiate(prefab, transform);
            button.GetComponent<TileButton>().tile = tile;
            tileButtons.Add(button);
        }
    }

    private void DestroyTiles()
    {
        foreach(GameObject tile in tileButtons)
        {
            Destroy(tile);
        }
    }
}

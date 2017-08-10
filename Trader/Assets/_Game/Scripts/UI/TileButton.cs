using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileButton : MonoBehaviour {

    internal PlanetTile tile;

	// Use this for initialization
	void Start () {
        var button = GetComponent<Button>();
        button.onClick.AddListener(ButtonClicked);
	}

    private void ButtonClicked()
    {
        PlanetPanel.instance.GetInfo(tile);
    }
}

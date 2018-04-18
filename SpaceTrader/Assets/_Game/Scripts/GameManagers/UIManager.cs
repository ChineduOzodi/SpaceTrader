using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeControl;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class UIManager: MonoBehaviour
    {
    public Canvas menuCanvas; //Main Menu
    public Canvas settingsCanvas; //Settings Menu
    public Canvas gameCanvas; //Game UI
    public Canvas infoPanelCanvas; //Object Info popup

    private void Awake()
    {
        
    }

    //public void OnGUI()
    //{
    //    GUI.Box(new Rect(20, 20, 50, 50), Units.ReadDistance(GameManager.instance.data.cameraGalaxyOrtho * GameDataModel.galaxyDistanceMultiplication));
    //}
}

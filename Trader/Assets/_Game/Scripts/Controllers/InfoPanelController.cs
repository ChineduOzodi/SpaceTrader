using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using System;
using UnityEngine.UI;

public class InfoPanelController : Controller<InfoPanelModel> {

    public Button uiButtonInstance;
    public GameObject contentPosition;
    internal List<Button> mainInfo = new List<Button>();

    protected override void OnInitialize()
    {
        if (model.target.Model.identityType == IdentityType.Government)
        {
            GovernmentModel gov = (GovernmentModel)model.target.Model;
            Button name = Instantiate(uiButtonInstance, contentPosition.transform);
            name.GetComponent<Image>().color = gov.spriteColor;
            mainInfo.Add(name);
            name.onClick.AddListener(() => GameManager.instance.GoToTarget(model.target.Model));
            name.GetComponentInChildren<Text>().text = gov.name;
            Button button = Instantiate(uiButtonInstance, contentPosition.transform);            
            mainInfo.Add(button);
            button.GetComponentInChildren<Text>().text = "Leader: " + gov.leaders[0];
            button = Instantiate(uiButtonInstance, contentPosition.transform);
            mainInfo.Add(button);
            button.GetComponentInChildren<Text>().text = "Capital: " + gov.location.Model.name;
            button.GetComponent<Image>().color = gov.location.Model.spriteColor;
            button = Instantiate(uiButtonInstance, contentPosition.transform);
            mainInfo.Add(button);
            button.GetComponentInChildren<Text>().text = "Money: " + gov.money;
            button = Instantiate(uiButtonInstance, contentPosition.transform);
            mainInfo.Add(button);
            button.GetComponentInChildren<Text>().text = "Star Count:" + gov.stars.Count;
            float totalInfluence = 0;
            foreach (SolarModel star in gov.stars)
            {
                totalInfluence += star.governmentInfluence;
            }
            button = Instantiate(uiButtonInstance, contentPosition.transform);
            mainInfo.Add(button);
            button.GetComponentInChildren<Text>().text = "Total Influence:" + totalInfluence;

        }
    }
    public void Close()
    {
        model.Delete();
    }
    // Update is called once per frame
    void Update () {

        if (model.target.Model.identityType == IdentityType.Government)
        {
            GovernmentModel gov = (GovernmentModel)model.target.Model;
            Button button = mainInfo[mainInfo.Count - 1];
            float totalInfluence = 0;
            foreach (SolarModel star in gov.stars)
            {
                totalInfluence += star.governmentInfluence;
            }
            button.GetComponentInChildren<Text>().text = "Total Influence:" + totalInfluence;

        }

    }
}

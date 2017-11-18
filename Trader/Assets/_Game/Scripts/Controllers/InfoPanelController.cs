using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using System;
using UnityEngine.UI;

public class InfoPanelController : Controller<InfoPanelModel> {

    public Button uiButtonInstance;
    public Text title;
    public GameObject titleContent;
    public GameObject infoContentPanel;
    public PlanetGridPanel planetTiles;
    
    public GameObject contentPosition;
    internal List<Button> mainInfo = new List<Button>();
    internal List<GameObject> menuContent = new List<GameObject>();

    private bool updatePopulation = false;
    private int populationId;

    public bool fix = true;

    protected override void OnInitialize()
    {
        if (model.targetType == TargetType.Identity)
        {
            if (model.target.Model.identityType == IdentityType.Government)
            {
                GovernmentModel gov = (GovernmentModel)model.target.Model;
                title.text = gov.name;
                Button button = Instantiate(uiButtonInstance, titleContent.transform);
                button.GetComponentInChildren<Text>().text = "Overview";
                button.GetComponent<Image>().color = gov.spriteColor;
                menuContent.Add(Instantiate(infoContentPanel,contentPosition.transform));
                var index = menuContent.Count - 1;
                button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
                var position = menuContent[index].transform;

                
                button = Instantiate(uiButtonInstance, position);
                mainInfo.Add(button);
                var creature = GameManager.instance.data.creatures.Model.GetCreature(gov.leaders[0]);
                button.GetComponentInChildren<Text>().text = "Leader: " + creature.name;
                button.onClick.AddListener(() => GameManager.instance.GoToTarget(creature.solarIndex));

                button = Instantiate(uiButtonInstance, position);
                mainInfo.Add(button);
                button.GetComponentInChildren<Text>().text = "Capital: " + GameManager.instance.data.getSolarBody(gov.solarIndex).name;
                button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(gov.solarIndex));

                button = Instantiate(uiButtonInstance, position);
                mainInfo.Add(button);
                button.GetComponentInChildren<Text>().text = "Money: " + gov.money;
                button.interactable = false;

                button = Instantiate(uiButtonInstance, position);
                mainInfo.Add(button);
                button.GetComponentInChildren<Text>().text = "Star Count:" + gov.stars.Count;
                button.interactable = false;

                double totalInfluence = 0;
                foreach (SolarModel star in gov.stars)
                {
                    totalInfluence += star.governmentInfluence;
                }
                button = Instantiate(uiButtonInstance, position);
                mainInfo.Add(button);
                button.GetComponentInChildren<Text>().text = "Total Influence:" + totalInfluence;
                button.interactable = false;

            }
        }
        else if (model.targetType == TargetType.SolarBody)
        {
            SolarBody body = GameManager.instance.data.getSolarBody(model.solarIndex);
            title.text = body.name;

            if (body.solarSubType != SolarSubType.GasGiant && body.solarType != SolarType.Star)
            {
                var tiles = Instantiate(planetTiles, contentPosition.transform);
                tiles.SelectPlanet(body);
            }

            Button button = Instantiate(uiButtonInstance, titleContent.transform);
            button.GetComponentInChildren<Text>().text = "Overview";
            //name.GetComponent<Image>().color = gov.spriteColor;
            menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
            var index = menuContent.Count - 1;
            button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
            var position = menuContent[index].transform;

            if (body.solarSubType != SolarSubType.GasGiant && body.solarType != SolarType.Star)
            {
                button = Instantiate(uiButtonInstance, position);
                mainInfo.Add(button);
                button.GetComponentInChildren<Text>().text = "Population: " + body.population;
                button.interactable = false;
                updatePopulation = true;
                populationId = mainInfo.Count - 1;
                
            }

            button = Instantiate(uiButtonInstance, position);
            mainInfo.Add(button);
            button.GetComponentInChildren<Text>().text = "Space Structure Count: " + body.spaceStructures.Count;
            button.interactable = false;

            if (body.solarSubType != SolarSubType.GasGiant && body.solarType != SolarType.Star)
            {
                button = Instantiate(uiButtonInstance, position);
                mainInfo.Add(button);
                button.GetComponentInChildren<Text>().text = "Ground Structure Count:" + body.groundStructures.Count;
                button.interactable = false;

                button = Instantiate(uiButtonInstance, position);
                mainInfo.Add(button);
                button.GetComponentInChildren<Text>().text = "Resource Types Count: " + body.rawResources.Count;
                button.interactable = false;

            }

            

            button = Instantiate(uiButtonInstance, titleContent.transform);
            button.GetComponentInChildren<Text>().text = "Orbit";
            //name.GetComponent<Image>().color = gov.spriteColor;
            menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
            index = menuContent.Count - 1;
            button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
            position = menuContent[index].transform;

            button = Instantiate(uiButtonInstance, position);
            mainInfo.Add(button);
            button.GetComponentInChildren<Text>().text = body.GetInfo(GameManager.instance.data.date.time);
            button.interactable = false;

            if (body.solarSubType != SolarSubType.GasGiant && body.solarType != SolarType.Star)
            {
                button = Instantiate(uiButtonInstance, titleContent.transform);
                button.GetComponentInChildren<Text>().text = "Resources";
                //name.GetComponent<Image>().color = gov.spriteColor;
                menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
                index = menuContent.Count - 1;
                button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
                position = menuContent[index].transform;

                button = Instantiate(uiButtonInstance, position);
                mainInfo.Add(button);
                var buttonText = button.GetComponentInChildren<Text>();
                buttonText.text = "";
                body.rawResources.ForEach(x => buttonText.text += "Resource: " + x.name + " - " + Units.ReadItem(x.amount)+"\n");
                button.interactable = false;

                button = Instantiate(uiButtonInstance, titleContent.transform);
                button.GetComponentInChildren<Text>().text = "Structures";
                //name.GetComponent<Image>().color = gov.spriteColor;
                menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
                index = menuContent.Count - 1;
                button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
                position = menuContent[index].transform;

                button = Instantiate(uiButtonInstance, position);
                mainInfo.Add(button);
                buttonText = button.GetComponentInChildren<Text>();
                buttonText.text = "";
                body.spaceStructures.ForEach(x => buttonText.text += "Name: " + x.name + " - " + x.owner.Model.name + "\n");
                button.interactable = false;

                button = Instantiate(uiButtonInstance, position);
                mainInfo.Add(button);
                buttonText = button.GetComponentInChildren<Text>();
                buttonText.text = "";
                body.groundStructures.ForEach(x => buttonText.text += "Name: " + x.name + " - " + x.owner.Model.name + "\n");
                button.interactable = false;

            }


            SelectPanel(0);
        }
        
        //else if (model.target.Model.identityType == IdentityType.Station)
        //{
        //    Station station = (Station)model.target.Model;
        //    Button name = Instantiate(uiButtonInstance, contentPosition.transform);
        //    name.GetComponent<Image>().color = station.spriteColor;
        //    mainInfo.Add(name);
        //    name.onClick.AddListener(() => GameManager.instance.GoToTarget(model.target.Model));
        //    name.GetComponentInChildren<Text>().text = station.name;
        //    Button button = Instantiate(uiButtonInstance, contentPosition.transform);
        //    mainInfo.Add(button);
        //    button.GetComponentInChildren<Text>().text = "Owner: " + station.owner.Model.name;
        //    button = Instantiate(uiButtonInstance, contentPosition.transform);
        //    mainInfo.Add(button);
        //    button.GetComponentInChildren<Text>().text = "Manager: " + station.manager.Model.name;
        //    button = Instantiate(uiButtonInstance, contentPosition.transform);
        //    mainInfo.Add(button);
        //    button.GetComponentInChildren<Text>().text = "Money: " + station.money;
        //    button = Instantiate(uiButtonInstance, contentPosition.transform);
        //    mainInfo.Add(button);
        //    button.GetComponentInChildren<Text>().text = "Input Goods:";
        //    //foreach (Items item in station.factory.inputItems)
        //    //{
        //    //    button = Instantiate(uiButtonInstance, contentPosition.transform);
        //    //    mainInfo.Add(button);
        //    //    button.GetComponentInChildren<Text>().text = item.name + ": "+ item.amount+" ("+item.pendingAmount+")";
        //    //    button.GetComponent<Image>().color = item.color;
        //    //}
        //    //button.GetComponentInChildren<Text>().text = "Output Goods:";
        //    //foreach (Items item in station.factory.outputItems)
        //    //{
        //    //    button = Instantiate(uiButtonInstance, contentPosition.transform);
        //    //    mainInfo.Add(button);
        //    //    button.GetComponentInChildren<Text>().text = item.name + ": " + item.amount + " (" + item.pendingAmount + ")";
        //    //    button.GetComponent<Image>().color = item.color;
        //    //}

        //}
    }
    public void Close()
    {
        model.Delete();
    }
    // Update is called once per frame
    void Update () {

        var scaler = GetComponent<CanvasScaler>();
        if (scaler.enabled)
        {
            scaler.enabled = false;
        }
        if (fix)
        {
            scaler.enabled = true;
            fix = false;
        }
        

        //if (model.target.Model.identityType == IdentityType.Government && model.targetType == TargetType.Identity)
        //{
        //    GovernmentModel gov = (GovernmentModel)model.target.Model;
        //    Button button = mainInfo[mainInfo.Count - 1];
        //    double totalInfluence = 0;
        //    foreach (SolarModel star in gov.stars)
        //    {
        //        totalInfluence += star.governmentInfluence;
        //    }
        //    button.GetComponentInChildren<Text>().text = "Total Influence:" + totalInfluence.ToString("0.00");

        //}

    }

    public void SelectPanel(int index)
    {
        for (int i = 0; i < menuContent.Count; i++)
        {
            if (i == index)
            {
                menuContent[i].SetActive(true);
            }
            else
            {
                menuContent[i].SetActive(false);
            }
        }
        fix = true;
    }
}

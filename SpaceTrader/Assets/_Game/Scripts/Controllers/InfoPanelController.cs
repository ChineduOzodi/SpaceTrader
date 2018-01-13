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
    internal List<GameObject> menuContent = new List<GameObject>();

    private Dictionary<string, Text> texts = new Dictionary<string, Text>();

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
                var creature = GameManager.instance.data.creatures.Model.GetCreature(gov.leaders[0]);
                button.GetComponentInChildren<Text>().text = "Leader: " + creature.name;
                button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(creature));

                button = Instantiate(uiButtonInstance, position);
                button.GetComponentInChildren<Text>().text = "Capital: " + GameManager.instance.data.getSolarBody(gov.solarIndex).name;
                button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(gov.solarIndex));

                button = Instantiate(uiButtonInstance, position);
                texts.Add("Money", button.GetComponentInChildren<Text>());
                texts["Money"].text = "Money: " + gov.money;
                button.interactable = false;

                button = Instantiate(uiButtonInstance, position);
                var text = button.GetComponentInChildren<Text>();
                texts.Add("Star Count", text);
                text.text = "Star Count:" + gov.stars.Count;
                button.interactable = false;

                double totalInfluence = 0;
                foreach (SolarModel star in gov.stars)
                {
                    totalInfluence += star.governmentInfluence;
                }
                button = Instantiate(uiButtonInstance, position);
                text = button.GetComponentInChildren<Text>();
                texts.Add("Influence", text);
                text.text = "Total Influence:" + Units.ReadItem(totalInfluence);
                button.interactable = false;

            }
        }
        else if (model.targetType == TargetType.SolarBody)
        {
            SolarBody body = GameManager.instance.data.getSolarBody(model.solarIndex);
            title.text = body.name;
            Text text;

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

            if (body.solarSubType != SolarSubType.GasGiant && body.solarType != SolarType.Star && body.inhabited)
            {
                button = Instantiate(uiButtonInstance, position);
                text = button.GetComponentInChildren<Text>();
                texts.Add("Population", text);
                text.text = "Population: " + Units.ReadItem(body.population.totalPopulation) + "\n"
                    + "Young: " + Units.ReadItem(body.population.young) + "\n"
                    + "Adult: " + Units.ReadItem(body.population.adult) + "\n"
                    + "Old: " + Units.ReadItem(body.population.old);
                button.interactable = false;

            }

            button = Instantiate(uiButtonInstance, position);
            text = button.GetComponentInChildren<Text>();
            texts.Add("SpaceStructureCount", text);
            button.GetComponentInChildren<Text>().text = "Space Structure Count: " + body.structures.Count;
            button.interactable = false;

            if (body.solarSubType != SolarSubType.GasGiant && body.solarType != SolarType.Star)
            {
                button = Instantiate(uiButtonInstance, position);
                text = button.GetComponentInChildren<Text>();
                texts.Add("GroundStructureCount", text);
                button.GetComponentInChildren<Text>().text = "Ground Structure Count:" + body.structures.Count;
                button.interactable = false;

                button = Instantiate(uiButtonInstance, position);
                text = button.GetComponentInChildren<Text>();
                texts.Add("ResourceCount", text);
                button.GetComponentInChildren<Text>().text = "Resource Types Count: " + body.rawResources.Count;
                button.interactable = false;

                foreach( IdentityModel x in body.companies){
                    button = Instantiate(uiButtonInstance, position);
                    text = button.GetComponentInChildren<Text>();
                    texts.Add("Companies" + x.name, text);
                    text.text = "Entity: " + x.name + " - " + Units.ReadItem(x.money);
                    button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(x));
                }

            }

            

            button = Instantiate(uiButtonInstance, titleContent.transform);
            button.GetComponentInChildren<Text>().text = "Orbit";
            //name.GetComponent<Image>().color = gov.spriteColor;
            menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
            index = menuContent.Count - 1;
            button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
            position = menuContent[index].transform;

            button = Instantiate(uiButtonInstance, position);
            text = button.GetComponentInChildren<Text>();
            texts.Add("OrbitInfo", text);
            text.text = body.GetInfo(GameManager.instance.data.date.time);
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

                body.rawResources.ForEach(x => {
                    button = Instantiate(uiButtonInstance, position);
                    text = button.GetComponentInChildren<Text>();
                    texts.Add("Resources" + x.id, text);
                    text.text = "Resource: " + x.name + " - " + Units.ReadItem(x.amount);
                    button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(x));
                });

                button = Instantiate(uiButtonInstance, titleContent.transform);
                button.GetComponentInChildren<Text>().text = "Structures";
                //name.GetComponent<Image>().color = gov.spriteColor;
                menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
                index = menuContent.Count - 1;
                button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
                position = menuContent[index].transform;

                body.structures.ForEach(x => {
                    button = Instantiate(uiButtonInstance, position);
                    text = button.GetComponentInChildren<Text>();
                    texts.Add("Structure" + x.id, text);
                    text.text = "Name: " + x.name + " - " + x.owner.Model.name;
                    button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(x));
                });

            }

            if (body.solarType == SolarType.Star)
            {
                SolarModel solar = GameManager.instance.data.stars[body.solarIndex[0]];

                if (solar.buyList.items.Count > 0)
                {
                    button = Instantiate(uiButtonInstance, titleContent.transform);
                    button.GetComponentInChildren<Text>().text = "Buying";
                    //name.GetComponent<Image>().color = gov.spriteColor;
                    menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
                    index = menuContent.Count - 1;
                    button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
                    position = menuContent[index].transform;

                    solar.buyList.items.ForEach(x => {
                        button = Instantiate(uiButtonInstance, position);
                        text = button.GetComponentInChildren<Text>();
                        texts.Add("BuyList" + x.id + x.structureId, text);
                        text.text = x.name + " - " + Units.ReadItem(x.amount) + "u for " + Units.ReadItem(x.price) + "c ";
                        var structure = body.GetStructure(x.structureId);
                        if (structure != null)
                            button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(structure));
                        else
                            button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(x));
                    });
                }

                if (solar.sellList.items.Count > 0)
                {
                    button = Instantiate(uiButtonInstance, titleContent.transform);
                    button.GetComponentInChildren<Text>().text = "Selling";
                    //name.GetComponent<Image>().color = gov.spriteColor;
                    menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
                    index = menuContent.Count - 1;
                    button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
                    position = menuContent[index].transform;

                    solar.sellList.items.ForEach(x => {
                        button = Instantiate(uiButtonInstance, position);
                        text = button.GetComponentInChildren<Text>();
                        texts.Add("SellList" + x.id + x.structureId, text);
                        text.text = x.name + " - " + Units.ReadItem(x.amount) + "u for " + Units.ReadItem(x.price) + "c ";
                        var structure = body.GetStructure(x.structureId);
                        if (structure != null)
                            button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(structure));
                        else
                            button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(x));
                    });
                }

                if (solar.supplyDemand.Count > 0)
                {
                    button = Instantiate(uiButtonInstance, titleContent.transform);
                    button.GetComponentInChildren<Text>().text = "Supply & Demand";
                    //name.GetComponent<Image>().color = gov.spriteColor;
                    menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
                    index = menuContent.Count - 1;
                    button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
                    position = menuContent[index].transform;

                    solar.supplyDemand.ForEach(x => {
                        button = Instantiate(uiButtonInstance, position);
                        text = button.GetComponentInChildren<Text>();
                        texts.Add("SupplyDemand" + x.itemId, text);
                        text.text = x.itemName + " - " + Units.ReadItem(x.marketPrice) + "c " + Units.ReadItem(x.itemDemand) + " (" + Units.ReadItem(x.averageItemBuyPrice)
                        + "c) : " + Units.ReadItem(x.itemSupply) + " (" + Units.ReadItem(x.averageItemSellPrice) + "c)";
                        button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(x.itemId, TargetType.Item));
                    });
                }
            }
            else
            {
                if (body.buyList.items.Count > 0)
                {
                    button = Instantiate(uiButtonInstance, titleContent.transform);
                    button.GetComponentInChildren<Text>().text = "Buying";
                    //name.GetComponent<Image>().color = gov.spriteColor;
                    menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
                    index = menuContent.Count - 1;
                    button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
                    position = menuContent[index].transform;

                    body.buyList.items.ForEach(x => {
                        button = Instantiate(uiButtonInstance, position);
                        text = button.GetComponentInChildren<Text>();
                        texts.Add("BuyList" + x.id + x.structureId, text);
                        text.text = x.name  + " - " + Units.ReadItem(x.amount) + "u for " + Units.ReadItem(x.price) + "c ";
                        var structure = body.GetStructure(x.structureId);
                        if (structure != null)
                            button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(structure));
                        else
                            button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(x));
                    });
                }

                if (body.sellList.items.Count > 0)
                {
                    button = Instantiate(uiButtonInstance, titleContent.transform);
                    button.GetComponentInChildren<Text>().text = "Selling";
                    //name.GetComponent<Image>().color = gov.spriteColor;
                    menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
                    index = menuContent.Count - 1;
                    button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
                    position = menuContent[index].transform;

                    body.sellList.items.ForEach(x => {
                        button = Instantiate(uiButtonInstance, position);
                        text = button.GetComponentInChildren<Text>();
                        texts.Add("SellList" + x.id + x.structureId, text);
                        text.text = x.name + " - " + Units.ReadItem(x.amount) + "u for " + Units.ReadItem(x.price) + "c ";
                        var structure = body.GetStructure(x.structureId);
                        if (structure != null)
                            button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(structure));
                        else
                            button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(x));
                    });
                }
            }




            SelectPanel(0);
        }
        else if (model.targetType == TargetType.Structure)
        {
            SolarBody body = GameManager.instance.data.getSolarBody(model.solarIndex);
            title.text = model.structure.name;
            Text text;
            Button button = Instantiate(uiButtonInstance, titleContent.transform);
            button.GetComponentInChildren<Text>().text = "Overview";
            //name.GetComponent<Image>().color = gov.spriteColor;
            menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
            var index = menuContent.Count - 1;
            button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
            var position = menuContent[index].transform;

            button = Instantiate(uiButtonInstance, position);
            var owner = model.structure.owner.Model;
            button.GetComponentInChildren<Text>().text = "Owner: " + owner.name;
            button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(owner));

            button = Instantiate(uiButtonInstance, position);
            button.GetComponentInChildren<Text>().text = "Location: " + body.name;
            button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(body.solarIndex));

            button = Instantiate(uiButtonInstance, position);
            text = button.GetComponentInChildren<Text>();
            texts.Add("Info", text);
            text.text = model.structure.info;
            button.interactable = false;

            if (model.structure.structureType == StructureTypes.GroundStorage)
            {
                button = Instantiate(uiButtonInstance, position);
                text = button.GetComponentInChildren<Text>();
                texts.Add("FillPercent", text);
                text.text = "Storage Filled: " + ((((GroundStorage)model.structure).currentStorageAmount / ((GroundStorage)model.structure).totalStorageAmount) * 100 * model.structure.count).ToString("0.00") + " %  " 
                    + Units.ReadItem(((GroundStorage)model.structure).currentStorageAmount) + " - " +
                     Units.ReadItem(((GroundStorage)model.structure).totalStorageAmount * model.structure.count);
            }
            if (model.structure.structureType == StructureTypes.Driller)
            {
                button = Instantiate(uiButtonInstance, position);
                text = button.GetComponentInChildren<Text>();
                text.text = "Mining: " + ((Driller)model.structure).productionItemName;
                button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(((Driller)model.structure).productionItemId, TargetType.RawResource));
            }
            if (model.structure.structureType == StructureTypes.Factory)
            {
                button = Instantiate(uiButtonInstance, position);
                button.GetComponentInChildren<Text>().text = "Producing Item: " + ((Factory)model.structure).productionItemName;
                button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(((Factory)model.structure).productionItemId, TargetType.Item));

                button = Instantiate(uiButtonInstance, position);
                button.GetComponentInChildren<Text>().text = "Modified Production Time: " + Dated.ReadTime(((Factory)model.structure).productionTime);
                button.interactable = false;

                button = Instantiate(uiButtonInstance, position);
                text = button.GetComponentInChildren<Text>();
                texts.Add("ProductionPercent", text);
                text.text = "Production Percent: " + ((((Factory)model.structure).productionProgress * 100).ToString("0.00") + " %");
            }


            button = Instantiate(uiButtonInstance, position);
            button.GetComponentInChildren<Text>().text = "Go to -->";
            button.onClick.AddListener(() => GameManager.instance.GoToTarget(body.solarIndex));

            ProductionStructure productionStructure = model.structure as ProductionStructure;
            if (productionStructure != null && (productionStructure.structureConnectionIdsIn.Count > 0 || productionStructure.structureConnectionIdsOut.Count > 0))
            {
                button = Instantiate(uiButtonInstance, titleContent.transform);
                button.GetComponentInChildren<Text>().text = "Connections";
                //name.GetComponent<Image>().color = gov.spriteColor;
                menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
                index = menuContent.Count - 1;
                button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
                position = menuContent[index].transform;
                if (productionStructure.structureConnectionIdsOut.Count > 0)
                {
                    button = Instantiate(uiButtonInstance, position);
                    text = button.GetComponentInChildren<Text>();
                    text.text = "Connections Out | Rate: " + 
                        (productionStructure.count/productionStructure.productionTime * Dated.Hour).ToString("0.00") +
                        " per Hour | Extra: " +
                        (productionStructure.extraProductionRate * Dated.Hour).ToString("0.00") +
                        " per Hour";
                    button.interactable = false;

                    productionStructure.structureConnectionIdsOut.ForEach(x => {
                        button = Instantiate(uiButtonInstance, position);
                        text = button.GetComponentInChildren<Text>();
                        IStructure outStructure = body.structures.Find(b => b.id == x);
                        text.text = outStructure.name + " Rate: " + (productionStructure.connectionOutRate[x] * Dated.Hour).ToString("0.00") + " per Hour";
                        button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(outStructure));
                    });
                }
                
                if (productionStructure.structureConnectionIdsIn.Count > 0)
                {
                    button = Instantiate(uiButtonInstance, position);
                    text = button.GetComponentInChildren<Text>();
                    text.text = "Connections In";
                    button.interactable = false;

                    productionStructure.structureConnectionIdsIn.ForEach(x => {
                        button = Instantiate(uiButtonInstance, position);
                        text = button.GetComponentInChildren<Text>();
                        IStructure inStructure = body.structures.Find(b => b.id == x);
                        text.text = inStructure.name;
                        button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(inStructure));
                    });
                }

                if (productionStructure.neededItemRate.Count > 0)
                {
                    button = Instantiate(uiButtonInstance, position);
                    text = button.GetComponentInChildren<Text>();
                    text.text = "Needed Items";
                    button.interactable = false;

                    foreach(KeyValuePair<int,double> neededItem in productionStructure.neededItemRate) {
                        button = Instantiate(uiButtonInstance, position);
                        text = button.GetComponentInChildren<Text>();
                        ItemBlueprint item = GameManager.instance.data.itemsData.Model.GetItem(neededItem.Key);
                        text.text = item.name;
                        button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(item.id, TargetType.Item));
                    }

                }

            }

            if (model.structure.structureType == StructureTypes.GroundStorage)
            {
                button = Instantiate(uiButtonInstance, titleContent.transform);
                button.GetComponentInChildren<Text>().text = "Items";
                //name.GetComponent<Image>().color = gov.spriteColor;
                menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
                index = menuContent.Count - 1;
                button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
                position = menuContent[index].transform;

                ((GroundStorage) model.structure).storage.items.ForEach(x => {
                    button = Instantiate(uiButtonInstance, position);
                    text = button.GetComponentInChildren<Text>();
                    texts.Add("Items"+ x.id, text);
                    text.text = "Name: " + x.name + " - " + Units.ReadItem(x.amount);
                    button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(x));
                });
            }

            if (model.structure.structureType == StructureTypes.Factory)
            {
                button = Instantiate(uiButtonInstance, titleContent.transform);
                button.GetComponentInChildren<Text>().text = "Items";
                //name.GetComponent<Image>().color = gov.spriteColor;
                menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
                index = menuContent.Count - 1;
                button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
                position = menuContent[index].transform;

                ((Factory)model.structure).storage.items.ForEach(x => {
                    button = Instantiate(uiButtonInstance, position);
                    text = button.GetComponentInChildren<Text>();
                    texts.Add("Items" + x.id, text);
                    text.text = "Name: " + x.name + " - " + Units.ReadItem(x.amount);
                    button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(x));
                });
            }

            if (model.structure.structureType == StructureTypes.SpaceStation)
            {
                button = Instantiate(uiButtonInstance, titleContent.transform);
                button.GetComponentInChildren<Text>().text = "Items";
                //name.GetComponent<Image>().color = gov.spriteColor;
                menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
                index = menuContent.Count - 1;
                button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
                position = menuContent[index].transform;

                ((Station)model.structure).storage.items.ForEach(x => {
                    button = Instantiate(uiButtonInstance, position);
                    text = button.GetComponentInChildren<Text>();
                    texts.Add("Items" + x.id, text);
                    text.text = "Name: " + x.name + " - " + Units.ReadItem(x.amount);
                    button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(x));
                });
            }

            SelectPanel(0);
        }
        else if (model.targetType == TargetType.Item)
        {
            var item = GameManager.instance.data.itemsData.Model.GetItem(model.targetId);
            title.text = item.name;

            Button button = Instantiate(uiButtonInstance, titleContent.transform);
            button.GetComponentInChildren<Text>().text = "Overview";
            //name.GetComponent<Image>().color = gov.spriteColor;
            menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
            var index = menuContent.Count - 1;
            button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
            var position = menuContent[index].transform;

            button = Instantiate(uiButtonInstance, position);
            button.GetComponentInChildren<Text>().text = "Type: " + item.itemType.ToString();
            button.interactable = false;

            button = Instantiate(uiButtonInstance, position);
            button.GetComponentInChildren<Text>().text = "Description: " + item.description;
            button.interactable = false;

            button = Instantiate(uiButtonInstance, position);
            button.GetComponentInChildren<Text>().text = "Production Time: " + Dated.ReadTime(item.productionTime);
            button.interactable = false;

            if (item.workers > 0)
            {
                button = Instantiate(uiButtonInstance, position);
                button.GetComponentInChildren<Text>().text = "Workers: " + Units.ReadItem(item.workers);
                button.interactable = false;
            }



            if (item.itemType != ItemType.RawMaterial)
            {
                button = Instantiate(uiButtonInstance, titleContent.transform);
                button.GetComponentInChildren<Text>().text = "Materials";
                //name.GetComponent<Image>().color = gov.spriteColor;
                menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
                index = menuContent.Count - 1;
                button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
                position = menuContent[index].transform;

                item.contstructionParts.ForEach(x => {
                    button = Instantiate(uiButtonInstance, position);
                    var buttonText = button.GetComponentInChildren<Text>();
                    buttonText.text = "Name: " + x.name + " - " + Units.ReadItem(x.amount);
                    button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(x));
                });
            }


            SelectPanel(0);
        }


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


        if (model.targetType == TargetType.Identity)
        {
            if (model.target.Model.identityType == IdentityType.Government)
            {
                GovernmentModel gov = (GovernmentModel)model.target.Model;
                var text = texts["Money"];
                text.text = "Money: " + gov.money;


                text = texts["Star Count"];
                text.text = "Star Count:" + gov.stars.Count;

                double totalInfluence = 0;
                foreach (SolarModel star in gov.stars)
                {
                    totalInfluence += star.governmentInfluence;
                }

                text = texts["Influence"];
                text.text = "Total Influence:" + Units.ReadItem(totalInfluence);

            }
        }
        else if (model.targetType == TargetType.SolarBody)
        {
            SolarBody body = GameManager.instance.data.getSolarBody(model.solarIndex);
            title.text = body.name;
            Text text;

            if (body.solarSubType != SolarSubType.GasGiant && body.solarType != SolarType.Star && body.inhabited)
            {
                text = texts["Population"];
                text.text = "Population: " + Units.ReadItem(body.population.totalPopulation) + "\n"
                    + "Young: " + Units.ReadItem(body.population.young) + "\n"
                    + "Adult: " + Units.ReadItem(body.population.adult) + "\n"
                    + "Old: " + Units.ReadItem(body.population.old);

                foreach (IdentityModel x in body.companies)
                {
                    text = texts["Companies" + x.name];
                    text.text = "Entity: " + x.name + " - " + Units.ReadItem(x.money) + "c";
                }
            }

            text = texts["SpaceStructureCount"];
            text.text = "Space Structure Count: " + body.structures.FindAll(x => x.structureType == StructureTypes.SpaceStation).Count;

            if (body.solarSubType != SolarSubType.GasGiant && body.solarType != SolarType.Star)
            {
                text = texts["GroundStructureCount"];
                text.text = "Ground Structure Count:" + body.structures.FindAll(x => x.structureType != StructureTypes.SpaceStation).Count;

                text = texts["ResourceCount"];
                text.text = "Resource Types Count: " + body.rawResources.Count;

            }

            if (body.solarType == SolarType.Star)
            {
                SolarModel solar = GameManager.instance.data.stars[body.solarIndex[0]];

                if (solar.buyList.items.Count > 0)
                {

                    solar.buyList.items.ForEach(x => {
                        if (texts.TryGetValue("SellList" + x.id  + x.structureId, out text))
                            text.text = x.name + " - " + Units.ReadItem(x.amount) + "u for " + Units.ReadItem(x.price) + "c";
                    });
                }

                if (solar.sellList.items.Count > 0)
                {
                    solar.sellList.items.ForEach(x => {
                        if (texts.TryGetValue("SellList" + x.id + x.structureId, out text))
                            text.text = x.name + " - " + Units.ReadItem(x.amount) + "u for " + Units.ReadItem(x.price) + "c";
                    });
                }

                if (solar.supplyDemand.Count > 0)
                {
                    solar.supplyDemand.ForEach(x => {
                        if (texts.TryGetValue("SupplyDemand" + x.itemId, out text))
                            text.text = x.itemName + " - " + Units.ReadItem(x.marketPrice) + "c " + Units.ReadItem(x.itemDemand) + " (" + Units.ReadItem(x.averageItemBuyPrice)
                        + "c) : " + Units.ReadItem(x.itemSupply) + " (" + Units.ReadItem(x.averageItemSellPrice) + "c)";
                    });
                }
            }
            else
            {
                if (body.buyList.items.Count > 0)
                {

                    body.buyList.items.ForEach(x => {
                        if (texts.TryGetValue("BuyList" + x.id + x.structureId, out text))
                            text.text = x.name + " - " + Units.ReadItem(x.amount) + "u for " + Units.ReadItem(x.price) + "c ";
                    });
                }

                if (body.sellList.items.Count > 0)
                {
                    body.sellList.items.ForEach(x => {
                        if (texts.TryGetValue("SellList" + x.id + x.structureId, out text))
                        text.text = x.name + " - " + Units.ReadItem(x.amount) + "u for " + Units.ReadItem(x.price) + "c ";
                    });
                }
            }



            text = texts["OrbitInfo"];
            text.text = body.GetInfo(GameManager.instance.data.date.time);

            if (body.solarSubType != SolarSubType.GasGiant && body.solarType != SolarType.Star)
            {

                body.rawResources.ForEach(x => {

                    if (texts.ContainsKey("Resources" + x.id))
                    {
                        text = texts["Resources" + x.id];
                        text.text = "Resource: " + x.name + " - " + Units.ReadItem(x.amount);
                        if (x.timeUntilDepleted > 0)
                        text.text += "Time to Depletion: " + Dated.ReadTime(x.timeUntilDepleted);
                    }
                    
                });

                body.structures.ForEach(x => {
                    if (texts.ContainsKey("Structure" + x.id))
                    {
                        text = texts["Structure" + x.id];
                        text.text = "Name: " + x.name + " - " + x.owner.Model.name;
                    }
                        
                });

            }
        }
        else if (model.targetType == TargetType.Structure)
        {
            //SolarBody body = GameManager.instance.data.getSolarBody(model.solarIndex);
            Text text;

            text = texts["Info"];
            text.text = model.structure.info;

            if (model.structure.structureType == StructureTypes.GroundStorage)
            {
                text = texts["FillPercent"];
                text.text = "Storage Filled: " + ((((GroundStorage)model.structure).currentStorageAmount / ((GroundStorage)model.structure).totalStorageAmount) * model.structure.count * 100).ToString("0.00") + " %  "
                    + Units.ReadItem(((GroundStorage)model.structure).currentStorageAmount) + " - " +
                     Units.ReadItem(((GroundStorage)model.structure).totalStorageAmount * model.structure.count);

                ((GroundStorage)model.structure).storage.items.ForEach(x => {
                    if (texts.ContainsKey("Items" + x.id))
                    {
                        text = texts["Items" + x.id];
                        text.text = "Name: " + x.name + " - " + Units.ReadItem(x.amount);
                    }
                    
                });
            }

            if (model.structure.structureType == StructureTypes.SpaceStation)
            {

                ((Station)model.structure).storage.items.ForEach(x => {
                    if (texts.TryGetValue("Items" + x.id, out text))
                    {
                        text.text = "Name: " + x.name + " - " + Units.ReadItem(x.amount);
                    }
                });
            }

            if (model.structure.structureType == StructureTypes.Factory)
            {
                text = texts["ProductionPercent"];
                text.text = "Production Percent: " + ((((Factory)model.structure).productionProgress * 100).ToString("0.00") + " %");

                ((Factory)model.structure).storage.items.ForEach(x => {
                    if (texts.ContainsKey("Items" + x.id))
                    {
                        text = texts["Items" + x.id];
                        text.text = "Name: " + x.name + " - " + Units.ReadItem(x.amount);
                    }
                });
            }
        }

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

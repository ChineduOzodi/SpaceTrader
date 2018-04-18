using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using System;
using UnityEngine.UI;

public class InfoPanelController : Controller<InfoPanelModel> {

    public WMG_Axis_Graph emptyGraphPrefab;
    public Button uiButtonInstance;
    public Text title;
    public GameObject titleContent;
    public ScrollView scrollViewPrefab;
    public GameObject infoContentPanel;
    public PlanetGridPanel planetTiles;
    public UpdateDel TextUpdates;
    public GameObject contentPosition;
    /// <summary>
    /// Used to create tabs in the info panel.
    /// </summary>
    internal List<GameObject> menuContent = new List<GameObject>();
    /// <summary>
    /// Used to update items that need updating.
    /// </summary>
    private Dictionary<string, Text> texts = new Dictionary<string, Text>();

    public Dictionary<string, Button> buttons = new Dictionary<string, Button>();
    
    public bool fix = true;

    protected override void OnInitialize()
    {
        TextUpdates = () => { };
        if (model.targetType == TargetType.Governments)
        {
            BuildGovernments();
        }
        else if (model.targetType == TargetType.Contract)
        {
            BuildContract();
        }
        else if (model.targetType == TargetType.Identity)
        {
            if (model.target.Model.GetType() == typeof(GovernmentModel))
            {

                BuildGovernment();
                
            }
            else
            {
                BuildCompany();
                
            }

            BuildIdentityModel();

            SelectPanel(0);
        }
        else if (model.targetType == TargetType.SolarBody)
        {
            SolarBody body = GameManager.instance.data.getSolarBody(model.id);
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
                Button(position, () =>
                {
                    return "Population: " + Units.ReadItem(body.population.totalPopulation) + "\n"
                    + "Young: " + Units.ReadItem(body.population.young) + "\n"
                    + "Adult: " + Units.ReadItem(body.population.adult) + "\n"
                    + "Old: " + Units.ReadItem(body.population.old);
                });
            }


            Button(position, () =>
            {
                return "Structure Count: " + body.structures.Count;
            });

            if (body.solarSubType != SolarSubType.GasGiant && body.solarType != SolarType.Star)
            {

                Button(position, () =>
                {
                    return "Resource Types Count: " + body.rawResources.Count;
                });

                Button(position, () =>
                {
                    return "Resource Types Count: " + body.rawResources.Count;
                });

                

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

                ButtonList<RawResource>(body.id, body.rawResources, position, (x) =>
                {
                    string output = "Resource: " + x.name + " - " + Units.ReadItem(x.amount);
                    if (x.timeUntilDepleted > 0) output += "Time to Depletion: " + Dated.ReadTime(x.timeUntilDepleted);
                    return output;
                }, (x) => () => GameManager.instance.OpenInfoPanel(x), (x, but) => {

                    Image image = but.GetComponent<Image>();
                    if (x.timeUntilDepleted > 0) image.color = Color.green;
                    else image.color = Color.white;
                });

                BuildList(body.id, body.companies.ToList());

                BuildList(body.id, body.structures);

            }

            SelectPanel(0);
        }
        else if (model.targetType == TargetType.Structure)
        {
            BuildStructurePanel();

            SelectPanel(0);
        }
        else if (model.targetType == TargetType.Item)
        {
            var item = GameManager.instance.data.itemsData.Model.GetItem(model.id);
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
            button.GetComponentInChildren<Text>().text = "Work Amount: " + item.workAmount;
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

                item.contstructionParts.ForEach(x =>
                {
                    button = Instantiate(uiButtonInstance, position);
                    var buttonText = button.GetComponentInChildren<Text>();
                    buttonText.text = "Name: " + x.name + " - " + Units.ReadItem(x.amount);
                    button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(x));
                });
            }


            SelectPanel(0);
        }


    }

    private void BuildStructurePanel()
    {
        model.structure = GameManager.instance.locations[model.id] as Structure;
        var positionEntity = GameManager.instance.locations[model.structure.referenceId];
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
        button.GetComponentInChildren<Text>().text = "Location: " + positionEntity.name;
        button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(positionEntity.id));

        Button(position, () =>
        {
            return model.structure.Info;
        });

        //Button(position, () =>
        //{
        //    return "Storage Filled: " + (((model.structure).currentStorageAmount / (model.structure).totalStorageAmount) * 100 * model.structure.Count).ToString("0.00") + " %  "
        //        + Units.ReadItem(((DistributionCenter)model.structure).currentStorageAmount) + " - " +
        //         Units.ReadItem(((DistributionCenter)model.structure).totalStorageAmount * model.structure.Count);
        //});

        if (model.structure.StructureType == StructureTypes.Driller)
        {
            button = Instantiate(uiButtonInstance, position);
            text = button.GetComponentInChildren<Text>();
            text.text = "Mining: " + ((Driller)model.structure).productionItemName;
            button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(((Driller)model.structure).productionItemId, TargetType.RawResource));
        }

        if (model.structure.GetType() == typeof(Factory))
        {
            button = Instantiate(uiButtonInstance, position);
            button.GetComponentInChildren<Text>().text = "Producing Item: " + ((Factory)model.structure).productionItemName;
            button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(((Factory)model.structure).productionItemId, TargetType.Item));

            Button(position, () =>
            {
                return "Modified Production Time: " + Dated.ReadTime(((Factory)model.structure).ProductionTime);
            });

            Button(position, () =>
            {
                return "Production Percent: " + ((((Factory)model.structure).productionProgress * 100).ToString("0.00") + " %");
            });
        }

        if (model.structure.GetType() == typeof(Ship))
        {
            Ship ship = model.structure as Ship;

            Button(position, () =>
            {
                return "Current Action: " + ship.shipAction.ToString();
            });

            Button(position, () =>
            {
                if (ship.contractId != null)
                {
                    Contract contract = GameManager.instance.contracts[ship.contractId];
                    return "Contract Name: " + contract.id +
                    "\nItem: " + contract.itemName +
                    "\nAmount: " + Units.ReadRate(contract.itemRate) +
                    "\nUnit Price: " + Units.ReadItem(contract.unitPrice) +
                    "c\nDistnace Cost: " + Units.ReadItem(contract.PricePerKm * contract.distance * 2);
                }
                return "Contract: None";
            }, () => {
                if (ship.contractId != null)
                {
                    Contract contract = GameManager.instance.contracts[ship.contractId];
                    GameManager.instance.OpenInfoPanel(contract.id, TargetType.Contract);
                }
            });

            Button(position, () =>
            {
                if (ship.shipTargetId != null)
                {
                    Structure structure = GameManager.instance.locations[ship.shipTargetId] as Structure;
                    double distance = Vector3d.Distance(structure.SystemPosition, ship.SystemPosition);
                    return "Target: " + structure.id +
                    "\nDistance: " + Units.ReadDistance(distance) +
                    "\nETA: " + Dated.ReadTime(distance / ship.SubLightSpeed);
                }
                return "Target: None";
            }, () => {
                if (ship.contractId != null)
                {
                    Structure structure = GameManager.instance.locations[ship.shipTargetId] as Structure;
                    GameManager.instance.OpenInfoPanel(structure);
                }
            });

            Button(position, () =>
            {
                if (ship.fuel != null)
                {
                    return "Fuel: " + (ship.fuel.amount / ship.fuelCapacity * 100).ToString("0.00") + " %" +
                    "\nDistance: " + Units.ReadDistance(ship.fuelRange * ship.fuel.amount) +
                "\nETA: " + Dated.ReadTime((ship.fuelRange * ship.fuel.amount) / ship.SubLightSpeed);
                }
                return "Fuel: Empty";
            }, () => {
                if (ship.fuel != null)
                {
                    GameManager.instance.OpenInfoPanel(ship.fuel);
                }
            });
        }

        button = Instantiate(uiButtonInstance, position);
        button.GetComponentInChildren<Text>().text = "Go to -->";
        button.onClick.AddListener(() => GameManager.instance.GoToTarget(positionEntity.id));

        ProductionStructure productionStructure = model.structure as ProductionStructure;
        if (productionStructure != null)
        {
            SolarBody body = positionEntity as SolarBody;
            button = Instantiate(uiButtonInstance, titleContent.transform);
            button.GetComponentInChildren<Text>().text = "Connections";
            //name.GetComponent<Image>().color = gov.spriteColor;
            menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
            index = menuContent.Count - 1;
            button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
            position = menuContent[index].transform;

            Button(position, () =>
            {
                return "Rate: " + Units.ReadRate(productionStructure.ProductionRateActual) + " of " + Units.ReadRate(productionStructure.ProductionRateOptimal) + " | Extra: " +
                Units.ReadRate(productionStructure.extraProductionRate);
            });

            Button(position, () =>
            {
                return "Connections Out: " + productionStructure.structureConnectionIdsOut.Count;
            });

            ButtonList<ProductionStructure>(productionStructure.id, productionStructure.GetStructuresOut(), position, (x) =>
            {
                string output = x.name + " Rate: " + Units.ReadRate(productionStructure.connectionOutRate[x.id]);
                return output;
            }, (x) => () => GameManager.instance.OpenInfoPanel(x), (x, but) => {

                Image image = but.GetComponent<Image>();
                if (x.isProducing) image.color = Color.green;
                else image.color = Color.yellow;
            });

            Button(position, () =>
            {
                return "Connections In: " + productionStructure.structureConnectionIdsIn.Count;
            });

            ButtonList<ProductionStructure>(productionStructure.id, productionStructure.GetStructuresIn(), position, (x) =>
            {
                string output = x.name + " " + (x.connectionOutRate[productionStructure.id] * 100 / (productionStructure.requiredItems.Find(b => b.id == x.productionItemId).amount * productionStructure.ProductionRateOptimal)).ToString("0.00") + " %";
                return output;
            }, (x) => () => GameManager.instance.OpenInfoPanel(x), (x, but) => {

                Image image = but.GetComponent<Image>();
                if (x.isProducing) image.color = Color.green;
                else image.color = Color.yellow;
            });

            Button(position, () =>
            {
                return "Contracts Out: " + productionStructure.clientContracts.Count;
            });

            ButtonList<Contract>(productionStructure.id, productionStructure.clientContracts, position, (contract) =>
            {
                string output = "Item: " + contract.itemName +
                        " | Amount: " + Units.ReadRate(contract.itemRate) +
                        "u | Monthly Cost: " + Units.ReadItem(contract.monthlyCost) + "c";
                if (contract.destinationId != null)
                {
                    output += "\nDestination: " + GameManager.instance.locations[contract.destinationId].name;
                }
                return output;
            }, (contract) => () => GameManager.instance.OpenInfoPanel(contract.id, TargetType.Contract), (contract, but) => {

                Image image = but.GetComponent<Image>();
                if (contract.contractState == ContractState.Active) image.color = Color.green;
                else image.color = Color.white;
            });

            Button(position, () =>
            {
                return "Contracts In: " + productionStructure.supplierContractIds.Count;
            });

            ButtonList<Contract>(productionStructure.id, productionStructure.GetSupplyContracts(), position, (contract) =>
            {
                string output = "Item: " + contract.itemName +
                        "\nAmount: " + Units.ReadRate(contract.itemRate) +
                        "\nMonthly Cost: " + Units.ReadItem(contract.monthlyCost) +
                        "c\nOrigin: " + contract.Origin.name;
                return output;
            }, (contract) => () => GameManager.instance.OpenInfoPanel(contract.id, TargetType.Contract), (contract, but) => {

                Image image = but.GetComponent<Image>();
                if (contract.contractState == ContractState.Active) image.color = Color.green;
                else image.color = Color.white;
            });

            Button(position, () =>
            {
                return "Needed Items: " + productionStructure.neededItemRate.Count;
            });

            ButtonList<string,double>(productionStructure.id, productionStructure.neededItemRate, position, (x) =>
            {
                ItemBlueprint item = GameManager.instance.data.itemsData.Model.GetItem(x.Key);
                string output = item.name + " | " + Units.ReadRate(x.Value);
                return output;
            }, (x) => () => GameManager.instance.OpenInfoPanel(x.Key, TargetType.Item));

        }

        button = Instantiate(uiButtonInstance, titleContent.transform);
        button.GetComponentInChildren<Text>().text = "Items";
        //name.GetComponent<Image>().color = gov.spriteColor;
        menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
        index = menuContent.Count - 1;
        button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
        position = menuContent[index].transform;

        ButtonList<Item>(model.structure.id, model.structure.itemsStorage, position, (x) =>
        {
            string output = "Name: " + x.name + " - " + Units.ReadItem(x.amount);
            if (x.destinationId != null) output += " | " + GameManager.instance.locations[x.destinationId];
            return output;
        }, (contract) => () => GameManager.instance.OpenInfoPanel(contract.id, TargetType.Contract));
    }

    private void BuildIdentityModel()
    {
        IdentityModel identityModel = model.target.Model;

        Button button = Instantiate(uiButtonInstance, titleContent.transform);
        button.GetComponentInChildren<Text>().text = "Money";
        //name.GetComponent<Image>().color = gov.spriteColor;
        menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
        int index = menuContent.Count - 1;
        var position = menuContent[index].transform;
        button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);

        var graph = Instantiate(emptyGraphPrefab,position);
        graph.yAxis.MaxAutoGrow = true;
        graph.yAxis.MaxAutoShrink = true;
        graph.yAxis.MinAutoGrow = true;
        graph.yAxis.MaxAutoShrink = true;
        graph.xAxis.MaxAutoGrow = true;
        graph.xAxis.MaxAutoShrink = true;

        TextUpdates += () =>
        {
            string name = "TotalMoney";
            WMG_List <Vector2> list = new WMG_List<Vector2>();
            //for (int i = 0; i < identityModel.moneyTotalMonth.Count; i++)
            //{
            //    list.Add(new Vector2(i, (float)identityModel.moneyTotalMonth[i]));
            //}

            //if (graph.lineSeries.Exists(x => x.name == name))
            //{
            //    graph.lineSeries.Find(x => x.name == name).GetComponent<WMG_Series>().pointValues = list;
            //}
            //else
            //{
            //    var series = graph.addSeries();
            //    series.gameObject.name = name;
            //    series.lineColor = Color.blue;
            //    series.seriesName = name;
            //    series.pointValues = list;
            //}

            name = "TotalEarned";
            list = new WMG_List<Vector2>();
            for (int i = 0; i < identityModel.moneyTotalMonth.Count; i++)
            {
                list.Add(new Vector2(i, (float)identityModel.moneyEarnedMonth[i]));
            }

            if (graph.lineSeries.Exists(x => x.name == name))
            {
                graph.lineSeries.Find(x => x.name == name).GetComponent<WMG_Series>().pointValues = list;

            }
            else
            {
                var series = graph.addSeries();
                series.gameObject.name = name;
                series.lineColor = Color.green;
                series.seriesName = name;
                series.pointValues = list;
            }

            list = new WMG_List<Vector2>();
            for (int i = 0; i < identityModel.moneyTotalMonth.Count; i++)
            {
                list.Add(new Vector2(i, (float)identityModel.moneySupplierContractsMonth[i]));
            }

            name = "TotalContracts";
            if (graph.lineSeries.Exists(x => x.name == name))
            {
                graph.lineSeries.Find(x => x.name == name).GetComponent<WMG_Series>().pointValues = list;
            }
            else
            {
                var series = graph.addSeries();
                series.gameObject.name = name;
                series.lineColor = Color.yellow;
                series.seriesName = name;
                series.pointValues = list;
            }


            //name = "TotalWorkers";
            //list = new WMG_List<Vector2>();
            //for (int i = 0; i < identityModel.moneyTotalMonth.Count; i++)
            //{
            //    list.Add(new Vector2(i, (float)identityModel.moneyWorkersMonth[i]));
            //}

            
            //if (graph.lineSeries.Exists(x => x.name == name))
            //{
            //    graph.lineSeries.Find(x => x.name == name).GetComponent<WMG_Series>().pointValues = list;
            //}
            //else
            //{
            //    var series = graph.addSeries();
            //    series.gameObject.name = name;
            //    series.lineColor = Color.red;
            //    series.seriesName = name;
            //    series.pointValues = list;
            //}

            //name = "TotalConstruction";
            //list = new WMG_List<Vector2>();
            //for (int i = 0; i < identityModel.moneyTotalMonth.Count; i++)
            //{
            //    list.Add(new Vector2(i, (float)identityModel.moneyConstructionMonth[i]));
            //}


            //if (graph.lineSeries.Exists(x => x.name == name))
            //{
            //    graph.lineSeries.Find(x => x.name == name).GetComponent<WMG_Series>().pointValues = list;
            //}
            //else
            //{
            //    var series = graph.addSeries();
            //    series.gameObject.name = name;
            //    series.lineColor = Color.gray;
            //    series.seriesName = name;
            //    series.pointValues = list;
            //}

            graph.Refresh();
        };
        graph.Init();


        button = Instantiate(uiButtonInstance, titleContent.transform);
        button.GetComponentInChildren<Text>().text = "Own Contracts";
        //name.GetComponent<Image>().color = gov.spriteColor;
        menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
        index = menuContent.Count - 1;
        position = menuContent[index].transform;
        button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);

        ButtonList<Contract>(identityModel.Id, identityModel.GetConstructionContracts(), position, (contract) =>
        {
            string output = "Constructing: " + contract.itemName +
                     " : " + Units.ReadItem(contract.monthlyCost) +
                     "c\nUnit Price: " + Units.ReadItem(contract.unitPrice) +
                     "c | Distnace Cost: " + Units.ReadItem(contract.PricePerKm * contract.distance * 2) + " c";
            return output;
        }, (contract) => () => GameManager.instance.OpenInfoPanel(contract.id, TargetType.Contract), (contract, but) => {

            Image image = but.GetComponent<Image>();
            if (contract.contractState == ContractState.Active) image.color = Color.green;
            else image.color = Color.white;
        });

        ButtonList<Contract>(identityModel.Id, identityModel.GetContracts(), position, (contract) =>
        {
            string output = "Item: " + contract.itemName +
                     " : " + Units.ReadRate(contract.itemRate) +
                     "\nUnit Price: " + Units.ReadItem(contract.unitPrice) +
                     "c | Distnace Cost: " + Units.ReadItem(contract.PricePerKm * contract.distance * 2) + " c";
            return output;
        }, (contract) => () => GameManager.instance.OpenInfoPanel(contract.id, TargetType.Contract), (contract, but) => {

            Image image = but.GetComponent<Image>();
            if (contract.contractState == ContractState.Active) image.color = Color.green;
            else image.color = Color.white;
        });

        

        button = Instantiate(uiButtonInstance, titleContent.transform);
        button.GetComponentInChildren<Text>().text = "Owned Ships";
        //name.GetComponent<Image>().color = gov.spriteColor;
        menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
        index = menuContent.Count - 1;
        position = menuContent[index].transform;
        button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);

        ButtonList<Ship>(identityModel.Id, identityModel.GetOwnedShips(), position, (ship) =>
        {
            string output = "Name : " + ship.name +
                "\nLocation: " + GameManager.instance.locations[ship.referenceId].name;
            if (ship.shipTargetId != null)
            {
                Structure structure = GameManager.instance.locations[ship.shipTargetId] as Structure;
                double distance = Vector3d.Distance(structure.SystemPosition, ship.SystemPosition);
                output += "\nTarget: " + structure.id +
                "\nDistance: " + Units.ReadDistance(distance) +
                " : ETA: " + Dated.ReadTime(distance / ship.SubLightSpeed);
            }
            return output;
        }, (ship) => () => GameManager.instance.OpenInfoPanel(ship), (ship, but) => {

            Image image = but.GetComponent<Image>();
            if (ship.shipAction == ShipAction.Moving) image.color = Color.green;
            else if (ship.shipAction == ShipAction.Refueling) image.color = Color.yellow;
            else if (ship.shipAction == ShipAction.Docked) image.color = Color.blue;
            else image.color = Color.white;
        });

        BuildList(identityModel.Id,identityModel.GetStructures());
    }

    private void BuildList(string name, List<Structure> list)
    {
        Button button = Instantiate(uiButtonInstance, titleContent.transform);
        button.GetComponentInChildren<Text>().text = "Structures";
        //name.GetComponent<Image>().color = gov.spriteColor;
        menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
        int index = menuContent.Count - 1;
        var position = menuContent[index].transform;
        button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);

        ButtonList<Structure>(name, list, position, (structure) =>
        {
            string output = "Name : " + +structure.Count + " " + structure.name;

            if (structure as ProductionStructure != null)
            {
                ProductionStructure productionStructure = (ProductionStructure)structure;
                output += " (" + productionStructure.clientContracts.Count + ")";
            }

            output += " - " + GameManager.instance.locations[structure.referenceId].name;
            return output;
        }, (item) => () => GameManager.instance.OpenInfoPanel(item), (structure, but) => {

            if (structure as ProductionStructure != null)
            {
                ProductionStructure productionStructure = (ProductionStructure)structure;
                Image image = but.GetComponent<Image>();
                if (productionStructure.isProducing && productionStructure.clientContracts.Count > 0)
                {
                    image.color = Color.green;
                }
                else if (productionStructure.isProducing) image.color = Color.yellow;
                else if (productionStructure.clientContracts.Count > 0) image.color = Color.blue;
                else image.color = Color.red;
            }
        });
    }

    private void BuildCompany()
    {
        CompanyModel comp = model.target.Model as CompanyModel;
        title.text = comp.name;
        Button button = Instantiate(uiButtonInstance, titleContent.transform);
        button.GetComponentInChildren<Text>().text = "Overview";
        button.GetComponent<Image>().color = comp.spriteColor;
        menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
        var index = menuContent.Count - 1;
        button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
        var position = menuContent[index].transform;


        button = Instantiate(uiButtonInstance, position);
        var creature = GameManager.instance.data.creatures.Model.GetCreature(comp.ceoId);
        button.GetComponentInChildren<Text>().text = "CEO: " + creature.name;
        button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(creature));

        button = Instantiate(uiButtonInstance, position);
        button.GetComponentInChildren<Text>().text = "Headquarters: " + GameManager.instance.data.getSolarBody(comp.referenceId).name;
        button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(comp.referenceId));

        Button(position, () =>
        {
            return "Money: " + Units.ReadItem(comp.moneyTotal) + "c";
        });
    }

    private void BuildGovernment()
    {
        GovernmentModel gov = (GovernmentModel)model.target.Model;
        title.text = gov.name;
        Button button = Instantiate(uiButtonInstance, titleContent.transform);
        button.GetComponentInChildren<Text>().text = "Overview";
        button.GetComponent<Image>().color = gov.spriteColor;
        menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
        var index = menuContent.Count - 1;
        button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
        var position = menuContent[index].transform;


        button = Instantiate(uiButtonInstance, position);
        var creature = GameManager.instance.data.creatures.Model.GetCreature(gov.leaders[0]);
        button.GetComponentInChildren<Text>().text = "Leader: " + creature.name;
        button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(creature));

        button = Instantiate(uiButtonInstance, position);
        button.GetComponentInChildren<Text>().text = "Capital: " + GameManager.instance.data.getSolarBody(gov.referenceId).name;
        button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(gov.referenceId));


        Button(position, () =>
        {
            return "Money: " + Units.ReadItem(gov.moneyTotal) + "c";
        });


        Button(position, () =>
        {
            return "Star Count:" + gov.stars.Count;
        });

        BuildList(gov.Id, gov.licensedCompanies.ToList());

        button = Instantiate(uiButtonInstance, titleContent.transform);
        button.GetComponentInChildren<Text>().text = "All Contracts";
        //name.GetComponent<Image>().color = gov.spriteColor;
        menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
        index = menuContent.Count - 1;
        position = menuContent[index].transform;
        button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);

        ButtonList<Contract>(gov.Id, gov.GetPostedContracts(), position, (contract) =>
        {
            string output = "Item: " + contract.itemName +
                     " : " + Units.ReadRate(contract.itemRate) +
                     "\nUnit Price: " + Units.ReadItem(contract.unitPrice) +
                     "c | Distnace Cost: " + Units.ReadItem(contract.PricePerKm * contract.distance * 2) + " c";
            return output;
        }, (contract) => () => GameManager.instance.OpenInfoPanel(contract.id, TargetType.Contract), (contract, but) => {

            Image image = but.GetComponent<Image>();
            if (contract.contractState == ContractState.Active) image.color = Color.green;
            else image.color = Color.white;
        });

        button = Instantiate(uiButtonInstance, titleContent.transform);
        button.GetComponentInChildren<Text>().text = "Supply and Demand";
        //name.GetComponent<Image>().color = gov.spriteColor;
        menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
        index = menuContent.Count - 1;
        position = menuContent[index].transform;
        button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);

        ButtonList<SupplyDemand>(gov.Id, gov.supplyDemand, position, (supplyDemand) =>
        {
            string output = "Item: " + supplyDemand.name +
                " | Amount: " + Units.ReadItem(supplyDemand.totalItemAmount) +
                "u\nSupply/Demand: " + Units.ReadItem(supplyDemand.itemSupply) +
                " : " + Units.ReadItem(supplyDemand.itemDemand) +
                "\nAverage Price: " + Units.ReadItem(supplyDemand.marketPrice) + "c";
            return output;
        }, (supplyDemand) => () => GameManager.instance.OpenInfoPanel(supplyDemand.id, TargetType.Item), (item, but) => {
            Image image = but.GetComponent<Image>();
            if (item.itemSupply > item.itemDemand)
            {
                if (item.itemDemand == 0)
                    image.color = Color.green;
                else
                    image.color = new Color(0, (float)(item.itemSupply / item.itemDemand / 5), 0);

            }
            else if (item.itemSupply < item.itemDemand)
            {
                if (item.itemSupply == 0)
                    image.color = Color.red;
                else
                    image.color = new Color((float)(item.itemDemand / item.itemSupply / 5), 0, 0);
            }
            else image.color = Color.black;
        });
    }

    private void BuildList(string name, List<IdentityModel> companies)
    {
        Button button = Instantiate(uiButtonInstance, titleContent.transform);
        button.GetComponentInChildren<Text>().text = "Companies";
        //name.GetComponent<Image>().color = gov.spriteColor;
        menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
        int index = menuContent.Count - 1;
        Transform position = menuContent[index].transform;
        button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);

        ButtonList<IdentityModel>(name, companies, position, (company) =>
        {
            string output = "Name: " + company.name + " | " + Units.ReadItem(company.moneyTotal) +
                    "c\nShips: " + Units.ReadItem(company.ownedShipIds.Count) +
                    "\nLocation: " + GameManager.instance.locations[company.referenceId].name;
            return output;
        }, (company) => () => GameManager.instance.OpenInfoPanel(company), (company, but) => {

            Image image = but.GetComponent<Image>();
            if (company.moneyTotal > 0) image.color = Color.green;
            else image.color = Color.red;
        });
    }

    private void BuildList(string name, List<CompanyModel> companies)
    {
        Button button = Instantiate(uiButtonInstance, titleContent.transform);
        button.GetComponentInChildren<Text>().text = "Companies";
        //name.GetComponent<Image>().color = gov.spriteColor;
        menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
        int index = menuContent.Count - 1;
        Transform position = menuContent[index].transform;
        button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);

        ButtonList<CompanyModel>(name, companies, position, (company) =>
        {
            string output = "Name: " + company.name + " | " + Units.ReadItem(company.moneyTotal) +
                    "c\nShips: " + Units.ReadItem(company.ownedShipIds.Count) +
                    "\nLocation: " + GameManager.instance.locations[company.referenceId].name;
            return output;
        }, (company) => () => GameManager.instance.OpenInfoPanel(company), (company, but) => {

            Image image = but.GetComponent<Image>();
            if (company.moneyTotal > 0) image.color = Color.green;
            else image.color = Color.red;
        });
    }

    private void BuildContract()
    {
        Contract contract = GameManager.instance.contracts[model.id];
        title.text = contract.itemName + " Contract: " + contract.id;

        Button button = Instantiate(uiButtonInstance, titleContent.transform);
        button.GetComponentInChildren<Text>().text = "Overview";
        //name.GetComponent<Image>().color = gov.spriteColor;
        menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
        var index = menuContent.Count - 1;
        button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
        var position = menuContent[index].transform;


        Button(position, () =>
        {
            return "State: " + contract.contractState.ToString();
        });

        Button(position, () =>
        {
            return "Duration: " + Dated.ReadTime(contract.duration);
        });

        if (contract.contractEndDate != null)
        {
            Button(position, () =>
            {
                return "Contract End Date: " + contract.contractEndDate.GetFormatedDate();
            });
        }

        Button(position, () =>
        {
            return "Renewable: " + contract.reknewable.ToString();
        });

        IdentityModel identity = contract.owner.Model;
        button = Instantiate(uiButtonInstance, position);
        //if (company.contractState == ContractState.Active) button.GetComponent<Image>().color = Color.green;
        Text text = button.GetComponentInChildren<Text>();
        text.text = "Owner: " + identity.name;
        button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(identity));

        if (contract.client != null)
        {
            identity = contract.client.Model;
            button = Instantiate(uiButtonInstance, position);
            //if (company.contractState == ContractState.Active) button.GetComponent<Image>().color = Color.green;
            text = button.GetComponentInChildren<Text>();
            text.text = "Client: " + identity.name;
            button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(identity));
        }
        

        button = Instantiate(uiButtonInstance, position);
        button.GetComponentInChildren<Text>().text = "Item: " + contract.itemName;
        button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(contract.itemId, TargetType.Item));

        Button(position, () =>
        {
            return "Cost Per Month: " + Units.ReadItem(contract.monthlyCost) + "c";
        });

        if (contract.payDate != null)
        {
            Button(position, () =>
            {
                return "Payment Date: " + contract.payDate.GetFormatedDate();
            });
        }

        Button(position, () =>
        {
            return "Item Rate: " + Units.ReadRate(contract.itemRate);
        });

        Button(position, () =>
        {
            return "Total Amount: " + Units.ReadItem(contract.itemAmount) + "u";
        });

        Button(position, () =>
        {
            return "Unit Price: " + Units.ReadItem(contract.unitPrice) + "c";
        });

        Button(position, () =>
        {
            return "Price Per Gm: " + Units.ReadItem(contract.PricePerKm * Units.G) + "c";
        });

        Button(position, () =>
        {
            return "Distance: " + Units.ReadDistance(contract.distance);
        });

        Button(position, () =>
        {
            return "Distnace Cost: " + Units.ReadItem(contract.PricePerKm * contract.distance * 2);
        });

        button = Instantiate(uiButtonInstance, position);
        button.GetComponentInChildren<Text>().text = "Origin: " + GameManager.instance.locations[contract.originId].name;
        button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(contract.originId));

        if (contract.destinationId != null)
        {
            button = Instantiate(uiButtonInstance, position);
            button.GetComponentInChildren<Text>().text = "Destination: " + GameManager.instance.locations[contract.destinationId].name;
            button.onClick.AddListener(() => GameManager.instance.OpenInfoPanel(contract.destinationId));
        }

        if (contract.shipIds.Count > 0)
        {
            

            button = Instantiate(uiButtonInstance, titleContent.transform);
            button.GetComponentInChildren<Text>().text = "Assigned Ships";
            //name.GetComponent<Image>().color = gov.spriteColor;
            menuContent.Add(Instantiate(infoContentPanel, contentPosition.transform));
            index = menuContent.Count - 1;
            position = menuContent[index].transform;
            button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
            var scroll = Instantiate(scrollViewPrefab, position);
            position = scroll.Content.transform;

            ButtonList<Ship>(contract.id, contract.GetShips(), position, (ship) =>
            {
                string output = "Name : " + ship.name +
                                "\nLocation: " + GameManager.instance.locations[ship.referenceId].name;
                if (ship.shipTargetId != null)
                {
                    Structure structure = GameManager.instance.locations[ship.shipTargetId] as Structure;
                    double distance = Vector3d.Distance(structure.SystemPosition, ship.SystemPosition);
                    output += "\nTarget: " + structure.id +
                        "\nDistance: " + Units.ReadDistance(distance) +
                        "\nETA: " + Dated.ReadTime(distance / ship.SubLightSpeed);
                }
                return output;
            }, (ship) => () => GameManager.instance.OpenInfoPanel(ship), (ship, but) => {

                Image image = but.GetComponent<Image>();
                if (ship.shipAction == ShipAction.Moving) image.color = Color.green;
                else image.color = Color.grey;
            });
        }

        SelectPanel(0);
    }

    private void BuildGovernments()
    {
        title.text = "Governments                                                              " + GameManager.instance.data.governments.Count;

        Button button = Instantiate(uiButtonInstance, titleContent.transform);
        button.GetComponentInChildren<Text>().text = "Overview";
        //name.GetComponent<Image>().color = gov.spriteColor;
        var scroll = Instantiate(scrollViewPrefab, contentPosition.transform);
        menuContent.Add(scroll.Content); 
        var index = menuContent.Count - 1;
        button.GetComponent<ButtonInfo>().SetPanelToggle(index, this);
        var position = menuContent[index].transform;

        foreach (GovernmentModel government in GameManager.instance.data.governments)
        {
            Button(position, () => 
            { return "Name: " + government.name + " | " + Units.ReadItem(government.moneyTotal) +
                    "c\nCompanies: " + Units.ReadItem(government.licensedCompanies.Count);
            }, () => GameManager.instance.OpenInfoPanel(government));
        }
    }

    public void ButtonList<TValue, TKey>(string name, Dictionary<TValue, TKey> list, Transform position, StringDel<KeyValuePair<TValue, TKey>> listStringDel, ListUnityAction<KeyValuePair<TValue, TKey>> listCall = null, ButtonUpdate<KeyValuePair<TValue, TKey>> updateDel = null)
    {
        if (list.Count > 5)
        {
            var scroll = Instantiate(scrollViewPrefab, position);
            position = scroll.Content.transform;
        }

        TextUpdates += () =>
        {
            foreach (KeyValuePair<TValue, TKey> item in list)
            {
                if (!buttons.ContainsKey(name + item.GetHashCode().ToString()))
                {
                    Button(name + item.GetHashCode().ToString(), position, () => listStringDel(item), listCall(item));
                }
                else
                {
                    Button button = buttons[name + item.GetHashCode()];
                    Image image = button.GetComponent<Image>();
                    if (updateDel != null)
                        updateDel(item, button);
                }
            }
        };
    }

    public void ButtonList<T>( string name, List<T> list, Transform position, StringDel<T> listStringDel, ListUnityAction<T> listCall = null, ButtonUpdate<T> updateDel = null)
    {
        if (list.Count > 5)
        {
            var scroll = Instantiate(scrollViewPrefab, position);
            position = scroll.Content.transform;
        }
        else position = Instantiate(infoContentPanel, position).transform;

        TextUpdates += () =>
        {
            foreach (T item in list)
            {
                if (!buttons.ContainsKey(name + item.GetHashCode().ToString()))
                {
                    Button(name + item.GetHashCode().ToString(), position, () => listStringDel(item), listCall(item));
                }
                else
                {
                    Button button = buttons[name + item.GetHashCode()];
                    Image image = button.GetComponent<Image>();
                    if (updateDel != null)
                        updateDel(item, button);
                }
            }
        };
    }

    public delegate void ButtonUpdate<T>(T item, Button button);

    public delegate string StringDel<T>(T item);

    public delegate UnityEngine.Events.UnityAction ListUnityAction<T>(T item);

    public Button Button(Transform position, StringDel stringDel, UnityEngine.Events.UnityAction call = null)
    {
        Button button = Instantiate(uiButtonInstance, position);
        Text text = button.GetComponentInChildren<Text>();
        TextUpdates += () =>
        {
            text.text = stringDel();
        };
        if (call != null)
        {
            button.onClick.AddListener(call);
        }
        else
        {
            button.interactable = false;
        }

        return button;
    }

    public Button Button( string id, Transform position, StringDel stringDel, UnityEngine.Events.UnityAction call = null)
    {
        Button button = Button(position, stringDel, call);
        button.name = id;
        buttons.Add(id, button);
        return button;
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

        TextUpdates();
    }

    public delegate void UpdateDel();
    public delegate string StringDel();

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

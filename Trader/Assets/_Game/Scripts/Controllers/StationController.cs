using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using UnityEngine.UI;

public class StationController : Controller<StationModel> {

    public Text infoText;

    public string info;

    protected override void OnInitialize()
    {
        infoText = GameObject.FindGameObjectWithTag("InfoText").GetComponent<Text>();

        name = model.name;

        transform.position = model.position;
    }
	
	// Update is called once per frame
	void Update () {

        int factoryStatus = model.factory.UpdateProduction(Time.deltaTime * Time.timeScale);

        UpdateInfo();
	
	}

    private void UpdateInfo()
    {
        info = "";
        info += "Factory Name: " + model.factory.name + "\n\n";
        info += "Progress: " + (model.factory.productionTime / model.factory.unitTime).ToString("0.00") + "\n\n";

        foreach (Items item in model.factory.inputItems)
        {
            if (item.selfProducing == false)
                info += "Input " + item.name + ": " + item.amount + "/" + item.maxAmount + "| Price: " + item.price.ToString("0.00") + " - " +item.basePrice +  "\n";
            else
                info += "Input " + item.name + ": ---\n";
        }
        foreach (Items item in model.factory.outputItems)
        {
            info += "Output " + item.name + ": " + item.amount + "/" + item.maxAmount + "| Price: " + item.price.ToString("0.00") + " - " + item.basePrice + "\n";
        }
    }
}

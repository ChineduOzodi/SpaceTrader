using UnityEngine;
using System.Collections;
using System;
using CodeControl;

public class Factory {

    
    public string name;
    public ModelRef<StationModel> parent = new ModelRef<StationModel>();
    public ProductionItem[] inputItems;
    public ProductionItem[] outputItems;
    public Color color;
    public bool isProducing;
    /// <summary>
    /// Time it takes to go through one cycle
    /// </summary>
    public float unitTime;
    /// <summary>
    /// Current cycle time
    /// </summary>
    public double productionTime = 0;
    public float supply = 1;
    public float demand = 1;
    private float basePriceProfit = 5;
    public float efficiency = 1;

    public Factory()
    {

    }

    public Factory(bool orbit, StationModel _parent)
    {
        parent.Model = _parent;
        if (orbit)
        {
            int factory = UnityEngine.Random.Range(0, 7);

            if (factory == 0)
            {
                inputItems = new ProductionItem[] { ProductionItem.Wheat * 5, ProductionItem.Rock * 3 };
                outputItems = new ProductionItem[] { ProductionItem.Food * 2 };
                unitTime = UnityEngine.Random.Range(18f, 25f);
            }
            else if (factory == 1)
            {
                inputItems = new ProductionItem[] { ProductionItem.Rock * 25 };
                outputItems = new ProductionItem[] { ProductionItem.Sculpture };
                unitTime = UnityEngine.Random.Range(30f, 45f);
            }
            else if (factory == 2)
            {
                inputItems = new ProductionItem[] { ProductionItem.Rock * 5, ProductionItem.Steel * 10, ProductionItem.Glass * 2 };
                outputItems = new ProductionItem[] {  };

                unitTime = UnityEngine.Random.Range(30f, 45f);
            }
            else if (factory == 3)
            {
                inputItems = new ProductionItem[] { ProductionItem.IronOre * 5, ProductionItem.CoalOre * 2 };
                outputItems = new ProductionItem[] { ProductionItem.Steel * 2 };
                unitTime = UnityEngine.Random.Range(8f, 10f);
            }
            else if (factory == 4)
            {
                inputItems = new ProductionItem[] { ProductionItem.Rock * 5 };
                outputItems = new ProductionItem[] { ProductionItem.Glass * 2 };
                unitTime = UnityEngine.Random.Range(4f, 5f);
            }
            else if (factory == 5)
            {
                inputItems = new ProductionItem[] { ProductionItem.Steel * 250, ProductionItem.Glass * 100, ProductionItem.Fuel * 200 };
                outputItems = new ProductionItem[] { ProductionItem.Ship };
                unitTime = UnityEngine.Random.Range(100f, 150f);
            }
            else if (factory == 6)
            {
                inputItems = new ProductionItem[] { };
                outputItems = new ProductionItem[] { ProductionItem.Wheat * 10 };
                unitTime = UnityEngine.Random.Range(30f, 40f);
            }
        }
        else
        {
            inputItems = new ProductionItem[] { ProductionItem.Sculpture, ProductionItem.Food * 10};
            outputItems = new ProductionItem[] { };
            inputItems = new ProductionItem[] { inputItems[UnityEngine.Random.Range(0, inputItems.Length)] };
            unitTime = UnityEngine.Random.Range(30f, 40f);
        }
        

    }

    public Factory(RawResources rawResource, StationModel _parent)
    {
        parent.Model = _parent;
        if (rawResource == RawResources.CoalRock)
        {
            inputItems = new ProductionItem[] {};
            outputItems = new ProductionItem[] { ProductionItem.CoalOre * 5, ProductionItem.Rock * 3 };
            unitTime = UnityEngine.Random.Range(4f, 5f);
        }
        else if (rawResource == RawResources.IronRock)
        {
            inputItems = new ProductionItem[] { };
            outputItems = new ProductionItem[] { ProductionItem.IronOre * 7, ProductionItem.Rock * 4 };
            unitTime = UnityEngine.Random.Range(5f, 7f);
        }
        else if (rawResource== RawResources.UnrefinedFuel)
        {
            inputItems = new ProductionItem[] { };
            outputItems = new ProductionItem[] { ProductionItem.Fuel };
            unitTime = UnityEngine.Random.Range(1f, 2f);
        }
    }

    /// <summary>
    /// Creates items and uses items based on the elapsed time
    /// </summary>
    /// <param name="elapsedTime">time elapsed (in seconds)</param>
    public int UpdateProduction(double elapsedTime)
    {
        if (!isProducing)
        {
            int productionRun = 0;

            foreach (ProductionItem item in inputItems)
            {
                if (item.amount - item.productionAmount < 0)
                {
                    isProducing = false;
                    return -1;
                }
            }

            productionTime += elapsedTime * efficiency;

            while (productionTime > unitTime)
            {
                foreach (ProductionItem item in inputItems)
                {
                    item.amount -= item.productionAmount;
                    item.pendingAmount -= item.productionAmount;
                }
                foreach (ProductionItem item in outputItems)
                {
                    item.amount += item.productionAmount;
                    item.pendingAmount += item.productionAmount;
                }

                productionTime -= unitTime;
                productionRun++;
            }
            return productionRun;
        }
        else
            return -1;
    }
}

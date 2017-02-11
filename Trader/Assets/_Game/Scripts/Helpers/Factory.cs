using UnityEngine;
using System.Collections;
using System;

public class Factory {

    public string name;

    public Items[] inputItems;
    public Items[] outputItems;

    public float unitTime;
    public float productionTime = 0;

    public float efficiency = 1;

    public Factory(string _name, Items[] _inputItems, Items[] _outputItems, float _unitTime)
    {
        name = _name;
        inputItems = _inputItems;
        outputItems = _outputItems;
        unitTime = _unitTime;

        SetPrices();
    }

    private void SetPrices()
    {
        foreach (Items item in inputItems)
        {
            //float priceRange = item.basePrice * .25f;
            float priceAdj = (1 - (float)item.amount / item.maxAmount) + .5f;
            item.price = item.basePrice * priceAdj;
        }
        foreach (Items item in outputItems)
        {
            //float priceRange = item.basePrice * .25f;
            float priceAdj = (1 - (float)item.amount / item.maxAmount) + .5f;
            item.price = item.basePrice * priceAdj;
        }
    }

    /// <summary>
    /// Creates items and uses items based on the elapsed time
    /// </summary>
    /// <param name="elapsedTime">time elapsed (in seconds)</param>
    public int UpdateProduction(float elapsedTime)
    {
        int productionRun = 0;

        foreach (Items item in inputItems)
        {
            if (item.amount - item.ratio < 0)
            {
                return -1;
            }
        }

        foreach (Items item in outputItems)
        {
            if (item.amount + item.ratio > item.maxAmount)
            {
                return -1;
            }
        }

        productionTime += elapsedTime * efficiency;

        while (productionTime > unitTime)
        {
            foreach (Items item in inputItems)
            {
                if (item.selfProducing == false)
                    item.amount -= item.ratio;
            }
            foreach (Items item in outputItems)
            {
                item.amount += item.ratio;
            }

            productionTime -= unitTime;
            productionRun++;
            SetPrices();
        }
        return productionRun;
    }
}

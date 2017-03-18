using UnityEngine;
using System.Collections;
using System;

public class Factory {

    public string name;

    public Items[] inputItems;
    public Items[] outputItems;
    public Color color;
    public float unitTime;
    public float productionTime = 0;
    private float basePriceProfit = 5;
    public float efficiency = 1;

    public Factory()
    {

    }

    public Factory(string _name, Items[] _inputItems, Items[] _outputItems)
    {
        name = _name;
        inputItems = _inputItems;
        outputItems = _outputItems;
        name = _name + " Factory";
        unitTime = 0;
        foreach (Items item in inputItems)
        {
            unitTime += item.processTime * item.ratio;
        }
        SetPrices();
    }

    public Factory(FactoryType type)
    {
        Items coalRock = new Items("Coal Rock", 7, 7, 7000);
        coalRock.processTime = 1;
        coalRock.basePrice = 1f;
        Items coalOre = new Items("Coal Ore", 100, 3, 3000);
        coalOre.basePrice = (coalRock.basePrice * coalRock.ratio / coalRock.ratio) * basePriceProfit;
        Items rock = new Items("Rock", 100, 3, 3000);
        rock.basePrice = (coalRock.basePrice * coalRock.ratio / rock.ratio) * basePriceProfit;

        //Iron
        Items ironRock = new Items("Iron Rock", 7, 7, 7000);
        ironRock.processTime = 1.5f;
        ironRock.basePrice = 1.5f;
        Items ironOre = new Items("Iron Ore", 100, 2, 2000);
        ironOre.basePrice = (ironRock.basePrice * ironRock.ratio / ironOre.ratio) * basePriceProfit;

        //Fuel
        Items unrefinedFuel = new Items("Unrefined Fuel", 10, 10, 10000);
        unrefinedFuel.processTime = .5f;
        unrefinedFuel.basePrice = 1;
        coalOre.ratio = 1;
        Items fuel = new Items("Fuel", 5000, 10, 10000);
        fuel.basePrice = (coalOre.basePrice * coalOre.ratio + unrefinedFuel.basePrice * unrefinedFuel.ratio) / fuel.ratio * basePriceProfit;

        //Steel
        ironOre.ratio = 5;
        coalOre.ratio = 2;
        Items steel = new Items("Steel", 100, 5, 500);
        steel.basePrice = (ironOre.basePrice * ironOre.ratio + coalOre.basePrice * coalOre.ratio) / steel.ratio * basePriceProfit;

        //Glass
        rock.ratio = 5;
        Items glass = new Items("Glass", 100, 2, 2000);
        glass.basePrice = rock.basePrice * rock.ratio / glass.ratio * basePriceProfit;

        //Ship
        glass.ratio = 100;
        fuel.ratio = 200;
        steel.ratio = 250;
        Items ship = new Items("Ship", 5, 1, 100);
        ship.basePrice = (glass.basePrice * glass.ratio + steel.basePrice * steel.ratio + fuel.ratio * fuel.basePrice) / ship.ratio * basePriceProfit;

        //Wheat
        Items seed = new Items("Seed", 2, 1, 1000);
        seed.basePrice = 5;
        Items wheat = new Items("Wheat", 0, 5, 5000);
        wheat.basePrice = seed.basePrice * seed.ratio / wheat.ratio * basePriceProfit;

        //Food
        rock.ratio = 3;
        wheat.ratio = 5;
        Items food = new Items("Food", 100, 2, 2000);
        food.basePrice = (rock.basePrice * rock.ratio + wheat.basePrice * wheat.ratio) / food.ratio * basePriceProfit;

        //Sculpure
        rock.ratio = 10;
        Items sculpt = new Items("Sculpture", 0, 1, 100);
        sculpt.basePrice = rock.basePrice * rock.ratio / sculpt.ratio * basePriceProfit;

        if (type == FactoryType.Coal)
        {
            coalRock.selfProducing = true;

            coalOre.ratio = 3;
            coalOre.maxAmount = 3000;
            coalOre.amount = coalOre.maxAmount / 2;

            rock.ratio = 3;
            rock.maxAmount = 3000;
            rock.amount = rock.maxAmount / 2;

            inputItems = new Items[] { coalRock };
            outputItems = new Items[] { coalOre, rock };
        }
        else if (type == FactoryType.Iron)
        {
            ironRock.selfProducing = true;

            ironOre.ratio = 2;
            ironOre.maxAmount = 2000;
            ironOre.amount = ironOre.maxAmount / 2;

            rock.ratio = 3;
            rock.maxAmount = 3000;
            rock.amount = rock.maxAmount / 2;

            inputItems = new Items[] { ironRock };
            outputItems = new Items[] { ironOre, rock };
        }
        else if (type == FactoryType.Fuel)
        {
            unrefinedFuel.selfProducing = true;

            coalOre.ratio = 2;
            coalOre.maxAmount = 2000;
            coalOre.amount = coalOre.maxAmount / 2;

            fuel.ratio = 10;
            fuel.maxAmount = 10000;
            fuel.amount = fuel.maxAmount / 2;

            inputItems = new Items[] { coalOre, unrefinedFuel };
            outputItems = new Items[] { fuel };
        }
        else if (type == FactoryType.Steel)
        {
            ironOre.ratio = 5;
            ironOre.maxAmount = 500;
            ironOre.amount = ironOre.maxAmount / 2;

            coalOre.ratio = 2;
            coalOre.maxAmount = 200;
            coalOre.amount = coalOre.maxAmount / 2;

            steel.ratio = 5;
            steel.maxAmount = 500;
            steel.amount = steel.maxAmount / 2;

            inputItems = new Items[] { ironOre, coalOre };
            outputItems = new Items[] { steel };
        }
        else if (type == FactoryType.Glass)
        {
            rock.ratio = 5;
            rock.maxAmount = 500;
            rock.amount = rock.maxAmount / 2;

            glass.ratio = 2;
            glass.maxAmount = 200;
            glass.amount = glass.maxAmount / 2;

            inputItems = new Items[] { rock };
            outputItems = new Items[] { glass };
        }
        else if (type == FactoryType.Ship)
        {
            glass.ratio = 100;
            glass.maxAmount = 1000;
            glass.amount = glass.maxAmount / 2;

            fuel.ratio = 200;
            fuel.maxAmount = 2000;
            fuel.amount = fuel.maxAmount / 2;

            steel.ratio = 250;
            steel.maxAmount = 2500;
            steel.amount = steel.maxAmount / 2;

            ship.ratio = 1;
            ship.maxAmount = 10;
            ship.selfProducing = true;

            inputItems = new Items[] { steel, glass, fuel };
            outputItems = new Items[] { ship };
        }
        else if (type == FactoryType.Wheat)
        {
            
            seed.selfProducing = true;

            wheat.ratio = 5;
            wheat.maxAmount = 500;
            wheat.amount = wheat.maxAmount / 2;

            inputItems = new Items[] { seed, };
            outputItems = new Items[] { wheat };
        }
        else if (type == FactoryType.Food)
        {            
            rock.ratio = 3;
            rock.maxAmount = 300;
            rock.amount = rock.maxAmount / 2;

            wheat.ratio = 5;
            wheat.maxAmount = 500;
            wheat.amount = wheat.maxAmount / 2;

            food.ratio = 2;
            food.maxAmount = 200;
            food.amount = food.maxAmount / 2;

            inputItems = new Items[] { wheat, rock };
            outputItems = new Items[] { food };
        }
        else if (type == FactoryType.Sculpture)
        {
            rock.ratio = 10;
            rock.maxAmount = 1000;
            rock.amount = rock.maxAmount / 2;

            sculpt.ratio = 1;
            sculpt.maxAmount = 100;
            sculpt.amount = sculpt.maxAmount / 2;

            inputItems = new Items[] { rock };
            outputItems = new Items[] { sculpt };
        }
        else if (type == FactoryType.Consumer)
        {
            glass.ratio = 10;
            glass.maxAmount = 1000;
            glass.amount = glass.maxAmount / 2;

            sculpt.ratio = 1;
            sculpt.maxAmount = 100;
            sculpt.amount = sculpt.maxAmount / 2;

            food.ratio = 10;
            food.maxAmount = 1000;
            food.amount = food.maxAmount / 2;

            steel.ratio = 10;
            steel.maxAmount = 1000;
            steel.amount = steel.maxAmount / 2;

            Items cons = new Items("Consume", 0, 1, 10);
            cons.selfProducing = true;
            int index = UnityEngine.Random.Range(0, 4);
            

            inputItems = new Items[] { glass, sculpt, food, steel };
            outputItems = new Items[] { cons };

            cons.basePrice = inputItems[index].basePrice * inputItems[index].ratio * 2;
            inputItems = new Items[] { inputItems[index] };
        }
        unitTime = 0;
        foreach (Items item in inputItems)
        {
            unitTime += Mathf.Pow(.1f, 1/item.basePrice) * item.ratio * basePriceProfit * .5f;
        }
        
        name = type.ToString() + " Factory";
        SetPrices();
    }

    public void SetPrices()
    {
        foreach (Items item in inputItems)
        {
            //float priceRange = item.basePrice * .25f;
            float priceAdj = (1 - (float)item.pendingAmount / item.maxAmount) + .5f;
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
                {
                    item.amount -= item.ratio;
                    item.pendingAmount -= item.ratio;
                }
                    
            }
            foreach (Items item in outputItems)
            {
                if (item.selfProducing == false)
                    item.amount += item.ratio;
            }

            productionTime -= unitTime;
            productionRun++;
            SetPrices();
        }
        return productionRun;
    }
}

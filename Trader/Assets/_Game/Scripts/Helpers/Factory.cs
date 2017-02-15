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
        Items coalOre = new Items("Coal Ore", 100, 3, 3000);
        coalOre.basePrice = (coalRock.basePrice * coalRock.ratio / coalRock.ratio) * 2;
        Items rock = new Items("Rock", 100, 3, 3000);
        rock.basePrice = (coalRock.basePrice * coalRock.ratio / rock.ratio) * 2;

        //Iron
        Items ironRock = new Items("Iron Rock", 7, 7, 7000);
        ironRock.basePrice = 1.5f;
        Items ironOre = new Items("Iron Ore", 100, 2, 2000);
        ironOre.basePrice = (ironRock.basePrice * ironRock.ratio / ironOre.ratio) * 2;

        //Fuel
        Items unrefinedFuel = new Items("Unrefined Fuel", 10, 10, 10000);
        coalOre.ratio = 2;
        Items fuel = new Items("Fuel", 100, 10, 10000);
        fuel.basePrice = (coalOre.basePrice * coalOre.ratio + unrefinedFuel.basePrice * unrefinedFuel.ratio) / fuel.ratio * 2;

        //Steel
        ironOre.ratio = 5;
        coalOre.ratio = 2;
        Items steel = new Items("Steel", 100, 5, 500);
        steel.basePrice = (ironOre.basePrice * ironOre.ratio + coalOre.basePrice * coalOre.ratio) / steel.ratio * 2;

        //Glass
        rock.ratio = 5;
        Items glass = new Items("Glass", 100, 2, 2000);
        glass.basePrice = rock.basePrice * rock.ratio / glass.ratio * 2;

        //Ship
        glass.ratio = 10;
        steel.ratio = 25;
        Items ship = new Items("Ship", 0, 1, 100);
        ship.basePrice = (glass.basePrice * glass.ratio + steel.basePrice * steel.ratio) / ship.ratio * 2;

        //Wheat
        Items seed = new Items("Seed", 2, 1, 1000);
        seed.basePrice = 5;
        Items wheat = new Items("Wheat", 0, 5, 5000);
        wheat.basePrice = seed.basePrice * seed.ratio / wheat.ratio * 2;

        //Food
        rock.ratio = 5;
        wheat.ratio = 5;
        Items food = new Items("Food", 100, 2, 2000);
        food.basePrice = (rock.basePrice * rock.ratio + wheat.basePrice * wheat.ratio) / food.ratio * 2;

        //Sculpure
        rock.ratio = 10;
        Items sculpt = new Items("Sculpture", 0, 1, 100);
        sculpt.basePrice = rock.basePrice * rock.ratio / sculpt.ratio * 2;

        if (type == FactoryType.Coal)
        {
            coalRock.selfProducing = true;

            coalOre.ratio = 3;
            coalOre.maxAmount = 3000;

            rock.ratio = 3;
            rock.maxAmount = 3000;

            inputItems = new Items[] { coalRock };
            outputItems = new Items[] { coalOre, rock };
        }
        else if (type == FactoryType.Iron)
        {
            ironOre.ratio = 2;
            ironOre.maxAmount = 2000;
            ironRock.selfProducing = true;

            rock.ratio = 3;
            rock.maxAmount = 3000;

            inputItems = new Items[] { ironRock };
            outputItems = new Items[] { ironOre, rock };
        }
        else if (type == FactoryType.Fuel)
        {
            unrefinedFuel.selfProducing = true;
            coalOre.ratio = 2;
            coalOre.maxAmount = 2000;

            inputItems = new Items[] { coalOre, unrefinedFuel };
            outputItems = new Items[] { fuel };
        }
        else if (type == FactoryType.Steel)
        {
            ironOre.ratio = 5;
            ironOre.maxAmount = 500;
            coalOre.ratio = 2;
            coalOre.maxAmount = 200;
            steel.ratio = 5;
            steel.maxAmount = 500;

            inputItems = new Items[] { ironOre, coalOre };
            outputItems = new Items[] { steel };
        }
        else if (type == FactoryType.Glass)
        {
            rock.ratio = 5;
            rock.maxAmount = 500;

            glass.ratio = 2;
            glass.maxAmount = 200;

            inputItems = new Items[] { rock };
            outputItems = new Items[] { glass };
        }
        else if (type == FactoryType.Ship)
        {
            glass.ratio = 10;
            glass.maxAmount = 100;
            
            steel.ratio = 25;
            steel.maxAmount = 250;

            ship.ratio = 1;
            ship.maxAmount = 10;

            inputItems = new Items[] { steel, glass };
            outputItems = new Items[] { ship };
        }
        else if (type == FactoryType.Wheat)
        {
            
            seed.selfProducing = true;

            wheat.ratio = 5;
            wheat.maxAmount = 500;

            inputItems = new Items[] { seed, };
            outputItems = new Items[] { wheat };
        }
        else if (type == FactoryType.Food)
        {            
            rock.ratio = 5;
            rock.maxAmount = 500;
            wheat.ratio = 5;
            wheat.maxAmount = 500;

            inputItems = new Items[] { wheat, rock };
            outputItems = new Items[] { food };
        }
        else if (type == FactoryType.Sculpture)
        {
            rock.ratio = 10;
            rock.maxAmount = 100;

            sculpt.ratio = 1;
            sculpt.maxAmount = 10;

            inputItems = new Items[] { rock };
            outputItems = new Items[] { sculpt };
        }
        else if (type == FactoryType.Consumer)
        {
            glass.ratio = 10;
            glass.maxAmount = 100;
            sculpt.ratio = 1;
            sculpt.maxAmount = 10;
            food.ratio = 10;
            food.maxAmount = 100;
            steel.ratio = 10;
            steel.maxAmount = 100;

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
            unitTime += item.basePrice * .25f * item.ratio;
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
                item.amount += item.ratio;
            }

            productionTime -= unitTime;
            productionRun++;
            SetPrices();
        }
        return productionRun;
    }
}

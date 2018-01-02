using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionStructure: IStructure {

    public List<Item> requiredItems { get; protected set; }

    public string productionItemName;
    public int productionItemId { get; protected set; }
    /// <summary>
    /// Time to complete one cycle in seconds.
    /// </summary>
    public double productionTime { get; protected set; }
    /// <summary>
    /// The ids of structures connecting into this structure
    /// </summary>
    public List<int> structureConnectionIdsIn = new List<int>();
    /// <summary>
    /// The ids of structures connecting out of this structure
    /// </summary>
    public List<int> structureConnectionIdsOut = new List<int>();
    /// <summary>
    /// Stores information of the items that are needed or that have surpluses.
    /// </summary>
    public List<KeyValuePair<int, double>> connectionItems = new List<KeyValuePair<int, double>>();
    public ItemStorage storage = new ItemStorage();

    public StructureTypes structureType { get; set; }

    public string name { get; set; }
    public string info { get; set; }
    public ModelRef<IdentityModel> owner { get; set; }
    public int managerId { get; set; }
    public float maxArmor { get; set; }
    public float currentArmor { get; set; }
    public int id { get; set; }
    public Dated dateCreated { get; set; }
    public Dated lastUpdated { get; set; }
    public bool deleteStructure { get; set; }

    public Vector2d galaxyPosition
    {
        get
        {
            return body.galaxyPosition;
        }

        set
        {
            throw new System.Exception("Can't set galaxyPosition, set solarIndex instead");
        }
    }

    public List<int> solarIndex { get; set; }
    public int structureId { get; set; }
    public int shipId { get; set; }

    public int count { get; set; }

    private SolarBody body
    {
        get
        {
            if (solarIndex.Count == 3)
            {
                return GameManager.instance.data.stars[solarIndex[0]].solar.satelites[solarIndex[1]].satelites[solarIndex[2]];
            }
            else if (solarIndex.Count == 2)
            {
                return GameManager.instance.data.stars[solarIndex[0]].solar.satelites[solarIndex[1]];
            }
            else
            {
                throw new System.Exception("Structure " + name + " solarIndex count incorrect: " + solarIndex.Count);
            }
        }
    }
    /// <summary>
    /// The rate of output to each connection. The key is the structure id.
    /// </summary>
    internal Dictionary<int, double> connectionOutRate = new Dictionary<int, double>();
    /// <summary>
    /// The item ids and rate of items the factory needs.
    /// </summary>
    internal Dictionary<int, double> neededItemRate = new Dictionary<int, double>();
    public double extraProductionRate = 0;

    public ProductionStructure() { }

    public ProductionStructure(IdentityModel owner, int _productionItemId, SolarBody body, int _count = 1)
    {
        storage = new ItemStorage();

        id = GameManager.instance.data.id++;
        this.owner = new ModelRef<IdentityModel>(owner);
        solarIndex = body.solarIndex;

        productionItemId = _productionItemId;
        var product = GameManager.instance.data.itemsData.Model.GetItem(productionItemId);
        name = product.name + " " + structureType.ToString() + " " + id;
        productionItemName = product.name;

        requiredItems = product.contstructionParts;
        requiredItems.ForEach(x => {
            x.price = GameManager.instance.data.getSolarBody(solarIndex).GetMarketPrice(x.id);
        });

        foreach (IStructure structure in body.structures)
        {

            //Look to see if similar structure already exists
            if (GetType() == structure.GetType())
            {
                //If the same structure is found, check ownership, and productionItemId
                ProductionStructure prodStruct = (ProductionStructure)structure;
                if (prodStruct.owner.Model == owner && prodStruct.productionItemId == _productionItemId)
                {
                    //If found, increase count
                    prodStruct.count += _count;
                    return;
                }
            }

            ProductionStructure productionStructure = structure as ProductionStructure;
            if (productionStructure != null && productionStructure.GetType() != Type.GetType("Station"))
            {
                foreach(Item item in productionStructure.requiredItems)
                {
                    if (item.id == productionItemId)
                    {
                        if (!structureConnectionIdsOut.Contains(productionStructure.id))
                        {
                            structureConnectionIdsOut.Add(productionStructure.id);
                            productionStructure.structureConnectionIdsIn.Add(id);
                        }
                    }
                }

                foreach (Item item in requiredItems)
                {
                    if (item.id == productionStructure.productionItemId)
                    {
                        if (!structureConnectionIdsIn.Contains(productionStructure.productionItemId))
                        {
                            structureConnectionIdsIn.Add(productionStructure.id);
                            productionStructure.structureConnectionIdsOut.Add(id);
                        }
                    }
                }
                
            }
        }

        
        owner.AddSolarBodyWithStructure(body);
        body.structures.Add(this);

        
        count = _count;
        
        structureId = -1;
        shipId = -1;

        
        

        maxArmor = 1000;
        currentArmor = maxArmor;
        productionTime = product.productionTime;

        dateCreated = new Dated(GameManager.instance.data.date.time);
        lastUpdated = new Dated(GameManager.instance.data.date.time);

        UpdateConnectionItems();
    }

    protected bool SearchRequiredItems(SolarBody parentBody, double deltaTime)
    {
        int neededItemCount = requiredItems.Count;
        int itemCount = 0;
        foreach (Item item in requiredItems)
        {
            double neededAmount = item.amount * count;
            if (storage.ContainsItem(item))
            {
                neededAmount -= storage.Find(item).amount;
            }
            if (neededAmount <= 0)
            {
                item.price = GameManager.instance.data.getSolarBody(solarIndex).GetMarketPrice(item.id);
                parentBody.RemoveBuying(item.id, id, item.amount);
                itemCount++;
            }
            else
            {
                var neededItem = new Item(item.id, neededAmount, item.price, solarIndex, id);
                var found = false;
                foreach (IStructure structure in parentBody.structures)
                {
                    if (structure.structureType == StructureTypes.GroundStorage)
                    {
                        GroundStorage groundStruct = (GroundStorage)structure;
                        if (groundStruct.owner.Model == owner.Model && groundStruct.ContainsItem(neededItem))
                        {
                            double itemAmount = neededItem.amount;
                            Item unusedItem = groundStruct.UseAsMuchItem(neededItem);
                            neededItem.SetAmount(itemAmount - unusedItem.amount, item.price);
                            storage.AddItem(neededItem);


                            parentBody.RemoveBuying(item.id, id, neededItem.amount);
                            parentBody.RemoveSelling(item.id, id, neededItem.amount);
                            neededAmount = itemAmount - neededItem.amount;
                            if (neededAmount <= 0)
                            {
                                itemCount++;
                                found = true;
                                break;
                            }
                            neededItem = new Item(item.id, neededAmount, item.price, solarIndex, id);
                        }
                    }
                }
                //Runs if there are still needed Items
                if (!found)
                {
                    parentBody.SetBuying(neededItem);
                    item.price += item.price * GameManager.instance.marketPriceMod * deltaTime * productionTime;
                    if (item.price < .1)
                    {
                        item.price = .1f;
                    }
                }
            }

        }
        return (neededItemCount == itemCount);
    }

    protected void UseRequiredItems(SolarBody parentBody)
    {
        foreach (Item item in requiredItems)
        {
            Item use = new Item(item.id, item.amount * count, item.price, item.solarIndex, item.structureId);
            var found = storage.UseItem(use);
            if (!found)
                throw new System.Exception("Item is not found in correct amount");
        }
    }

    protected bool StoreCreatedItem(SolarBody parentBody, double amount)
    {
        var found = false;
        
        foreach( KeyValuePair<int,double> outRate in connectionOutRate)
        {
            ((ProductionStructure)parentBody.GetStructure(outRate.Key)).storage.AddItem(
                new Item(productionItemId, amount / count * productionTime * outRate.Value, parentBody.GetMarketPrice(productionItemId), solarIndex, id));
        }
        if (extraProductionRate > 0)
        {
            Item item = new Item(productionItemId, amount / count * productionTime * extraProductionRate, parentBody.GetMarketPrice(productionItemId), solarIndex, id);
            foreach (IStructure structure in parentBody.structures)
            {
                if (structure.structureType == StructureTypes.GroundStorage)
                {
                    GroundStorage groundStruct = (GroundStorage)structure;
                    if (groundStruct.owner.Model == owner.Model &&
                    groundStruct.CanAddItem(item))
                    {
                        groundStruct.AddItem(item);
                        //parentBody.SetSelling(item);
                        found = true;
                        break;
                    }
                }

            }
            if (!found)
                throw new System.Exception("Component storage not found or nearly full");
        }
        
        return true;
    }

    public void UpdateConnectionItems()
    { 
        connectionItems = new List<KeyValuePair<int, double>>();
        connectionOutRate = new Dictionary<int, double>();
        double productionRate = count / productionTime;
        Dictionary<int,double> connectionProductionRate = new Dictionary<int, double>();
        double totalConnectionRate = 0;

        //Figures out the rate of material to give to each of it out connections
        foreach ( int connectionId in structureConnectionIdsOut)
        {
            ProductionStructure structure = body.GetStructure(connectionId) as ProductionStructure;

            //Determine which production rates to consider
            
            if  (structure.storage.ContainsItem(productionItemId) && 
                structure.storage.items.Find(x => x.id == productionItemId).amount > 
                structure.requiredItems.Find(x => x.id == productionItemId).amount * 3 * structure.count)
            {
                connectionProductionRate[connectionId] = 0;
            }
            else
            {
                double rate = structure.count / structure.productionTime;
                connectionProductionRate[connectionId] = rate;
                totalConnectionRate += rate;
            }
            
        }

        //Figure out whether the rate of production is greater or less than the rate of consumption
        extraProductionRate = productionRate - totalConnectionRate;

        if (totalConnectionRate <= productionRate)
        {
            //More than enough
            foreach(KeyValuePair<int,double> rate in connectionProductionRate)
            {
                connectionOutRate[rate.Key] = rate.Value;
            }
            
        }
        else
        {
            //Not enough
            double conversionFactor = productionRate / totalConnectionRate;
            foreach (KeyValuePair<int, double> rate in connectionProductionRate)
            {
                connectionOutRate[rate.Key] = rate.Value * conversionFactor;
            }
        }

        //Figures out which materials that are not accounted for and creates a dictionry for the needed rates.
        neededItemRate = new Dictionary<int, double>();

        foreach(Item item in requiredItems)
        {
            neededItemRate[item.id] = item.amount * productionRate;
        }
        foreach (int structureId in structureConnectionIdsIn)
        {
            neededItemRate.Remove(((ProductionStructure)body.GetStructure(structureId)).productionItemId);
        }
    }
}

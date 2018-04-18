using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionStructure: Structure, IWorkers
{

    /// <summary>
    /// required items and amounts to make one unit of the production item
    /// </summary>
    public List<Item> requiredItems = new List<Item>();
    public double unitPrice
    {
        get
        {
            if (ProductionRateActual == 0)
                return 0;
            //Calculate Unit Price
            double averageUnitPrice = workers * workerPayRate * ProductionTime;
            //Add cost of materials
            foreach (Structure structure in GetStructuresIn())
            {
                ProductionStructure pStructure = structure as ProductionStructure;

                double rate = pStructure.connectionOutRate[id] / ProductionRateActual;

                averageUnitPrice +=  pStructure.unitPrice * rate;
            }

            foreach (Contract contract in GetSupplyContracts())
            {

                double rate = contract.itemRate / ProductionRateActual;

                averageUnitPrice += contract.unitPrice * rate;
            }

            foreach (Contract clientContract in clientContracts)
            {
                averageUnitPrice += clientContract.unitPrice;
            }
            averageUnitPrice /= clientContracts.Count + 1;
            return averageUnitPrice;
        }
    }
    public string productionItemName;
    public string productionItemId { get; protected set; }
    /// <summary>
    /// Amount of work that has to be done for a single item. Typically 1 work done per worker per hour.
    /// </summary>
    public double workAmount { get; set; }
    public bool isProducing;
    public bool on = true;
    /// <summary>
    /// Amount of production done, with 1 being 1 item.
    /// </summary>
    public double productionProgress = 0;
    public ProductionState productionState = ProductionState.LackMaterials;
    /// <summary>
    /// The ids of structures connecting into this structure
    /// </summary>
    public List<string> structureConnectionIdsIn = new List<string>();
    /// <summary>
    /// The ids of structures connecting out of this structure
    /// </summary>
    public List<string> structureConnectionIdsOut = new List<string>();
    /// <summary>
    /// Stores information of the items that are needed or that have surpluses.
    /// </summary>
    public List<KeyValuePair<int, double>> connectionItems = new List<KeyValuePair<int, double>>();
    public List<Contract> clientContracts = new List<Contract>();
    public List<string> supplierContractIds = new List<string>();
    public string constructionContract;
    public StructureTypes structureType { get; set; }
    /// <summary>
    /// Production rate per second.
    /// </summary>
    public double ProductionRateOptimal
    {
        get { return workers / (workAmount * Dated.Hour); }
    }
    public double ProductionRateActual
    {
        get
        {
            if (neededItemRate.Count > 0)
            {
                double percentProductionRate = 1;
                foreach (KeyValuePair<string, double> rate in neededItemRate)
                {
                    foreach (Item item in requiredItems)
                    {
                        if (rate.Key == item.id)
                        {
                            double thisPercent = (ProductionRateOptimal * item.amount - rate.Value) / ProductionRateOptimal;
                            if (thisPercent < percentProductionRate)
                                percentProductionRate = thisPercent;
                            break;
                        }
                    }

                }
                return ProductionRateOptimal * percentProductionRate;
            }
            else
                return ProductionRateOptimal;
        }
    }
    /// <summary>
    /// Time to produce one item in seconds.
    /// </summary>
    public double ProductionTime
    {
        get { return workAmount / workers * Dated.Hour; }
    }

    public int count { get; set; }

    /// <summary>
    /// Assumes parent of production Structure is a solarbody.
    /// </summary>
    private SolarBody Body
    {
        get
        {
            return (SolarBody)GameManager.instance.locations[referenceId];
        }
    }

    public int workers { get; set; }

    public double workerPayRate { get; set; }

    //Ships
    /// <summary>
    /// The ship blueprint that the factory will be using as fleet
    /// </summary>
    public string shipBlueprintId;

    /// <summary>
    /// The rate of output to each connection. The key is the structure id.
    /// </summary>
    internal Dictionary<string, double> connectionOutRate = new Dictionary<string, double>();
    /// <summary>
    /// The item ids and rate of items the factory needs.
    /// </summary>
    internal Dictionary<string, double> neededItemRate = new Dictionary<string, double>();
    public double extraProductionRate = 0;

    public ProductionStructure() { }

    public ProductionStructure(IdentityModel owner, string referenceId, Vector3d _localPosition):
        base(owner, referenceId, _localPosition)
    {
        owner.AddStructure(id, referenceId);
    }

    public ProductionStructure(IdentityModel owner, string _productionItemId, string referenceId, Vector3d _localPosition, int _count = 1):
        base( owner, referenceId, _localPosition)
    {
        var body = ReferenceBody;
        owner.AddStructure(id, referenceId);
        productionItemId = _productionItemId;
        var product = GameManager.instance.data.itemsData.Model.GetItem(productionItemId);

        if (product != null)
        {
            name = product.name + " " + structureType.ToString() + " " + id;
            productionItemName = product.name;

            requiredItems = product.contstructionParts;

            foreach (Structure structure in body.structures)
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
                if (productionStructure != null && productionStructure.GetType() != typeof(Station))
                {
                    foreach (Item item in productionStructure.requiredItems)
                    {
                        if (item.id == productionItemId)
                        {
                            if (!structureConnectionIdsOut.Contains(productionStructure.id))
                            {
                                structureConnectionIdsOut.Add(productionStructure.id);
                                connectionOutRate[productionStructure.id] = 0;
                                productionStructure.structureConnectionIdsIn.Add(id);
                            }
                        }
                    }

                    foreach (Item item in requiredItems)
                    {
                        if (item.id == productionStructure.productionItemId)
                        {
                            if (!structureConnectionIdsIn.Contains(productionStructure.id))
                            {
                                structureConnectionIdsIn.Add(productionStructure.id);
                                productionStructure.structureConnectionIdsOut.Add(id);
                                productionStructure.connectionOutRate[id] = 0;
                            }
                        }
                    }

                }
            }

            body.structures.Add(this);

            count = _count;

            maxArmor = 1000;
            currentArmor = maxArmor;
            workAmount = product.workAmount;
        }
            
        //requiredItems.ForEach(x =>
        //{
        //    x.price = GameManager.instance.data.getSolarBody(referenceId).GetMarketPrice(x.id);
        //});

        

        //Find Suitable ship blueprint
        SetShipBlueprintId();

        dateCreated = new Dated(GameManager.instance.data.date.time);
        lastUpdated = new Dated(GameManager.instance.data.date.time);

        UpdateConnectionItems();
    }

    private void SetShipBlueprintId()
    {
        //Get Blueprint
        var blueprints = GameManager.instance.data.itemsData.Model.blueprints.FindAll(x => x.itemType == ItemType.SpaceShip);

        shipBlueprintId = null;

        foreach (ItemBlueprint blueprint in blueprints)
        {
            ShipBlueprint ship = blueprint as ShipBlueprint;
            if (ship.shipType == ShipType.Cargo)
            {
                shipBlueprintId = ship.id;
                return;
            }
        }
        if (blueprintId == null)
            throw new System.Exception("No cargo ship blueprints found");
    }

    public override void Update()
    {
        base.Update();
        UpdateConnectionItems();
        UpdateConstruction();
    }

    public ItemBlueprint GetBlueprint()
    { return GameManager.instance.data.itemsData.Model.GetItem(blueprintId); }
    public List<Contract> GetSupplyContracts()
    {
        List<Contract> list = new List<Contract>();
        foreach (string structureId in supplierContractIds)
        {
            Contract structure = GameManager.instance.contracts[structureId];
            list.Add(structure);
        }

        return list;
    }
    public List<ProductionStructure> GetStructuresOut()
    {
        List<ProductionStructure> list = new List<ProductionStructure>();
        foreach (string structureId in structureConnectionIdsOut)
        {
            ProductionStructure structure = ReferenceBody.structures.Find(b => b.id == structureId) as ProductionStructure;
            list.Add(structure);
        }

        return list;
    }
    public List<ProductionStructure> GetStructuresIn()
    {
        List<ProductionStructure> list = new List<ProductionStructure>();
        foreach (string structureId in structureConnectionIdsIn)
        {
            ProductionStructure structure = ReferenceBody.structures.Find(b => b.id == structureId) as ProductionStructure;
            list.Add(structure);
        }

        return list;
    }
    /// <summary>
    /// Makes sure that the factory is at the maximum production rate by finding contracts with known companies for needed materials
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="productionRate">Needed production Rate per second</param>
    public virtual void SearchContracts(string itemId, double productionRate)
    {
        //Find a contract
        string contractId = "";
        double requiredItemRate = productionRate;
        contractId = owner.Model.Government.GetCheapestContractId(itemId, id, requiredItemRate);

        if (contractId != "" && contractId != null)
        {
            supplierContractIds.Add(contractId);
            Contract contract = GameManager.instance.contracts[contractId];

            //Set wanted conditions;
            contract.client = new ModelRef<IdentityModel>(owner.Model);
            contract.destinationId = id;
            contract.contractState = ContractState.Sent;
            
            if (requiredItemRate < contract.itemRate)
            {
                contract.alternateItemRate = requiredItemRate;
            }
            else
            {
                contract.alternateItemRate = contract.itemRate;
            }
        }

    }

    protected bool SearchRequiredItems()
    {
        int neededItemCount = requiredItems.Count;
        int itemCount = 0;
        foreach (Item item in requiredItems)
        {
            double neededAmount = item.amount;
            if (ContainsItem(item.id))
            {
                neededAmount -= Find(item).amount;
            }
            if (neededAmount <= 0)
            {
                //item.price = ReferenceBody.GetMarketPrice(item.id);
                ReferenceBody.RemoveBuying(item.id, id, item.amount);
                itemCount++;
            }
            else
            {
                var neededItem = new Item(item.id, neededAmount, id);
                return false;
            }

        }
        return true;
    }

    protected void UseRequiredItems()
    {
        foreach (Item item in requiredItems)
        {
            var found = UseItem(item.id, item.amount);
            if (!found)
                throw new System.Exception("Item is not found in correct amount");
        }
        StoreCreatedItem(1);
    }

    protected bool StoreCreatedItem(double amount)
    {
        foreach (KeyValuePair<string, double> outRate in connectionOutRate)
        {
            ((ProductionStructure)ReferenceBody.GetStructure(outRate.Key)).AddItem(
                new Item(productionItemId, amount * (outRate.Value / ProductionRateActual) , outRate.Key));
        }
        if (clientContracts.Count > 0)
        {
            foreach (Contract contract in clientContracts)
            {
                if (contract.contractState == ContractState.Active)
                {
                    Item item = new Item(productionItemId, amount * (contract.itemRate / ProductionRateActual), id, contract.destinationId);
                    AddItem(item);
                }

            }
        }
        if (extraProductionRate > 0)
        {
            Item item = new Item(productionItemId, amount * (extraProductionRate / ProductionRateActual), id);
            AddItem(item);
        }

        return true;
    }

    public void UpdateConnectionItems()
    { 
        connectionItems = new List<KeyValuePair<int, double>>();
        connectionOutRate = new Dictionary<string, double>();
        Dictionary<string,double> connectionProductionRate = new Dictionary<string, double>();
        double totalConnectionRate = 0;

        //Figures out which materials that are not accounted for and creates a dictionry for the needed rates.
        neededItemRate = new Dictionary<string, double>();
        foreach (Item item in requiredItems)
        {
            neededItemRate[item.id] = item.amount * ProductionRateOptimal;
        }

        foreach (string structureId in structureConnectionIdsIn)
        {
            ProductionStructure structure = (ProductionStructure)GameManager.instance.locations[structureId];
            //Figures the rate of needed items for to produce at maximum capacity
            try
            {
                neededItemRate[structure.productionItemId] -= structure.connectionOutRate[id];

                if (neededItemRate[structure.productionItemId] <= 0)
                    neededItemRate.Remove(structure.productionItemId);
            }
            catch (KeyNotFoundException e)
            {

            }


        }

        foreach (string contractId in supplierContractIds)
        {
            Contract contract = GameManager.instance.contracts[contractId];

            if (neededItemRate.ContainsKey(contract.itemId) && contract.contractState == ContractState.Active)
            {
                try
                {
                    neededItemRate[contract.itemId] -= contract.itemRate;

                    if (neededItemRate[contract.itemId] <= 0)
                        neededItemRate.Remove(contract.itemId);
                }
                catch (KeyNotFoundException e)
                {

                }
            }


        }

        foreach (KeyValuePair<string, double> itemRate in neededItemRate)
        {
            //Check to see if a contract exists to fill the the missing production has been created
            bool searchForContract = true;
            foreach (string contractId in supplierContractIds)
            {
                Contract contract = GameManager.instance.contracts[contractId];
                if (contract.itemId == itemRate.Key)
                {
                    if (contract.contractState != ContractState.Active)
                    {
                        searchForContract = false;
                        if (contract.contractState == ContractState.Review)
                        {

                            //Verify that contract will work, accept or reject
                            contract.contractState = ContractState.Accepted;
                        }
                        else if (contract.contractState == ContractState.Renew)
                        {
                            //Reset ships
                            foreach (string shipId in contract.shipIds)
                            {
                                ((Ship)GameManager.instance.locations[shipId]).contractId = null;
                            }

                            //Set wanted conditions;
                            contract.destinationId = id;
                            contract.contractState = ContractState.Sent;

                            if (itemRate.Value < contract.itemRate)
                            {
                                contract.alternateItemRate = itemRate.Value;
                            }
                            else
                            {
                                contract.alternateItemRate = contract.itemRate;
                            }
                        }
                    }

                }
            }

            if (searchForContract)
            {
                SearchContracts(itemRate.Key, itemRate.Value);
            }
        }


        //Figures out the rate of material to give to each of it out connections
        foreach ( string connectionId in structureConnectionIdsOut)
        {
            ProductionStructure structure = Body.GetStructure(connectionId) as ProductionStructure;

            //Determine which production rates to consider

            double rate = structure.ProductionRateOptimal;
            connectionProductionRate[connectionId] = rate * structure.requiredItems.Find(x => x.id == productionItemId).amount;
            totalConnectionRate += connectionProductionRate[connectionId];

        }
        foreach(Contract contract in clientContracts)
        {
            if (contract.contractState == ContractState.Active)
            {
                totalConnectionRate += contract.itemRate;
            }
        }

        //Figure out whether the rate of production is greater or less than the rate of consumption
        extraProductionRate = ProductionRateActual - totalConnectionRate;

        //Make sure there is a contract for extra production Rate
        bool newContract = true;
        bool repeat = true;
        while (repeat)
        {
            repeat = false;

            foreach (Contract contract in clientContracts)
            {
                if (contract.contractState != ContractState.Active)
                {
                    newContract = false;
                    if (contract.contractState == ContractState.Rejected)
                    {
                        newContract = false;
                        clientContracts.Remove(contract);
                        GameManager.instance.contracts.Remove(contract.id);
                        owner.Model.Government.postedContractIds.Remove(contract.id);
                        owner.Model.contracts.Remove(contract.id);
                        if (contract.destinationId != null)
                        {
                            //Remove destination supply listing
                            ProductionStructure destination = GameManager.instance.locations[contract.destinationId] as ProductionStructure;
                            destination.supplierContractIds.Remove(contract.id);
                        }
                        foreach (string shipId in contract.shipIds)
                        {
                            ((Ship)GameManager.instance.locations[shipId]).contractId = null;
                        }

                        repeat = true;
                        break;
                    }
                    else if (contract.contractState == ContractState.Sent)
                    {
                        //Make sure that the contract is profitable and then set to review, check that ship can deliver the amount of goods needed per month, etc.
                        contract.itemRate = contract.alternateItemRate;


                        //Set due date
                        contract.contractEndDate = new Dated(GameManager.instance.data.date.time + contract.duration);
                        if (contract.reknewable)
                            contract.itemAmount = Mathd.Floor(contract.duration * contract.itemRate);
                        else
                        {
                            contract.duration = contract.itemAmount / contract.itemRate;
                        }
                        while (contract.itemAmount == 0)
                        {
                            contract.duration *= 2;
                            contract.contractEndDate = new Dated(GameManager.instance.data.date.time + contract.duration);
                            contract.itemAmount = Mathd.Floor(contract.duration * contract.itemRate);
                        }

                        //Setup Costs
                        double distance = Vector3d.Distance(GameManager.instance.locations[contract.originId].SystemPosition, GameManager.instance.locations[contract.destinationId].SystemPosition);
                        contract.CalculateMonthlyCost(distance, contract.itemRate);
                        contract.payDate = new Dated(GameManager.instance.data.date.time + Dated.Month);

                        contract.contractState = ContractState.Review;
                    }
                    else if (contract.contractState == ContractState.Accepted)
                    {
                        //Initial Payment
                        if (contract.reknewable)
                        {
                            contract.client.Model.PayContract(contract.monthlyCost);
                            owner.Model.EarnMoney(contract.monthlyCost);

                            if (((ProductionStructure)GameManager.instance.locations[contract.destinationId]).GetType() == typeof(DistributionCenter))
                            {
                                //Undo payment if going to DC
                                contract.client.Model.PayContract(-contract.monthlyCost);
                            }
                        }
                        else
                        {
                            double cost = contract.monthlyCost * (contract.duration / Dated.Month);
                            contract.client.Model.PayContract(cost);
                            owner.Model.EarnMoney(cost);

                            if (((ProductionStructure)GameManager.instance.locations[contract.destinationId]).GetType() == typeof(DistributionCenter))
                            {
                                //Undo payment if going to DC
                                contract.client.Model.PayContract(-cost);
                            }
                        }

                        //Setup Ships
                        for (int i = 0; i < contract.shipCount; i++)
                        {
                            bool newShip = true;
                            //Create a new ship if needed or assign ship
                            foreach (string shipId in owner.Model.ownedShipIds)
                            {
                                Ship ship = GameManager.instance.locations[shipId] as Ship;
                                if (ship.contractId == null && ship.blueprintId == shipBlueprintId)
                                {
                                    newShip = false;
                                    ship.contractId = contract.id;
                                    contract.shipIds.Add(ship.id);
                                }
                            }

                            if (newShip)
                            {
                                Ship ship = new Ship(shipBlueprintId, owner.Model, new Creature("Bob", 10000, id, GameManager.instance.data.date, new Dated(Dated.Year * 30), CreatureType.Human), id)
                                {
                                    contractId = contract.id
                                };
                                contract.shipIds.Add(ship.id);
                            }
                        }
                        contract.contractState = ContractState.Active;
                    }
                    else if (contract.contractState == ContractState.Initial)
                    {
                        contract.itemRate = extraProductionRate;
                        contract.unitPrice -= contract.unitPrice * deltaTime / Dated.Year;
                        if (contract.unitPrice < 1)
                        {
                            contract.unitPrice = 1;
                        }
                    }
                }
                else
                {
                    if (contract.itemAmount <= 0 && GameManager.instance.data.date > contract.contractEndDate)
                    {
                        //Contract has ended
                        if (contract.reknewable)
                        {
                            //Send a renewal offer
                            Contract innactiveContract = clientContracts.Find(x => x.contractState == ContractState.Initial);
                            if (innactiveContract != null)
                            {
                                contract.unitPrice = ((contract.unitPrice * contract.itemRate) + (innactiveContract.itemRate * innactiveContract.unitPrice)) / (contract.itemRate + innactiveContract.itemRate);

                                //Remove initial contract
                                innactiveContract.contractState = ContractState.Rejected;
                            }

                            //Increase unit price
                            contract.unitPrice *= 1.2;

                            contract.contractEndDate = new Dated(GameManager.instance.data.date.time + contract.duration);
                            contract.itemAmount = Mathd.Floor(contract.duration * contract.itemRate);
                            contract.itemRate += extraProductionRate;
                            contract.contractState = ContractState.Renew;
                        }
                        else
                        {
                            //End contract
                            contract.contractState = ContractState.Rejected;
                        }
                    }

                    //PayDate
                    if (contract.payDate < GameManager.instance.data.date)
                    {
                        //if (contract.reknewable && GameManager.instance.data.date < contract.contractEndDate)
                        //{

                        //    contract.client.Model.PayContract(contract.monthlyCost);
                        //    owner.Model.EarnMoney(contract.monthlyCost);
                        //    if (((ProductionStructure)GameManager.instance.locations[contract.destinationId]).GetType() == typeof(DistributionCenter))
                        //    {
                        //        contract.client.Model.PayContract(-contract.monthlyCost);
                        //    }
                        //}
                    }
                }
            }
        }
        

        if (newContract && extraProductionRate > 0 && ProductionRateActual != 0 && GetType() !=  typeof(DistributionCenter))
        {

            //Create New Contract 
            var contract = new Contract(owner.Model, productionItemId, extraProductionRate, id,shipBlueprintId, unitPrice * 1.25);
            clientContracts.Add(contract);
            owner.Model.Government.postedContractIds.Add(contract.id);
            owner.Model.contracts.Add(contract.id);
        }

        if (totalConnectionRate >= ProductionRateOptimal)
        {

            //Check to see if there is a greater demand
            SupplyDemand sd = owner.Model.Government.supplyDemand.Find(x => x.id == productionItemId);
            if (sd != null)
            {
                if (sd.itemSupply < sd.itemDemand)
                {
                    //Create another factory
                    if (constructionContract == null)
                    {
                        //Find Construction Contract
                        SearchContractsConstruction(blueprintId);
                    }
                }
            }
        }

        if (totalConnectionRate <= ProductionRateActual)
        {
            //More than enough
            foreach(KeyValuePair<string,double> rate in connectionProductionRate)
            {
                connectionOutRate[rate.Key] = rate.Value;
            }
        }
        else
        {
            //Not enough
            double conversionFactor = ProductionRateActual / totalConnectionRate;
            foreach (KeyValuePair<string, double> rate in connectionProductionRate)
            {
                connectionOutRate[rate.Key] = rate.Value * conversionFactor;
            }

            //Remove new contracts
            foreach (Contract contract in clientContracts)
            {
                if (contract.contractState != ContractState.Active)
                {
                    contract.contractState = ContractState.Rejected;
                }
            }
        }

        

    }

    private void SearchContractsConstruction(string blueprintId)
    {
        //Find a contract
        string contractId = "";
        double rate = 1.0 / Dated.Month;
        contractId = owner.Model.Government.GetCheapestContractId(blueprintId, id, rate);

        if (contractId != "" && contractId != null)
        {
            constructionContract = contractId;
            Contract contract = GameManager.instance.contracts[contractId];

            //Set wanted conditions;
            contract.client = new ModelRef<IdentityModel>(owner.Model);
            contract.destinationId = id;
            contract.contractState = ContractState.Sent;
            contract.reknewable = false;
            contract.duration = Dated.Month;
            contract.itemAmount = 1;
            contract.contractType = ContractType.Construction;

            if (rate < contract.itemRate)
            {
                contract.alternateItemRate = rate;
            }
            else
            {
                contract.alternateItemRate = contract.itemRate;
            }
        }
    }

    public void UpdateConstruction()
    {
        //TODO: build construction que, this will also allow proper tracking of construction demand; can also be done with ships
        if (constructionContract != null)
        {
            Contract contract = GameManager.instance.contracts[constructionContract];
            if (contract.contractState != ContractState.Active)
            {
                if (contract.contractState == ContractState.Rejected)
                {
                    constructionContract = null;
                }
                else if (contract.contractState == ContractState.Review)
                {

                    //Verify that contract will work, accept or reject
                    contract.contractState = ContractState.Accepted;
                }
            }
        }
    }
}



public enum ProductionState
{
    LackMaterials,
    Inactive,
    Active
}

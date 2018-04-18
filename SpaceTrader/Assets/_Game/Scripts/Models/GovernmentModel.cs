using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class GovernmentModel : StructureModel {

    public List<string> leaders = new List<string>();
    public ModelRefs<SolarModel> stars = new ModelRefs<SolarModel>();
    public ModelRefs<CompanyModel> localCompanies = new ModelRefs<CompanyModel>();
    public ModelRefs<CompanyModel> licensedCompanies = new ModelRefs<CompanyModel>();
    public ModelRefs<CompanyModel> knownCompanies = new ModelRefs<CompanyModel>();
    public ModelRefs<CompanyModel> bannedCompanies = new ModelRefs<CompanyModel>();
    public ModelRefs<GovernmentModel> enemyGovernments = new ModelRefs<GovernmentModel>();
    public ModelRefs<GovernmentModel> alliedGovernments = new ModelRefs<GovernmentModel>();
    public ModelRefs<GovernmentModel> knownGovernments = new ModelRefs<GovernmentModel>();
    public List<string> postedContractIds = new List<string>();

    //----------------Commerce-----------------------------//
    internal double supplyDemandProgress = Dated.Hour - 30;
    public double supplyDemandUpdateTime = Dated.Hour;
    public List<SupplyDemand> supplyDemand = new List<SupplyDemand>();

    public GovernmentModel() {
    }

    public GovernmentModel(string _name)
    {
        name = _name;
        StartingBalance(100000000);
        GameManager.instance.data.governments.Add(this);

        supplyDemandProgress = Dated.Hour - UnityEngine.Random.Range(0, 300);
        
    }

    /// <summary>
    /// Returns the id of the contract with with the smallest overall cost
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public string GetCheapestContractId(string itemId, string destinationId, double itemRate)
    {
        var items = postedContractIds.FindAll(x => GameManager.instance.contracts[x].itemId == itemId && GameManager.instance.contracts[x].contractState == ContractState.Initial);

        double totalMonthlyCost = double.PositiveInfinity;
        string cheapestContractId = "";
        for (int i = 0; i < items.Count; i++)
        {
            Contract contract = GameManager.instance.contracts[items[i]];
            double distance = Vector3d.Distance(GameManager.instance.locations[contract.originId].SystemPosition, GameManager.instance.locations[destinationId].SystemPosition);
            contract.CalculateMonthlyCost(distance, itemRate);
            double cost = contract.monthlyCost;
            if (cost < totalMonthlyCost)
            {
                totalMonthlyCost = cost;
                cheapestContractId = contract.id;
            }
        }

        return cheapestContractId;
    }

    #region "Commerce"

    public List<Contract> GetPostedContracts()
    {
        List<Contract> contracts = new List<Contract>();
        foreach (string contractId in postedContractIds)
        {
            contracts.Add(GameManager.instance.contracts[contractId]);
        }
        return contracts;
    }

    public void UpdateSupplyDemand(double deltaTime)
    {
        //Commerce
        supplyDemandProgress += deltaTime;

        if (supplyDemandProgress > supplyDemandUpdateTime)
        {
            supplyDemandProgress -= supplyDemandUpdateTime;

            //Reset Stats

            foreach(SupplyDemand sD in supplyDemand)
            {
                sD.Reset();
            }

            //Find Supply and Demand and estimated Price
            foreach (CompanyModel company in licensedCompanies)
            {
                foreach (string structureId in company.structureIds)
                {
                    ProductionStructure structure = GameManager.instance.locations[structureId] as ProductionStructure;
                    if (structure != null)
                    {
                        //Find Supply and Add factory
                        int index = supplyDemand.FindIndex(x => x.id == structure.productionItemId);

                        if (index >= 0)
                        {
                            supplyDemand[index].itemSupply += structure.extraProductionRate * Dated.Year;
                            supplyDemand[index].factoryCount++;
                            if (structure.ProductionRateActual > 0)
                            {
                                supplyDemand[index].totalItemAmount++;
                                supplyDemand[index].totalItemPrice += structure.unitPrice;
                            }
                        }
                        else
                        {
                            supplyDemand.Add(new SupplyDemand(structure.productionItemId, structure.extraProductionRate * Dated.Year, 0,1, structure.unitPrice));
                        }

                        

                        //Find Demand
                        foreach (KeyValuePair<string, double> item in structure.neededItemRate)
                        {
                            index = supplyDemand.FindIndex(x => x.id == item.Key);

                            if (index >= 0)
                            {
                                supplyDemand[index].itemDemand += item.Value * Dated.Month;
                            }
                            else
                            {
                                supplyDemand.Add(new SupplyDemand(item.Key, 0, item.Value * Dated.Month));
                            }
                        }

                        //Find price, add to supply and demand
                        foreach (Contract contract in structure.clientContracts)
                        {
                            if (contract.contractState == ContractState.Active)
                            {
                                index = supplyDemand.FindIndex(x => x.id == structure.productionItemId);

                                if (index >= 0)
                                {
                                    supplyDemand[index].itemSupply += contract.itemRate * Dated.Month;
                                    supplyDemand[index].itemDemand += contract.itemRate * Dated.Month;
                                    supplyDemand[index].contractsCount++;
                                }
                                else
                                {
                                    supplyDemand.Add(new SupplyDemand(structure.productionItemId, contract.itemRate * Dated.Month,
                                        contract.itemRate * Dated.Month) { contractsCount = 1 });
                                }
                            }
                        }

                        

                    }
                }
            }            
        }
    }

    public double GetMarketPrice(string itemId)
    {
        int index = supplyDemand.FindIndex(x => x.id == itemId);

        if (index >= 0)
        {
            return supplyDemand[index].marketPrice;
        }
        else
        {
            return GameManager.instance.data.itemsData.Model.GetItem(itemId).estimatedValue;
        }
    }

    public void SetBuying(Item item)
    {
        //int itemIndex = -1;
        //if (item.structureId == "")
        //{
        //    itemIndex = buyList.itemsStorage.FindIndex(x => x.id == item.id);
        //}
        //else
        //{
        //    itemIndex = buyList.itemsStorage.FindIndex(x => x.id == item.id && x.structureId == item.structureId);
        //}
        //if (itemIndex >= 0)
        //{
        //    buyList.itemsStorage[itemIndex].SetAmount(item.amount, item.price);
        //}
        //else
        //{
        //    buyList.AddItem(item);
        //}
    }

    public void RemoveBuying(string itemId, string structureId, double amount)
    {

        //int itemIndex = -1;
        //if (structureId == "")
        //{
        //    itemIndex = buyList.itemsStorage.FindIndex(x => x.id == itemId);
        //}
        //else
        //{
        //    itemIndex = buyList.itemsStorage.FindIndex(x => x.id == itemId && x.structureId == structureId);
        //}

        //if (itemIndex >= 0)
        //{
        //    buyList.itemsStorage[itemIndex].RemoveAmount(amount);
        //    if (buyList.itemsStorage[itemIndex].amount == 0)
        //        buyList.itemsStorage.RemoveAt(itemIndex);
        //}
    }

    public void SetSelling(Item item)
    {
        //int itemIndex = -1;
        //if (item.structureId == "")
        //{
        //    itemIndex = sellList.itemsStorage.FindIndex(x => x.id == item.id);
        //}
        //else
        //{
        //    itemIndex = sellList.itemsStorage.FindIndex(x => x.id == item.id && x.structureId == item.structureId);
        //}
        //if (itemIndex >= 0)
        //{
        //    sellList.itemsStorage[itemIndex].SetAmount(item.amount, item.price);
        //}
        //else
        //{
        //    sellList.AddItem(item);
        //}
    }
    public void SetSellingPrice(Item item)
    {
        //int itemIndex = -1;
        //if (item.structureId == "")
        //{
        //    itemIndex = sellList.itemsStorage.FindIndex(x => x.id == item.id);
        //}
        //else
        //{
        //    itemIndex = sellList.itemsStorage.FindIndex(x => x.id == item.id && x.structureId == item.structureId);
        //}
        //if (itemIndex >= 0)
        //{
        //    sellList.itemsStorage[itemIndex].price = item.price;
        //}
    }

    public void RemoveSelling(string itemId, string structureId, double amount)
    {

        //int itemIndex = -1;
        //if (structureId == "")
        //{
        //    itemIndex = sellList.itemsStorage.FindIndex(x => x.id == itemId);
        //}
        //else
        //{
        //    itemIndex = sellList.itemsStorage.FindIndex(x => x.id == itemId && x.structureId == structureId);
        //}

        //if (itemIndex >= 0)
        //{
        //    sellList.itemsStorage[itemIndex].RemoveAmount(amount);
        //    if (sellList.itemsStorage[itemIndex].amount == 0)
        //        sellList.itemsStorage.RemoveAt(itemIndex);
        //}
    }

    #endregion
}


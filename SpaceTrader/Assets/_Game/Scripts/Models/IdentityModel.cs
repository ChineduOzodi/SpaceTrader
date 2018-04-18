using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class IdentityModel : Model {

    public string name;
    public IdentityState identityState = IdentityState.Initial;
    public Color spriteColor;

    public ModelRef<IdentityModel> owner = new ModelRef<IdentityModel>();
    public ModelRef<IdentityModel> entities = new ModelRef<IdentityModel>();

    public string referenceId { get; set; }
    public List<string> ownedShipIds = new List<string>();
    public List<string> structureIds = new List<string>();
    public List<string> knownSolarBodyIds = new List<string>();

    public Dated dateCreated { get; set; }//new Dated(GameManager.instance.data.date.time);
    public Dated lastUpdated { get; set; }//new Dated(GameManager.instance.data.date.time);

    public GovernmentModel Government
    {
        get
        {
            if (GetType() == typeof(GovernmentModel))
            {
                return this as GovernmentModel;
            }
            else if (GetType() == typeof(CompanyModel))
            {
                return ((CompanyModel)this).governmentAccess[0];
            }
            else
            {
                return owner.Model.Government;
            }
        }
    }

    public List<string> contracts = new List<string>();

    /// <summary>
    /// Used to update money at a given interval;
    /// </summary>
    public double timeUpdate;

    //----------------Money-----------------------------//
    internal double moneyProgress = 0;
    public double moneyUpdateTime = Dated.Month;

    public List<double> moneyTotalMonth = new List<double>() { 0 };
    public List<double> moneyEarnedMonth = new List<double>() { 0 };
    public List<double> moneyWorkersMonth = new List<double>() { 0 };
    public List<double> moneySupplierContractsMonth = new List<double>() { 0 };
    public List<double> moneyConstructionMonth = new List<double>() { 0 };

    public double assets = 0; //TODO: should be a calculation of the current market value of all assets
    public double moneyTotal { get { return moneyTotalMonth[moneyTotalMonth.Count - 1]; } }
    public IdentityModel()
    {
        dateCreated = new Dated(GameManager.instance.data.date.time);
        lastUpdated = new Dated(GameManager.instance.data.date.time);
        spriteColor = Random.ColorHSV(.5f, 1f, .5f, 1f);
        spriteColor.a = 1;
    }

    public void UpdateMoney(double deltaTime)
    {
        moneyProgress += deltaTime;
        
        if (moneyProgress > moneyUpdateTime)
        {
            moneyProgress -= moneyUpdateTime;

            int index = moneyTotalMonth.Count - 1;

            moneyTotalMonth.Add(moneyTotalMonth[index]);
            moneyEarnedMonth.Add(0);
            moneyWorkersMonth.Add(0);
            moneySupplierContractsMonth.Add(0);
            moneyConstructionMonth.Add(0);
        }
        else
        {
            
        }
    }
    public List<Ship> GetOwnedShips()
    {
        var ships = new List<Ship>();
        foreach (string shipId in ownedShipIds)
        {
            ships.Add(GameManager.instance.data.ships.Model.ships.Find(x => x.id == shipId));
        }
        return ships;
    }

    public List<Structure> GetStructures()
    {
        var structure = new List<Structure>();
        foreach (string item in structureIds)
        {
            structure.Add(GameManager.instance.locations[item] as Structure);
        }
        return structure;
    }
    public List<Contract> GetContracts()
    {
        List<Contract> contracts = new List<Contract>();
        foreach (string contractId in this.contracts)
        {
            contracts.Add(GameManager.instance.contracts[contractId]);
        }
        return contracts;
    }

    public List<Contract> GetConstructionContracts()
    {
        List<Contract> contracts = new List<Contract>();
        foreach (Structure structure in GetStructures())
        {
            ProductionStructure productionStructure = structure as ProductionStructure;
            if (productionStructure != null)
            {
                if (productionStructure.constructionContract != null)
                {
                    contracts.Add(GameManager.instance.contracts[productionStructure.constructionContract]);
                }
            }
            
        }
        return contracts;
    }
    public void SetLocation(string _referenceId)
    {
        referenceId = _referenceId;
        AddKnownSolarBodyId(referenceId);
    }

    public void AddStructure(string structureId, string referenceId)
    {
        if (!structureIds.Contains(structureId))
        {
            structureIds.Add(structureId);
        }
        AddKnownSolarBodyId(referenceId);
    }

    public void AddKnownSolarBodyId(string body)
    {
        if (!knownSolarBodyIds.Contains(body) && body != null && body.Contains("SolarBody"))
        {
            knownSolarBodyIds.Add(body);
        }
        
    }
    public void StartingBalance(double amount)
    {
        int index = moneyTotalMonth.Count - 1;
        moneyTotalMonth[index] += amount;
    }
    public void EarnMoney(double amount)
    {
        int index = moneyTotalMonth.Count - 1;
        moneyTotalMonth[index] += amount;
        moneyEarnedMonth[index] += amount;
    }

    public void PayWorkers(double amount)
    {
        int index = moneyTotalMonth.Count - 1;
        moneyTotalMonth[index] -= amount;

        moneyWorkersMonth[index] -= amount;
    }

    public void PayContract(double amount)
    {
        int index = moneyTotalMonth.Count - 1;
        moneyTotalMonth[index] -= amount;
        moneySupplierContractsMonth[index] -= amount;
    }

    public void PayConstruction(double amount)
    {
        int index = moneyTotalMonth.Count - 1;
        moneyTotalMonth[index] -= amount;
        moneyConstructionMonth[index] -= amount;
    }
}



public enum IdentityState
{
    Initial,
    Explore,
    Grow
}

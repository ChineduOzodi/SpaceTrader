using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contract {

    public string id;
    public ContractType contractType = ContractType.Supply;
    public ModelRef<IdentityModel> owner;
    public ModelRef<IdentityModel> client;
    public List<string> shipIds = new List<string>();

    public double duration = Dated.Year; // 6, 12, 24, 36 month. Should either be based on relationShip of company, amount of items, or size of company, or combination
    public Dated contractEndDate;

    public bool reknewable = true; //whether to try to renew the contract at the end of the duration

    public ContractState contractState = ContractState.Initial; //tracks the state of the contract

    List<string> potentialClientIds;

    /// <summary>
    /// How much a unit will cost
    /// </summary>
    public double unitPrice;
    public double monthlyCost;
    public Dated payDate;
    public int shipCount;
    public double distance;

    public string itemId;
    public string itemName;

    /// <summary>
    /// Total amount of items to deliver
    /// </summary>
    public double itemAmount; //amount to deliver per month

    /// <summary>
    /// item delivery rate in seconds
    /// </summary>
    public double itemRate; //amount to deliver per month
    /// <summary>
    /// Origin id
    /// </summary>
    public string originId; //origin factory id
    public ProductionStructure Origin
    {
        get { return GameManager.instance.locations[originId] as ProductionStructure; }
    }
    /// <summary>
    /// Destination id
    /// </summary>
    public string destinationId; //destination factory id

    /// <summary>
    /// Used by client to alter the potential contract
    /// </summary>
    public double alternateItemRate;

    //Cost details

    // Ship details

    public string shipBlueprintId;

    public ShipBlueprint GetShipBlueprint()
    {
        return GameManager.instance.data.itemsData.Model.GetItem(shipBlueprintId) as ShipBlueprint;
    }

    public List<Ship> GetShips()
    {
        var ships = new List<Ship>();
        foreach (string shipId in shipIds)
        {
            ships.Add(GameManager.instance.data.ships.Model.ships.Find(x => x.id == shipId));
        }

        return ships;
    }
    /// <summary>
    /// How much it will cost to ship a unit per Km
    /// </summary>
    public double PricePerKm { get { return GetShipBlueprint().ApproxFuelCostPerKm; } }
    /// <summary>
    /// Speed in Km/sec
    /// </summary>
    public double ShipSpeed { get { return GetShipBlueprint().subLightSpeed; } }
    /// <summary>
    /// Carry capacity
    /// </summary>
    public double ShipCapacity { get { return GetShipBlueprint().cargoCapacity; } }

    public Contract()
    {
        id = GetType().ToString() + GameManager.instance.data.id++.ToString();
    }

    public Contract(IdentityModel owner, string _itemId, double _itemRate, string _originId, string _shipBlueprintId, double _unitPrice)
    {
        id = GetType().ToString() + GameManager.instance.data.id++.ToString();
        itemId = _itemId;
        this.owner = new ModelRef<IdentityModel>(owner);

        itemName = GameManager.instance.data.itemsData.Model.GetItem(itemId).name;
        itemRate = _itemRate;
        shipBlueprintId = _shipBlueprintId;
        originId = _originId;
        unitPrice = _unitPrice;
        
        GameManager.instance.contracts[id] = this;
    }

    public void CalculateMonthlyCost(double distance, double rate)
    {
        this.distance = distance;
        double timeToDestination = (distance / ShipSpeed) * 2;
        double transportUnitsRate = ShipCapacity / timeToDestination;
        int numberOfShips = 1;
        while (transportUnitsRate < rate)
        {
            numberOfShips++;
            transportUnitsRate += transportUnitsRate;
        }
        shipCount = numberOfShips;
        double UnitCostRate = unitPrice * itemRate;
        double TransportCostRate = PricePerKm * distance * transportUnitsRate;
        monthlyCost = UnitCostRate * Dated.Month + TransportCostRate * Dated.Month;
    }
}

public enum ContractState
{
    Initial,
    Sent,
    Review,
    Accepted,
    Active,
    Rejected,
    Renew
}

public enum ContractType
{
    Supply,
    Construction,
    Fuel
}

/*

Create contracts based on market demand and company need.

 

For each factories advertise the amount of items needed per month to max output on the factory.

 

Checks for each station it owns

                *station does nothing if no potential destinations or no suppliers

                looks for potential destinations of known companies

                if not at max production and no free ship and potential destinations of known companies

                                buy ship * random size ship of the correct type

                if at max capacity and potential destinations:

                                build another station

                if free ship:

                                Creates potential contract:

                                                looks for potential destinations of known companies and saves to contract

               

For each potential contract:

                phases: initial, waiting, review, active, rejected

                Initial:

                                Sends to one of potential destinations with initial demands

                                (cost of transport per item * the distance from source to destinations and maximum amount delivered per month (considering available ships) sent to destination companies in known companies)

                                set to waiting

                Review:

                                check if adjusted contract is cost effective (cost of deliver less than profit per month)

                                if true:

                                                set to active

                                else:

                                                set to rejected

                Rejected:

                                remove potential company

                                if no potential companies

                                                delete contract

                                else:

                                                set to initial

                               

For each contract sent by supplier:

                if still need items to max out factory

                                set contract to review

                                if need less than contract max item to max out factory

                                                change item amount per month

                                                set contract to review

                else

                                set contract to rejected

                               

Adds to potential clients, and look for supplies that fit into the budget

using preferences with the potential to take on more contracts as business grows.

Company Created

                company properties: sells one item

                                needs ships to deliver that item to clients

                                needs to find suppliers for needed items.

                               

Contracts:

                Possible contract distribution:

                                Contract type: transportation cost per item per km

                                                contracts stored on solar bodies, discovered by companies who have access to solar body.

                                Contract type: cost of transport per item * the distance from source to destinations and maximum amount delivered per month (considering available ships) sent to destination companies in known companies

                                                contracts created in company and sent to known companies

                Approved contracts

                                Looks for ships without contracts (or ships with partial contracts <-- later addition) and assigns contract

                               

Checks station to see if at production capacity

                if no and no ships:

                                orders a ship

                                               

                                               

Shipping:

                One ship per contract <--- For now

                                ship assigned contract, picks up available goods, delivers it to destination

                One ship multiple contracts

                                if contract does not

Contract phase when company script is run

                Company looks at number of ships with no contracts

               

*/

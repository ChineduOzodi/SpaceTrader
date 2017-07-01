using UnityEngine;
using System.Collections;

public enum IdentityType {
    Government,
    Company,
    Station,
    Ship,
    Human
}

public enum ItemTypes
{
    Coal,
    Fuel,
    Iron,
    Rock,
    Steel,
    Ship,
    Glass,
    Sculpture,
    Wheat,
    Food,
}

public enum RawResources
{
    CoalRock,
    IronRock,
    UnrefinedFuel,
    None
}

public enum SolarType
{
    Star,
    RockyPlanet,
    GasGiant,
    DwarfPlanet,
    Moon,
    Meteor,
    Asteroid,
    Structure
}

public enum ShipMode
{
    Buy,
    Sell,
    SearchingTradeRoute,
    Idle
}

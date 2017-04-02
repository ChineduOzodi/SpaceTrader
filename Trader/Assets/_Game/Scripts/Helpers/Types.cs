using UnityEngine;
using System.Collections;

public enum IdentityType {

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

public enum PlanetType
{
    Gas,
    Dwarf,
    Regular
}

public enum SolarType
{
    Star,
    Planet,
    Moon,
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

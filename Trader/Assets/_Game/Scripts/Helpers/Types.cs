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
    Planet,
    DwarfPlanet,
    Moon,
    Comet,
    Asteroid,
    Structure
}

public enum SolarSubType
{
    SuperGiant,
    WhiteDwarf,
    MainSequence,
    Rocky,
    GasGiant,
    Desert,
    Ocean,
    EarthLike,
    Volcanic,
    Ice
}

public enum PlanetTileType
{
    Rocky,
    Desert,
    Ocean,
    Grasslands,
    Volcanic,
    Ice
}

public enum RawResourceType
{
    Silvite,
    Goldium,
    Quananoid,
    Horizorium,
    Dodite,
    Uronimum,
    Doronimum,
    Astodium,
    Limoite,
    Galiditum
}

public enum ShipMode
{
    Buy,
    Sell,
    SearchingTradeRoute,
    Idle
}

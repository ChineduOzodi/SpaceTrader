using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetTile {

    public PlanetTileType planetTileType;
    public int count = 1;

    public PlanetTile() { }

    public PlanetTile(SolarSubType solarSubType)
    {
        if (solarSubType == SolarSubType.Rocky)
        {
            var rand = Random.value;
            if (rand < .85)
            {
                planetTileType = PlanetTileType.Rocky;
            }
            else if (rand < .9)
            {
                planetTileType = PlanetTileType.Ice;
            }
            else if (rand < .95)
            {
                planetTileType = PlanetTileType.Desert;
            }
            else
            {
                planetTileType = PlanetTileType.Volcanic;
            }
        }
        else if (solarSubType == SolarSubType.EarthLike)
        {
            var rand = Random.value;
            if (rand < .75)
            {
                planetTileType = PlanetTileType.Ocean;
            }
            else if (rand < .85)
            {
                planetTileType = PlanetTileType.Grasslands;
            }
            else if (rand < .88)
            {
                planetTileType = PlanetTileType.Desert;
            }
            else if (rand < .93)
            {
                planetTileType = PlanetTileType.Ice;
            }
            else if (rand < .96)
            {
                planetTileType = PlanetTileType.Rocky;
            }
            else
            {
                planetTileType = PlanetTileType.Volcanic;
            }
        }
        else if (solarSubType == SolarSubType.Ice)
        {
            var rand = Random.value;
            if (rand < .85)
            {
                planetTileType = PlanetTileType.Ice;
            }
            else if (rand < .95)
            {
                planetTileType = PlanetTileType.Rocky;
            }
            else
            {
                planetTileType = PlanetTileType.Ocean;
            }
        }
        else if (solarSubType == SolarSubType.Desert)
        {
            var rand = Random.value;
            if (rand < .85)
            {
                planetTileType = PlanetTileType.Desert;
            }
            else if (rand < .95)
            {
                planetTileType = PlanetTileType.Rocky;
            }
            else
            {
                planetTileType = PlanetTileType.Volcanic;
            }
        }
        else if (solarSubType == SolarSubType.Ocean)
        {
            var rand = Random.value;
            if (rand < .9)
            {
                planetTileType = PlanetTileType.Ocean;
            }
            else if (rand < .95)
            {
                planetTileType = PlanetTileType.Rocky;
            }
            else
            {
                planetTileType = PlanetTileType.Ice;
            }
        }
        else if (solarSubType == SolarSubType.Volcanic)
        {
            var rand = Random.value;
            if (rand < .70)
            {
                planetTileType = PlanetTileType.Volcanic;
            }
            else if (rand < .95)
            {
                planetTileType = PlanetTileType.Rocky;
            }
            else
            {
                planetTileType = PlanetTileType.Desert;
            }
        }
        else
        {
            throw new System.Exception("Unknown Subtype");
        }
    }

    public string GetInfo()
    {
        string resources = "Resources:\n";
        //foreach (RawResourceBlueprint resource in rawResources)
        //{
        //    resources += string.Format("{0} - {1} kg - {2}\n",
        //        resource.rawResourceType.ToString(),
        //        resource.amount.ToString("G"),
        //        resource.accesibility.ToString("0.00"));
        //}

        return string.Format("Tile Type: {0}\n{1}",
            planetTileType.ToString(),
            resources);
    }    
}



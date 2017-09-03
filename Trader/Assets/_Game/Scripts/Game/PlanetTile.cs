using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetTile {

    public PlanetTileType planetTileType;

    public List<RawResource> rawResources = new List<RawResource>();

    public GroundStructure[] structures;

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

        GenerateRawMaterials();
    }

    private void GenerateRawMaterials()
    {
        if (planetTileType == PlanetTileType.Desert)
        {
            var rand = Random.value;
            if (rand < .05) //Goldium
            {
                rawResources.Add(new RawResource(RawResourceType.Goldium, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .05) //Siliute
            {
                rawResources.Add(new RawResource(RawResourceType.Silvite, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .01) //Quananoid
            {
                rawResources.Add(new RawResource(RawResourceType.Quananoid, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .5) //Horizorium
            {
                rawResources.Add(new RawResource(RawResourceType.Horizorium, Random.Range(0, 10000000), Random.value));
            }
            rand = Random.value;
            if (rand < .1) //Dodite
            {
                rawResources.Add(new RawResource(RawResourceType.Dodite, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .1) //uranimum
            {
                rawResources.Add(new RawResource(RawResourceType.Uronimum, Random.Range(0, 10000), Random.value));
            }
            rand = Random.value;
            if (rand < .1) //Doronimum
            {
                rawResources.Add(new RawResource(RawResourceType.Doronimum, Random.Range(0, 10000), Random.value));
            }
            rand = Random.value;
            if (rand < .1) //Astodium
            {
                rawResources.Add(new RawResource(RawResourceType.Astodium, Random.Range(0, 100000), Random.value));
            }
            rand = Random.value;
            if (rand < .01) //Limoite
            {
                rawResources.Add(new RawResource(RawResourceType.Limoite, Random.Range(0, 100000), Random.value));
            }
            rand = Random.value;
            if (rand < .95) //Galiditium
            {
                rawResources.Add(new RawResource(RawResourceType.Galiditum, Random.Range(0, 1000000), Random.value));
            }
        }
        else if (planetTileType == PlanetTileType.Grasslands)
        {
            var rand = Random.value;
            if (rand < .05) //Goldium
            {
                rawResources.Add(new RawResource(RawResourceType.Goldium, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .05) //Siliute
            {
                rawResources.Add(new RawResource(RawResourceType.Silvite, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .01) //Quananoid
            {
                rawResources.Add(new RawResource(RawResourceType.Quananoid, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .75) //Horizorium
            {
                rawResources.Add(new RawResource(RawResourceType.Horizorium, Random.Range(0, 10000000), Random.value));
            }
            rand = Random.value;
            if (rand < .25) //Dodite
            {
                rawResources.Add(new RawResource(RawResourceType.Dodite, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .25) //uranimum
            {
                rawResources.Add(new RawResource(RawResourceType.Uronimum, Random.Range(0, 10000), Random.value));
            }
            rand = Random.value;
            if (rand < .1) //Doronimum
            {
                rawResources.Add(new RawResource(RawResourceType.Doronimum, Random.Range(0, 10000), Random.value));
            }
            rand = Random.value;
            if (rand < .5) //Astodium
            {
                rawResources.Add(new RawResource(RawResourceType.Astodium, Random.Range(0, 100000), Random.value));
            }
            rand = Random.value;
            if (rand < .01) //Limoite
            {
                rawResources.Add(new RawResource(RawResourceType.Limoite, Random.Range(0, 100000), Random.value));
            }
            rand = Random.value;
            if (rand < .25) //Galiditium
            {
                rawResources.Add(new RawResource(RawResourceType.Galiditum, Random.Range(0, 1000000), Random.value));
            }
        }
        else if (planetTileType == PlanetTileType.Ice)
        {
            var rand = Random.value;
            if (rand < .05) //Goldium
            {
                rawResources.Add(new RawResource(RawResourceType.Goldium, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .05) //Siliute
            {
                rawResources.Add(new RawResource(RawResourceType.Silvite, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .1) //Quananoid
            {
                rawResources.Add(new RawResource(RawResourceType.Quananoid, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .25) //Horizorium
            {
                rawResources.Add(new RawResource(RawResourceType.Horizorium, Random.Range(0, 10000000), Random.value));
            }
            rand = Random.value;
            if (rand < .1) //Dodite
            {
                rawResources.Add(new RawResource(RawResourceType.Dodite, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .1) //uranimum
            {
                rawResources.Add(new RawResource(RawResourceType.Uronimum, Random.Range(0, 10000), Random.value));
            }
            rand = Random.value;
            if (rand < .1) //Doronimum
            {
                rawResources.Add(new RawResource(RawResourceType.Doronimum, Random.Range(0, 10000), Random.value));
            }
            rand = Random.value;
            if (rand < .1) //Astodium
            {
                rawResources.Add(new RawResource(RawResourceType.Astodium, Random.Range(0, 100000), Random.value));
            }
            rand = Random.value;
            if (rand < .01) //Limoite
            {
                rawResources.Add(new RawResource(RawResourceType.Limoite, Random.Range(0, 100000), Random.value));
            }
            rand = Random.value;
            if (rand < .25) //Galiditium
            {
                rawResources.Add(new RawResource(RawResourceType.Galiditum, Random.Range(0, 1000000), Random.value));
            }
        }
        else if (planetTileType == PlanetTileType.Ocean)
        {
            var rand = Random.value;
            if (rand < .01) //Goldium
            {
                rawResources.Add(new RawResource(RawResourceType.Goldium, Random.Range(0, 100000), Random.value));
            }
            rand = Random.value;
            if (rand < .01) //Siliute
            {
                rawResources.Add(new RawResource(RawResourceType.Silvite, Random.Range(0, 100000), Random.value));
            }
            rand = Random.value;
            if (rand < .01) //Quananoid
            {
                rawResources.Add(new RawResource(RawResourceType.Quananoid, Random.Range(0, 100000), Random.value));
            }
            rand = Random.value;
            if (rand < .1) //Horizorium
            {
                rawResources.Add(new RawResource(RawResourceType.Horizorium, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .05) //Dodite
            {
                rawResources.Add(new RawResource(RawResourceType.Dodite, Random.Range(0, 100000), Random.value));
            }
            rand = Random.value;
            if (rand < .75) //uranimum
            {
                rawResources.Add(new RawResource(RawResourceType.Uronimum, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .75) //Doronimum
            {
                rawResources.Add(new RawResource(RawResourceType.Doronimum, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .5) //Astodium
            {
                rawResources.Add(new RawResource(RawResourceType.Astodium, Random.Range(0, 100000), Random.value));
            }
            rand = Random.value;
            if (rand < .01) //Limoite
            {
                rawResources.Add(new RawResource(RawResourceType.Limoite, Random.Range(0, 10000), Random.value));
            }
            rand = Random.value;
            if (rand < .1) //Galiditium
            {
                rawResources.Add(new RawResource(RawResourceType.Galiditum, Random.Range(0, 100000), Random.value));
            }
        }
        else if (planetTileType == PlanetTileType.Rocky)
        {
            var rand = Random.value;
            if (rand < .1) //Goldium
            {
                rawResources.Add(new RawResource(RawResourceType.Goldium, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .1) //Siliute
            {
                rawResources.Add(new RawResource(RawResourceType.Silvite, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .01) //Quananoid
            {
                rawResources.Add(new RawResource(RawResourceType.Quananoid, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .95) //Horizorium
            {
                rawResources.Add(new RawResource(RawResourceType.Horizorium, Random.Range(0, 10000000), Random.value));
            }
            rand = Random.value;
            if (rand < .5) //Dodite
            {
                rawResources.Add(new RawResource(RawResourceType.Dodite, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .1) //uranimum
            {
                rawResources.Add(new RawResource(RawResourceType.Uronimum, Random.Range(0, 10000), Random.value));
            }
            rand = Random.value;
            if (rand < .1) //Doronimum
            {
                rawResources.Add(new RawResource(RawResourceType.Doronimum, Random.Range(0, 10000), Random.value));
            }
            rand = Random.value;
            if (rand < .05) //Astodium
            {
                rawResources.Add(new RawResource(RawResourceType.Astodium, Random.Range(0, 100000), Random.value));
            }
            rand = Random.value;
            if (rand < .01) //Limoite
            {
                rawResources.Add(new RawResource(RawResourceType.Limoite, Random.Range(0, 100000), Random.value));
            }
            rand = Random.value;
            if (rand < .15) //Galiditium
            {
                rawResources.Add(new RawResource(RawResourceType.Galiditum, Random.Range(0, 1000000), Random.value));
            }
        }
        else if (planetTileType == PlanetTileType.Volcanic)
        {
            var rand = Random.value;
            if (rand < .05) //Goldium
            {
                rawResources.Add(new RawResource(RawResourceType.Goldium, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .05) //Siliute
            {
                rawResources.Add(new RawResource(RawResourceType.Silvite, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .1) //Quananoid
            {
                rawResources.Add(new RawResource(RawResourceType.Quananoid, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .5) //Horizorium
            {
                rawResources.Add(new RawResource(RawResourceType.Horizorium, Random.Range(0, 10000000), Random.value));
            }
            rand = Random.value;
            if (rand < .25) //Dodite
            {
                rawResources.Add(new RawResource(RawResourceType.Dodite, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .25) //uranimum
            {
                rawResources.Add(new RawResource(RawResourceType.Uronimum, Random.Range(0, 100000), Random.value));
            }
            rand = Random.value;
            if (rand < .5) //Doronimum
            {
                rawResources.Add(new RawResource(RawResourceType.Doronimum, Random.Range(0, 100000), Random.value));
            }
            rand = Random.value;
            if (rand < .75) //Astodium
            {
                rawResources.Add(new RawResource(RawResourceType.Astodium, Random.Range(0, 1000000), Random.value));
            }
            rand = Random.value;
            if (rand < .01) //Limoite
            {
                rawResources.Add(new RawResource(RawResourceType.Limoite, Random.Range(0, 100000), Random.value));
            }
            rand = Random.value;
            if (rand < .35) //Galiditium
            {
                rawResources.Add(new RawResource(RawResourceType.Galiditum, Random.Range(0, 1000000), Random.value));
            }
        }
        else
        {
            throw new System.Exception("Missed Planet tile type");
        }
    }

    public string GetInfo()
    {
        string resources = "Resources:\n";
        foreach (RawResource resource in rawResources)
        {
            resources += string.Format("{0} - {1} kg - {2}\n",
                resource.rawResourceType.ToString(),
                resource.amount.ToString("G"),
                resource.accesibility.ToString("0.00"));
        }

        return string.Format("Tile Type: {0}\n{1}",
            planetTileType.ToString(),
            resources);
    }

    protected struct GroundStuct
    {
        public CompanyModel owner;


    }
    
}



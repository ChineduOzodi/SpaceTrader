using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionComponent {

    public string name;
    public string description;

    public ConstructionComponentType componentType;

    public List<RawResource> rawResources;

    public List<ConstructionComponent> components;

    public int amount;

    public float baseArmor;

    public float GetArmor()
    {
        float armor = 0;
        foreach (ConstructionComponent comp in components)
        {
            armor += comp.GetArmor();
        }

        return armor + baseArmor;
    }

    /// <summary>
    /// workers to operate 1 unit
    /// </summary>
    public int workers;

}

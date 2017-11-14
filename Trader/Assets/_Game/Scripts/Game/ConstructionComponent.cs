using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores a list of Construction components.
/// </summary>
public class ConstructionComponents
{
    private List<ConstructionComponent> contructionComponents;

    public ConstructionComponents() { }

    public ConstructionComponents(List<ConstructionComponent> components)
    {
        contructionComponents = components;
    }

    /// <summary>
    /// Finds amount of raw resouce in list. Returns 0 if not found or amount = 0.
    /// </summary>
    /// <param name="comp"></param>
    /// <returns></returns>
    public float FindAmount(ConstructionComponentType comp)
    {
        foreach (ConstructionComponent component in contructionComponents)
        {
            if (component.componentType == comp)
            {
                return component.amount;
            }
        }
        return 0;
    }

    public void AddAmount(ConstructionComponentType comp, int amount)
    {
        if (FindAmount(comp) > 0)
        {
            foreach(ConstructionComponent component in contructionComponents)
            {
                if (component.componentType == comp)
                {
                    component.AddAmount(amount);
                }
            }
        }
    }
}

public struct ConstructionComponent {

    public string name;
    public string description;

    public ConstructionComponentType componentType;
    /// <summary>
    /// Required resources to create one component.
    /// </summary>
    public RawResources rawResources;

    public int amount;

    public float baseArmor;

    public void AddAmount(int amnt)
    {
        amount += amnt;
    }

    /// <summary>
    /// workers to operate 1 unit
    /// </summary>
    public int workers;

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using System.Xml.Serialization;

[XmlInclude(typeof(Station))]
public class Structure {

    public StructureTypes structureType;

    public string name;

    public ModelRef<IdentityModel> owner { get; protected set; }
    public int managerId { get; protected set; }

    public float maxArmor { get; protected set; }
    public float currentArmor { get; protected set; }

    public int workers { get; protected set; }
    public double workerPayRate = .00116;
    public List<int> solarIndex { get; protected set;}
    public int id { get; protected set; }

    public Dated dateCreated = new Dated(GameManager.instance.data.date.time);
    public Dated lastUpdated = new Dated(GameManager.instance.data.date.time);



    public enum StructureTypes
    {
        Factory,
        Driller,
        GroundStorage,
        LivingQuarters,
        SpaceStation,
        Ship,
        BuildStructure
    }

    public Structure()
    {
    }
}

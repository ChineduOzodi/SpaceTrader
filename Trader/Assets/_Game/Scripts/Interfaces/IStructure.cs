using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using System.Xml.Serialization;

public interface IStructure: IPositionEntity {

    StructureTypes structureType { get; set; }

    string name { get; set; }
    string info { get; set; }

    ModelRef<IdentityModel> owner { get; set; }
    int managerId { get; set; }

    float maxArmor { get; set; }
    float currentArmor { get; set; }

    int id { get; set; }

    Dated dateCreated { get; set; }//new Dated(GameManager.instance.data.date.time);
    Dated lastUpdated { get; set; }//new Dated(GameManager.instance.data.date.time);

    bool deleteStructure { get; set; }
}

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

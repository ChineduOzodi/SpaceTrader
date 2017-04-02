using UnityEngine;
using System.Collections;
using NeuralNetwork;
using CodeControl;
using System.Collections.Generic;

public class ShipModel: StructureModel {
    public ModelRef<CreatureModel> captain = new ModelRef<CreatureModel>();
    public IdentityType type = IdentityType.Ship;
    public ShipMode mode = ShipMode.Idle;
    public bool hyperSpace = false;
    public Vector3 hyperSpacePosition;
    public ModelRef<StructureModel> target = new ModelRef<StructureModel>();
    public ModelRef<StructureModel> sellTarget = new ModelRef<StructureModel>();
    public int capacity;
    
    public float speed;
    public float rotateSpeed = 1f;

    public float runningCost = 1f;

    public List<Items> items = new List<Items>();

    public Items fuel = new Items( ItemTypes.Fuel, 100);
    public float fuelEfficiency = 5;
    public int fuelCapacity = 100;
    public Color spriteColor;
    
}

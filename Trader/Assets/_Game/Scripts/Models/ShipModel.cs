using UnityEngine;
using System.Collections;
using NeuralNetwork;
using CodeControl;

public class ShipModel: StructureModel {

    

    public StructureType type = StructureType.Ship;
    

    

    public int capacity;
    internal int index = 0;
    public float speed;
    public float rotateSpeed = 1f;

    public float runningCost = 1f;

    public Items item = new Items("Nothing", 0);

    public Items fuel = new Items("Fuel", 100);
    public float fuelEfficiency = 5;
    public int fuelCapacity = 100;

}

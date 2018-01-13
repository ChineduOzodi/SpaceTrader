using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class OrbitTestModel : Model {


    public Orbit orbit;
    public ModelRef<OrbitTestModel> parent;

    public OrbitTestModel() { }
    public OrbitTestModel(Orbit orbit)
    {
        this.orbit = orbit;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class StructureModel : IdentityModel {

    public ModelRef<IdentityModel> owner = new ModelRef<IdentityModel>();

    public int workers;
    public int workerCapacity;

}


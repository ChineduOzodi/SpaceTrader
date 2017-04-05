using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using UnityEngine.UI;

public class InfoPanelModel : Model {

    public ModelRef< IdentityModel> target;

    public InfoPanelModel() { }
    public InfoPanelModel(IdentityModel _target)
    {
        target = new ModelRef<IdentityModel>( _target);
    }
}

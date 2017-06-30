#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

public class Hierarchy2RT : vlbRTBase {
	[SerializeField] public List<h2GOHLInfo> listHL;
	[SerializeField] public List<h2GOActiveInfo> listActive;
    public override vlbRTBase Init() {
	    if (listHL == null) listHL = new List<h2GOHLInfo>();
	    if (listActive == null) listActive = new List<h2GOActiveInfo>();
        return this;
    }
}

[Serializable]
public class h2GOHLInfo {
    public GameObject go;
    public Color c;
}

[Serializable]
public class h2GOActiveInfo {
	public GameObject go;
	public bool active;
}
#endif
using UnityEngine;
using System.Collections;

public class Colorer : MonoBehaviour {

    public void SetColor(Color color) {        
        GetComponent<Renderer>().material.SetColor("_BaseColor", color);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconManager : MonoBehaviour {
    public List<GameObject> icons = new List<GameObject>();

    public void ClearIcons()
    {
        foreach(GameObject icon in icons)
        {
            Destroy(icon);
        }

        icons = new List<GameObject>();
    }
}

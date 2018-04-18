using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainHudPanel : MonoBehaviour {

    public static MainHudPanel instance;
    public Text dateInfo;
    public Text namesText;

	// Use this for initialization
	void Start () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (GameManager.instance && !GameManager.instance.setup)
        {
            dateInfo.text = GameManager.instance.data.date.GetDateTime() + "\nTimescale: " + GameManager.instance.timeScale + "\n";
        }
	}

    public void SetNameText(string _name)
    {
        namesText.text = _name;
    }
}

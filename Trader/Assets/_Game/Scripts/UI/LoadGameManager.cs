using CodeControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameManager : MonoBehaviour {

    public GameObject loadingPanel;
    public Slider progressBar;
    public Text loadingText;

    // Update is called once per frame
    void Update () {
		
	}

    public void LoadGame()
    {
        GameManager.instance.setup = true;
        Model.DeleteAll();
        Model.Load("TraderSaves", OnStart, OnProgress, OnDone, OnError);
    }
    private void OnStart()
    {
        loadingText.text = "Loading Save...";
    }
    private void OnProgress(float obj)
    {
        progressBar.value = obj;
    }

    private void OnDone()
    {
        loadingText.text = "Done";
        GameManager.instance.data = Model.First<GameDataModel>();
        GameManager.instance.setup = false;
    }

    private void OnError(string obj)
    {
        loadingText.text= "Error: " + obj;
    }
}

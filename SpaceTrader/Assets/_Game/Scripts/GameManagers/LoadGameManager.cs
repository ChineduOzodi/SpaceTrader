using CodeControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameManager : MonoBehaviour {

    public GameObject loadingPanel;
    public Slider progressBar;
    public Text loadingText;

    internal GameManager game;
    internal ViewManager galaxy;

    public void LoadGame()
    {
        galaxy = ViewManager.instance;
        game = GameManager.instance;

        loadingPanel.SetActive(true);

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
        GameManager.instance.data = Model.First<GameDataModel>();
        loadingText.text = "Done";
        GameManager.instance.setup = false;

        loadingPanel.SetActive(false);
        gameObject.SetActive(false);
        game.StartGame();
    }

    private void OnError(string obj)
    {
        loadingText.text= "Error: " + obj;
    }
}

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
    internal GalaxyManager galaxy;

    public void LoadGame()
    {
        galaxy = GalaxyManager.instance;
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
        game.galaxy.gameObject.SetActive(true); //Sets the galaxy manager to active
        game.pathfindingManager.SetActive(true); //Sets pathfinding to active

        GameManager.instance.data = Model.First<GameDataModel>();
        LoadStars();
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

    public void LoadStars()
    {
        foreach (SolarModel star in game.data.stars)
        {
            Controller.Instantiate<SolarController>("solar", star);
        }
    }
}

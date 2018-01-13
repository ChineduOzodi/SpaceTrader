using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour {

    public GameObject loadingPanel;
    public Slider progressBar;
    public Text loadingText;

	// Use this for initialization
	void Start () {

        loadingPanel.GetComponent<RectTransform>().localPosition = Vector3.zero;
        //loadingPanel.SetActive(false);
		
	}
	
    public void LoadAssets()
    {
        GameManager.instance.gameInitiated = false;
        loadingPanel.SetActive(true);
        StartCoroutine("AssetLoading"); //Will be used to load assets when starting, allowing for mods and alterations to the game
    }

    IEnumerator AssetLoading()
    {
        //Show Panel
        loadingPanel.SetActive(true);
        progressBar.value = 0;
        loadingText.text = "Loading Assets...";
        yield return new WaitForSeconds(1);
        progressBar.value = 1;
        loadingText.text = "Done!";
        loadingPanel.SetActive(false);
        GameManager.instance.gameInitiated = true;
    }
}

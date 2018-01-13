using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour {
    public GameObject tooltipPanel;
    public Text tooltipTitle;
    public Text tooltipInfo;

    internal static ToolTip instance;
	// Use this for initialization
	void Start () {

        tooltipPanel.SetActive(false);
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
        if (tooltipPanel.activeSelf)
        tooltipPanel.transform.localPosition = Input.mousePosition - new Vector3(Camera.main.pixelWidth * .5f,Camera.main.pixelHeight * .5f);
	}

    /// <summary>
    /// Sets information to be displayed on tooltip
    /// </summary>
    /// <param name="tooltipTitleText"></param>
    /// <param name="tooltipInfoText"></param>
    public void SetTooltip(string tooltipTitleText, string tooltipInfoText)
    {
        tooltipPanel.SetActive(true);
        tooltipTitle.text = tooltipTitleText;
        tooltipInfo.text = tooltipInfoText;
    }

    public void CancelTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}

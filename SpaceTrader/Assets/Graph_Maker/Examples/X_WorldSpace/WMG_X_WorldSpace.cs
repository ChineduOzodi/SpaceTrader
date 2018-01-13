using UnityEngine;
using System.Collections;

public class WMG_X_WorldSpace : MonoBehaviour {

	public WMG_Axis_Graph graph;
	public GameObject toolTipPanel;
	public GameObject toolTipLabel;

	// Use this for initialization
	void Start () {
		graph.Init (); // ensure graph Start() method called before this Start() method
		graph.theTooltip.SetTooltipObject (toolTipPanel, toolTipLabel);
	}
}

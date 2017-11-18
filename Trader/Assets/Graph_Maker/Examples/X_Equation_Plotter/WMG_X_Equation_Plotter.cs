using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class WMG_X_Equation_Plotter : MonoBehaviour {

	public Object emptyGraphPrefab;
	public string equationStr;
	public float minX;
	public float maxX;
	public float intervalX;
	public int numDecimalsToRound; // round data to this number of decimals, otherwise can get something like 5.00000002

	WMG_Axis_Graph graph;
	WMG_Series series;
	float decimalsMultiplier;
	Regex operatorAndParenthesesRegex;

	// Use this for initialization
	void Start () {
		GameObject graphGO = GameObject.Instantiate(emptyGraphPrefab) as GameObject;
		graphGO.transform.SetParent(this.transform, false);
		graph = graphGO.GetComponent<WMG_Axis_Graph>();

		graph.legend.hideLegend = true;
		graph.changeSpriteSize (graphGO, 800, 600);
		graph.axesType = WMG_Axis_Graph.axesTypes.CENTER;
		graph.yAxis.AxisMinValue = -20;
		graph.yAxis.AxisMaxValue = 20;
		graph.yAxis.AxisNumTicks = 11;
		graph.yAxis.numDecimalsAxisLabels = 2;
		// auto grow / shrink the y-axis min and max values based on series data
		graph.yAxis.MaxAutoGrow = true;
		graph.yAxis.MaxAutoShrink = true;
		graph.yAxis.MinAutoGrow = true;
		graph.yAxis.MinAutoShrink = true;
		graph.xAxis.AxisMinValue = -10;
		graph.xAxis.AxisMaxValue = 10;
		graph.xAxis.AxisNumTicks = 11;
		graph.xAxis.MaxAutoGrow = true;
		graph.xAxis.MaxAutoShrink = true;
		graph.xAxis.MinAutoGrow = true;
		graph.xAxis.MinAutoShrink = true;
		graph.xAxis.LabelType = WMG_Axis.labelTypes.ticks;
		graph.xAxis.SetLabelsUsingMaxMin = true;

		series = graph.addSeries ();
		series.lineScale = 0.5f;
		series.pointColor = Color.red;
		series.linePadding = 0.2f;

		List<string> operatorsAndParentheses = new List<string>(new string[] {
			@"\(", @"\)", @"\+", "-", @"\*", "/", @"\^", "sqrt", "abs", "log", "ln", "sin", "cos", "tan"});
		string operatorAndParenthesesRegexStr = " ?(";
		for (int i = 0; i < operatorsAndParentheses.Count; i++) {
			operatorAndParenthesesRegexStr += "(";
			operatorAndParenthesesRegexStr += operatorsAndParentheses[i];
			if (i == operatorsAndParentheses.Count - 1) {
				operatorAndParenthesesRegexStr += ")";
			}
			else {
				operatorAndParenthesesRegexStr += ")|";
			}
		}
		operatorAndParenthesesRegexStr += ") ?";
		operatorAndParenthesesRegex = new Regex(operatorAndParenthesesRegexStr);

		decimalsMultiplier = Mathf.Pow(10f, numDecimalsToRound);
	}

	public void OnEquationStringChange(string newStr) {
		equationStr = newStr;
	}

	public void OnPlot() {
		series.seriesName = equationStr;
		series.pointValues.Clear();

		string formattedEquationStr = ensureDelimiterAroundOperatorsAndParentheses(equationStr);
		List<string> rpnString = WMG_Util.ShuntingYardAlgorithm(formattedEquationStr);
		
		for (float i = minX; i <= (maxX + Mathf.Epsilon); i += intervalX) {
			i = Mathf.Round(i*decimalsMultiplier)/decimalsMultiplier;
			Vector2 expResult = WMG_Util.ExpressionEvaluator(rpnString, i);
			if (!float.IsNaN(expResult.y)) {
				series.pointValues.Add(expResult);
			}
		}
	}

	// Separate all the mathematical operators by spaces, so can split the string into a list
	string ensureDelimiterAroundOperatorsAndParentheses(string input) {
		string result = operatorAndParenthesesRegex.Replace(input, @" $& ");
		Regex multiWhitespaceRegex = new Regex (@"\s+");
		result = multiWhitespaceRegex.Replace (result, " ");
		return result.Trim();
	}

}

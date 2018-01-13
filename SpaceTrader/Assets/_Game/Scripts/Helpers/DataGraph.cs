using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGraph {
    public string title;
    public string xLabel;
    public string yLabel;

    internal Dictionary<string,List<Stat>> data;

    public DataGraph()
    {
    }

	public DataGraph(string title, string xValueTitle, string yValueTitle, Dictionary<string,List<Stat>> stats = null)
    {
        this.title = title;
        xLabel = xValueTitle;
        yLabel = yValueTitle;

        data = stats;

        if (data == null)
            data = new Dictionary<string, List<Stat>>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Research {

	public string name { get; set; }
    public string id { get; set; }
    public int totalResearchPoints { get; set; }
    public int currentResearchPoints { get; set; }
    public string description { get; set; }
    public string[] requiredResearch
    {
        get { return _requiredResearch; }
    }


    private string[] _requiredResearch;

    public Research(string _name, string _description, int _totalResearchPoints, string[] _requiredResearch)
    {
        name = _name;
        description = _description;
        totalResearchPoints = _totalResearchPoints;
        currentResearchPoints = 0;
        this._requiredResearch = _requiredResearch;
    }
}

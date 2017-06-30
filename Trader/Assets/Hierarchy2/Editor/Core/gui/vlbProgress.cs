/*
------------------------------------------------
 vlbProgress for Unity3d by VietLabs
------------------------------------------------
	version : 1.0.0
	release : 02 May 2013
	require : Unity3d 4.3+
	website : http://vietlabs.net/vlbProgress
--------------------------------------------------
Show a progress bar, do some work per frame

Usage : Call StartWork() with correct parameters
--------------------------------------------------
*/

using System;
using UnityEditor;
using UnityEngine;
using vietlabs;

public class vlbProgress { 
	//2DO : save workList to some temporary invisible GameObject to prevent workList being destroyed ?
	public static string Title;
	public static string Status;
	public static int Total;
	public static int Current;
	public static Action<int> Work;
	public static Action Finish;
	private static bool IsWorking;

	private const string DefaultStatus = "{0} Working on ... {1}";
	private const float timeBudget = 0.1f; //10 ms max for an Update
	
	static public void Start(string title, Action<int> w, int total, Action f = null){
		if (IsWorking) {
			Debug.LogWarning("Some work is in progress, please wait ");
		}
		Title	= title;
		Status	= null;
		Total	= total;
		Current = 0;
		IsWorking = true;
		
		Work = w;
		Finish = f;

		EditorApplication.update -= Update;
		EditorApplication.update += Update;
	}
	
	static void Update(){
		var stTime = Time.realtimeSinceStartup;

		//vlbUnityEditor.BeginGC();
		while (Current < Total) {
			Work(Current++);
			//vlbUnityEditor.CheckGC();
			var realTime = Time.realtimeSinceStartup;
			if ((realTime - stTime) >= timeBudget) break; 
		}
		//vlbUnityEditor.EndGC();
		
	    if (Current < Total) {
	    	var progress = Current/(float) Total;
			if (string.IsNullOrEmpty(Status)) Status = DefaultStatus;
			if (EditorUtility.DisplayCancelableProgressBar(Title, string.Format(Status,
				"[" + Current + "\u2044" + Total + "]",
				" (" + Mathf.RoundToInt(progress * 1000f) / 10f + "%)"), progress)
			){
				Stop();
		    }
	    } else {
			Stop();
	    }
		
	}
	
	static void Stop(){
		Total = 0;
	    Current = 0;
	    Work = null;
	    IsWorking = false;
		
		EditorUtility.ClearProgressBar();
		EditorApplication.update -= Update;
	    if (Finish != null) Finish();
	}
	
}
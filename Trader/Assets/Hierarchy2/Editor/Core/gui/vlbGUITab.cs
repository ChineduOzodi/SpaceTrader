using System;
using UnityEditor;
using UnityEngine;
namespace vietlabs {
internal class vlbGUITab
{
	int activeIdx;
	private bool showContent = true;
	Action[] TabDrawer;
	GUIContent[] TabList;

	internal vlbGUITab(string[] tabs, Action[] drawers) {
		if (tabs.Length != drawers.Length)
		{
			Debug.LogError("vlbGUITab error - number of tabs["+ tabs.Length+"] should be equal to number of drawers[" + drawers.Length+"]");
		}

		TabList = new GUIContent[tabs.Length];
		for (var i = 0; i < tabs.Length; i++) {
			TabList[i] = new GUIContent(tabs[i]);
		}
		TabDrawer = drawers;
	}

	public void Draw()
	{
		//var h = 18;
		//var d = showContent ? 2 : 0;
		//var r = GUILayoutUtility.GetRect(Screen.width-18f, 18);
		
		GUILayout.Box("", GUIStyle.none, GUILayout.Height(16f));
		var r = GUILayoutUtility.GetLastRect();
		//if (showContent) GUI.Box(r.AddHeight(-5),"");

		/*GUILayout.BeginHorizontal();
		//var r = GUILayoutUtility.GetRect(20, 20);
		//GUI.DrawTexture(r, vlbGUISkin.icoEye(showContent));
		if (GUILayout.Button(vlbGUISkin.icoEye(showContent), EditorStyles.label)) {
			showContent = !showContent;
		}

		activeIdx = GUILayout.Toolbar(activeIdx, TabList);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();*/
		if (showContent) TabDrawer[activeIdx]();

		if (GUI.Button(r.w(20f).dx(2).dy(1), EditorResource.GetTexture2D("eye"), EditorStyles.label)) {
			showContent = !showContent;
		}

		activeIdx = GUI.Toolbar(r.dx(25).dw(-3).dh(0), activeIdx, TabList);
	}
}
}
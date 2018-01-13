using System;
using UnityEditor;
using UnityEngine;

namespace vietlabs {
internal class vlbGUISearch {

	internal string gID = "vlbGUISearch";
	internal Rect rect = new Rect(0,0,0,0);
	//internal int CatWidth = 90;

	internal bool gainFocus = true;
	internal bool focus;

    //internal string[]	CategoryList;
    //internal int Category;
    //internal string CategoryTitle;
	internal string	SearchTerm;

	internal GUIStyle TextFieldStyle;
	internal GUIStyle CloseBtnStyle;
	internal GUIStyle EmptyBtnStyle;
	//internal GUIStyle ToolbarBtnStyle;

	internal vlbGUISearch(string term = null) {//, params string[] list
		SearchTerm = term ?? "";
		//CategoryList = list;
	}

	void StartSearch() {
		gainFocus = true;
	}

	void StopSearch() {
		gainFocus	= false;
		focus		= false;
		SearchTerm	= "";
		GUI.FocusControl(null);
		Event.current.Use();
	}

    //internal void Reset() {
		
    //}

    //void SetCategory(int idx) {
    //    Category = idx;
    //    CategoryTitle = null;
    //}

	internal void Draw(Action<string> onSearch) {
		var evt	= Event.current;
        rect = GuiX.FlexibleSpace().dy(2f);
        //Debug.Log("----> "+ rect + ":" + evt);

		//GUI.BeginGroup(rect, "", EditorStyles.toolbar);
        //GUILayout.BeginHorizontal();
        //GUILayoutUtility.GetRect(rect.width, rect.height);
        //GUILayout.EndHorizontal();

		if (TextFieldStyle == null) {
			TextFieldStyle	= new GUIStyle("ToolbarSeachTextFieldPopup");// {fixedHeight = 14f};
			CloseBtnStyle	= new GUIStyle("ToolbarSeachCancelButton");
			EmptyBtnStyle	= new GUIStyle("ToolbarSeachCancelButtonEmpty");
			//ToolbarBtnStyle = new GUIStyle(EditorStyles.toolbarPopup) {fixedWidth = CatWidth /*, alignment = TextAnchor.MiddleCenter*/};
		}

		var r2 = rect;

		GUI.SetNextControlName(gID);
		if (gainFocus) {
			GUI.FocusControl(gID);
			EditorGUI.FocusTextInControl(gID);

			if (focus && evt.type == EventType.Repaint) {
				gainFocus = false;
			}
		}

		focus = GUI.GetNameOfFocusedControl() == gID;

		if (focus) {
			//ESC is down without any modifier
			if (evt.type == EventType.KeyDown) Debug.Log("--->"+evt.keyCode);

			if (KeyCode.Escape.xKey_isDown().noModifier) {
				StopSearch();
			}
            if (KeyCode.Return.xKey_isDown().noModifier && !string.IsNullOrEmpty(SearchTerm)) {
				if (onSearch != null) {
                    onSearch(SearchTerm);//, CategoryList[Category]
				} else {
					Debug.Log(string.Format("Search for <{0}> has no handler", SearchTerm));
				}

				Event.current.Use();
			}
		} else {
			//normal click without any modifier
			if (rect.xLMB_isDown().noModifier) StartSearch();
			//TODO : Support Validate / ExecuteCommand
			//if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "Find") {
			//	Event.current.Use();
			//	return;
			//} else if (Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "Find") {
			//	focusSearchBox = true;
			//}
		}

		SearchTerm = EditorGUI.TextField(r2.dw(-20f), SearchTerm ?? "", TextFieldStyle);

		if (GUI.Button(r2.xHzSubRectsRight(20f)[0], string.Empty, SearchTerm == string.Empty ? EmptyBtnStyle : CloseBtnStyle)) {
			StopSearch(); 
		}

        //EditorGUI.BeginChangeCheck();
        //GUI.Button(r2.xSlide(1f).dy(-2).w(CatWidth), CategoryTitle ?? (CategoryTitle = CategoryList[Category].Split('&')[0]), ToolbarBtnStyle);
				
        //if (EditorGUI.EndChangeCheck()) {
        //    var cm = new GenericMenu();
        //    for (var i = 0; i < CategoryList.Length; i++) {
        //        var idx = i;
        //        cm.xAdd(CategoryList[i], () => SetCategory(idx));
        //    }
        //    cm.ShowAsContext();
        //    //EditorUtility.DisplayCustomMenu(r2, CategoryList.ToGUIContent(), Category, null, null);
        //}

		//GUI.EndGroup();
	}
}

}

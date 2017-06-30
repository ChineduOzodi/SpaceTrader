using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class WindowX {
    private static Dictionary<string, EditorWindow> _windowDict;
    public static void xClearDefinitionCache() { _windowDict = new Dictionary<string, EditorWindow>(); }

    public static EditorWindow xGetEditorWindowByName(this string className) {
        if (_windowDict == null) _windowDict = new Dictionary<string, EditorWindow>();
        var hasCache = _windowDict.ContainsKey(className);
        var window = hasCache ? _windowDict[className] : null;

        if (hasCache) {
            if (window != null) return window;
            _windowDict.Remove(className);
        }
        var typeT = className.xGetTypeByName();

        //Debug.Log("xGetEditorWindowByName ---------------------> " + className);

        var list = Resources.FindObjectsOfTypeAll<EditorWindow>();
        foreach (var item in list) {
            if (item.GetType() == typeT) {
                _windowDict.Add(className, item);
                return item;
            }
        }

        window = EditorWindow.GetWindow(typeT);
        if (window != null) _windowDict.Add(className, window); 
        return window;
    }

    public static EditorWindow Inspector {
        get { return "UnityEditor.InspectorWindow".xGetEditorWindowByName(); }
    }

    public static EditorWindow Hierarchy {
        get {
            var window =
#if UNITY_4_5 || UNITY_4_6 || UNITY_5
                "UnityEditor.SceneHierarchyWindow".xGetEditorWindowByName();
#else
            "UnityEditor.HierarchyWindow".xGetEditorWindowByName(); 
#endif
            return window;
        }
    }

    internal static T xAsDropdown<T>(this Rect rect, bool convertPosition = false) where T : EditorWindow {
        var edw = ScriptableObject.CreateInstance<T>();

        if (convertPosition) {
            var r2 = GUIUtility.GUIToScreenPoint(rect.XY_AsVector2());
            rect.x = r2.x;
            rect.y = r2.y;    
        }

        edw.ShowAsDropDown(rect.h(18f), rect.WH_AsVector2());
        edw.Focus();
        edw.xGetField("m_Parent")
            .xInvoke("AddToAuxWindowList", "UnityEditor.GUIView".xGetTypeByName());
        edw.wantsMouseMove = true;
        return edw;
    }

    /*internal static T xAsDropdown<T>(this Rect rect) where T : EditorWindow
    {
        var edw = ScriptableObject.CreateInstance<T>();
        var r2 = GUIUtility.GUIToScreenPoint(rect.XY_AsVector2());
        rect.x = r2.x;
        rect.y = r2.y;

        edw.ShowAsDropDown(rect.h(18f), rect.WH_AsVector2());
        edw.Focus();
        edw.xGetField("m_Parent")
            .xInvoke("AddToAuxWindowList", "UnityEditor.GUIView".xGetTypeByName("UnityEditor"));
        edw.wantsMouseMove = true;
        return edw;
    }*/


    internal static void xSetSearchFilterTerm(this EditorWindow window, string term) {
        var sWindow = "UnityEditor.SearchableEditorWindow".xGetTypeByName();
        window.xInvoke(
            "SetSearchFilter", sWindow, null, new object[] {term, SearchableEditorWindow.SearchMode.All, true});
        window.xSetField("m_HasSearchFilterFocus", true, sWindow);

        EditorGUI.FocusTextInControl("SearchFilter");
        window.Repaint();
    }

    internal static string xGetSearchFilterTerm(this EditorWindow window) {
        var sWindow = "UnityEditor.SearchableEditorWindow".xGetTypeByName();
        return (string)window.xGetField("m_SearchFilter", sWindow);
    }
}
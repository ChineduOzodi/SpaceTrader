using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using vietlabs;

public class IsolateWindow : SearchableEditorWindow {
    public static IsolateWindow window;
    public static bool isShowing;
    public static Rect wPosition;

    public string wtitle;
    vlbGUIList<GOInfo> guiList;

    public IsolateWindow SetData(string pTitle, List<GameObject> goList) {
        var infoList = new List<GOInfo>();

        for (var i = 0; i < goList.Count; i++) {
            var go = goList[i];
            infoList.Add(new GOInfo() {
                Name = go.name,
                go = go
            });
        }

        wtitle = pTitle;
        guiList = new vlbGUIList<GOInfo>(infoList);
        Repaint();
        return this; 
    }

    static void refreshGUI() {
        if (isShowing && window == null) {
            //Debug.Log("----> NULL " + wPosition);
            var ofocus = focusedWindow;
            window = wPosition.xAsDropdown<IsolateWindow>();
            window.position = wPosition;
            if (ofocus != null) ofocus.Focus();
        }
    }

    public void OnGUI() {

        if (!isShowing) {
            isShowing = true;
            window = this;
            wPosition = window.position;
            
            EditorApplication.update -= refreshGUI;
            EditorApplication.update += refreshGUI;
        }

        //Debug.Log(Event.current + ":" + window.position);
        
        using (GuiX.hzLayout) {
            GUILayout.Label(wtitle ?? "", GUILayout.Width(50f));
            this.xInvoke("SearchFieldGUI", typeof(SearchableEditorWindow));    
            GUILayout.Space(20f);

            var r = GUILayoutUtility.GetLastRect().dx(2f).dy(1f).wh(18f, 18f);
            //if (r.Contains(Event.current.mousePosition)) GUI.Box(r, " ");
            GUI.Label(r, "X");

            if (r.xLMB_isUp().noModifier) {
                isShowing = false;
                window = null;
                Close();
            } 
        }

        if (guiList == null) return;
        //GUILayout.Label(title + "[" + guiList.CacheList.Count + "]");

        guiList.Draw((r, info, idx) => {
            if (r.xLMB_isDown()) info.go.xPing();
            GUI.Label(r, info.Name);
            return 20;
        });

        GUILayout.Space(20f);
    }
}

public class GOInfo {
    public string Name;
    public GameObject go;

}
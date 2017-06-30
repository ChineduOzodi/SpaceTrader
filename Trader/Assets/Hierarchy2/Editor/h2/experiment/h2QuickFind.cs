using UnityEditor;
using UnityEngine;
using vietlabs;

public class h2QuickFind : SearchableEditorWindow
{
    const float LabelW = 40f;
    const float guiH = 50f;

    static Rect drawRect;

    static public void OpenAt(EditorWindow w) {
        drawRect = w.position.dt(w.position.height - guiH-2f).dw(-14f);
        drawRect.xAsDropdown<h2QuickFind>(); 
    }

    vlbGUISearch uiSearch;

    void OnGUI() {
        Debug.Log("OnGUI Quickfind : " + position);
        position = drawRect;

        using (GuiX.HzLayout2(EditorStyles.toolbar)) {
            if (GUILayout.Button("Find", EditorStyles.toolbarDropDown, GUILayout.Width(LabelW))) {
                
            }
            GUILayout.Space(5f);
            if (uiSearch == null) uiSearch = new vlbGUISearch();
            uiSearch.Draw(onSearch);
        }
    }

    void onSearch(string term) {
        Debug.Log("----> " + term);
    }
}

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using vietlabs;
using Object = UnityEngine.Object;

public class vlbStringArray {
    public string title;
    public string focusName = "vlbStringArray.editName";
    private string editName;
    private int editIndex = -1;
    private bool _needRepaint;
    public bool isExpand;

    public List<string> list;
    private vlbGUIList<string> drawer;

    public bool needRepaint {
        get { return _needRepaint || drawer.NeedRepaint; }
    }

    internal vlbStringArray(string title, List<string> plist, GUIListTheme theme = null, Object listId = null) { //List<string> list, GUIListTheme theme = null, Object listId = null
        list = plist;
        this.title = title;
        drawer = new vlbGUIList<string>(plist, theme, listId);
    }

    public void Draw(Action<int, int, string> onReorder = null,
        Action<string, string, int> onRename = null,
        Action<string, int, vlbGUIList<string>> OnRightClick = null,
        Rect? drawRect = null) {

        _needRepaint = false;
        //isExpand = GuiX.xDrawTitleBar(title, 5f, null, isExpand);

        GDrawX.Bar().xDrawL_Arrow(ref isExpand)
            .xDrawL_BoldLabel(title)
            .xDrawSub(subRect => {
                using (GuiX.DisableGroup(editIndex != -1)) {
                    if (subRect.dy(2f).xMiniButton("+", false)) {
                        list.Add("");
                        editIndex = drawer.CacheList.Count - 1;
                        editName = "";
                        isExpand = true;
                        _needRepaint = true;
                    }
                }
            }, 16f, false, new Vector2(-2f, 0f));

        //var rAdd = GUILayoutUtility.GetLastRect().xAdjustTL(16).dy(2f).dw(-2f);
        //if (rAdd.xMiniButton("+", false)) {
        //    list.Add("");
        //    editIndex = drawer.CacheList.Count - 1;
        //    editName = "";
        //    isExpand = true;
        //    _needRepaint = true;
        //}

        if (isExpand) {
            drawer.Draw((r, v, idx) => {
                //if (idx == 0) Debug.Log("d2StringGUI " + idx + ":" + r);

                if (idx == editIndex)
                {
                    RenameGUI(r);
                }
                else
                {
                    var v1 = EditorGUI.TextField(r.dw(-20f).dt(2f), v);
                    if (v1 != v) {
                        list[idx] = v1;
                        if (onRename != null) onRename(v, v1, idx);
                    }

                    //Debug.Log(Event.current + ":" + r.Contains(Event.current.mousePosition) + ":" + GUI.GetNameOfFocusedControl
                    //remove focus when click outside
                    if (GUI.GetNameOfFocusedControl() == focusName && !r.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown) {
                        GUI.FocusControl(null);
                        _needRepaint = true;
                    }

                    /*EditorGUI.LabelField(r.dl(r.width - 80f).dw(-10f).dt(2f).db(-2f), "" + depth * idx,
                        GuiX.miniLabelGrayStyle);*/
                }

                //if (v != "Default") { // Can not remove group / Stacks Default
                var remRect = r.xSubRectRight(16f).dx(-4f);
                GUI.DrawTexture(remRect, EditorResource.GetTexture2D("remove"));

                if (remRect.xLMB_isDown().noModifier) {
                    //updateComponent(v, null);
                    if (editIndex == idx) editIndex = -1;
                    list.RemoveAt(idx);
                    _needRepaint = true;
                }
                //}

                return 18;
            }, onReorder, OnRightClick, null, drawRect);
        }
    }

    private void RenameGUI(Rect r) {
        r = r.dw(-25f);

        var w = 60f;
        GUI.SetNextControlName(focusName);
        editName = EditorGUI.TextField(r.dw(-w).dt(2f), editName ?? "");
        if (GUI.GetNameOfFocusedControl() != focusName) {
            GUI.FocusControl(focusName);
        }

        var e = Event.current;
        var isEnter = e.keyCode == KeyCode.Return; // && e.type == EventType.KeyDown

        //Debug.Log("isEnter="+ isEnter);

        if (r.xAdjustTL(w - 10f).dy(2).xMiniButton("Done", false) || isEnter) {
            var trimName = editName.Trim();
            if (!string.IsNullOrEmpty(trimName)) {
                var enIdx = list.IndexOf(trimName);
                if (enIdx != -1) {
                    Debug.LogWarning("Duplicated name <" + editName + "> @" + enIdx + ":" + list.Count);
                    GUI.FocusControl(focusName);
                } else {
                    if (!string.IsNullOrEmpty(editName)) {
                        list[editIndex] = editName;
                    }
                    else {
                        list.RemoveAt(list.Count - 1);
                    }

                    editName = null;
                    editIndex = -1;
                    EditorGUI.FocusTextInControl(null);
                }
            }
        }
    }
}
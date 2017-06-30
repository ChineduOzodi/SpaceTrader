using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using vietlabs;

public static class HierarchyX {
    public static GameObject renameGO;
    public static int renameStep;

    internal static bool HasFocusOnHierarchy
    {
        get
        {
            var fw = EditorWindow.focusedWindow;
            if (fw == null) return false;

            //Debug.Log(fw.GetType().ToString());

            return fw.GetType().ToString() ==
                #if UNITY_4_5 || UNITY_4_6
                    "UnityEditor.SceneHierarchyWindow";
                #else
                    "UnityEditor.HierarchyWindow";
                #endif
        }
    }

    internal static bool hHasStatic(this List<GameObject> list) {
        var hasStatic = false;
        foreach (var go in list) {
            hasStatic = go.isStatic;
            if (hasStatic) break;
        }
        return hasStatic;
    }

    private static List<object> GetChildrenTreeItem(this object treeItem, Type itemType, bool deep) {
        var tempList = treeItem.xGetField("m_Children", itemType);
        var result = new List<object>();

        //Debug.Log(treeItem + ":" + tempList);

        if (tempList is bool) return result;
        if (tempList == null) return result;

        var tempList2 = (IList) tempList;
        for (var i = 0; i < tempList2.Count; i++) {
            var item = tempList2[i];
            result.Add(item);

            if (deep) {
                //Debug.Log("deep start <" + (item==null) + ">");
                result.AddRange(item.GetChildrenTreeItem(itemType, true));
                //Debug.Log("deep end");
            }

            item.xSetField("m_Depth", 0, itemType);
            item.xSetField("m_Children", new List<object>().xToListT(itemType), itemType);
        }
        return result;
    }

    internal static int[] GetFilterInstanceIDs(Func<GameObject, bool> func) {
        var list = new List<GameObject>();
        foreach (var child in TransformX.RootT) {
            AppendChildren(child, ref list, true);
        }

        var result = new List<int>();
        for (var i = 0; i < list.Count; i++) {
            var c = list[i];
            var isValid = func(c);
            if (isValid) result.Add(c.GetInstanceID());

            //Debug.Log(i + ":" + list[i] + ":::" + isValid);
        }

        return result.ToArray();
        //return (list.Where(item => func(item)).Select(item => item.GetInstanceID())).ToArray();
    }

//    private static int[] isolateIDs;
//    private static string isolateTitle;
//
//    internal static void RefreshSearchFilter(EditorWindow w) {
//        w.SetSearchFilter(isolateIDs, isolateTitle);
//        //EditorX.xDelayCall(w.Repaint);
//    }
//
//
//    internal static bool HasSearchFilter(this EditorWindow window) {
//        var treeView = window.xGetField("m_TreeView");
//        var ds = treeView.xGetProperty("data");
//        return !string.IsNullOrEmpty((string)ds.xGetField("m_SearchString", TypeX.GameObjectTreeViewDataSourceT));
//    }


    
//    internal static void SetSearchFilter(this EditorWindow window, int[] instanceIDs, string title) {
//        if (window == null) {
//            WindowX.xClearDefinitionCache();
//            window = WindowX.Hierarchy;
//        }
//
//        var goList = new List<GameObject>();
//
//        for (var i = 0; i < instanceIDs.Length; i++) {
//            var go = (GameObject)EditorUtility.InstanceIDToObject(instanceIDs[i]);
//            if (go != null) goList.Add(go);
//        }
//
//        //var w = EditorWindow.GetWindowWithRect<IsolateWindow>(window.position.xSlide(1f).dh(-100f).dy(55f));
//
//        /*var isChromeless = true;
//
//        if (isChromeless) {
//            //var v = window.position.xSlide(1f).dh(-40f).dx(2f);//.dy(3f)
//            var v = window.position.xSlide(-1f).dy(-18f);
//            v.xAsDropdown<IsolateWindow>().SetData("Isolate", goList);
//        } else {
//            var v = window.position.xSlide(1f).dh(-40f).dx(2f);//.dy(3f)
//            EditorWindow.GetWindowWithRect<IsolateWindow>(v).SetData("Isolate", goList);
//        }
//        
//        return;*/
//
//        //* temp *//  Hierarchy2._isIsolating = true;
//        //isolateIDs = instanceIDs;
//        //isolateTitle = title;
//
//		//if (instanceIDs.Length == 0) {
//			window.xInvoke(
//				"SetSearchFilter", null, null,
//				new object[] {"Hierarchy2tempfixemptysearch", SearchableEditorWindow.SearchMode.All, false});
//			window.xSetSearchFilterTerm("iso:" + title);
//			return;
//		//}
//
//        /**/
//
///*#if UNITY_4_6
//        //var sf = typeof(SearchableEditorWindow).xInvoke("CreateFilter", null, null, "iso:" +title, SearchableEditorWindow.SearchMode.All);
//        window.xInvoke("SetSearchFilter", null, null, "iso:" + title, SearchableEditorWindow.SearchMode.All, false);
//        window.xSetField("m_HasSearchFilterFocus", true, typeof(SearchableEditorWindow));
//
//        var treeView        = window.xGetField("m_TreeView");
//        var ds              = treeView.xGetProperty("data");
//        var rootItem        = ds.xGetField("m_RootItem");
//        //var children        = rootItem.GetChildrenTreeItem(TypeX.TreeViewItemT, true);
//        var childrenList    = TypeX.TreeViewItemT.xNewListT();
//        var list            = new List<int>();
//
//        for (var i = 0; i < instanceIDs.Length; i++) {
//            //if (list.Contains(instanceIDs[i])) continue;
//
//            var hProperty = Activator.CreateInstance(TypeX.HierarchyPropertyT, HierarchyType.GameObjects);
//            hProperty.xInvoke("Find", null, null, instanceIDs[i], null);
//            var govi = ds.xInvoke("CreateTreeViewItem", TypeX.GameObjectTreeViewDataSourceT, null, hProperty, false, 0, true);
//            childrenList.Add(govi);
//
//            //Debug.Log(i + "---> " + govi);
//            //if (!dict.ContainsKey(instanceIDs[i])) 
//            //list.Add(instanceIDs[i]);
//        }
//
//        TypeX.TreeViewUtilityT.xInvoke("SetChildParentReferences", null, null, childrenList, rootItem);
//        ds.xSetField("m_SearchString", title, TypeX.GameObjectTreeViewDataSourceT);
//        ds.xSetProperty("expandedIDs", instanceIDs.ToList());
//        //ds.xSetProperty("expandedIDs", list);
//        //ds.xSetProperty("expandedIDs", dict.Keys.ToList());
//
//        
//        ds.xSetField("m_VisibleRows", childrenList);//.xToListT(TypeX.TreeViewItemT)
//        treeView.xSetField("m_AllowRenameOnMouseUp", false);
//
//        //window.Repaint();
//
//# el*/
//#if UNITY_4_5 || UNITY_4_6 || UNITY_5
//        //var treeViewSrcT    = "UnityEditor.TreeViewDataSource".xGetTypeByName("UnityEditor");
//        var treeViewItemT = "UnityEditor.TreeViewItem".xGetTypeByName("UnityEditor");
//        var treeView = WindowX.Hierarchy.xGetField("m_TreeView");
//        var treeViewData = treeView.xGetProperty("data");
//        var rootItem = treeViewData.xGetField("m_RootItem");
//        var children = rootItem.GetChildrenTreeItem(treeViewItemT, true);
//        var expandIds = treeViewData.xInvoke("GetExpandedIDs"); //save the expand state to restore
//
//        foreach (var t in children) { // expand all children
//            if (t != null) treeViewData.xInvoke("SetExpandedWithChildren", null, null, t, true);
//        }
//
//	    //Debug.Log("ids :: " + instanceIDs.Length);
//
//        var children1 = (IList) treeViewData.xInvoke("GetVisibleRows");
//        var childrenList = treeViewItemT.xNewListT();
//        for (var i = 0; i < children1.Count; i++) {
//            var child = children1[i];
//
//            if (instanceIDs.Contains((int) child.xGetField("m_ID", treeViewItemT))) {
//                child.xSetField("m_Depth", 0, treeViewItemT);
//                childrenList.Add(child);
//            }
//        }
//
//        // restore the expand state for children
//        treeViewData.xInvoke("SetExpandedIDs", null, null, expandIds);
//
//        window.xInvoke(
//            "SetSearchFilter", null, null, new object[] {"iso:" + title, SearchableEditorWindow.SearchMode.All, false});
//        treeViewData.xSetField("m_VisibleRows", childrenList.xToListT(treeViewItemT));
//        treeView.xSetField("m_AllowRenameOnMouseUp", false);
//        treeView.xInvoke("Repaint");
//#else
//	    var TBaseProjectWindow = "UnityEditor.BaseProjectWindow".xGetTypeByName("UnityEditor");
//	    var TFilteredHierarchy = "UnityEditor.FilteredHierarchy".xGetTypeByName("UnityEditor");
//
//	    window.SetSearchFilter("iso:" + title);
//
//		var instIDsParams = new object[] { instanceIDs };
//    	var fh = window.xGetField("m_FilteredHierarchy", TBaseProjectWindow);
//    	var sf = (SearchFilter)fh.xGetField("m_SearchFilter", TFilteredHierarchy);
//
//		sf.ClearSearch();
//		sf.referencingInstanceIDs = instanceIDs;
//	    fh.xInvoke("SetResults", TFilteredHierarchy, null, instIDsParams);
//
//	    var arr = (object[])fh.xGetProperty("results", TFilteredHierarchy, null);//(FilteredHierarchyType.GetProperty("results").GetValue(fh, null));
//			var list = new List<int>();
//
//			//patch
//			var nMissing = 0;
//			foreach (var t in arr) {
//				if (t == null) {
//					nMissing++;
//					continue;
//				}
//				var id = (int)t.xGetField("instanceID");
//				if (!list.Contains(id)) list.Add(id);
//			}
//
//			if (nMissing > 0) Debug.LogWarning("Filtered result may not be correct, missing " + nMissing + " results, please help report it to unity3d@vietlab.net");
//			instanceIDs = list.ToArray();
//
//			//reapply
//			sf.ClearSearch();
//			sf.referencingInstanceIDs = instanceIDs;
//	    fh.xInvoke("SetResults", TFilteredHierarchy, null, new object[] { instanceIDs });
//			window.Repaint();
//#endif
//    }
    internal static bool IsExpanded(this GameObject go) {
        var mExpand =

#if UNITY_4_5 || UNITY_4_6
        (int[]) WindowX.Hierarchy.xGetField("m_TreeView").xGetProperty("data").xInvoke("GetExpandedIDs");
#else 
        (int[]) WindowX.Hierarchy.xGetField(
            "m_ExpandedArray", "UnityEditor.BaseProjectWindow".xGetTypeByName()
        );
#endif

        return mExpand.Contains(go.GetInstanceID());
    }

    internal static bool IsRenaming() {
        var oFocus = EditorWindow.focusedWindow;

#if UNITY_4_5 || UNITY_4_6 || UNITY_5
        var result = false;
        var tvState = WindowX.Hierarchy.xGetField("m_TreeViewState");
        if (tvState != null) {
            var overlay = tvState.xGetField("m_RenameOverlay");
            if (overlay != null) result = (bool) overlay.xGetField("m_IsRenaming");
        }
#else 
            var hWindow = WindowX.Hierarchy;
	    var type = "UnityEditor.BaseProjectWindow".xGetTypeByName();
	    var result = (int)hWindow.xGetField("m_RealEditNameMode", type) == 2;
#endif

        if (oFocus != null && oFocus != EditorWindow.focusedWindow) oFocus.Focus();

        return result;
    }

    internal static void Rename(this GameObject go) {
        var hWindow = WindowX.Hierarchy;

        if (Event.current != null && Event.current.keyCode == KeyCode.Escape) {
            renameGO = null;
            renameStep = 0;
            hWindow.Repaint();
            return;
        }

        if (renameGO != go) {
            renameGO = go;
            renameStep = 2;
        }

        //Debug.Log("Rename : " + go + ":" + renameStep);

        if (!IsRenaming()) {
            //not yet in edit name mode, try to do it now
            Selection.activeGameObject = go;

#if UNITY_4_5 || UNITY_4_6  || UNITY_5
            var treeView = WindowX.Hierarchy.xGetField("m_TreeView");
            var data = treeView.xGetProperty("data");
            var gui = treeView.xGetProperty("gui");
            var item = data.xInvoke("FindItem", null, null, go.GetInstanceID());

            if (item != null && gui != null) gui.xInvoke("BeginRename", null, null, item, 0f);
#else
                var property = new HierarchyProperty(HierarchyType.GameObjects);
                property.Find(go.GetInstanceID(), null);

	        hWindow.xInvoke("BeginNameEditing", TypeX.BaseProjectWindowT, null, go.GetInstanceID());
	        hWindow.xSetField("m_NameEditString", go.name, TypeX.BaseProjectWindowT); //name will be missing without this line
                hWindow.Repaint();
#endif
        } else {
            if (Event.current == null) {
                renameStep = 2;
                //Debug.Log("How can Event.current be null ?");
                return;
            }

            if (Event.current.type == EventType.repaint && renameStep > 0) {
                renameStep--;
                //hWindow.Repaint();
            }

            if (Event.current.type != EventType.repaint && renameStep == 0) renameGO = null;
        }
        //}
    }

    //-------------------------------- FLAG ----------------------------

    internal static void SetDeepFlag(this GameObject go, HideFlags flag, bool value, bool includeMe = true,
        bool recursive = true) {
        if (includeMe) go.xSetFlag(flag, value);

        var trans = go.transform;
        for (var i = 0; i < trans.childCount; i++) {
            var t = trans.GetChild(i);

            if (recursive) SetDeepFlag(t.gameObject, flag, value);
            else t.gameObject.xSetFlag(flag, value);
        }

        /*foreach (Transform t in go.transform) {
            if (recursive) SetDeepFlag(t.gameObject, flag, value);
            else t.gameObject.xSetFlag(flag, value);
        }*/
    }

    internal static bool HasFlagChild(this GameObject go, HideFlags flag) {
        var t = go.transform;
        for (var i = 0; i < t.childCount; i++) {
            if (t.GetChild(i).xGetFlag(flag)) return true;
        }
        return false;
    }

    internal static bool HasFlagChild(this List<GameObject> list, HideFlags flag) {
        //var has = false;

        /*foreach (var child in list)
        {
            child.ForeachChild2(child2 =>
            {
                has = child2.GetFlag(flag);
                return !has;
            });
            if (has) break;
        }*/

        return list.Any(item => item.HasFlagChild(flag));
    }

    internal static bool HasFlag(this List<GameObject> list, HideFlags flag) {
        var hasActive = false;
        foreach (var go in list) {
            hasActive = go.xGetFlag(flag);
            if (hasActive) break;
        }
        return hasActive;
    }

    internal static void SetDeepFlag(this List<GameObject> list, bool value, HideFlags flag, bool includeMe) {
        foreach (var go in list) {
            go.SetDeepFlag(flag, value, includeMe);
        }
    }

    //-------------------------------- ACTIVE ----------------------------

    internal static bool HasActiveChild(this GameObject go) {
        var has = false;
        go.xForeachChild2(
            child => {
                has = child.activeSelf;
                return !has;
            });
        return has;
    }
    internal static bool HasGrandChild(this GameObject go) {
        var has = false;
        go.xForeachChild2(
            child => {
                has = child.transform.childCount > 0;
                return !has;
            });
        return has;
    }


    internal static bool HasActiveSibling(this GameObject go) {
        var has = false;
        go.xForeachSibling2(
            sibl => {
                has = sibl.activeSelf;
                return !has;
            });
        return has;
    }

    internal static bool HasActiveParent(this GameObject go) {
        var has = false;
        go.xForeachParent2(
            p => {
                has = p.activeSelf;
                return !has;
            });
        return has;
    }

    internal static bool HasActive(this List<GameObject> list) {
        bool hasActive = false;
        foreach (var go in list) {
            hasActive = go.activeSelf;
            if (hasActive) break;
        }
        return hasActive;
    }

    internal static void SetActive(this List<GameObject> list, bool value, bool deep)
    {
        foreach (var go in list)
        {
            if (deep) go.hSetActiveChildren(value, false);
            if (go.activeSelf != value) go.SetActive(value);
        }
    }

    internal static void SetGOStatic(GameObject go, bool value, bool deep = false, string undoKey = "h@-auto") {
        if (undoKey == "h@-auto") undoKey = value ? "Static" : "UnStatic";

        go.xRecordUndo(undoKey, true);
        go.isStatic = value;

        if (deep) {
            go.xForeachChild(
                child => {
                    child.xRecordUndo(undoKey, true);
                    child.isStatic = value;
                }, true);
        }
    }

    internal static void ToggleStatic(this GameObject go, string undoKey = "h@-auto") {
        SetGOStatic(go, !go.isStatic, false, undoKey);
    }

    internal static void SetStatic(this GameObject go, bool invertMe, bool smartInvert) { //smart mode : auto-deepLock
        var isStatic = go.isStatic;
        var key = isStatic ? "Static" : "UnStatic";

        go.xForeachSelected(
            (item, idx) => SetGOStatic(
                item, (!invertMe || (item == go)) ? !isStatic : isStatic, // invert static 
                idx == -1 && smartInvert == isStatic, // deep-lock if isLock=true
                key));
    }

    internal static void InvertStatic(this GameObject go) {
        go.xForeachSelected((item, idx) => item.ToggleStatic("Invert Static"));
    }

    internal static void ToggleSiblingStatic(this GameObject go, bool deep = false) {
        var isStatic = go.isStatic;
        var key = isStatic ? "Static siblings" : "UnStatic siblings";

        go.ToggleStatic(key);
        go.xForeachSibling(sibl => sibl.ToggleStatic(key));
    }

    internal static void RecursiveStatic(bool value) {
        var key = value ? "Recursive Static" : "Recursive Unstatic";
        TransformX.RootT.ForEach(t => SetGOStatic(t.gameObject, value, true, key));
    }

    


    /*private static Dictionary<Type, int> scriptCountCache;

    internal static bool xHasMissingScript(this GameObject go) {
        if (go == null) return false; //destroyed

        var list = go.GetComponents<MonoBehaviour>();
        if (list.Length == 0) return false;

        foreach (MonoBehaviour t in list) {
            if (t == null) return true;
        }

        return false;
    }

    internal static bool xHasScript(this GameObject go) {
        if (go == null) return false; //destroyed

        var list = go.GetComponents<MonoBehaviour>();
        if (list.Length == 0) return false;

        if (scriptCountCache == null) scriptCountCache = new Dictionary<Type, int>();
        var paths = h2Settings.ignoreScriptPaths;

        foreach (MonoBehaviour t in list) {
            if (t == null) return false;

            var typeT = t.GetType();
            if (scriptCountCache.ContainsKey(typeT)) return scriptCountCache[typeT] == 1;

            var mono = MonoScript.FromMonoBehaviour(t);
            var monoPath = AssetDatabase.GetAssetPath(mono);

            for (var j = 0; j < paths.Length; j++) {
                if (monoPath.Contains(paths[j])) {
                    scriptCountCache.Add(typeT, 0); //Debug.Log("Ignoring ... " + monoPath);
                    return false;
                }
            }

            scriptCountCache.Add(typeT, 1);
            return true;
        }

        return false;
    }


    internal static int numScript(this GameObject go) {
        if (go == null) return 0; //destroyed

        var list = go.GetComponents<MonoBehaviour>();
        if (list.Length == 0) return 0;

        if (scriptCountCache == null) scriptCountCache = new Dictionary<Type, int>();

        var paths = h2Settings.ignoreScriptPaths;
        var cnt = 0;
       for (int i = 0; i < list.Length; i++) {
            if (list[i] == null) continue; //ignore missing
            var typeT = list[i].GetType();

            // From Cache
            if (scriptCountCache.ContainsKey(typeT)) return scriptCountCache[typeT];

            var mono = MonoScript.FromMonoBehaviour(list[i]);
            var monoPath = AssetDatabase.GetAssetPath(mono);

            for (var j = 0; j < paths.Length; j++) {
                if (monoPath.Contains(paths[j])) {
                    list[i] = null;
                    scriptCountCache.Add(typeT, 0); //Debug.Log("Ignoring ... " + monoPath);
                    break;
                }
            }

            if (list[i] != null) {
                cnt++;
                scriptCountCache.Add(typeT, 1);
            }
        }

        return cnt;
    }

    internal static int numScriptMissing(this GameObject go) {
        if (go == null) return 0;
        var list = go.GetComponents<MonoBehaviour>();
        var cnt = 0;
        if (list.Any(item => item == null)) cnt++;
        return cnt;
    }*/

    internal static void AppendChildren(Transform t, ref List<GameObject> list, bool deep = false) {
        if (t == null) return;
        list.Add(t.gameObject);

        for (var i = 0; i < t.childCount; i++) {
            var child = t.GetChild(i);
            list.Add(child.gameObject);
            if (deep && child.childCount > 0) AppendChildren(child, ref list, true);
        }

        /*foreach (Transform child in t) {
            if (child != t) {
                list.Add(child.gameObject);
                if (deep && child.childCount > 0) AppendChildren(child, ref list, true);
            }
        }*/
    }


    

    /*internal static void CreateEmptyChild(GameObject go, bool useEvent = false) {
        //var willPing = go.transform.childCount == 0 || !go.IsExpanded();

        TransformX.xNewTransform(name: "New".GetNewName(go.transform, "Empty"), undo: "NewEmptyChild", p: go.transform);
        //.PingAndUseEvent(willPing, useEvent);

        if (useEvent) Event.current.Use();
    }

    internal static void CreateEmptySibling(GameObject go, bool useEvent = false) {
        TransformX.xNewTransform(
            name: "New".GetNewName(go.transform, "Empty"), undo: "NewEmptySibling", p: go.transform.parent);
        //.PingAndUseEvent(false, useEvent);
        if (useEvent) Event.current.Use();
    }

    internal static void CreateParentAtMyPosition(GameObject go, bool useEvent = false) {
        Selection.activeGameObject = go;
        var goT = go.transform;
        var p = "NewEmpty".GetNewName(goT.parent, "_parent").xNewTransform(undo: "NewParent1", p: goT.parent, pos: goT.localPosition, scl: goT.localScale, rot: goT.localEulerAngles);

        goT.xReparent("NewParent1", p);
        //p.gameObject.RevealChildrenInHierarchy();

        if (useEvent) Event.current.Use();
    }

    internal static void CreateParentAtOrigin(GameObject go, bool useEvent = false) {
        Selection.activeGameObject = go;
        var goT = go.transform;
        var p = TransformX.xNewTransform(
            name: "NewEmpty".GetNewName(goT.parent, "_parent"), undo: "NewParent2", p: goT.parent);

        goT.xReparent("NewParent2", p);
        //p.gameObject.RevealChildrenInHierarchy();
        //p.Ping();
        if (useEvent) Event.current.Use();
    }*/

    public static void RevealChildrenInHierarchy(this GameObject go, bool pingMe = false) {
        if (go.transform.childCount == 0) return;
#if UNITY_4_5 || UNITY_4_6
        var tree = WindowX.Hierarchy.xGetField("m_TreeView");
        //var c = go.transform.childCount > 0 ? go.transform.GetChild(0).gameObject : go;
        //tree.Invoke("RevealNode", null, null, c.GetInstanceID());
        var item = tree.xInvoke("FindNode", null, null, go.GetInstanceID());
        if (item != null) {
            tree.xGetProperty("data")
                .xInvoke("SetExpanded", "UnityEditor.ITreeViewDataSource".xGetTypeByName(), null, item, true);
        }
        //vlbEditorWindow.Hierarchy.Repaint();
#else
            foreach (Transform child in go.transform)
            {
                if (child == go.transform) continue;
                WindowX.Hierarchy.xInvoke("PingTargetObject", null, null, new object[] { child.GetInstanceID() });
                if (pingMe) WindowX.Hierarchy.xInvoke("PingTargetObject", null, null, new object[] { go.GetInstanceID() });
                return;
            }
#endif
    }
}
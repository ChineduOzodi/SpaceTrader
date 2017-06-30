#if UNITY_4_3
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Isolate43 {
    internal static void SetSearchFilter(EditorWindow window, int[] instanceIDs, string title) {
        if (window == null) {
            WindowX.xClearDefinitionCache();
            window = WindowX.Hierarchy;
        }

        var goList = new List<GameObject>();

        for (var i = 0; i < instanceIDs.Length; i++) {
            var go = (GameObject)EditorUtility.InstanceIDToObject(instanceIDs[i]);
            if (go != null) goList.Add(go);
        }

		if (instanceIDs.Length == 0) {
			window.xInvoke("SetSearchFilter", null, null, new object[] {"Hierarchy2tempfixemptysearch", SearchableEditorWindow.SearchMode.All, false});
			window.xSetSearchFilterTerm("iso:" + title);
			return;
		}

	    var TBaseProjectWindow = "UnityEditor.BaseProjectWindow".xGetTypeByName("UnityEditor");
	    var TFilteredHierarchy = "UnityEditor.FilteredHierarchy".xGetTypeByName("UnityEditor");
		window.xSetSearchFilterTerm("iso:" + title);

		var instIDsParams = new object[] { instanceIDs };
    	var fh = window.xGetField("m_FilteredHierarchy", TBaseProjectWindow);
    	var sf = (SearchFilter)fh.xGetField("m_SearchFilter", TFilteredHierarchy);

		sf.ClearSearch();
		sf.referencingInstanceIDs = instanceIDs;
	    fh.xInvoke("SetResults", TFilteredHierarchy, null, instIDsParams);

	    var arr = (object[])fh.xGetProperty("results", TFilteredHierarchy, null);//(FilteredHierarchyType.GetProperty("results").GetValue(fh, null));
		var list = new List<int>();

		//patch
		var nMissing = 0;
		foreach (var t in arr) {
			if (t == null) {
				nMissing++;
				continue;
			}
			var id = (int)t.xGetField("instanceID");
			if (!list.Contains(id)) list.Add(id);
		}

		if (nMissing > 0) Debug.LogWarning("Filtered result may not be correct, missing " + nMissing + " results, please help report it to unity3d@vietlab.net");
		instanceIDs = list.ToArray();

		//reapply
		sf.ClearSearch();
		sf.referencingInstanceIDs = instanceIDs;
        fh.xInvoke("SetResults", TFilteredHierarchy, null, instanceIDs);
		window.Repaint();
    }

    internal static void Context_Isolate(GenericMenu menu, GameObject go, string category = "Isolate/") {
        //if (missingCount > 0) {
            //menu.xAdd(category + "Missing Behaviours [" + missingCount +"] &M", () => Isolate_MissingBehaviours());
        //}

        //menu.xAdd(category + "Has Behaviour &B", () => h2Api.Isolate_ObjectsHasScript());
        if (Selection.instanceIDs.Length > 1) menu.xAdd(category + "Selected Objects &S", () => Isolate_SelectedObjects());
        menu.xAddSep(category);
        menu.xAdd(category + "Locked Objects &L", () => Isolate_LockedObjects());
        menu.xAdd(category + "InActive Objects &I", () => Isolate_InActiveObjects());
        menu.xAdd(category + "Combined Objects &Y", () => Isolate_CombinedObjects());
        menu.xAddSep(category);
    }

    /*internal static void Isolate_MissingBehaviours(bool useEvent = false) {
        WindowX.Hierarchy.SetSearchFilter(
            HierarchyX.GetFilterInstanceIDs(item => item), "Missing");
        if (useEvent) Event.current.Use();
    }

    internal static void Isolate_ObjectsHasScript(bool useEvent = false) {
        WindowX.Hierarchy.SetSearchFilter(HierarchyX.GetFilterInstanceIDs(item => item.numScript() > 0), "Script");
        if (useEvent) Event.current.Use();
    }*/

    internal static void Isolate_SelectedObjects(bool useEvent = false) {
		SetSearchFilter(WindowX.Hierarchy, Selection.instanceIDs, "Selected");
        if (useEvent) Event.current.Use();
    }

    internal static void Isolate_LockedObjects(bool useEvent = false) {
		SetSearchFilter(WindowX.Hierarchy,
            HierarchyX.GetFilterInstanceIDs(item => item.xGetFlag(HideFlags.NotEditable)), "Locked");
        if (useEvent) Event.current.Use();
    }

    internal static void Isolate_InActiveObjects(bool useEvent = false) {
		SetSearchFilter(WindowX.Hierarchy, HierarchyX.GetFilterInstanceIDs(item => !item.activeSelf), "InActive");
        if (useEvent) Event.current.Use();
    }

    internal static void Isolate_CombinedObjects(bool useEvent = false) {
		SetSearchFilter(WindowX.Hierarchy,
            HierarchyX.GetFilterInstanceIDs(item => item.HasFlagChild(HideFlags.HideInHierarchy)), "Combined");
        if (useEvent) Event.current.Use();
    }

    internal static void Isolate_ComponentType(Type t) {
		SetSearchFilter(WindowX.Hierarchy,
            HierarchyX.GetFilterInstanceIDs(item => (item.GetComponent(t) != null)), t.ToString());
    }

    internal static void Isolate_Component(Component c) {
		SetSearchFilter(WindowX.Hierarchy,
            HierarchyX.GetFilterInstanceIDs(item => (item.GetComponent(c.GetType()) != null)), c.xGetTitle(false));
    }

    internal static void Isolate_Layer(string layerName) {
        var layer = LayerMask.NameToLayer(layerName);
		SetSearchFilter(WindowX.Hierarchy, HierarchyX.GetFilterInstanceIDs(item => item.layer == layer), layerName);
    }

    internal static void Isolate_Tag(string tagName) {
		SetSearchFilter(WindowX.Hierarchy, HierarchyX.GetFilterInstanceIDs(item => (item.tag == tagName)), tagName);
    }
}
#endif

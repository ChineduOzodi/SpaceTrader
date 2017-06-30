using System;
using System.Collections.Generic;
using UnityEngine;

public static class TypeX {
    public static Type WindZoneT {
        get { return "UnityEngine.WindZone".xGetTypeByName(); }
    }

    public static Type WindZoneModeT {
        get { return "UnityEngine.WindZoneMode".xGetTypeByName(); }
    }

    public static Type BaseProjectWindowT {
        get { return "UnityEditor.BaseProjectWindow".xGetTypeByName(); }
    }

    public static Type FilteredHierarchyT {
        get { return "UnityEditor.FilteredHierarchy".xGetTypeByName(); }
    }

    public static Type SearchableEditorWindowT {
        get { return "UnityEditor.SearchableEditorWindow".xGetTypeByName(); }
    }

    public static Type SearchFilterT {
        get { return "UnityEditor.SearchFilter".xGetTypeByName(); }
    }

    public static Type TreeViewT {
        get { return "UnityEditor.TreeView".xGetTypeByName(); }
    }

    public static Type TreeViewItemT {
        get { return "UnityEditor.TreeViewItem".xGetTypeByName(); }
    }
    
    public static Type TreeViewUtilityT {
        get { return "UnityEditor.TreeViewUtility".xGetTypeByName(); }
    }

    public static Type ITreeViewDataSourceT {
        get { return "UnityEditor.ITreeViewDataSource".xGetTypeByName();/*"UnityEditor"*/ }
    }

    public static Type HierarchyPropertyT {
        get { return "UnityEditor.HierarchyProperty".xGetTypeByName();/*"UnityEditor"*/ }
    }

    public static Type GameObjectTreeViewItemT {
        get { return "UnityEditor.GameObjectTreeViewItem".xGetTypeByName(); /*"UnityEditor"*/ }
    }
    public static Type GameObjectTreeViewDataSourceT {
        get { return "UnityEditor.GameObjectTreeViewDataSource".xGetTypeByName();/*"UnityEditor"*/ }
    }

    private static Dictionary<string, Type> _typeDict;

    public static Type xGetTypeByName(this string className, bool noWarning = false) {/*string classPackage, */
        if (_typeDict == null) _typeDict = new Dictionary<string, Type>();
        var hasCache = _typeDict.ContainsKey(className);
        var def = hasCache ? _typeDict[className] : null;

        if (hasCache) {
            if (def != null) return def;
            _typeDict.Remove(className);
        }

        var asmList = AppDomain.CurrentDomain.GetAssemblies();
        for (var i = 0; i < asmList.Length; i++) {
            def = asmList[i].GetType(className);
            if (def != null) break; // found !
        }

        if (def != null) _typeDict.Add(className, def);
        else if (!noWarning) Debug.LogWarning(string.Format("Type <{0}> not found", className));

        return def;
    }

    public static bool xAssemblyExist(this string assemblyName) {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var a in assemblies) {
            //Debug.Log(a.GetName().Name);
            if (a.GetName().Name == assemblyName) return true;// || a.FullName.Contains(assemblyName)
        }

        return false;
    }

    /*public static Type xGetEditorTypeByName(this string fullClassName)
    {
        if (_typeDict == null) _typeDict = new Dictionary<string, Type>();
        var hasCache = _typeDict.ContainsKey(fullClassName);
        var def = hasCache ? _typeDict[fullClassName] : null;

        if (hasCache)
        {
            if (def != null) return def;
            _typeDict.Remove(fullClassName);
        }



        def = Types.GetType(className, classPackage);
        if (def != null) _typeDict.Add(className, def);
        else if (!noWarning) Debug.LogWarning(string.Format("Type <{0}> not found in package <{1}>", className, classPackage));

        return def;
    }*/

    public static bool xIsEquals<T>(this T a, T b) {
        return EqualityComparer<T>.Default.Equals(a, b);
    }
}
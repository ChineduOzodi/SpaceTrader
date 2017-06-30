using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class SelectionX {
    internal static void xForeachSelected(this GameObject go, Action<GameObject, int> func) {
        var selected = Selection.objects;
        if (selected.Length == 0 || !selected.ToList()
            .Contains(go) || (selected.Length == 1 && selected.Contains(go))) {
            func(go, -1);
            return;
        }

        var cnt = 0;
        foreach (var item in selected) {
            if (item is GameObject) func((GameObject) item, cnt++);
        }
    }
}
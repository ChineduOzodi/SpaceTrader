using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class ObjectX {
    public static bool xGetFlag(this Object go, HideFlags flag) { return (go != null) && (go.hideFlags & flag) > 0; }

    public static void xSetFlag(this Object go, HideFlags flag, bool value, string undoName = null) {
        if (go == null) return;
        if (!string.IsNullOrEmpty(undoName)) Undo.RecordObject(go, undoName);
        if (value) go.hideFlags |= flag;
        else go.hideFlags &= ~flag;
    }

    public static void xToggleFlag(this Object go, HideFlags flag, string undoName = null) {
        go.xSetFlag(flag, !go.xGetFlag(flag), undoName);
    }

    public static void xSetFlag(this IList<Object> list, HideFlags flag, Func<int, Object, bool> func,
        string undoName = null) {
        for (var i = 0; i < list.Count; i++) {
            if (list[i] != null) list[i].xSetFlag(flag, func(i, list[i]), undoName);
        }
    }
}
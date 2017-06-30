using UnityEditor;
using UnityEngine;

public static class UndoX {
    public static void xRecordUndo(this Object go, string undoKey, bool full = false) {
        if (string.IsNullOrEmpty(undoKey)) return;
        if (full) Undo.RegisterCompleteObjectUndo(go, undoKey);
        else Undo.RecordObject(go, undoKey);
    }
}
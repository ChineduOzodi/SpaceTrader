using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class EditorX {
    public static bool xIsAsset(this Object obj) {
        return obj != null && !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(obj));
    }

    public static void xPing(this Object obj) {
        EditorGUIUtility.PingObject(obj);

        if (obj is MonoBehaviour) EditorGUIUtility.PingObject(MonoScript.FromMonoBehaviour(obj as MonoBehaviour));
        else if (obj is ScriptableObject) EditorGUIUtility.PingObject(MonoScript.FromScriptableObject(obj as ScriptableObject));
    }

    public static bool xGetEditorFlag(this Object obj, HideFlags flag) {
        var editor = obj as Editor;
        return editor != null && editor.target.xGetFlag(flag);
    }

    public static void xSetEditorFlag(this Object obj, HideFlags flag, bool value) {
        var editor = obj as Editor;
        if (editor != null) editor.target.xSetFlag(flag, value);
    }

    public static void xSetEditorEnable(this Object editor, bool isEnable) {
        var editor1 = editor as Editor;
        if (editor1 != null) EditorUtility.SetObjectEnabled(editor1.target, isEnable);
    }

    public static bool xGetEditorEnable(this Object editor) {
        if (editor == null) return false;
        var editor1 = editor as Editor;
        return editor1 != null && EditorUtility.GetObjectEnabled(editor1.target) == 1;
    }

    public static void xToggleEditorEnable(this Object editor) {
        if (editor != null) editor.xSetEditorEnable(!editor.xGetEditorEnable());
    }

    internal static string xGetTitle(this Object obj, bool nicify = true) {
        if (obj == null) return "Null";

        var name = obj is MonoBehaviour
            ? MonoScript.FromMonoBehaviour((MonoBehaviour) obj)
                .name
            : ObjectNames.GetClassName(obj);

        return nicify ? name : ObjectNames.NicifyVariableName(name);
    }

    internal static void xOpenScript(this Object obj) {
        AssetDatabase.OpenAsset(
            MonoScript.FromMonoBehaviour((MonoBehaviour) obj)
                .GetInstanceID());
    }

    internal static void xPingAndUseEvent(this Transform obj, bool ping = true, bool useEvent = true) {
        if (obj == null) return;
        var go = obj.gameObject;

        if (useEvent) Event.current.Use();
        if (!ping) return;

        if (go != null && !go.xGetFlag(HideFlags.HideInHierarchy)) {
            Selection.activeObject = go;
            EditorGUIUtility.PingObject(go);
        } else {
            //Debug.Log("Can not ping a null or hidden target ---> " + go + ":" + go.hideFlags);
        }
    }


    internal static Func<bool> _delayCalls;
    internal static void xDelayCall(Func<bool> act) {
        _delayCalls -= act;
        _delayCalls += act;

        EditorApplication.update -= onDelayCall;
        EditorApplication.update += onDelayCall;
    }

    static void onDelayCall() {
        if (_delayCalls == null) return;
        var callAgain = _delayCalls();
        if (!callAgain) EditorApplication.update -= onDelayCall;
    }
	
	private static long _memory;
	static public void xBeginGC() {
		if (_memory > 0) {
			Debug.LogWarning("Already inside GC checking block, ignoring ...");
			return;
		}
		_memory = GC.GetTotalMemory(false);
	}
	static public void xEndGC() { _memory = 0; }
	static public void xCheckGC(int threshold = 100 * 1024 * 1024) { //30 MB is enough
		if ((GC.GetTotalMemory(false) - _memory) > threshold) {
			Resources.UnloadUnusedAssets();
			GC.Collect(GC.MaxGeneration);
				//small hack to force Unity waits until GC is all collected
			GC.GetTotalMemory(true);
		}
	}
	
	
	
	
}
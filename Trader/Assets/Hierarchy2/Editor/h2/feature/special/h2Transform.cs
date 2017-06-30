using System;
using UnityEditor;
using UnityEngine;


namespace vietlabs {
    internal class h2Transform {
        static Transform tempT;

        static public void Init() {
            h2Shortcut.Add(h2Shortcut.NEW_CHILD,                cmdNewChild);
            h2Shortcut.Add(h2Shortcut.NEW_PARENT,               cmdNewParent);
            h2Shortcut.Add(h2Shortcut.NEW_SIBLING,              cmdNewSibling);

            h2Shortcut.Add(h2Shortcut.RESET_LOCAL_POSITION,     cmdResetPosition);
            h2Shortcut.Add(h2Shortcut.RESET_LOCAL_ROTATION,     cmdResetRotation);
            h2Shortcut.Add(h2Shortcut.RESET_LOCAL_SCALE,        cmdResetScale);
            h2Shortcut.Add(h2Shortcut.RESET_LOCAL_TRANSFORM,    cmdResetTransform);

            h2Shortcut.Add(h2Shortcut.FREEZE_CHILDREN_RESET_LOCAL_POSITION,  cmdResetPosition);
            h2Shortcut.Add(h2Shortcut.FREEZE_CHILDREN_RESET_LOCAL_ROTATION,  cmdResetRotation);
            h2Shortcut.Add(h2Shortcut.FREEZE_CHILDREN_RESET_LOCAL_SCALE,     cmdResetScale);
            h2Shortcut.Add(h2Shortcut.FREEZE_CHILDREN_RESET_LOCAL_TRANSFORM, cmdResetTransform);
        }

        private static void SaveChildren(Transform t) {
            if (tempT == null) tempT = new GameObject().transform;
            tempT.hideFlags = HideFlags.None; //making tempT visible in case of crash, to recover children
            if (tempT.childCount > 0) {
                Debug.LogWarning("Multiple SaveChildren detected ... ignoring");
                return;
            }

            //prevent magics happens when 3rd plugins detects the changes
            tempT.parent = t.parent;
            tempT.gameObject.layer          = t.gameObject.layer;
            tempT.transform.localPosition   = t.localPosition;
            tempT.transform.localScale      = t.localScale;
            tempT.transform.localRotation   = t.localRotation;

            for (var i = t.childCount-1;  i >=0 ; i--) {
                t.GetChild(i).parent = tempT;
            }
        }
        private static void RestoreChildren(Transform t) {
            tempT.hideFlags = HideFlags.HideAndDontSave;
            for (var i = tempT.childCount - 1; i >= 0; i--) {
                tempT.GetChild(i).parent = tempT;
            }
        }


        // Reset 
        internal static void ResetLocalPosition(Transform t, string undo = null, bool saveChildrenGlobalTransform = false) {
            Selection.activeGameObject = t.gameObject;
            if (!string.IsNullOrEmpty(undo)) Undo.RecordObject(t, undo);

            if (saveChildrenGlobalTransform) SaveChildren(t);
            t.localPosition = Vector3.zero;
            if (saveChildrenGlobalTransform) RestoreChildren(t);
        }
        internal static void ResetLocalRotation(Transform t, string undo = null, bool saveChildren = false) {
            Selection.activeGameObject = t.gameObject;
            if (!string.IsNullOrEmpty(undo)) Undo.RecordObject(t, undo);
            if (saveChildren) SaveChildren(t);
            t.localRotation = Quaternion.identity;
            if (saveChildren) RestoreChildren(t);
        }
        internal static void ResetLocalScale(Transform t, string undo = null, bool saveChildren = false) {
            Selection.activeGameObject = t.gameObject;
            if (!string.IsNullOrEmpty(undo)) Undo.RecordObject(t, undo);
            if (saveChildren) SaveChildren(t);
            t.localScale = Vector3.one;
            if (saveChildren) RestoreChildren(t);
        }
        internal static void ResetLocalTransform(Transform t, string undo = null, bool saveChildren = false) {
            Selection.activeGameObject = t.gameObject;
            if (!string.IsNullOrEmpty(undo)) Undo.RecordObject(t, undo);
            if (saveChildren) SaveChildren(t);
            t.localScale    = Vector3.one;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            if (saveChildren) RestoreChildren(t);
        }
        internal static void ResetContext(Transform t, GenericMenu m, string path) {
            m   .xAdd(path + "Reset Local Position",    () => ResetLocalPosition(t))
                .xAdd(path + "Reset Local Rotation",    () => ResetLocalRotation(t))
                .xAdd(path + "Reset Local Scale",       () => ResetLocalScale(t))
                .xAdd(path + "Reset Local Transform",   () => ResetLocalTransform(t));
        }


        // Create New
        internal static void NewChild(Transform t) {
            const string name = "New Child";
            var c = name.xNewTransform(name, t);
            Selection.activeGameObject = c.gameObject;
        }
        internal static void NewSibling(Transform t) {
            const string name = "New Sibling";
            var s = name.xNewTransform(name, t.parent);
            Selection.activeGameObject = s.gameObject;
        }
        /*internal static void NewParent(Transform t) {
            const string name = "New Parent";
            var p = name.xNewTransform(name, t.parent, t.localPosition, t.localScale, t.localRotation);
            t.xReparent(name, p);
            Selection.activeGameObject = p.gameObject;
        }
        internal static void NewParentAtOrigin(Transform t) {
            const string name = "New Parent";
            var p = name.xNewTransform(name, t.parent);
            t.xReparent(name, p);
            Selection.activeGameObject = p.gameObject;
        }*/

        [MenuItem("GameObject/Transform/Reset Position ", false, 0)]
        internal static void cmdResetPosition() {
            foreach(var go in h2Info.SelectedGameObjects) {
                ResetLocalPosition(go.transform, "Reset Position");
            }
        }
        [MenuItem("GameObject/Transform/Reset Rotation ", false, 0)]
        internal static void cmdResetRotation() {
            foreach(var go in h2Info.SelectedGameObjects) {
                ResetLocalRotation(go.transform, "Reset Rotation");
            }
        }
        [MenuItem("GameObject/Transform/Reset Scale ", false, 0)]
        internal static void cmdResetScale() {
            foreach(var go in h2Info.SelectedGameObjects) {
                ResetLocalScale(go.transform, "Reset Scale");
            }
        }
        [MenuItem("GameObject/Transform/Reset Transform ", false, 0)]
        internal static void cmdResetTransform() {
            foreach (var go in h2Info.SelectedGameObjects) {
                ResetLocalTransform(go.transform, "Reset Transform");
            }
        }

        [MenuItem("GameObject/Transform/New Child ", false, 0)]
        internal static void cmdNewChild() {
            if (Selection.activeGameObject == null) return;
            NewChild(Selection.activeTransform);
        }

        [MenuItem("GameObject/Transform/New Parent ", false, 0)]
        internal static void cmdNewParent() {
            if (Selection.activeGameObject == null) return;
            //NewParent(Selection.activeTransform);

            const string name = "New Parent";
            var t = Selection.activeTransform;
            var p = name.xNewTransform(name, t.parent, t.localPosition, t.localScale, t.localRotation);

            for (var i = 0; i < h2Info.SelectionCount; i++) {
                var go = h2Info.SelectedGameObjects[i];
                if (go == null) continue;

                go.transform.xReparent(name, p);
            }

            Selection.activeGameObject = p.gameObject;
        }

        [MenuItem("GameObject/Transform/New Sibling ", false, 0)]
        internal static void cmdNewSibling() {
            if (Selection.activeGameObject == null) return;
            NewSibling(Selection.activeTransform);
        }


        // Attach
        // Plane / Cube / 2D UI / Collider / RigidBody



        // Remove Collider / RigidBody


    }

    static public class h2TransformUtils {
        public static Transform xNewTransform(this string name, string undo, Transform p, Vector3? pos = null,
            Vector3? scl = null, Quaternion? rot = null) {
            var go = new GameObject { name = name };
            
            if (!string.IsNullOrEmpty(undo)) Undo.RegisterCreatedObjectUndo(go, undo);
            go.transform.xReparent(undo, p);
            go.transform.xSetLocalTransform(undo, pos ?? Vector3.zero, scl ?? Vector3.one, rot ?? Quaternion.identity);

#if UNITY_4_6 || UNITY_5
            //support add RectTransform if it's being children of a Canvas
            if (p != null) {
                var rectT = p.GetComponentInParent<Canvas>();
                if (rectT != null) {
                    go.AddComponent<RectTransform>();
                }    
            }
#endif
            return go.transform;
        }

        public static GameObject xNewPrimity(this PrimitiveType type, string name, string undo, Transform p,
            Vector3? pos = null, Vector3? scl = null, Quaternion? rot = null) {
            GameObject primity = GameObject.CreatePrimitive(type);
            if (!string.IsNullOrEmpty(undo)) Undo.RegisterCreatedObjectUndo(primity, undo);
            primity.transform.xReparent(undo, p);
            primity.name = name;
            primity.transform.xSetLocalTransform(undo, pos ?? Vector3.zero, scl ?? Vector3.one, rot ?? Quaternion.identity);
            return primity;
        }

        public static void xReparent(this Transform t, string undo, Transform parent) {
        if (t == null || t == parent || t.parent == parent) return;
            t.gameObject.layer = parent.gameObject.layer;
            if (!string.IsNullOrEmpty(undo)) Undo.SetTransformParent(t.transform, parent, undo);
        }

        public static void xSetLocalTransform(this Transform t, string undo, Vector3? pos = null, Vector3? scl = null,
            Quaternion? rot = null) {
            if (!string.IsNullOrEmpty(undo)) Undo.RecordObject(t, undo);
            if (scl != null) t.localScale       = scl.Value;
            if (rot != null) t.localRotation    = rot.Value;
            if (pos != null) t.localPosition    = pos.Value;
        }

        internal static string GetNewName(this string baseName, Transform p, string suffix = "") {
            var name = baseName.Contains(suffix) ? baseName : (baseName + suffix);
            if (p == null) return name;
            var namesList = new string[p.childCount];
            for (var i = 0; i < namesList.Length; i++) {
                namesList[i] = p.GetChild(i).name;
            }

            if (Array.IndexOf(namesList, name) == -1) return name;
            var counter = 1;
            while (Array.IndexOf(namesList, name + counter) == -1) counter++;
            return name + counter;
        }
    }
}



using UnityEditor;
using UnityEngine;

namespace vietlabs {
    public class h2Lock : h2Icon {
        // UNITY's Bug : Register complete object undo -> crash !
        internal static bool Patch_Undo = true;

        internal h2Lock() {
            texList = new[] {
                EditorResource.GetTexture2D("lock_dis"),
                EditorResource.GetTexture2D("lock")
            };

            texColor = new[] {
                h2Color.Get(h2ColorType.Unlock),
                h2Color.Get(h2ColorType.Lock)
            };
        }

        protected override string getUndoName(bool set, h2IGroup group = h2IGroup.Target, h2IValue value = h2IValue.Same) {
            var str = set ? "Lock " : "Unlock ";
            if (group == h2IGroup.Target) return str + target.name;

            var g = group == h2IGroup.Selection ? "Selection" : "Siblings";

            if (value != h2IValue.InvertTarget) return "Toggle " + str + g;
            return set ? "Unlock " : "Lock " + g; 
        }
        protected override GenericMenu GetMenu(GameObject go)
        {
            var menu = new GenericMenu();
            menu.xAdd("Deep Lock children", () => go.hSetLock(true, true, "Deep lock children"));
            menu.xAdd("Deep Unlock children", () => go.hSetLock(false, true, "Deep unlock children"));
            return menu;
        }
        protected override bool autoSetChildren { get { return true; }}
        protected override bool Get(GameObject go) { return go.xGetFlag(HideFlags.NotEditable); }
        protected override void Set(GameObject go, bool value, string undoName) {
            if (!Patch_Undo) Undo.RegisterCompleteObjectUndo(go, undoName);
            go.xSetFlag(HideFlags.NotEditable, value);
            WindowX.Inspector.Repaint();
        }
    
        private static h2Lock _api;
        static internal void Draw(h2Info info, Rect r, GameObject go) {
            (_api ?? (_api = new h2Lock())).DrawTarget(r, go);
        }

    }

    static public class h2LockUtils {
        internal static void hSetLock(this GameObject go, bool value, bool deep = false, string undoKey = "h@-auto")
        {
            if (undoKey == "h@-auto") undoKey = value ? "Lock" : "UnLock";
            if (!h2Lock.Patch_Undo) go.xRecordUndo(undoKey, true);
            go.xSetFlag(HideFlags.NotEditable, value);

            foreach (var c in go.GetComponents<Component>())
            {
                if (!(c is Transform))
                {
                    c.xSetFlag(HideFlags.NotEditable, value);
                    c.xSetFlag(HideFlags.HideInHierarchy, value);
                }
            }

            if (deep)
            {
                go.xForeachChild(
                    child => {
                        if (!h2Lock.Patch_Undo) child.xRecordUndo(undoKey, true);
                        child.xSetFlag(HideFlags.NotEditable, value);

                        foreach (var c in child.GetComponents<Component>()) {
                            if (!(c is Transform)) {
                                c.xSetFlag(HideFlags.NotEditable, value);
                                c.xSetFlag(HideFlags.HideInHierarchy, value);
                            }
                        }
                    }, true);
            }
        }

        internal static void hToggleLock(this GameObject go, string undoKey = "h@-auto") {
            go.hSetLock(!go.xGetFlag(HideFlags.NotEditable), false, undoKey);
        }

        /*static internal void SetNaiveLock(this GameObject go, bool value, bool deep, bool invertMe) {
            var isLock = go.GetFlag(HideFlags.NotEditable);
            go.ForeachSelected((item, idx) => SetLock(item,
                (!invertMe || (item == go)) ? !isLock : isLock, deep)
            );
        }*/

        /*    internal static void hRemoveMissingBehaviour(this GameObject go) {
                Debug.Log("hRemoveMissing ... " + go);

                var cList = go.GetComponents<Component>();
                for (var i = 0; i < cList.Length; i++) {
                    if (cList[i] != null) continue;

                    Debug.Log(i + ":" + cList[i]);
                    var editor = Editor.CreateEditor(cList[i], typeof(NullEditor));
                    Debug.LogWarning("missing found " + cList[i] +":"+ editor);
                }
            }*/

        internal static void hSetSmartLock(this GameObject go, bool invertMe, bool smartInvert)
        { //smart mode : auto-deepLock
            var isLock = go.xGetFlag(HideFlags.NotEditable);
            var key = isLock ? "Lock" : "Unlock";

            //Debug.Log("hSetSmartLock ... " + go);
            go.xForeachSelected(
                (item, idx) => item.hSetLock(
                    (!invertMe || (item == go)) ? !isLock : isLock, // invert lock 
                    idx == -1 && smartInvert == isLock, // deep-lock if isLock=true
                    key));
        }

        internal static void hInvertLock(this GameObject go)
        {
            go.xForeachSelected((item, idx) => item.hToggleLock("Invert Lock"));
        }

        internal static void hToggleSiblingLock(this GameObject go, bool deep = false)
        {
            var isLock = go.xGetFlag(HideFlags.NotEditable);
            var key = isLock ? "Lock siblings" : "Unlock siblings";

            go.hToggleLock(key);
            go.xForeachSibling(sibl => sibl.hToggleLock(key));
        }

        internal static void hRecursiveLock(bool value)
        {
            var key = value ? "Recursive Lock" : "Recursive Unlock";
            TransformX.RootT.ForEach(rootGO => rootGO.gameObject.hSetLock(value, true, key));
        }
    }
}



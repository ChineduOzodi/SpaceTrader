using UnityEngine;

namespace vietlabs {
    public class h2Combine : h2Icon/*RectGUIToggle<GameObject>*/
    {
        public int maxChildCount;

        protected override string getUndoName(bool set, h2IGroup group = h2IGroup.Target, h2IValue value = h2IValue.Same) {
            var str = set ? "Combine " : "Expand ";
            if (group == h2IGroup.Target) {//single
                return str + target.name;
            }

            var g = group == h2IGroup.Siblings ? "Siblings" : group == h2IGroup.Selection ? "Selection" : "";

            if (value != h2IValue.InvertTarget) return "Toggle " + str + g;
            return set ? "Expand " : "Combine " + g;
        }
        protected override void Set(GameObject go, bool value, string undoName) {
            go.xForeachChild(
                child => {
                    //if (undoName != null) child.xRecordUndo(undoName, true);
                    child.xSetFlag(HideFlags.HideInHierarchy, value);
                }
           );

    #if UNITY_4_5 || UNITY_4_6 || UNITY_5
            //workaround for Hierarchy not update
            var old = go.activeSelf;
            go.SetActive(!old);
            go.SetActive(old);
    #endif
        }
        protected override bool Get(GameObject go) {
            return go != null && go.HasFlagChild(HideFlags.HideInHierarchy);
        }

        private static h2Combine _api;
        internal static h2Combine Api { get { return _api ?? (_api = new h2Combine()); } }

        static internal void Draw(h2Info info, Rect r, GameObject go) {
            var tInfo = info.Transform;
            if (tInfo.childCount == 0) return;

            Api.target = info.go;
            var value = _api.Get(info.go);

            if (r.Contains(Event.current.mousePosition)) _api.ReadModifier().ReadMouse().Check();
            r.xMiniButton(info.Transform.lbCombine, false, 1f, value);
            if (_api.maxChildCount < tInfo.childCount) _api.maxChildCount = tInfo.childCount;
        }

        static internal float MaxWidth {
            get {
                return Api.maxChildCount < 100 ? 20f : _api.maxChildCount < 1000 ? 28f : 36f;
            }
        }
    }

    static public class h2CombineUtils {
        internal static bool xIsCombined(this GameObject go) { return go.HasFlagChild(HideFlags.HideInHierarchy); }
        internal static void hSetCombine(this GameObject go, bool value, bool deep = false, string undoKey = "h@-auto")
        {
            if (undoKey == "h@-auto") undoKey = value ? "Combine GameObject" : "Expand GameObject";
            go.xForeachChild(
                child => {
                //Undo.RegisterCompleteObjectUndo(child, undoKey);
                child.xRecordUndo(undoKey, true);
                    child.xSetFlag(HideFlags.HideInHierarchy, value);
                }, deep);

#if UNITY_4_5 || UNITY_4_6
            //workaround for Hierarchy not update
            var old = go.activeSelf;
            go.SetActive(!old);
            go.SetActive(old);
#endif
        }
        internal static void hToggleCombine(this GameObject go, bool deep = false)
        {
            var isCombined = go.xIsCombined();
            var key = isCombined ? "Combine Selected GameObjects" : "Expand Selected GameObjects";
            go.xForeachSelected((item, index) => item.hSetCombine(!isCombined, deep, key));
            /*if (isCombined && go.transform.childCount > 0) {
                go.transform.GetChild(0)
                    .xPing();
            }*/
        }
        internal static void hToggleCombineChildren(this GameObject go)
        {
            var val = false;

            go.xForeachChild2(
                child => {
                    val = child.xIsCombined();
                    return !val;
                });

            var key = val ? "Expand Children" : "Combine Children";
            go.hSetCombine(false, false, key);
            go.xForeachChild(child => child.hSetCombine(!val, false, key));
        }
        internal static void hSetCombineSibling(this GameObject go, bool value)
        {
            var key = value ? "Expand Siblings" : "Combine siblings";

            go.hSetCombine(value, false, key);
            go.xForeachSibling(sibl => sibl.hSetCombine(!value, false, key));
            if (!value) go.RevealChildrenInHierarchy(true);
        }
        internal static void hRecursiveCombine(bool value)
        {
            var key = value ? "Recursive Combine" : "Recursive Expand";
            TransformX.RootT.ForEach(
                root => {
                    var list = root.xGetChildren<Transform>(true);
                    foreach (var child in list)
                    {
                        child.xRecordUndo(key, true);
                    }
                    root.gameObject.SetDeepFlag(HideFlags.HideInHierarchy, value);
                });
        }


    }
}

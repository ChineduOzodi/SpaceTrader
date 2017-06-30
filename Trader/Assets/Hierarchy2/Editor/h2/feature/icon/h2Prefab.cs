using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace vietlabs {
    /*internal class h2PrefabInfo {
        public GameObject root;
        public List<int> usage;
        public PrefabType type;
        public string label;
    }*/


    internal class h2Prefab {
        private static Texture2D texPrefab;
        private static Texture2D texModel;
        private static Texture2D texMissing;
        private static Texture2D texDisconnect;

        internal static void Init() {
            h2Shortcut.Add(h2Shortcut.BREAK_PREFAB, cmdBreakPrefab);
        }

        static public void cmdBreakPrefab() {
            if (Selection.activeGameObject == null) return;
            Selection.activeGameObject.xBreakPrefab();
        }

        static Texture2D getTexture(PrefabType p) {
            return  p == PrefabType.PrefabInstance          ? texPrefab  ?? (texPrefab  = ColorHSL.blue.dS(-0.7f).xProSkinAdjust().xGetTexture2D()) :
                    p == PrefabType.ModelPrefabInstance     ? texModel   ?? (texModel   = ColorHSL.cyan.dS(-0.7f).xProSkinAdjust().xGetTexture2D()) :
                    p == PrefabType.MissingPrefabInstance   ? texMissing ?? (texMissing = ColorHSL.red.dS(-0.7f).xProSkinAdjust().xGetTexture2D())  :
                    texDisconnect ?? (texDisconnect = ColorHSL.gray.dS(-0.7f).xProSkinAdjust().xGetTexture2D());
        }

        public static void checkPrefab(h2Info info) {
            if (info.prefabTimeStamp == h2Info.timeStamp) return;
            info.prefabTimeStamp = h2Info.timeStamp;

            var go = info.go;
            var prefabType = PrefabUtility.GetPrefabType(go);
            if (prefabType == PrefabType.None) return;

            var Prefab = info.Prefab;
            if (Prefab != null) { //clear old prefab info
                var arr = Prefab.childrenInstIDs;

                for (var i = 0; i < arr.Count; i++)
                {
                    var info1 = h2Info.Get(arr[i]);
                    if (info1 != null) {
                        info1.Prefab = null;
                    }
                }
            }

            var rootGO = PrefabUtility.FindRootGameObjectWithSameParentPrefab(go);
            if (rootGO != go)
            {
                var rootInfo = h2Info.Get(rootGO.GetInstanceID(), true);
                if (rootInfo.Prefab == null) checkPrefab(rootInfo);
                Prefab = rootInfo.Prefab;
                if (Prefab.childrenInstIDs.Contains(info.instID)) {
                    Debug.LogWarning("Something went wrong ... a prefab being check-in twice into its Root");
                }
                Prefab.childrenInstIDs.Add(info.instID);
            }
            else
            {
                var p = (GameObject)PrefabUtility.GetPrefabParent(rootGO);
                info.Prefab = new h2iPrefab
                {
                    type = prefabType,
                    prefab = p,
                    label = p.name,
                    rootInstID = info.instID,
                    childrenInstIDs = new List<int> { info.instID }
                };
            }
        }

        internal static void Draw(h2Info info, Rect r, GameObject go) {
            checkPrefab(info);

            if (info.Prefab == null) return;

            using (GuiX.DisableGroup(info.Prefab.type == PrefabType.MissingPrefabInstance)) {
                if (r.xMiniTag(info.Prefab.label, getTexture(info.Prefab.type))) {
                    go.xSelectPrefab();   
                }
            }
        }
    }
    internal static class h2PrefabUtils {
        internal static void xBreakPrefab(this GameObject go, string tempName = "h2_dummy.prefab") {
            var go2 = PrefabUtility.FindRootGameObjectWithSameParentPrefab(go);

            PrefabUtility.DisconnectPrefabInstance(go2);
            var prefab = PrefabUtility.CreateEmptyPrefab("Assets/" + tempName);
            PrefabUtility.ReplacePrefab(go2, prefab, ReplacePrefabOptions.ConnectToPrefab);
            PrefabUtility.DisconnectPrefabInstance(go2);
            AssetDatabase.DeleteAsset("Assets/" + tempName);

            //temp fix to hide Inspector's dirty looks
            Selection.instanceIDs = new int[] { };
        }

        internal static void xSelectPrefab(this GameObject go) {
            var prefab = PrefabUtility.GetPrefabParent(PrefabUtility.FindRootGameObjectWithSameParentPrefab(go));
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab.GetInstanceID());
        }
    }

}
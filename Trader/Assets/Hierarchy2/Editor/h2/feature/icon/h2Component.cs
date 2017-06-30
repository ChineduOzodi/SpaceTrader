using UnityEditor;
using UnityEngine;

namespace vietlabs {
    public class h2Component {
        public static int MaxWidth;

        static bool showComponents;
        static bool showPhysics;
        static bool showRender;
        static bool showUnusual;

        static public void Init() {
            h2Shortcut.Add(h2Shortcut.TOGGLE_COMPONENT,         cmdComponent);
            h2Shortcut.Add(h2Shortcut.TOGGLE_COMPONENT_PHYSICS, cmdPhysics);
            h2Shortcut.Add(h2Shortcut.TOGGLE_COMPONENT_RENDER,  cmdRender);
            h2Shortcut.Add(h2Shortcut.TOGGLE_COMPONENT_UNUSUAL, cmdUnusual);
        }

        static void cmdComponent() {
            MaxWidth = 0;
            
            showComponents = !showComponents;
            WindowX.Hierarchy.Repaint();
        }

        static void cmdPhysics() {
            MaxWidth = 0;
            showComponents = true;
            showPhysics = !showPhysics;
            WindowX.Hierarchy.Repaint();
        }

        static void cmdRender() {
            MaxWidth = 0;
            showComponents = true;
            showRender = !showRender;
            WindowX.Hierarchy.Repaint();
        }

        static void cmdUnusual() {
            MaxWidth = 0;
            showComponents = true;
            showUnusual = !showUnusual;
            WindowX.Hierarchy.Repaint();
        }

        internal static void Draw(h2Info info, Rect r, GameObject go) {
            if (!showComponents) return;
            //if ((info.detail & h2iDetail.Component) == 0) info.ReadComponentInfo();

            var arr = info.Component.components;
            var w = 0;
            var r0 = r.dl(r.width - 16f);

            for (var i = 0;i < arr.Count; i++) {
                var c = arr[i];

                if (c == null) continue;
                if (!showUnusual && (c is Transform || c is MeshFilter
#if UNITY_4_6 || UNITY_5
                    || c is CanvasRenderer
#endif                                   
                )) continue;
                if (!showRender && (c is Renderer || c is MeshFilter
#if UNITY_4_6 || UNITY_5
                    || c is CanvasRenderer
#endif
                )) continue;
                if (!showPhysics && (c is Collider || c is Rigidbody || c is Rigidbody2D || c is Collider2D)) continue;

                var icon        = AssetPreview.GetMiniThumbnail(c);
                var hasProp     = arr[i].xHasProperty("enabled");
                var willClear   = hasProp && !(bool)arr[i].xGetProperty("enabled");

                using (GuiX.GUIColor(willClear ? Color.clear.xAlpha(0.2f) : Color.white)) {// : 
                    GUI.DrawTexture(r0, icon);

                    if (Selection.activeGameObject == go && hasProp && r0.xLMB_isDown().noModifier) {
                        Undo.RecordObject(c, "Toggle enable");
                        c.xSetProperty("enabled", willClear);
                        WindowX.Inspector.Repaint();
                    }
                }

                r0 = r0.dx(-16f);
                w += 16;
            }

            if (MaxWidth < w) MaxWidth = w;
        }
    }
}

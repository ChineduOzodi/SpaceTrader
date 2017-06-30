#region
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#endregion

namespace vietlabs {
    public class h2SceneViewHL {
        //private static bool _isFocus;
        private static int _hoveredInstance;
        private static readonly Color HoverColor = new Color(0f, 1f, 1f, 0.75f);
        private static readonly Color DragColor = new Color(1f, 1f, 0, 0.75f);
        private static GameObject cacheTarget;
        private static Bounds? selfBound;
        private static Bounds? fullBound;
        private static void OnSceneGUIDelegate(SceneView sceneView) {
            switch (Event.current.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                case EventType.DragExited:
                    sceneView.Repaint();
                    break;
            }

            if (Event.current.type == EventType.repaint) {
                var drawnInstanceIDs = new HashSet<int>();

                Color handleColor = Handles.color;

                Handles.color = DragColor;
                foreach (var objectReference in DragAndDrop.objectReferences) {
                    var gameObject = objectReference as GameObject;

                    if (gameObject && gameObject.activeInHierarchy) {
                        DrawObjectBounds(gameObject);

                        drawnInstanceIDs.Add(gameObject.GetInstanceID());
                    }
                }

                Handles.color = HoverColor;
                if (_hoveredInstance != 0 && !drawnInstanceIDs.Contains(_hoveredInstance)) {
                    GameObject sceneGameObject = EditorUtility.InstanceIDToObject(_hoveredInstance) as GameObject;

                    if (sceneGameObject) {
                        DrawObjectBounds(sceneGameObject);
                    }
                }

                Handles.color = handleColor;
            }
        }

        static Bounds? Renderer2Bound(Renderer[] renderers) {
            if (renderers.Length > 0) {
                var b = renderers[0].bounds;
                for (var i = 1; i < renderers.Length; i++) {
                    b.Encapsulate(renderers[i].bounds);
                }
                return b;
            }

            return null;
        }
#if UNITY_4_6 || UNITY_5
        static Bounds? RectTransform2Bound(RectTransform[] rectTrans) {
            if (rectTrans.Length > 0) {
                var min     = new Vector3();
                var max     = new Vector3();
                var corners = new Vector3[4];

                for (var i = 0; i < rectTrans.Length; i++) {
                    rectTrans[i].GetWorldCorners(corners);

                    if (i == 0) {
                        min = corners[0];
                        max = corners[0];
                    }
                    
                    for (var j = 0; j < corners.Length; j++) {
                        var v = corners[j];

                        min.x = Mathf.Min(v.x, min.x);
                        min.y = Mathf.Min(v.y, min.y);
                        min.z = Mathf.Min(v.z, min.z);

                        max.x = Mathf.Max(v.x, max.x);
                        max.y = Mathf.Max(v.y, max.y);
                        max.z = Mathf.Max(v.z, max.z);
                    }
                }
                
                return new Bounds( 0.5f * (min + max), max-min);
            }

            return null;
        }
#endif

        private static void DrawObjectBounds(GameObject go) {
            if (cacheTarget != go) {
                cacheTarget = go;
                if (cacheTarget != null) {
#if UNITY_4_6 || UNITY_5
                    var rectTrans = cacheTarget.GetComponent<RectTransform>();

                    if (rectTrans != null) {
                        selfBound = RectTransform2Bound(go.GetComponents<RectTransform>());
                        fullBound = RectTransform2Bound(go.GetComponentsInChildren<RectTransform>());
                    } else

#endif
                    {
                        selfBound = Renderer2Bound(go.GetComponents<Renderer>());
                        fullBound = Renderer2Bound(go.GetComponentsInChildren<Renderer>());
                    }
                }
                else {
                    selfBound = null;
                }
            }


            DrawBox(selfBound, null, Color.cyan);
            DrawBox(fullBound, go.transform, Color.yellow);
        }
        private static void DrawBox(Bounds? bound, Transform t, Color c) {
            var oColor = Handles.color;
            Handles.color = Color.cyan.xAlpha(0.5f);

            if (bound != null) {
                var b = bound.Value;
                var v0 = b.min;
                var v7 = b.max;

                var v1 = new Vector3(v0.x, v0.y, v7.z);
                var v2 = new Vector3(v0.x, v7.y, v7.z);
                var v3 = new Vector3(v0.x, v7.y, v0.z);

                var v4 = new Vector3(v7.x, v0.y, v7.z);
                var v5 = new Vector3(v7.x, v0.y, v0.z);
                var v6 = new Vector3(v7.x, v7.y, v0.z);


                Handles.DrawPolyLine(
                    v0, v1, v2, v3, v0,
                    v5, v6, v7, v4, v5
                    );

                Handles.DrawLine(v1, v4);
                Handles.DrawLine(v2, v7);
                Handles.DrawLine(v3, v6);
            }

            if (t != null) {
                //Handles.ArrowCap(0, t.position, t.rotation, 1f);
                if (bound != null) {
                    Handles.DrawLine(t.position, bound.Value.center);
                }

                var sz = HandleUtility.GetHandleSize(t.position);
                var a = 0.5f;

                Handles.color = Color.green.xAlpha(a);
                Handles.ArrowCap(0,
                    t.position,
                    Quaternion.LookRotation(t.up),
                    sz);
                Handles.color = Color.blue.xAlpha(a);
                Handles.ArrowCap(0,
                    t.position,
                    Quaternion.LookRotation(t.forward),
                    sz);
                Handles.color = Color.red.xAlpha(a);
                Handles.ArrowCap(0,
                    t.position,
                    Quaternion.LookRotation(t.right),
                    sz);
            }

            Handles.color = oColor;
        }
        internal static void Draw(int instID, Rect r) {
            //Debug.Log(Event.current + ":" + WindowX.Hierarchy.wantsMouseMove);

            if (Event.current.type == EventType.Layout) {
                //_isFocus = EditorWindow.focusedWindow == WindowX.Hierarchy;
                return;
            }

            if (_hoveredInstance == instID) { //Draw an indicator
                GUI.DrawTexture(r.l(0), Color.green.xAlpha(0.1f).xGetTexture2D());
            }

            /*if (!_isFocus) {
                return;
            }*/

            var current = Event.current;
            switch (Event.current.type) {
                case EventType.MouseMove:
                    var mouse = current.mousePosition;
                    var inside = r.yMin < mouse.y && mouse.y < r.yMax;
                    if (inside) {
                        _hoveredInstance = instID;
                        WindowX.Hierarchy.Repaint();
                    }
                    else if (_hoveredInstance == instID) {
                        _hoveredInstance = 0;
                    }

                    var sv = SceneView.lastActiveSceneView;
                    if (sv) {
                        sv.Repaint();
                    }

                    break;

                case EventType.MouseDrag:
                case EventType.DragUpdated:
                case EventType.DragPerform:
                case EventType.DragExited:
                    sv = SceneView.lastActiveSceneView;
                    if (sv) {
                        sv.Repaint();
                    }
                    break;
            }


            //Debug.Log("Focus " + _hoveredInstance + ":" + Event.current.type + ":" + r);
        }
        public static void StartMonitor() {
            SceneView.onSceneGUIDelegate -= OnSceneGUIDelegate;
            SceneView.onSceneGUIDelegate += OnSceneGUIDelegate;

            EditorApplication.hierarchyWindowItemOnGUI -= Draw;
            EditorApplication.hierarchyWindowItemOnGUI += Draw;
            WindowX.Hierarchy.wantsMouseMove = true;
            h2Settings.useSceneViewHL = true;
        }
        public static void StopMonitor() {
            SceneView.onSceneGUIDelegate -= OnSceneGUIDelegate;
            EditorApplication.hierarchyWindowItemOnGUI -= Draw;
            WindowX.Hierarchy.wantsMouseMove = false;
            h2Settings.useSceneViewHL = false;
        }
        public static void Init() {
            h2Shortcut.Add(h2Shortcut.TOGGLE_SCENEVIEW_HIGHLIGHT, cmdToggleSVHL);
        }
        internal static void cmdToggleSVHL() {
            if (h2Settings.useSceneViewHL || !h2Settings.enable) {
                StopMonitor();
            } else {
                StartMonitor();
            }
        }
        internal static void AttachMenu(GenericMenu m, string prefix) {
            var isHL = h2Settings.useSceneViewHL;
            m.xAdd("Highlight SceneView", isHL ? StopMonitor : (Action) StartMonitor, isHL);
        }
    }
}
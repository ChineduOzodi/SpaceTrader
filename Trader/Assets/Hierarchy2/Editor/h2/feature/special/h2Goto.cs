using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace vietlabs {
    internal class h2Goto {
        static public void Init() {
            h2Shortcut.Add(h2Shortcut.GOTO_PARENT,  cmdPingParent);
            h2Shortcut.Add(h2Shortcut.GOTO_CHILD,   cmdPingChild);
            h2Shortcut.Add(h2Shortcut.GOTO_SIBLING, cmdNextSibling);
        }

        private static List<Transform> _pingList;
        internal static void hPingParent(Transform t, bool useEvent = false) {
            var p = t.parent;
            if (p == null) return;

            //clear history when select other GO
            if (_pingList == null || (_pingList.Count > 0 && _pingList[_pingList.Count - 1].parent != t)) _pingList = new List<Transform>();

            _pingList.Add(t);
            p.xPingAndUseEvent(true, useEvent);
        }
        internal static void hPingChild(Transform t, bool useEvent = false) {
            Transform pingT = null;

            if (_pingList == null) _pingList = new List<Transform>();

            if (_pingList.Count > 0) {
                var idx = _pingList.Count - 1;
                var c = _pingList[idx];
                _pingList.Remove(c);

                pingT = c;
            } else if (t.childCount > 0) pingT = t.GetChild(0);

            if (pingT != null) pingT.xPingAndUseEvent(true, useEvent);
        }
        internal static void hPingSibling(Transform t, bool useEvent = false) {
            NextSibling(t).xPingAndUseEvent(true, useEvent);
        }

        internal static Transform NextSibling(Transform t) {
            if (t == null) {
                Debug.LogWarning("Transform should not be null ");
                return null;
            }

            var p = t.parent;
            if (t.parent != null) {
                var cnt = 0;
                while (p.GetChild(cnt) != t) cnt++;
                return (cnt < p.childCount - 1) ? p.GetChild(cnt + 1) : p.GetChild(0);
            }

            var rootList = TransformX.RootT;
            var idx = rootList.IndexOf(t);
            if (idx != -1) return rootList[(idx < rootList.Count - 1) ? idx + 1 : 0].transform;
            Debug.LogWarning("Root Object not in RootList " + t + ":" + rootList);
            return t;
        }

        internal static void cmdPingParent() {
            if (Selection.activeGameObject == null) return;
            hPingParent(Selection.activeTransform);
        }
        internal static void cmdPingChild() {
            if (Selection.activeGameObject == null) return;
            hPingChild(Selection.activeTransform);
        }
        internal static void cmdNextSibling() {
            if (Selection.activeGameObject == null) return;
            hPingSibling(Selection.activeTransform);
        }
    }
}


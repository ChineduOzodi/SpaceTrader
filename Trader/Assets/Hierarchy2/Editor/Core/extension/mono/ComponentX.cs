using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

public static class ComponentX {
    public static List<GameObject> xToGOList(IList listComponent) {
        var result = new List<GameObject>();
        for (var i = 0; i < listComponent.Count; i++) {
            var c = listComponent[i];
            result.Add(c == null ? null : (c is Component) ? (c as Component).gameObject : null);
        }
        return result;
    }

    public static List<T> xToComponentList<T>(IList goList, bool removeNull = false) where T : Component {
        var result = new List<T>();
        for (var i = 0; i < goList.Count; i++) {
            var go = (GameObject) goList[i];
            if (go == null) {
                if (!removeNull) result.Add(null);
                continue;
            }

            var c = (T) go.GetComponent(typeof (T));
            if (!removeNull) result.Add(c);
        }
        return result;
    }

    public static void xMove(this Component c, int delta) {
        while (delta > 0) {
            ComponentUtility.MoveComponentDown(c);
            delta--;
        }

        while (delta < 0) {
            ComponentUtility.MoveComponentUp(c);
            delta++;
        }
    }

    //    internal static Type xGetComponentTypeByName(this string cName) {
    //        var tempGO = new GameObject();
    //        tempGO.xSetFlag(HideFlags.HideAndDontSave, true);
    //        UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(tempGO, "Assets/Plugins/Editor/Vietlabs/Core/extension/mono/ComponentX.cs (48,9)", cName);
    //        var c = tempGO.GetComponent(cName);
    //        var t = c.GetType();
    //        Object.DestroyImmediate(tempGO);
    //        return t;
    //    }


#if UNITY_4_3
    public static T GetComponentInParent<T>(this GameObject go) where T : Component {
        var t = go.transform;
        var p = t.parent;
        while (p != null) {
            var c = p.GetComponent<T>();
            if (c != null) return c;

            p = p.parent;
        }
        return null;
    }
#endif
}
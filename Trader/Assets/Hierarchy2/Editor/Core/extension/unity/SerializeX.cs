using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SerializeX {

/*    internal static T[] GetArray<T>(this SerializedObject so, string propertyName) {
        var prop = so.FindProperty(propertyName);
        if (prop == null || !prop.isArray) return null;

        var result = new T[prop.arraySize];
        for (var i =0; i< result.Length; i++) {

            result[i] = prop.GetArrayElementAtIndex(i).boolValue;
        }
    }

    internal static T GetValue<T>(this SerializedProperty prop) {
        var typeT = typeof(T);

        if (prop.objectReferenceInstanceIDValue) {
            
        }

        //2DO : find a better way to prevent boxing / unboxing, maybe use ChangeType instead
        if (typeT == typeof(float))             return (T)(object)prop.floatValue;
        if (typeT == typeof(int))               return (T)(object)prop.intValue;
        if (typeT == typeof(bool))              return (T)(object)prop.boolValue;
        if (typeT == typeof(Color))             return (T)(object)prop.colorValue;
        if (typeT == typeof(Bounds))            return (T)(object)prop.boundsValue;
        if (typeT == typeof(AnimationCurve))    return (T)(object)prop.animationCurveValue;

        Debug.LogWarning("Unsupported type <" + typeT +"> used in SerializedProperty");

        return default(T);
    }*/


    internal static SerializedProperty[] xGetSerializedProperties(this Object go) {
        var so = new SerializedObject(go);
        so.Update();
        var result = new List<SerializedProperty>();

        var iterator = so.GetIterator();
        while (iterator.NextVisible(true)) result.Add(iterator.Copy());
        return result.ToArray();
    }

    internal static Dictionary<string, object> xGetDump(this SerializedObject obj) {
        var iterator = obj.GetIterator();
        var first = true;
        var result = new Dictionary<string, object>();

        var isHidden = obj.targetObject.xGetFlag(HideFlags.HideInInspector);
        if (isHidden) Debug.Log(obj + ": is Hidden");

        while (iterator.NextVisible(first)) {
            first = false;
            //if (!result.ContainsKey(iterator.name)) {

            if (iterator.isArray) {
                /*if (iterator.arraySize == 0) {
                    result.Add(iterator.name, iterator.propertyType);
                }

                var list = new List<Dictionary<string, object>>();

                for (var i = 0;i < iterator.arraySize; i++) {
                    var item = iterator.GetArrayElementAtIndex(i);
                    if (item.propertyType == SerializedPropertyType.ObjectReference) {
                        list.Add(new SerializedObject(item.objectReferenceValue).xGetDump());
                    } else {
                        list.Add(item.name, iterator.propertyType);
                    }
                }*/

                Debug.Log("---> " + iterator.name + ":" + iterator.type + ":" + iterator.arraySize);
            } else {
                result.Add(iterator.name, iterator.propertyType);
            }
        }

        return result;
    }
}
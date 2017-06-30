using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

static public class EditorPersist {
    private static Dictionary<string, object> _cache;

    internal static void ClearCache() { if (_cache != null) _cache.Clear(); }
    private static bool validate(string varName) {
        if (_cache == null) _cache = new Dictionary<string, object>();
        if (string.IsNullOrEmpty(varName)) {
            Debug.LogWarning("varName should not be null or empty");
            return false;
        }
        return true;
    }
    public static T Get<T>(string varName, T defaultValue = default(T)) {
        if (!validate(varName)) return defaultValue;
        if (_cache.ContainsKey(varName)) {
            var v = _cache[varName];
            if (typeof(T) != v.GetType()) {
                EditorPrefs.DeleteKey(varName);
                _cache.Remove(varName);
                Debug.LogWarning("Should not happen, varName must be duplicated, implicit :: " + typeof(T) + ":" + v.GetType() + ":" + v + ":" + varName);
            }
            return (T) _cache[varName];
        }

        var typeT = typeof(T);

        if (typeT == typeof(bool)) {
            var boolVal = EditorPrefs.GetBool(varName, (bool) (object) defaultValue);
            _cache.Add(varName, boolVal);
            return (T) (object) boolVal;
        }

        if (typeT == typeof(int)) {
            var intVal = EditorPrefs.GetInt(varName, (int) (object) defaultValue);
            _cache.Add(varName, intVal);
            return (T) (object) intVal;
        }

        if (typeT == typeof(float)) {
            var floatVal = EditorPrefs.GetFloat(varName, (float) (object) defaultValue);
            _cache.Add(varName, floatVal);
            return (T) (object) floatVal;
        }

        if (typeT == typeof(string)) {
            var stringVal = EditorPrefs.GetString(varName, (string) (object) defaultValue);
            _cache.Add(varName, stringVal);
            return (T) (object) stringVal;
        }

        // CUSTOM TYPES
        var str = EditorPrefs.GetString(varName, defaultValue.xStringEncode());
        var val = str.xStringDecode();
        _cache.Add(varName, val);

        //Debug.Log("custom decode ... " + varName + ":" + val);

        return (T)val;
    }
    public static void Set<T>(string varName, T value) {
        if (_cache == null) _cache = new Dictionary<string, object>();
        if (_cache.ContainsKey(varName)) {
            if (_cache[varName].Equals(value)) return; //same value : ignore !
            _cache[varName] = value; //overwrite
        } else {
            _cache.Add(varName, value);
        }

        var typeT = typeof(T);

        if      (typeT == typeof(bool))      EditorPrefs.SetBool(varName,    (bool)  (object) value);
        else if (typeT == typeof(int))       EditorPrefs.SetInt(varName,     (int)   (object) value);
        else if (typeT == typeof(float))     EditorPrefs.SetFloat(varName,   (float) (object) value);
        else if (typeT == typeof(string))    EditorPrefs.SetString(varName,  (string)(object) value);
        else EditorPrefs.SetString(varName,  value.xStringEncodeT());
    }


    //GUI SUPPORT
    /*public static void Toggle(string label, string varName, out bool val, float w = 130) {
        var value = Get<bool>(varName);
        var v = EditorGUILayout.ToggleLeft(label, value, GUILayout.Width(w));
        if (v != value) {
            EditorPrefs.SetBool(varName, v);
            _cache[varName] = v;
        }
        val = v;
    }
    public static string TextField(string label, string varName, float w) {
        var value = Get<string>(varName);
        EditorGUIUtility.labelWidth = w - 20f;
        var v = GUILayout.TextField(value, GUILayout.Width(w));
        if (v == value) return v;

        EditorPrefs.SetString(varName, v);
        _cache[varName] = v;
        return v;
    }
    public static Color ColorPicker(string label, string varName, float w) {
        var value = Get<Color>(varName);
        var newColor = EditorGUILayout.ColorField("", value, GUILayout.Width(w));
        if (newColor == value) return value;

        EditorPrefs.SetString(varName, ((Color32)newColor).xStringEncode());
        _cache[varName] = newColor;
        return newColor;
    }*/
}

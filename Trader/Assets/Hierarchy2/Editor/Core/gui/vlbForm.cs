/*
------------------------------------------------
 vlbForm for Unity3d by VietLabs
------------------------------------------------
	version : 1.0.0
	release : 02 May 2013
	require : Unity3d 4.3+
	website : http://vietlabs.net/vlbForm
--------------------------------------------------
Show a form to read user input
Usage :
Call vlbForm.Show with correct parameters
--------------------------------------------------
*/

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class vlbForm : EditorWindow { 
	
	static vlbFormData data;
    static string formId = "";

	public static vlbFormData Show(string title, Action<Dictionary<string, object>> onSubmit){
		
        data = new vlbFormData { onSubmit = onSubmit };
	    var window = GetWindow<vlbForm>(title, true);
		window.Show(true);
		return data;	
	}

	void DrawField(vlbField field) {
		if (field.type == vlbFieldType.Unknown) return;

		switch (field.type){
            case vlbFieldType.HzBegin       : GUILayout.BeginHorizontal();          break;
            case vlbFieldType.HzEnd         : GUILayout.EndHorizontal();            break;
            case vlbFieldType.VtBegin       : GUILayout.BeginVertical();            break;
            case vlbFieldType.VtEnd         : GUILayout.EndVertical();              break;
            case vlbFieldType.Space         : GUILayout.Space((float)field.value);  break;
            case vlbFieldType.FlexibleSpace : GUILayout.FlexibleSpace();            break;

            case vlbFieldType.Int: {
		        var val = (int) field.value;
		        var newVal = EditorGUILayout.IntField(field.title, val);
		        if (val != newVal) {
		            EditorPrefs.SetInt(formId + "." + field.varName, newVal);
                }

		        field.value = newVal;
		    }
			break;

			case vlbFieldType.Bool		    :
				field.value = EditorGUILayout.ToggleLeft(field.title, (bool)field.value, GUILayout.Width(data.titleWidth));
			break;

			case vlbFieldType.Float		    :
				field.value = EditorGUILayout.FloatField(field.title, (float) field.value);
			break;

			case vlbFieldType.String	    :
				field.value = EditorGUILayout.TextField(field.title, (string)field.value);
			break;

			case vlbFieldType.Color		:
				field.value = EditorGUILayout.ColorField(field.title, (Color)field.value);
			break;

			case vlbFieldType.Vector2	:
				field.value = EditorGUILayout.Vector2Field(field.title, (Vector2)field.value);
			break;

			case vlbFieldType.Vector3	:
				field.value = EditorGUILayout.Vector3Field(field.title, (Vector3)field.value);
			break;

			case vlbFieldType.Rect		:
				field.value = EditorGUILayout.RectField(field.title, (Rect)field.value);
			break;

			case vlbFieldType.Enum		:
				field.value = EditorGUILayout.EnumPopup(field.title, (Enum) field.value);
			break;
		}
	}
	
	void OnGUI() {
		if (data == null) return;
        var lbw = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = data.titleWidth;
		for (var i = 0; i < data.fields.Count; i++) {
			DrawField(data.fields[i]);
		}
	    EditorGUIUtility.labelWidth = lbw;
		
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("OK")) {
			if (data.onSubmit != null) data.onSubmit(data.ToDictionary());
			Close();
		}
		if (GUILayout.Button("Cancel")){
			data = null;
			Close();
		}
		GUILayout.EndHorizontal();
	}
}

public enum vlbFieldType {
	Unknown,

    HzBegin,
    HzEnd,

    VtBegin,
    VtEnd,

    Space,
    FlexibleSpace,

	Int,
	Bool,
	Float,
	String,
	Color,
	Vector2,
	Vector3,
	Rect,
	Enum
}

public class vlbField {
	public vlbFieldType type;
	public string title; //display label
	
	public object value;
	public string varName; //name to back into result Dictionary
}

public class vlbFormData {
    public int titleWidth = 120;

	public List<vlbField> fields;
	public Action<Dictionary<string, object>> onSubmit;
	
	vlbFieldType GetFieldType(Type typeT){
		if (typeT == typeof (int)) return vlbFieldType.Int;
		if (typeT == typeof (bool)) return vlbFieldType.Bool;
		if (typeT == typeof (float)) return vlbFieldType.Float;
		if (typeT == typeof (string)) return vlbFieldType.String;
		if (typeT == typeof (Color))  return vlbFieldType.Color;
		if (typeT == typeof (Vector2)) return vlbFieldType.Vector2;
		if (typeT == typeof (Vector3)) return vlbFieldType.Vector3;
		if (typeT == typeof (Rect)) return vlbFieldType.Rect;
		if (typeT.IsEnum) return vlbFieldType.Enum;
		
		return vlbFieldType.Unknown;
	}
	
	public Dictionary<String, object> ToDictionary() {
		var result = new Dictionary<string,object>();
		if (fields == null || fields.Count == 0) return result;
		for (int i =0; i< fields.Count; i++){
            if (string.IsNullOrEmpty(fields[i].varName)) continue;
			result.Add(fields[i].varName, fields[i].value);
		}
		return result;
	}
	
	public vlbFormData AddField(string varName, string title, object value, Type type = null){
		if (fields== null) fields = new List<vlbField>();
		fields.Add(new vlbField {
			title = title,
			varName = varName,
			value = value,
			type = GetFieldType(type ?? value.GetType())
		});

		return this;
	}

    public vlbFormData BeginHorizontal {
        get {
            fields.Add(new vlbField { type = vlbFieldType.HzBegin });
            return this;    
        }
    }

    public vlbFormData EndHorizontal {
        get {
            fields.Add(new vlbField { type = vlbFieldType.HzEnd });
            return this;    
        }
    }

    public vlbFormData BeginVertical {
        get {
            fields.Add(new vlbField { type = vlbFieldType.VtBegin });
            return this;    
        }
    }

    public vlbFormData EndVertical {
        get {
            fields.Add(new vlbField { type = vlbFieldType.VtEnd });
            return this;
        }
    }

    public vlbFormData Space(float space) {
        fields.Add(new vlbField { type = vlbFieldType.Space, value = space });
        return this;
    }

    public vlbFormData FlexibleSpace {
        get {
            fields.Add(new vlbField { type = vlbFieldType.FlexibleSpace });
            return this;
        }
    }
}
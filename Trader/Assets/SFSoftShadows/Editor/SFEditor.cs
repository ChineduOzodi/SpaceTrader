// Super Fast Soft Lighting. Copyright 2015 Howling Moon Software, LLP

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class SFAbstractEditor : Editor {
	protected delegate void ValidateBlock(SerializedProperty property);
	
	protected void PropertyField(string name, ValidateBlock validate = null){
		var property = this.serializedObject.FindProperty(name);
		
		EditorGUI.BeginChangeCheck(); {
			EditorGUILayout.PropertyField(property, new GUIContent(property.displayName));
			if(GUI.changed && validate != null) validate(property);
		} EditorGUI.EndChangeCheck();
	}

	protected void PropertySliderField(string name){

		var property = this.serializedObject.FindProperty(name);

		EditorGUI.BeginChangeCheck(); {
			EditorGUILayout.Slider(property, 0f, 1f);
		} EditorGUI.EndChangeCheck();
	}
}

[CustomEditor(typeof(SFLight))]
[CanEditMultipleObjects]
public class SFLightEditor : SFAbstractEditor {
	public override void OnInspectorGUI(){
		this.serializedObject.Update();
		
		PropertyField("_radius", (p) => p.floatValue = Mathf.Max(0.0f, p.floatValue));
		PropertyField("_color");
		PropertyField("_cookieTexture");
		PropertyField("_shadowLayers");
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	private void OnSceneGUI(){
		var light = this.target as SFLight;
		
		Handles.matrix = light.transform.localToWorldMatrix;

		var size = light.GetComponent<RectTransform>().sizeDelta;
		var max = Mathf.Min(size.x, size.y);
		
		Handles.color = Color.yellow;
		light.radius = Mathf.Min(max, Handles.RadiusHandle(Quaternion.identity, Vector3.zero, light.radius));
		
		Handles.matrix *= light._lightMatrix;
		Handles.color = Color.white;
		Handles.DrawSolidRectangleWithOutline(new Vector3[]{
			new Vector3(-1.0f, -1.0f, 0.0f),
			new Vector3(-1.0f,  1.0f, 0.0f),
			new Vector3( 1.0f,  1.0f, 0.0f),
			new Vector3( 1.0f, -1.0f, 0.0f),
		}, new Color(0.5f, 0.5f, 0.5f, 0.1f), Color.white);
		
		if(GUI.changed) EditorUtility.SetDirty(target);
	}
}

[CustomEditor(typeof(SFPolygon))]
[CanEditMultipleObjects]
public class SFPolygonEditor : SFAbstractEditor {

	public static bool inEditMode = false;

	public void OnDisable(){
		// OnDisable for editor scripts is essentially used when you change focus.
		inEditMode = false;
	}

	private static Vector2 ClosestHelper(Vector2 p, Vector2 a, Vector2 b)
	{
		var delta = a - b;
		var t = Mathf.Clamp01(Vector2.Dot(delta, p - b)/delta.sqrMagnitude);
		return b + delta*t;
	}

	protected Vector3 ClosestPoint(Transform t, List<Vector2> verts, out int index){
//		// Duplicate the verts list and make it into a loop.
//		var loop = new List<Vector2>(verts);
//		loop.Add(verts[0]);
//		Vector3[] arr = loop.Select<Vector2, Vector3>((arg) => (Vector3)arg).ToArray();

		var mouse = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
		var p = t.InverseTransformPoint(mouse);

		float closest = Mathf.Infinity;
		index = 0;

		for(int i=1; i<verts.Count; i++){
			var a = verts[i - 1];
			var b = verts[i];

			var d = HandleUtility.DistancePointToLineSegment(p, a, b);
			if(d < closest){
				closest = d;
				index = i;
			}
		}

		return ClosestHelper(p, verts[index - 1], verts[index]);
	}

	protected void OnSceneGUI(){
		var poly = target as SFPolygon;
		if (poly == null )
			return;

		bool invalidVerts = true;
		for (int i = 0; i < poly.verts.Length && invalidVerts; i++) {
			invalidVerts &= (poly.verts [i] == Vector2.zero);
		}

		bool dirty = false;

		if (invalidVerts) {
			// copy in collider verts if possible.
			poly._CopyVertsFromCollider();
			dirty = true;
		}

		var verts = new List<Vector2> (poly.verts);

		// Copy of verts that is looped if the polygon is looped.
		var looped = new List<Vector2> (verts);
		if (poly.looped)
			looped.Add (looped [0]);

		Handles.matrix = poly.transform.localToWorldMatrix;


		if (!inEditMode) {
			Handles.color = new Color(1f, 1f, 0f, 0.4f);
		} else {
			SetupUndo ("edited ChipmunkPolyShape");

			var removePressed = ((Event.current.modifiers & (EventModifiers.Command | EventModifiers.Control)) != 0);
			var addPressed = ((Event.current.modifiers & (EventModifiers.Shift)) != 0);

			if (removePressed) {
				if (verts.Count > 2) {
					Handles.color = Color.red;
				
					for (int i = 0; i < verts.Count; i++) {
						var handleSize = 0.05f * HandleUtility.GetHandleSize (verts [i]);
						if (Handles.Button (verts [i], Quaternion.identity, handleSize, handleSize, Handles.DotCap)) {
							verts.RemoveAt (i);
							dirty = true;
							break;
						}
					}
				}
			} else if (addPressed) {
				int insertIndex = 0;
				Vector3 insertPosition = ClosestPoint (poly.transform, looped, out insertIndex);
				var s = HandleUtility.GetHandleSize (insertPosition) * 0.05f;

				// Draw the existing vertexes
				Handles.color = Color.white;
				for (int i = 0; i < verts.Count; i++) {
					Handles.DotCap (0, verts [i], Quaternion.identity, s);
				}

				// Draw the insert handle
				Handles.color = Color.green;
				if (Handles.Button (insertPosition, Quaternion.identity, s, s, Handles.DotCap)) {
					verts.Insert (insertIndex, (Vector2)insertPosition);
					dirty = true;
				}

				HandleUtility.Repaint ();
			} else {
				// Move an existing vertex
				Handles.color = Color.white;
				for (int i = 0; i < verts.Count; i++) {
					Vector3 v = verts [i];
					Vector2 delta = DotHandle (v) - (Vector2)v;
					if (delta != Vector2.zero) {
						verts [i] = (Vector2)v + delta;
						dirty = true;
					}
				}
			}
			Handles.color = Color.white;
		}


		Handles.DrawPolyLine(looped.Select(v => (Vector3)v).ToArray());

		if(dirty){
			poly.verts = verts.ToArray();
			EditorUtility.SetDirty(target);
		}
	}
	protected Vector2 CircleHandle(Vector3 pos){
		float size = HandleUtility.GetHandleSize(pos)*0.2f;
		return Handles.FreeMoveHandle(pos, Quaternion.identity, size, Vector3.zero, Handles.CircleCap);
	}

	protected Vector2 DotHandle(Vector3 pos, float size = 0.05f){
		float s = HandleUtility.GetHandleSize(pos)*size;
		return Handles.FreeMoveHandle(pos, Quaternion.identity, s, Vector3.zero, Handles.DotCap);
	}

	protected void SetupUndo(string message){
		if(Input.GetMouseButtonDown(0)){
			Undo.RecordObject (target, message);
		}
	}

	public override void OnInspectorGUI(){
		this.serializedObject.Update();

		SFPolygon poly = this.target as SFPolygon;

		EditorGUILayout.HelpBox ("When editing the shadow geometry, shift+click to add new points. Command+click or control+click to remove point.", MessageType.Info);
	
		GUILayout.BeginHorizontal("box");

		if(GUILayout.Button(SFPolygonEditor.inEditMode ? "Stop Editing" : "Edit Shadow Geometry"))
		{
			SFPolygonEditor.inEditMode = !SFPolygonEditor.inEditMode;
			EditorUtility.SetDirty(target);
		} 
		if(GUILayout.Button("Flip Inside-Out"))
		{
			foreach (SFPolygon t in this.targets) {
				t._FlipInsideOut ();
				EditorUtility.SetDirty (t);
			}
		}
		if (poly.GetComponent<PolygonCollider2D> () != null) {
			if(GUILayout.Button("Copy from Collider"))
			{
				foreach (SFPolygon t in this.targets) {
					t._CopyVertsFromCollider ();
					EditorUtility.SetDirty (t);
				}
			}

		}
		GUILayout.EndHorizontal ();

		PropertyField("_looped");
		PropertyField("_shadowLayers");
//		PropertyField ("softness", (p) => p.floatValue = Math.Max(0.0f, p.floatValue));

		this.serializedObject.ApplyModifiedProperties();
	}

}

[CustomEditor(typeof(SFRenderer))]
[CanEditMultipleObjects]
public class SFRendererEditor : SFAbstractEditor {
	public override void OnInspectorGUI(){
		this.serializedObject.Update();
		
		PropertyField("_linearLightBlending");
		PropertyField("_shadows");
		PropertyField("_ambientLight");
		PropertyField("_globalDynamicRange", (p) => p.floatValue = Mathf.Max(0.0f, p.floatValue));
		//		PropertyField("_globalSoftening", (p) => p.floatValue = Math.Max(0.0f, p.floatValue));
		
		PropertyField("_lightMapScale", (p) => p.floatValue = Mathf.Max(1.0f, p.floatValue));
		PropertyField("_shadowMapScale", (p) => p.floatValue = Mathf.Max(1.0f, p.floatValue));
		
		PropertyField("_fogColor");
		PropertyField("_scatterColor");
		PropertySliderField("_softHardMix");
		
		this.serializedObject.ApplyModifiedProperties();
	}
}

public class SFMenus : ScriptableObject {
	public static void AddComponents<T>() where T : Component{
		foreach(var go in Selection.gameObjects){
			Undo.RecordObject(go, "add " + typeof(T).Name);
			go.AddComponent<T>();
		}
	}

	const string ROOT = "Component/SFShadow/";
	
	[MenuItem (ROOT + "SFRenderer", false, 1000)]
	public static void AddRenderer(){
		AddComponents<SFRenderer>();
	}
	
	[MenuItem (ROOT + "SFRenderer", true, 1000)]
	public static bool CheckRenderer(){
		return Selection.activeGameObject;
	}
	
	[MenuItem (ROOT + "SFLight", false, 1100)]
	public static void AddLight(){
		AddComponents<SFLight>();

		var cookie = AssetDatabase.LoadAssetAtPath("Assets/SFSoftShadows/CookieTextures/RadialFalloff.png", typeof(Texture2D)) as Texture2D;
		foreach(var go in Selection.gameObjects){
			go.GetComponent<SFLight>().cookieTexture = cookie;
		}
	}
	
	[MenuItem (ROOT + "SFLight", true, 1100)]
	public static bool CheckLight(){
		return Selection.activeGameObject;
	}
	
	[MenuItem (ROOT + "SFPolygon", false, 1101)]
	public static void AddPolygon(){
		AddComponents<SFPolygon>();
	}
	
	[MenuItem (ROOT + "SFPolygon", true, 1101)]
	public static bool CheckPolygon(){
		return Selection.activeGameObject;
	}
}

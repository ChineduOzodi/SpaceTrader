#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

public abstract class SgtEditor<T> : Editor
	where T : MonoBehaviour
{
	protected T Target;

	protected T[] Targets;

	public override void OnInspectorGUI()
	{
		SgtHelper.BaseRect    = SgtHelper.Reserve(0.0f);
		SgtHelper.BaseRectSet = true;

		//SgtHelper.BeginUndo("Changes to " + typeof(T).Name, targets);

		EditorGUI.BeginChangeCheck();
		{
			serializedObject.UpdateIfDirtyOrScript();

			Target  = (T)target;
			Targets = targets.Select(t => (T)t).ToArray();

			Separator();

			OnInspector();

			Separator();

			serializedObject.ApplyModifiedProperties();
		}
		if (EditorGUI.EndChangeCheck() == true)
		{
			GUI.changed = true; Repaint();

			foreach (var t in Targets)
			{
				SgtHelper.SetDirty(t);
			}
		}

		SgtHelper.BaseRectSet = false;
	}

	protected virtual void OnSceneGUI()
	{
		Target = (T)target;

		OnScene();

		if (GUI.changed == true)
		{
			SgtHelper.SetDirty(target);
		}
	}

	protected void Each(System.Action<T> update)
	{
		foreach (var t in Targets)
		{
			update(t);
		}
	}

	protected bool Any(System.Func<T, bool> check)
	{
		foreach (var t in Targets)
		{
			if (check(t) == true)
			{
				return true;
			}
		}

		return false;
	}

	protected bool All(System.Func<T, bool> check)
	{
		foreach (var t in Targets)
		{
			if (check(t) == false)
			{
				return false;
			}
		}

		return true;
	}

	protected bool Button(string text)
	{
		var rect = SgtHelper.Reserve();

		return GUI.Button(rect, text);
	}

	protected bool HelpButton(string helpText, UnityEditor.MessageType type, string buttonText, float buttonWidth)
	{
		var clicked = false;

		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.HelpBox(helpText, type);

			var style = new GUIStyle(GUI.skin.button); style.wordWrap = true;

			clicked = GUILayout.Button(buttonText, style, GUILayout.ExpandHeight(true), GUILayout.Width(buttonWidth));
		}
		EditorGUILayout.EndHorizontal();

		return clicked;
	}

	protected void BeginError(bool error = true)
	{
		EditorGUILayout.BeginVertical(error == true ? SgtHelper.Error : SgtHelper.NoError);
	}

	protected void EndError()
	{
		EditorGUILayout.EndVertical();
	}

	protected void BeginMixed(bool mixed = true)
	{
		EditorGUI.showMixedValue = mixed;
	}

	protected void EndMixed()
	{
		EditorGUI.showMixedValue = false;
	}

	protected void BeginDisabled(bool disabled = true)
	{
		EditorGUI.BeginDisabledGroup(disabled);
	}

	protected void EndDisabled()
	{
		EditorGUI.EndDisabledGroup();
	}

	protected void DrawDefault(string proeprtyPath)
	{
		EditorGUILayout.BeginVertical(SgtHelper.NoError);
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty(proeprtyPath), true);
		}
		EditorGUILayout.EndVertical();
	}

	protected void Separator()
	{
		EditorGUILayout.Separator();
	}

	protected void BeginIndent()
	{
		EditorGUI.indentLevel += 1;
	}

	protected void EndIndent()
	{
		EditorGUI.indentLevel -= 1;
	}

	protected virtual void OnInspector()
	{
	}

	protected virtual void OnScene()
	{
	}

	protected void RequireObserver()
	{
		if (SgtObserver.AllObservers.Count == 0)
		{
			Separator();

			if (HelpButton("Your scene contains no SgtObservers", MessageType.Error, "Fix", 50.0f) == true)
			{
				SgtHelper.ClearSelection();

				foreach (var camera in Camera.allCameras)
				{
					SgtHelper.AddToSelection(camera.gameObject);

					SgtHelper.GetOrAddComponent<SgtObserver>(camera.gameObject);
				}
			}
		}
	}
}
#endif

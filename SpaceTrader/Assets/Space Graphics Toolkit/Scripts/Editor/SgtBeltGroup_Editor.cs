using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtBeltGroup))]
public class SgtBeltGroup_Editor : SgtEditor<SgtBeltGroup>
{
	protected override void OnInspector()
	{
		EditorGUI.BeginDisabledGroup(true);
		{
			DrawDefault("Belt");
			DrawDefault("MainTex");
			DrawDefault("HeightTex");
		}
		EditorGUI.EndDisabledGroup();
	}
}
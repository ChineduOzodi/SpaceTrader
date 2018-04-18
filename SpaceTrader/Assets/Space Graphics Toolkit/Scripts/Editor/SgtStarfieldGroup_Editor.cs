using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtStarfieldGroup))]
public class SgtStarfieldGroup_Editor : SgtEditor<SgtStarfieldGroup>
{
	protected override void OnInspector()
	{
		EditorGUI.BeginDisabledGroup(true);
		{
			DrawDefault("Texture");
		}
		EditorGUI.EndDisabledGroup();
	}
}
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtBeltModel))]
public class SgtBeltModel_Editor : SgtEditor<SgtBeltModel>
{
	protected override void OnInspector()
	{
		EditorGUI.BeginDisabledGroup(true);
		{
			DrawDefault("Group");
		}
		EditorGUI.EndDisabledGroup();
	}
}
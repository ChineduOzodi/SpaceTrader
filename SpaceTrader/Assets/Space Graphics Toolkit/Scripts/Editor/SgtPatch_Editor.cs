using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtPatch))]
public class SgtPatch_Editor : SgtEditor<SgtPatch>
{
	protected override void OnInspector()
	{
		EditorGUI.BeginChangeCheck();
		{
			DrawDefault("Material");
		}
		if (EditorGUI.EndChangeCheck() == true)
		{
			Each(t => t.MarkMaterialAsDirty());
		}
	}
}

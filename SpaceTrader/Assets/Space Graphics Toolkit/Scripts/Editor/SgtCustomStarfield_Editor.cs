using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtCustomStarfield))]
public class SgtCustomStarfield_Editor : SgtStarfield_Editor<SgtCustomStarfield>
{
	protected override void OnInspector()
	{
		base.OnInspector();
		
		EditorGUI.BeginChangeCheck();
		{
			DrawDefault("Stars");
		}
		if (EditorGUI.EndChangeCheck() == true)
		{
			Each(t => t.MarkMeshAsDirty());
		}
		
		RequireObserver();
	}
}
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtComponentPool))]
public class SgtComponentPool_Editor : SgtEditor<SgtComponentPool>
{
	protected override void OnInspector()
	{
		EditorGUILayout.HelpBox("SgtComponentPools are not saved to your scene, so don't worry if you see it in edit mode.", MessageType.Info);
		
		DrawDefault("Count");
	}
}
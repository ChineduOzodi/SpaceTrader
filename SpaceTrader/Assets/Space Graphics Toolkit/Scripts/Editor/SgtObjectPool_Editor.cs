using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtObjectPool))]
public class SgtObjectPool_Editor : SgtEditor<SgtObjectPool>
{
	protected override void OnInspector()
	{
		EditorGUILayout.HelpBox("SgtObjectPools are not saved to your scene, so don't worry if you see it in edit mode.", MessageType.Info);
		
		DrawDefault("Count");
	}
}
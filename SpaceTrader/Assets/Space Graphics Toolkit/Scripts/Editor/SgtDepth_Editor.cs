using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtDepth))]
public class SgtDepth_Editor : SgtEditor<SgtDepth>
{
	protected override void OnInspector()
	{
		DrawDefault("RenderQueue");
		
		DrawDefault("RenderQueueOffset");
		
		Separator();
		
		DrawDefault("Renderers");
	}
}
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtSkysphere))]
public class SgtSkysphere_Editor : SgtEditor<SgtSkysphere>
{
	protected override void OnInspector()
	{
		DrawDefault("Color");
		
		BeginError(Any(t => t.Brightness < 0.0f));
		{
			DrawDefault("Brightness");
		}
		EndError();
		
		DrawDefault("RenderQueue");
		
		DrawDefault("RenderQueueOffset");
		
		Separator();
		
		DrawDefault("MainTex");
		
		Separator();
		
		DrawDefault("FollowObservers");
		
		Separator();
		
		BeginError(Any(t => t.Meshes.Count == 0 || t.Meshes.FindIndex(m => m == null) != -1));
		{
			DrawDefault("Meshes");
		}
		EndError();
		
		if (Any(t => t.FollowObservers == true))
		{
			RequireObserver();
		}
	}
}
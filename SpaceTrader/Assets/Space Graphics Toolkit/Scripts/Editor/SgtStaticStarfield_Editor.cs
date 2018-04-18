using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtStaticStarfield))]
public class SgtStaticStarfield_Editor : SgtStarfield_Editor<SgtStaticStarfield>
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
		
		EditorGUI.BeginChangeCheck();
		{
			DrawDefault("Seed");
			
			BeginError(Any(t => t.Radius <= 0.0f));
			{
				DrawDefault("Radius");
			}
			EndError();
			
			DrawDefault("Symmetry");
			
			Separator();
			
			BeginError(Any(t => t.StarCount < 0));
			{
				DrawDefault("StarCount");
			}
			EndError();
			
			BeginError(Any(t => t.StarRadiusMin < 0.0f || t.StarRadiusMin > t.StarRadiusMax));
			{
				DrawDefault("StarRadiusMin");
			}
			EndError();
			
			BeginError(Any(t => t.StarRadiusMax < 0.0f || t.StarRadiusMin > t.StarRadiusMax));
			{
				DrawDefault("StarRadiusMax");
			}
			EndError();
			
			BeginError(Any(t => t == null || t.StarSprites.Count == 0 || t.StarSprites.Contains(null) == true));
			{
				DrawDefault("StarSprites");
			}
			EndError();
		}
		if (EditorGUI.EndChangeCheck() == true)
		{
			Each(t => t.MarkMeshAsDirty());
		}
		
		RequireObserver();
	}
}
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtEllipticalStarfield))]
public class SgtEllipticalStarfield_Editor : SgtStarfield_Editor<SgtEllipticalStarfield>
{
	protected override void OnInspector()
	{
		base.OnInspector();
		
		EditorGUI.BeginChangeCheck();
		{
			DrawDefault("Seed");
			
			BeginError(Any(t => t.Radius <= 0.0f));
			{
				DrawDefault("Radius");
			}
			EndError();
			
			DrawDefault("Symmetry");
			
			DrawDefault("Offset");
			
			DrawDefault("Inverse");
			
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
			
			DrawDefault("StarPulseMax");
			
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
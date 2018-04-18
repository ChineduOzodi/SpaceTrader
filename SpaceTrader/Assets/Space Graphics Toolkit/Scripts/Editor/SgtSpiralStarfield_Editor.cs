using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtSpiralStarfield))]
public class SgtSpiralStarfield_Editor : SgtStarfield_Editor<SgtSpiralStarfield>
{
	protected override void OnInspector()
	{
		base.OnInspector();
		
		EditorGUI.BeginChangeCheck();
		{
			DrawDefault("Seed");
			
			DrawDefault("Radius");
			
			BeginError(Any(t => t.ArmCount <= 0));
			{
				DrawDefault("ArmCount");
			}
			EndError();
			
			DrawDefault("Twist");
			
			DrawDefault("Thickness");
			
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
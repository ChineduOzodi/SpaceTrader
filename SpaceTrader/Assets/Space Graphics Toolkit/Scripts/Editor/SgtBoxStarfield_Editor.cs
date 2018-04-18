using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtBoxStarfield))]
public class SgtBoxStarfield_Editor : SgtStarfield_Editor<SgtBoxStarfield>
{
	protected override void OnInspector()
	{
		base.OnInspector();
		
		EditorGUI.BeginChangeCheck();
		{
			DrawDefault("Seed");
			
			BeginError(Any(t => t.Extents == Vector3.zero));
			{
				DrawDefault("Extents");
			}
			EndError();
			
			DrawDefault("Offset");
			
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
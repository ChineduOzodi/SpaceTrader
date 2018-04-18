using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtSimpleBelt))]
public class SgtSimpleBelt_Editor : SgtBelt_Editor<SgtSimpleBelt>
{
	protected override void OnInspector()
	{
		base.OnInspector();
		
		EditorGUI.BeginChangeCheck();
		{
			DrawDefault("Seed");
			
			DrawDefault("Thickness");
			
			BeginError(Any(t => t.InnerRadius < 0.0f || t.InnerRadius >= t.OuterRadius));
			{
				DrawDefault("InnerRadius");
			}
			EndError();
			
			DrawDefault("InnerSpeed");
			
			BeginError(Any(t => t.OuterRadius < 0.0f || t.InnerRadius >= t.OuterRadius));
			{
				DrawDefault("OuterRadius");
			}
			EndError();
			
			DrawDefault("OuterSpeed");
			
			Separator();
			
			BeginError(Any(t => t.AsteroidCount < 0));
			{
				DrawDefault("AsteroidCount");
			}
			EndError();
			
			DrawDefault("AsteroidSpin");
			
			BeginError(Any(t => t.AsteroidRadiusMin < 0.0f || t.AsteroidRadiusMin >= t.AsteroidRadiusMax));
			{
				DrawDefault("AsteroidRadiusMin");
			}
			EndError();
			
			BeginError(Any(t => t.AsteroidRadiusMax < 0.0f || t.AsteroidRadiusMin >= t.AsteroidRadiusMax));
			{
				DrawDefault("AsteroidRadiusMax");
			}
			EndError();
			
			BeginError(Any(t => t == null || t.AsteroidVariants.Count == 0 || t.AsteroidVariants.Exists(v => v == null || v.MainTex == null || v.HeightTex == null)));
			{
				DrawDefault("AsteroidVariants");
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
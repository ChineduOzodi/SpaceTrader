using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtAdvancedBelt))]
public class SgtAdvancedBelt_Editor : SgtBelt_Editor<SgtAdvancedBelt>
{
	protected override void OnInspector()
	{
		base.OnInspector();
		
		EditorGUI.BeginChangeCheck();
		{
			DrawDefault("Seed");
			
			DrawDefault("DistanceDistribution");
			
			DrawDefault("HeightDistribution");
			
			DrawDefault("SpeedDistribution");
			
			DrawDefault("SpeedNoiseDistribution");
			
			DrawDefault("RadiusDistribution");
			
			DrawDefault("SpinDistribution");
			
			DrawDefault("AsteroidCount");
			
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
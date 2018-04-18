using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtTerrainDisplacer))]
public class SgtTerrainDisplacer_Editor : SgtEditor<SgtTerrainDisplacer>
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.Heightmap == null));
		{
			DrawDefault("Heightmap");
		}
		EndError();

		BeginError(Any(t => t.Strength == 0.0f));
		{
			DrawDefault("Strength");
		}
		EndError();
	}
}

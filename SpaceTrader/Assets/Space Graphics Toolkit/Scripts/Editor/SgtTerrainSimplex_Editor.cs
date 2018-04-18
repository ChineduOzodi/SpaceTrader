using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtTerrainSimplex))]
public class SgtTerrainSimplex_Editor : SgtEditor<SgtTerrainSimplex>
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.Scale == 0.0f));
		{
			DrawDefault("Scale");
		}
		EndError();

		BeginError(Any(t => t.Strength == 0.0f));
		{
			DrawDefault("Strength");
		}
		EndError();
	}
}

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtNebulaStarfield))]
public class SgtNebulaStarfield_Editor : SgtStarfield_Editor<SgtNebulaStarfield>
{
	protected override void OnInspector()
	{
		base.OnInspector();

		EditorGUI.BeginChangeCheck();
		{
			DrawDefault("Seed");

			BeginError(Any(t => t.SourceTex == null));
			{
				DrawDefault("SourceTex");
			}
			EndError();

			BeginError(Any(t => t.Resolution <= 0.0f));
			{
				DrawDefault("Resolution");
			}
			EndError();

			DrawDefault("Threshold");

			DrawDefault("Jitter");

			DrawDefault("HeightSource");

			BeginError(Any(t => t.Size.x <= 0.0f || t.Size.y <= 0.0f || t.Size.z <= 0.0f));
			{
				DrawDefault("Size");
			}
			EndError();

			Separator();

			BeginError(Any(t => t.HorizontalBrightness < 0.0f));
			{
				DrawDefault("HorizontalBrightness");
			}
			EndError();

			BeginError(Any(t => t.HorizontalPower < 0.0f));
			{
				DrawDefault("HorizontalPower");
			}
			EndError();

			Separator();

			BeginError(Any(t => t.StarRadiusMin < 0.0f || t.StarRadiusMin >= t.StarRadiusMax));
			{
				DrawDefault("StarRadiusMin");
			}
			EndError();

			BeginError(Any(t => t.StarRadiusMax < 0.0f || t.StarRadiusMin >= t.StarRadiusMax));
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

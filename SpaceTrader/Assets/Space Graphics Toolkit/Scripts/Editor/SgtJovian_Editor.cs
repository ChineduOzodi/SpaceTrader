using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtJovian))]
public class SgtJovian_Editor : SgtEditor<SgtJovian>
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.Lights.Exists(l => l == null)));
		{
			DrawDefault("Lights");
		}
		EndError();

		BeginError(Any(t => t.Shadows.Exists(s => s == null)));
		{
			DrawDefault("Shadows");
		}
		EndError();

		Separator();

		DrawDefault("Color");

		BeginError(Any(t => t.Brightness < 0.0f));
		{
			DrawDefault("Brightness");
		}
		EndError();

		DrawDefault("RenderQueue");

		DrawDefault("RenderQueueOffset");

		DrawDefault("Smooth");

		Separator();

		BeginError(Any(t => t.MainTex == null));
		{
			DrawDefault("MainTex");
		}
		EndError();

		DrawDefault("Scattering");

		if (Any(t => t.Scattering == true))
		{
			BeginIndent();
			{
				DrawDefault("MieSharpness");

				DrawDefault("MieStrength");

				DrawDefault("LimitAlpha");
			}
			EndIndent();
		}

		Separator();

		EditorGUI.BeginChangeCheck();
		{
			DrawDefault("LightingBrightness");

			DrawDefault("LightingColor");

			DrawDefault("RimColor");
		}
		if (EditorGUI.EndChangeCheck() == true)
		{
			Each(t => t.MarkLutAsDirty());
		}

		Separator();

		DrawDefault("DensityMode");

		BeginError(Any(t => t.Density < 0.0f));
		{
			DrawDefault("Density");
		}
		EndError();

		BeginError(Any(t => t.Power < 0.0f));
		{
			DrawDefault("Power");
		}
		EndError();

		BeginError(Any(t => t.MeshRadius <= 0.0f));
		{
			DrawDefault("MeshRadius");
		}
		EndError();

		BeginError(Any(t => t.Meshes.Count == 0));
		{
			DrawDefault("Meshes");
		}
		EndError();

		RequireObserver();
	}
}

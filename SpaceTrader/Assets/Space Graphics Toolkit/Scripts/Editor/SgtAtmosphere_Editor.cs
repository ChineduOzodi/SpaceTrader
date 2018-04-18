using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtAtmosphere))]
public class SgtAtmosphere_Editor : SgtEditor<SgtAtmosphere>
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

		BeginError(Any(t => t.Fog >= 1.0f));
		{
			DrawDefault("Fog");
		}
		EndError();

		DrawDefault("Smooth");

		Separator();

		DrawDefault("Scattering");

		if (Any(t => t.Scattering == true))
		{
			BeginIndent();
			{
				DrawDefault("MieSharpness");
				DrawDefault("MieStrength");
				DrawDefault("RayleighStrength");
				DrawDefault("LimitBrightness");
				DrawDefault("GroundScattering");

				if (Any(t => t.GroundScattering == true))
				{
					BeginIndent();
					{
						DrawDefault("GroundPower");
						DrawDefault("GroundMieSharpness");
						DrawDefault("GroundMieStrength");
					}
					EndIndent();
				}
			}
			EndIndent();
		}

		Separator();

		BeginError(Any(t => t.Height <= 0.0f));
		{
			DrawDefault("Height");
		}
		EndError();

		BeginError(Any(t => t.InnerPower < 0.0f));
		{
			DrawDefault("InnerPower");
		}
		EndError();

		BeginError(Any(t => t.InnerMeshRadius <= 0.0f));
		{
			DrawDefault("InnerMeshRadius");
		}
		EndError();

		// This may be 0/null if there is a corona/atmosphere linking to this
		//BeginError(Any(t => t.InnerRenderers.Count == 0 || t.InnerRenderers.Exists(r => r == null) == true));
		{
			DrawDefault("InnerRenderers");
		}
		//EndError();

		Separator();

		BeginError(Any(t => t.MiddlePower < 0.0f));
		{
			DrawDefault("MiddlePower");
		}
		EndError();

		BeginError(Any(t => t.MiddleRatio >= 1.0f));
		{
			DrawDefault("MiddleRatio");
		}
		EndError();

		Separator();

		BeginError(Any(t => t.OuterPower < 0.0f));
		{
			DrawDefault("OuterPower");
		}
		EndError();

		BeginError(Any(t => t.OuterMeshRadius <= 0.0f));
		{
			DrawDefault("OuterMeshRadius");
		}
		EndError();

		BeginError(Any(t => t.OuterMeshes.Count == 0 || t.OuterMeshes.Exists(m => m == null) == true));
		{
			DrawDefault("OuterMeshes");
		}
		EndError();

		Separator();

		EditorGUI.BeginChangeCheck();
		{
			DrawDefault("LightingBrightness");
			DrawDefault("LightingColor");
			DrawDefault("DensityColor");
		}
		if (EditorGUI.EndChangeCheck() == true)
		{
			Each(t => t.MarkLutAsDirty());
		}

		BeginError(Any(t => t.DensityScale < 0.0f));
		{
			DrawDefault("DensityScale");
		}
		EndError();

		RequireObserver();
	}
}

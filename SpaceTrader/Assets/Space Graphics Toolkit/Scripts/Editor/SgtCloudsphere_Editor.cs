using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtCloudsphere))]
public class SgtCloudsphere_Editor : SgtEditor<SgtCloudsphere>
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

		Separator();

		BeginError(Any(t => t.MainTex == null));
		{
			DrawDefault("MainTex");
		}
		EndError();

		BeginError(Any(t => t.Radius < 0.0f));
		{
			DrawDefault("Radius");
		}
		EndError();

		DrawDefault("ObserverOffset");

		DrawDefault("FadeNear");

		if (Any(t => t.FadeNear == true))
		{
			BeginIndent();
			{
				BeginError(Any(t => t.FadeInnerRadius < 0.0f || t.FadeInnerRadius >= t.FadeOuterRadius));
				{
					DrawDefault("FadeInnerRadius");
				}
				EndError();

				BeginError(Any(t => t.FadeOuterRadius < 0.0f || t.FadeInnerRadius >= t.FadeOuterRadius));
				{
					DrawDefault("FadeOuterRadius");
				}
				EndError();
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

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtAccretion))]
public class SgtAccretion_Editor : SgtEditor<SgtAccretion>
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

		DrawDefault("Brightness");

		DrawDefault("RenderQueue");

		DrawDefault("RenderQueueOffset");

		DrawDefault("Age");

		DrawDefault("TimeScale");

		Separator();

		BeginError(Any(t => t.MainTex == null));
		{
			DrawDefault("MainTex");
		}
		EndError();

		BeginError(Any(t => t.DustTex == null));
		{
			DrawDefault("DustTex");
		}
		EndError();

		DrawDefault("LightingBias");

		DrawDefault("LightingSharpness");

		DrawDefault("Scattering");

		if (Any(t => t.Scattering == true))
		{
			BeginIndent();
			{
				DrawDefault("MieSharpness");

				DrawDefault("MieStrength");
			}
			EndIndent();
		}

		Separator();

		EditorGUI.BeginChangeCheck();
		{
			BeginError(Any(t => t.InnerRadius < 0.0f || t.InnerRadius >= t.OuterRadius));
			{
				DrawDefault("InnerRadius");
			}
			EndError();

			BeginError(Any(t => t.OuterRadius < 0.0f || t.InnerRadius >= t.OuterRadius));
			{
				DrawDefault("OuterRadius");
			}
			EndError();

			BeginError(Any(t => t.SegmentCount < 1));
			{
				DrawDefault("SegmentCount");
			}
			EndError();

			BeginError(Any(t => t.SegmentDetail < 3));
			{
				DrawDefault("SegmentDetail");
			}
			EndError();

			BeginError(Any(t => t.BoundsShift < 0.0f));
			{
				DrawDefault("BoundsShift");
			}
			EndError();
		}
		if (EditorGUI.EndChangeCheck() == true)
		{
			Each(t => t.MarkMeshAsDirty());
		}

		Separator();

		DrawDefault("Twist");

		DrawDefault("TwistBias");

		DrawDefault("ReverseTwist");

	}
}

using UnityEngine;
using UnityEditor;

public abstract class SgtBelt_Editor<T> : SgtEditor<T>
	where T : SgtBelt
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

		DrawDefault("Age");

		DrawDefault("TimeScale");

		DrawDefault("AutoRegenerate");

		if (Any(t => t.AutoRegenerate == false))
		{
			if (Button("Regenerate") == true)
			{
				Each(t => t.Regenerate());
			}
		}

		Separator();
	}
}

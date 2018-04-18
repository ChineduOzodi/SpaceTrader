using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtLightningSpawner))]
public class SgtLightningSpawner_Editor : SgtEditor<SgtLightningSpawner>
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.DelayMin > t.DelayMax));
		{
			DrawDefault("DelayMin");

			DrawDefault("DelayMax");
		}
		EndError();

		Separator();

		BeginError(Any(t => t.LifeMin > t.LifeMax));
		{
			DrawDefault("LifeMin");

			DrawDefault("LifeMax");
		}
		EndError();

		Separator();

		BeginError(Any(t => t.SizeMin > t.SizeMax));
		{
			DrawDefault("SizeMin");

			DrawDefault("SizeMax");
		}
		EndError();

		Separator();

		BeginError(Any(t => t.Detail <= 0.0f));
		{
			DrawDefault("Detail");
		}
		EndError();

		BeginError(Any(t => t.Radius <= 0.0f));
		{
			DrawDefault("Radius");
		}
		EndError();

		DrawDefault("Colors");

		DrawDefault("Brightness");

		BeginError(Any(t => t.Sprites == null || t.Sprites.Count == 0));
		{
			DrawDefault("Sprites");
		}
		EndError();
	}
}

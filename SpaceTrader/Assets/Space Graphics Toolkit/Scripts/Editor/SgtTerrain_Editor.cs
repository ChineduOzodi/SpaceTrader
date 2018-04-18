using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtTerrain))]
public class SgtTerrain_Editor : SgtEditor<SgtTerrain>
{
	protected override void OnInspector()
	{
		base.OnInspector();

		EditorGUI.BeginChangeCheck();
		{
			BeginError(Any(t => t.Resolution <= 0));
			{
				DrawDefault("Resolution");
			}
			EndError();

			DrawDefault("SkirtThickness");

			BeginError(Any(t => t.RadiusMin >= t.RadiusMax));
			{
				DrawDefault("RadiusMin");

				DrawDefault("RadiusMax");
			}
			EndError();

			BeginError(Any(t => t.MaxSplitsInEditMode < 0 || t.MaxSplitsInEditMode > t.SplitDistances.Length));
			{
				DrawDefault("MaxSplitsInEditMode");
			}
			EndError();
		}
		if (EditorGUI.EndChangeCheck() == true)
		{
			Each(t => t.MarkMeshAsDirty());
		}

		EditorGUI.BeginChangeCheck();
		{
			DrawDefault("MaxColliderDepth");
		}
		if (EditorGUI.EndChangeCheck() == true)
		{
			Each(t => t.MarkStateAsDirty());
		}

		DrawDefault("SplitDistances");

		DrawDefault("Material");

		DrawDefault("Corona");

		RequireObserver();
	}
}

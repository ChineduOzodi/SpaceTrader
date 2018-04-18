using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtSnapToTerrain))]
public class SgtSnapToTerrain_Editor : SgtEditor<SgtSnapToTerrain>
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.Terrain == null));
		{
			DrawDefault("Terrain");
		}
		EndError();

		Separator();

		DrawDefault("SnapPosition");

		if (Any(t => t.SnapPosition == true))
		{
			BeginIndent();
			{
				DrawDefault("SnapOffset");

				DrawDefault("SnapMoveDampening");
			}
			EndIndent();

			Separator();
		}

		DrawDefault("SnapRotation");

		if (Any(t => t.SnapRotation == true))
		{
			BeginIndent();
			{
				DrawDefault("SnapRightDistance");

				DrawDefault("SnapForwardDistance");

				DrawDefault("SnapTurnDampening");
			}
			EndIndent();
		}
	}
}

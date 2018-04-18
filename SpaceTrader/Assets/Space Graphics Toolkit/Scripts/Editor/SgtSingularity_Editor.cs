using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtSingularity))]
public class SgtSingularity_Editor : SgtEditor<SgtSingularity>
{
	protected override void OnInspector()
	{
		DrawDefault("Color");
		
		BeginError(Any(t => t.Brightness < 0.0f));
		{
			DrawDefault("Brightness");
		}
		EndError();
		
		DrawDefault("RenderQueue");
		
		DrawDefault("RenderQueueOffset");
		
		Separator();
		
		BeginError(Any(t => t.Power < 0.0f));
		{
			DrawDefault("Power");
		}
		EndError();
		
		BeginError(Any(t => t.EdgePower < 0.0f));
		{
			DrawDefault("EdgePower");
		}
		EndError();
		
		Separator();
		
		DrawDefault("Hole");
		
		if (Any(t => t.Hole == true))
		{
			DrawDefault("HoleSize");
			
			BeginError(Any(t => t.HolePower < 0.0f));
			{
				DrawDefault("HolePower");
			}
			EndError();
		}
		
		Separator();
		
		BeginError(Any(t => t.Meshes.Count == 0 || t.Meshes.FindIndex(m => m == null) != -1));
		{
			DrawDefault("Meshes");
		}
		EndError();
	}
}
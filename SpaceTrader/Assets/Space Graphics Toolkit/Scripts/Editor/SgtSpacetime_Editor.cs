using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtSpacetime))]
public class SgtSpacetime_Editor : SgtEditor<SgtSpacetime>
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
		
		BeginError(Any(t => t.MainTex == null));
		{
			DrawDefault("MainTex");
		}
		EndError();
		
		BeginError(Any(t => t.Tile <= 0));
		{
			DrawDefault("Tile");
		}
		EndError();
		
		DrawDefault("Effect");
		
		BeginIndent();
		{
			DrawDefault("Accumulate");
			
			if (Any(t => t.Effect == SgtSpacetimeEffect.Pinch))
			{
				BeginError(Any(t => t.Power < 0.0f));
				{
					DrawDefault("Power");
				}
				EndError();
			}
		}
		EndIndent();
		
		if (Any(t => t.Effect == SgtSpacetimeEffect.Offset))
		{
			BeginIndent();
			{
				DrawDefault("Offset");
			}
			EndIndent();
		}
		
		Separator();
		
		BeginError(Any(t => t.Renderers.Count == 0 || t.Renderers.Exists(r => r == null) == true));
		{
			DrawDefault("Renderers");
		}
		EndError();
		
		Separator();
		
		DrawDefault("UseAllWells");
		
		BeginIndent();
		{
			if (Any(t => t.UseAllWells == true))
			{
				DrawDefault("RequireSameLayer");
				DrawDefault("RequireSameTag");
				DrawDefault("RequireNameContains");
			}
			
			if (Any(t => t.UseAllWells == false))
			{
				BeginError(Any(t => t.Wells.Count == 0 || t.Wells.Exists(r => r == null) == true));
				{
					DrawDefault("Wells");
				}
				EndError();
			}
		}
		EndIndent();
	}
}
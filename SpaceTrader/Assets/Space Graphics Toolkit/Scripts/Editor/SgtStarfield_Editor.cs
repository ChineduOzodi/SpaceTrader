using UnityEngine;
using UnityEditor;

public class SgtStarfield_Editor<T> : SgtEditor<T>
	where T : SgtStarfield
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
		
		DrawDefault("Age");
		
		DrawDefault("TimeScale");
		
		DrawDefault("Softness");
		
		if (Any(t => t.Softness > 0.0f))
		{
			foreach (var camera in Camera.allCameras)
			{
				if (camera.depthTextureMode == DepthTextureMode.None)
				{
					EditorGUILayout.HelpBox("You have enabled soft particles, but none of your cameras write depth textures.", MessageType.Error);
				}
			}
		}
		
		DrawDefault("AutoRegenerate");
		
		if (Any(t => t.AutoRegenerate == false))
		{
			if (Button("Regenerate") == true)
			{
				Each(t => t.Regenerate());
			}
		}
		
		Separator();
		
		DrawDefault("FadeNear");
		
		if (Any(t => t.FadeNear == true))
		{
			BeginIndent();
			{
				BeginError(Any(t => t.FadeNearRadius < 0.0f));
				{
					DrawDefault("FadeNearRadius");
				}
				EndError();
				
				BeginError(Any(t => t.FadeNearThickness <= 0.0f));
				{
					DrawDefault("FadeNearThickness");
				}
				EndError();
			}
			EndIndent();
		}
		
		DrawDefault("FadeFar");
		
		if (Any(t => t.FadeFar == true))
		{
			BeginIndent();
			{
				BeginError(Any(t => t.FadeFarRadius < 0.0f));
				{
					DrawDefault("FadeFarRadius");
				}
				EndError();
				
				BeginError(Any(t => t.FadeFarThickness <= 0.0f));
				{
					DrawDefault("FadeFarThickness");
				}
				EndError();
			}
			EndIndent();
		}
		
		if (Any(t => t is SgtWrappedStarfield == false))
		{
			DrawDefault("FollowObservers");
		}
		
		DrawDefault("StretchToObservers");
		
		if (Any(t => t.StretchToObservers == true))
		{
			BeginIndent();
			{
				BeginError(Any(t => t.StretchScale <= 0.0f));
				{
					DrawDefault("StretchScale");
				}
				EndError();
				
				DrawDefault("StretchOverride");
				
				if (Any(t => t.StretchOverride == true))
				{
					DrawDefault("StretchVector");
				}
			}
			EndIndent();
		}
		
		DrawDefault("AllowPulse");
		
		Separator();
	}
}
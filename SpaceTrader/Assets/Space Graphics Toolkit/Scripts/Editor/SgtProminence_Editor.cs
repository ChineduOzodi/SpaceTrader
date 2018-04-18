using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtProminence))]
public class SgtProminence_Editor : SgtEditor<SgtProminence>
{
	protected override void OnInspector()
	{
		DrawDefault("Color");
		
		DrawDefault("Brightness");
		
		DrawDefault("RenderQueue");
		
		DrawDefault("RenderQueueOffset");
		
		Separator();
		
		BeginError(Any(t => t.MainTex == null));
		{
			DrawDefault("MainTex");
		}
		EndError();
		
		DrawDefault("ObserverOffset");
		
		EditorGUI.BeginChangeCheck();
		{
			DrawDefault("Seed");
			
			BeginError(Any(t => t.PlaneCount < 1));
			{
				DrawDefault("PlaneCount");
			}
			EndError();
			
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
			
			BeginError(Any(t => t.Detail < 3));
			{
				DrawDefault("Detail");
			}
			EndError();
		}
		if (EditorGUI.EndChangeCheck() == true)
		{
			Each(t => t.MarkAsDirty());
		}
		
		Separator();
		
		DrawDefault("FadeEdge");
		
		if (Any(t => t.FadeEdge == true))
		{
			BeginIndent();
			{
				BeginError(Any(t => t.FadePower < 0.0f));
				{
					DrawDefault("FadePower");
				}
				EndError();
			}
			EndIndent();
		}
		
		DrawDefault("ClipNear");
		
		if (Any(t => t.ClipNear == true))
		{
			BeginIndent();
			{
				BeginError(Any(t => t.ClipPower < 0.0f));
				{
					DrawDefault("ClipPower");
				}
				EndError();
			}
			EndIndent();
		}
		
		if (Any(t => t.ObserverOffset != 0.0f))
		{
			RequireObserver();
		}
	}
}
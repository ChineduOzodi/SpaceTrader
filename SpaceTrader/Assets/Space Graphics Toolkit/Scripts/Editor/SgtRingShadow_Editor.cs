using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtRingShadow))]
public class SgtRingShadow_Editor : SgtEditor<SgtRingShadow>
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.Light == null));
		{
			DrawDefault("Light");
		}
		EndError();
		
		BeginError(Any(t => t.Texture == null));
		{
			DrawDefault("Texture");
		}
		EndError();
		
		DrawDefault("Ring");
		
		EditorGUI.BeginDisabledGroup(Any(t => SgtHelper.Enabled(t.Ring)));
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
		}
		EditorGUI.EndDisabledGroup();
	}
}
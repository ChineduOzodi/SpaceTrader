using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtSphereShadow))]
public class SgtSphereShadow_Editor : SgtEditor<SgtSphereShadow>
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.Light == null));
		{
			DrawDefault("Light");
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
		
		EditorGUI.BeginChangeCheck();
		{
			DrawDefault("PenumbraBrightness");
			
			DrawDefault("PenumbraColor");
		}
		if (EditorGUI.EndChangeCheck() == true)
		{
			Each(t => t.MarkLutAsDirty());
		}
	}
}
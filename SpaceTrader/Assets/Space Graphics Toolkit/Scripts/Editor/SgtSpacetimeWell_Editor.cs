using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtSpacetimeWell))]
public class SgtSpacetimeWell_Editor : SgtEditor<SgtSpacetimeWell>
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.Radius < 0.0f));
		{
			DrawDefault("Radius");
		}
		EndError();
		
		DrawDefault("Age");
		
		DrawDefault("Oscillate");
		
		if (Any(t => t.Oscillate == true))
		{
			BeginIndent();
			{
				BeginError(Any(t => t.Amplitude <= 0.0f));
				{
					DrawDefault("Amplitude");
				}
				EndError();
				
				BeginError(Any(t => t.Frequency <= 0.0f));
				{
					DrawDefault("Frequency");
				}
				EndError();
				
				DrawDefault("Offset");
			}
			EndIndent();
		}
		
		if (Any(t => t.Oscillate == false))
		{
			DrawDefault("Strength");
		}
	}
}
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtThruster))]
public class SgtThruster_Editor : SgtEditor<SgtThruster>
{
	protected override void OnInspector()
	{
		DrawDefault("Throttle");
		
		DrawDefault("Dampening");
		
		DrawDefault("Age");
		
		DrawDefault("TimeScale");
		
		DrawDefault("Flicker");
		
		Separator();
		
		DrawDefault("Rigidbody");
		
		if (Any(t => t.Rigidbody != null))
		{
			DrawDefault("ForceType");
			
			DrawDefault("ForceMode");
			
			DrawDefault("ForceMagnitude");
		}
		
		Separator();
		
		DrawDefault("FlameSprite");
		
		DrawDefault("FlameScale");
		
		Separator();
		
		DrawDefault("FlareSprite");
		
		DrawDefault("FlareScale");
		
		DrawDefault("FlareMask");
		
		RequireObserver();
	}
}
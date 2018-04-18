using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtThrusterFlare))]
public class SgtThrusterFlare_Editor : SgtEditor<SgtThrusterFlare>
{
	protected override void OnInspector()
	{
		DrawDefault("CurrentScale");
		
		DrawDefault("Dampening");
	}
}
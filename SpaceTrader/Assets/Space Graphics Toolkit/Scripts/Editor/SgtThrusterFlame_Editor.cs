using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtThrusterFlame))]
public class SgtThrusterFlame_Editor : SgtEditor<SgtThrusterFlame>
{
	protected override void OnInspector()
	{
		DrawDefault("CurrentScale");
	}
}
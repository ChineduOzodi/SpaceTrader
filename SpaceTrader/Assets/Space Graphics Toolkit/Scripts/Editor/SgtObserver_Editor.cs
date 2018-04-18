using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtObserver))]
public class SgtObserver_Editor : SgtEditor<SgtObserver>
{
	protected override void OnInspector()
	{
		DrawDefault("RollAngle");
	}
}
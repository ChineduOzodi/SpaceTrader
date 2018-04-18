using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SgtSeedAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SgtSeedAttribute))]
public class SgtSeedDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (SgtHelper.BaseRectSet == true)
		{
			position.x     = SgtHelper.BaseRect.x;
			position.width = SgtHelper.BaseRect.width;
		}
		
		var rect1 = position; rect1.xMax = position.xMax - 20;
		var rect2 = position; rect2.xMin = position.xMax - 18;
		
		EditorGUI.PropertyField(rect1, property, label);
		
		if (GUI.Button(rect2, "R") == true)
		{
			property.intValue = Random.Range(int.MinValue, int.MaxValue);
		}
	}
}
#endif
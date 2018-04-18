using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SgtRangeAttribute : PropertyAttribute
{
	public float Min;
	
	public float Max;
	
	public SgtRangeAttribute(float newMin, float newMax)
	{
		Min = newMin;
		Max = newMax;
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SgtRangeAttribute))]
public class SgtRangeDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (SgtHelper.BaseRectSet == true)
		{
			position.x     = SgtHelper.BaseRect.x;
			position.width = SgtHelper.BaseRect.width;
		}
		
		var Attribute = (SgtRangeAttribute)attribute;
		
		EditorGUIUtility.LookLikeControls();
		
		switch (property.propertyType)
		{
			case SerializedPropertyType.Float:
			{
				EditorGUI.Slider(position, property, Attribute.Min, Attribute.Max, label);
			}
			break;
			
			case SerializedPropertyType.Integer:
			{
				EditorGUI.IntSlider(position, property, (int)Attribute.Min, (int)Attribute.Max, label);
			}
			break;
		}
	}
}
#endif
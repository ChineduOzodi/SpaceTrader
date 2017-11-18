namespace Sirenix.OdinInspector.Demos
{
	using UnityEngine;
	public class PropertyTooltipExamples : MonoBehaviour
	{
		[InfoBox("PropertyTooltip is used to add tooltips to properties in the inspector.\nPropertyTooltip can also be applied to properties and methods, unlike Unity's Tooltip attribute.")]
		[PropertyTooltip("This is tooltip on an int property.")]
		public int MyInt;
	}
}
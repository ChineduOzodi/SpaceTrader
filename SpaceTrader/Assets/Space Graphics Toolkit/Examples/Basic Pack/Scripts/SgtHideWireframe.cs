using UnityEngine;

// This component will hide all children wireframes in edit mode
[ExecuteInEditMode]
public class SgtHideWireframe : MonoBehaviour
{
#if UNITY_EDITOR
	protected virtual void Update()
	{
		var renderers = GetComponentsInChildren<Renderer>();
		
		for (var i = renderers.Length - 1; i >= 0; i--)
		{
			UnityEditor.EditorUtility.SetSelectedWireframeHidden(renderers[i], true);
		}
	}
	
	protected virtual void OnDisable()
	{
		var renderers = GetComponentsInChildren<Renderer>();
		
		for (var i = renderers.Length - 1; i >= 0; i--)
		{
			UnityEditor.EditorUtility.SetSelectedWireframeHidden(renderers[i], false);
		}
	}
#endif
}
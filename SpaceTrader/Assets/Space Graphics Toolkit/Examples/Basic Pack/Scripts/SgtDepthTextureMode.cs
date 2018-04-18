using UnityEngine;

// This component allows you to control a Camera component's depthTextureMode setting.
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Depth Texture Mode")]
public class SgtDepthTextureMode : MonoBehaviour
{
	public DepthTextureMode DepthMode = DepthTextureMode.None;

	private Camera thisCamera;

	protected virtual void Update()
	{
		if (thisCamera == null) thisCamera = GetComponent<Camera>();

		thisCamera.depthTextureMode = DepthMode;
	}
}

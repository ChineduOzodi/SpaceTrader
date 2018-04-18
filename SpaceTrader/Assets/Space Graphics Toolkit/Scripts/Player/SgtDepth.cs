using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Depth")]
public class SgtDepth : MonoBehaviour
{
	public static List<SgtDepth> AllDepthDrawers = new List<SgtDepth>();
	
	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;
	
	public int RenderQueueOffset = 1;
	
	public List<Renderer> Renderers = new List<Renderer>();
	
	[SerializeField]
	private Material depthMaterial;
	
	public Material DepthMaterial
	{
		get
		{
			if (depthMaterial == null)
			{
				UpdateMaterial();
			}
			
			return depthMaterial;
		}
	}
	
	public void AddRenderer(Renderer renderer)
	{
		if (renderer != null)
		{
			if (Renderers.Contains(renderer) == false)
			{
				if (depthMaterial != null)
				{
					SgtHelper.AddMaterial(renderer, depthMaterial);
				}
				
				Renderers.Add(renderer);
			}
		}
	}
	
	public void RemoveRenderer(Renderer renderer)
	{
		if (renderer != null)
		{
			if (depthMaterial != null)
			{
				SgtHelper.RemoveMaterial(renderer, depthMaterial);
			}
			
			Renderers.Add(renderer);
		}
	}
	
	protected virtual void OnEnable()
	{
#if UNITY_EDITOR
		if (AllDepthDrawers.Count == 0)
		{
			SgtHelper.RepaintAll();
		}
#endif
		AllDepthDrawers.Add(this);
	}
	
	protected virtual void OnDisable()
	{
		AllDepthDrawers.Remove(this);
		
		RemoveMaterial();
	}
	
	protected virtual void OnDestroy()
	{
		RemoveMaterial();
		
		SgtHelper.Destroy(depthMaterial);
	}
	
	protected virtual void LateUpdate()
	{
		UpdateMaterial();
		
		for (var i = Renderers.Count - 1; i >= 0; i--)
		{
			var renderer = Renderers[i];
			
			SgtHelper.BeginStealthSet(renderer);
			{
				SgtHelper.AddMaterial(renderer, depthMaterial);
			}
			SgtHelper.EndStealthSet();
		}
	}
	
	private void UpdateMaterial()
	{
		if (depthMaterial == null) depthMaterial = SgtHelper.CreateTempMaterial(SgtHelper.ShaderNamePrefix + "Depth");
		
		depthMaterial.renderQueue = (int)RenderQueue + RenderQueueOffset;
	}
	
	private void RemoveMaterial()
	{
		for (var i = Renderers.Count - 1; i >= 0; i--)
		{
			var renderer = Renderers[i];
			
			SgtHelper.BeginStealthSet(renderer);
			{
				SgtHelper.RemoveMaterial(renderer, depthMaterial);
			}
			SgtHelper.EndStealthSet();
		}
	}
}
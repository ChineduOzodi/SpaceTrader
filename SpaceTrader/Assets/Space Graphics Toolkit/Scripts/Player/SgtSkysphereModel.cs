using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class SgtSkysphereModel : MonoBehaviour
{
	public SgtSkysphere Skysphere;

	public MeshFilter MeshFilter;

	public MeshRenderer MeshRenderer;

	[System.NonSerialized]
	public bool TempSet;

	[System.NonSerialized]
	public Vector3 TempPosition;

	public void ManualUpdate(Mesh mesh, Material material)
	{
		if (Skysphere != null)
		{
			if (MeshFilter == null) MeshFilter = gameObject.AddComponent<MeshFilter>();

			if (MeshRenderer == null) MeshRenderer = gameObject.AddComponent<MeshRenderer>();

			if (MeshFilter.sharedMesh != mesh)
			{
				SgtHelper.BeginStealthSet(MeshFilter);
				{
					MeshFilter.sharedMesh = mesh;
				}
				SgtHelper.EndStealthSet();
			}

			if (MeshRenderer.sharedMaterial != material)
			{
				SgtHelper.BeginStealthSet(MeshRenderer);
				{
					MeshRenderer.sharedMaterial = material;
				}
				SgtHelper.EndStealthSet();
			}
		}
	}

	public static SgtSkysphereModel Create(SgtSkysphere skysphere)
	{
		var model = SgtComponentPool<SgtSkysphereModel>.Pop("Model", skysphere.gameObject.layer, skysphere.transform);

		model.Skysphere = skysphere;

		return model;
	}

	public static void Pool(SgtSkysphereModel model)
	{
		if (model != null)
		{
			model.Skysphere = null;

			SgtComponentPool<SgtSkysphereModel>.Add(model);
		}
	}

	public static void MarkForDestruction(SgtSkysphereModel model)
	{
		if (model != null)
		{
			model.Skysphere = null;

			model.gameObject.SetActive(true);
		}
	}

	protected virtual void Update()
	{
		if (Skysphere == null)
		{
			Pool(this);
		}
	}
}

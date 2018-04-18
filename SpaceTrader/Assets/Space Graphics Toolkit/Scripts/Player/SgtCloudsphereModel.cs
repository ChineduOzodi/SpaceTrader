using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class SgtCloudsphereModel : MonoBehaviour
{
	public SgtCloudsphere Cloudsphere;

	public MeshFilter MeshFilter;

	public MeshRenderer MeshRenderer;

	[System.NonSerialized]
	public bool TempSet;

	[System.NonSerialized]
	public Vector3 TempPosition;

	public void ManualUpdate(Mesh mesh, Material material, float scale)
	{
		if (Cloudsphere != null)
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

			SgtHelper.SetLocalScale(transform, scale);
		}
	}

	public static SgtCloudsphereModel Create(SgtCloudsphere cloudsphere)
	{
		var model = SgtComponentPool<SgtCloudsphereModel>.Pop("Model", cloudsphere.gameObject.layer, cloudsphere.transform);

		model.Cloudsphere = cloudsphere;

		return model;
	}

	public static void Pool(SgtCloudsphereModel model)
	{
		if (model != null)
		{
			model.Cloudsphere = null;

			SgtComponentPool<SgtCloudsphereModel>.Add(model);
		}
	}

	public static void MarkForDestruction(SgtCloudsphereModel model)
	{
		if (model != null)
		{
			model.Cloudsphere = null;

			model.gameObject.SetActive(true);
		}
	}

	protected virtual void Update()
	{
		if (Cloudsphere == null)
		{
			Pool(this);
		}
	}
}

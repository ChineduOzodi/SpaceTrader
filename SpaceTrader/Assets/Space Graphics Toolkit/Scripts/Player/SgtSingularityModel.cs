using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class SgtSingularityModel : MonoBehaviour
{
	public SgtSingularity Singularity;

	public MeshFilter MeshFilter;

	public MeshRenderer MeshRenderer;

	public void ManualUpdate(Mesh mesh, Material material)
	{
		if (Singularity != null)
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

	public static SgtSingularityModel Create(SgtSingularity singularity)
	{
		var model = SgtComponentPool<SgtSingularityModel>.Pop("Model", singularity.gameObject.layer, singularity.transform);

		model.Singularity = singularity;

		return model;
	}

	public static void Pool(SgtSingularityModel model)
	{
		if (model != null)
		{
			model.Singularity = null;

			SgtComponentPool<SgtSingularityModel>.Add(model);
		}
	}

	public static void MarkForDestruction(SgtSingularityModel model)
	{
		if (model != null)
		{
			model.Singularity = null;

			model.gameObject.SetActive(true);
		}
	}

	protected virtual void Update()
	{
		if (Singularity == null)
		{
			Pool(this);
		}
	}
}

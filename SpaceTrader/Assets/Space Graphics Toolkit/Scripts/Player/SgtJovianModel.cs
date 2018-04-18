using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class SgtJovianModel : MonoBehaviour
{
	public SgtJovian Jovian;

	public MeshFilter MeshFilter;

	public MeshRenderer MeshRenderer;

	public void ManualUpdate(Mesh mesh, Material material)
	{
		if (Jovian != null)
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

	public static SgtJovianModel Create(SgtJovian jovian)
	{
		var model = SgtComponentPool<SgtJovianModel>.Pop("Model", jovian.gameObject.layer, jovian.transform);

		model.Jovian = jovian;

		return model;
	}

	public static void Pool(SgtJovianModel model)
	{
		if (model != null)
		{
			model.Jovian = null;

			SgtComponentPool<SgtJovianModel>.Add(model);
		}
	}

	public static void MarkForDestruction(SgtJovianModel model)
	{
		if (model != null)
		{
			model.Jovian = null;

			model.gameObject.SetActive(true);
		}
	}

	protected virtual void Update()
	{
		if (Jovian == null)
		{
			Pool(this);
		}
	}
}

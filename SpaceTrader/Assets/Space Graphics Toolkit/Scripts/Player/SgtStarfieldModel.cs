using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class SgtStarfieldModel : MonoBehaviour
{
	public SgtStarfieldGroup Group;

	public MeshFilter MeshFilter;

	public MeshRenderer MeshRenderer;

	[System.NonSerialized]
	public Mesh Mesh;

	public void PoolMeshNow()
	{
		Mesh = SgtObjectPool<Mesh>.Add(Mesh, m => m.Clear());
	}

	public void ManualUpdate()
	{
		if (Group != null)
		{
			if (MeshFilter == null) MeshFilter = gameObject.AddComponent<MeshFilter>();

			if (MeshRenderer == null) MeshRenderer = gameObject.AddComponent<MeshRenderer>();

			if (MeshFilter.sharedMesh != Mesh)
			{
				SgtHelper.BeginStealthSet(MeshFilter);
				{
					MeshFilter.sharedMesh = Mesh;
				}
				SgtHelper.EndStealthSet();
			}

			if (MeshRenderer.sharedMaterial != Group.Material)
			{
				SgtHelper.BeginStealthSet(MeshRenderer);
				{
					MeshRenderer.sharedMaterial = Group.Material;
				}
				SgtHelper.EndStealthSet();
			}
		}
	}

	public static SgtStarfieldModel Create(SgtStarfieldGroup group)
	{
		var model = SgtComponentPool<SgtStarfieldModel>.Pop("Model", group.gameObject.layer, group.transform);

		model.Group = group;

		group.Models.Add(model);

		return model;
	}

	public static void Pool(SgtStarfieldModel model)
	{
		if (model != null)
		{
			model.Group = null;

			model.PoolMeshNow();

			SgtComponentPool<SgtStarfieldModel>.Add(model);
		}
	}

	public static void MarkForDestruction(SgtStarfieldModel model)
	{
		if (model != null)
		{
			model.Group = null;

			model.gameObject.SetActive(true);
		}
	}

	protected virtual void OnDestroy()
	{
		PoolMeshNow();
	}

	protected virtual void Update()
	{
		if (Group == null)
		{
			Pool(this);
		}
	}
}

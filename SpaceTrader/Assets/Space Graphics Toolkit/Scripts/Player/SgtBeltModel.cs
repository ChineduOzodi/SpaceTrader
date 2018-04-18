using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class SgtBeltModel : MonoBehaviour
{
	public SgtBeltGroup Group;

	public MeshRenderer MeshRenderer;

	public MeshFilter MeshFilter;

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

	public static SgtBeltModel Create(SgtBeltGroup group)
	{
		var model = SgtComponentPool<SgtBeltModel>.Pop("Model", group.gameObject.layer, group.transform);

		model.Group = group;

		group.Models.Add(model);

		return model;
	}

	public static void Pool(SgtBeltModel model)
	{
		if (model != null)
		{
			model.Group = null;

			model.PoolMeshNow();

			SgtComponentPool<SgtBeltModel>.Add(model);
		}
	}

	public static void MarkForDestruction(SgtBeltModel model)
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

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class SgtProminencePlane : MonoBehaviour
{
	public SgtProminence Prominence;

	public MeshFilter MeshFilter;

	public MeshRenderer MeshRenderer;

	[System.NonSerialized]
	public bool TempSet;

	[System.NonSerialized]
	public Vector3 TempPosition;

	public void ManualUpdate(Mesh mesh, Material material, Quaternion rotation)
	{
		if (Prominence != null)
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

			SgtHelper.SetLocalRotation(transform, rotation);
		}
	}

	public static SgtProminencePlane Create(SgtProminence prominence)
	{
		var plane = SgtComponentPool<SgtProminencePlane>.Pop("Plane", prominence.gameObject.layer, prominence.transform);

		plane.Prominence = prominence;

		return plane;
	}

	public static void Pool(SgtProminencePlane plane)
	{
		if (plane != null)
		{
			plane.Prominence = null;

			SgtComponentPool<SgtProminencePlane>.Add(plane);
		}
	}

	public static void MarkForDestruction(SgtProminencePlane plane)
	{
		if (plane != null)
		{
			plane.Prominence = null;

			plane.gameObject.SetActive(true);
		}
	}

	protected virtual void Update()
	{
		if (Prominence == null)
		{
			Pool(this);
		}
	}
}

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class SgtRingSegment : MonoBehaviour
{
	public SgtRing Ring;

	public MeshFilter MeshFilter;

	public MeshRenderer MeshRenderer;

	public void ManualUpdate(Mesh mesh, Material material, Quaternion rotation)
	{
		if (Ring != null)
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

			SgtHelper.BeginStealthSet(transform);
			{
				SgtHelper.SetLocalRotation(transform, rotation);
			}
			SgtHelper.EndStealthSet();
		}
	}

	public static SgtRingSegment Create(SgtRing ring)
	{
		var segment = SgtComponentPool<SgtRingSegment>.Pop("Segment", ring.gameObject.layer, ring.transform);

		segment.Ring = ring;

		return segment;
	}

	public static void Pool(SgtRingSegment segment)
	{
		if (segment != null)
		{
			segment.Ring = null;

			SgtComponentPool<SgtRingSegment>.Add(segment);
		}
	}

	public static void MarkForDestruction(SgtRingSegment segment)
	{
		if (segment != null)
		{
			segment.Ring = null;

			segment.gameObject.SetActive(true);
		}
	}

	protected virtual void Update()
	{
		if (Ring == null)
		{
			Pool(this);
		}
	}
}

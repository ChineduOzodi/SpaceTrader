using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class SgtCoronaOuter : MonoBehaviour
{
	public SgtCorona Corona;

	public MeshFilter MeshFilter;

	public MeshRenderer MeshRenderer;

	public void ManualUpdate(Mesh mesh, Material outerMaterial, float outerScale)
	{
		if (Corona != null)
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

			if (MeshRenderer.sharedMaterial != outerMaterial)
			{
				SgtHelper.BeginStealthSet(MeshRenderer);
				{
					MeshRenderer.sharedMaterial = outerMaterial;
				}
				SgtHelper.EndStealthSet();
			}

			SgtHelper.SetLocalScale(transform, outerScale);
		}
	}

	public static SgtCoronaOuter Create(SgtCorona corona)
	{
		var outer = SgtComponentPool<SgtCoronaOuter>.Pop("Outer", corona.gameObject.layer, corona.transform);

		outer.Corona = corona;

		return outer;
	}

	public static void Pool(SgtCoronaOuter outer)
	{
		if (outer != null)
		{
			outer.Corona = null;

			SgtComponentPool<SgtCoronaOuter>.Add(outer);
		}
	}

	public static void MarkForDestruction(SgtCoronaOuter outer)
	{
		if (outer != null)
		{
			outer.Corona = null;

			outer.gameObject.SetActive(true);
		}
	}

	protected virtual void Update()
	{
		if (Corona == null)
		{
			Pool(this);
		}
	}
}

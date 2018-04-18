using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class SgtPatch : MonoBehaviour
{
	public SgtTerrain Terrain;

	public SgtPatch Parent;

	public Material Material;

	public Material FinalMaterial;

	public int Depth;

	public Vector3 PointBL;

	public Vector3 PointBR;

	public Vector3 PointTL;

	public Vector3 PointTR;

	public Vector2 CoordBL;

	public Vector2 CoordBR;

	public Vector2 CoordTL;

	public Vector2 CoordTR;

	public SgtPatch ChildBL;

	public SgtPatch ChildBR;

	public SgtPatch ChildTL;

	public SgtPatch ChildTR;

	public Vector3 MeshCenter;

	[System.NonSerialized]
	public Mesh Mesh;

	[SerializeField]
	private MeshFilter meshFilter;

	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private MeshCollider meshCollider;

	private float updateTime;

	private static Material[] sharedMaterials1 = new Material[1];

	private static Material[] sharedMaterials2 = new Material[2];

	[System.NonSerialized]
	private bool materialDirty;

	public bool ChildrenExist
	{
		get
		{
			return ChildBL != null || ChildBR != null || ChildTL != null || ChildTR != null;
		}
	}

	public void MarkMaterialAsDirty()
	{
#if UNITY_EDITOR
		SgtHelper.SetDirty(this);
#endif
		materialDirty = true;
	}

	public void UpdateStates()
	{
		UpdateState();

		if (ChildBL != null) ChildBL.UpdateStates();
		if (ChildBR != null) ChildBR.UpdateStates();
		if (ChildTL != null) ChildTL.UpdateStates();
		if (ChildTR != null) ChildTR.UpdateStates();
	}

	public void UpdateState()
	{
		if (Material != null)
		{
			FinalMaterial = Material;
		}
		else
		{
			if (Parent != null)
			{
				FinalMaterial = Parent.FinalMaterial;
			}
			else
			{
				FinalMaterial = Terrain.Material;
			}
		}

		if (ChildrenExist == true)
		{
			if (meshRenderer != null) SgtHelper.SetEnabled(meshRenderer, false);

			// Has colliders if it's the last depth
			UpdateCollider(Depth + 1 == Terrain.MaxColliderDepth);
		}
		else
		{
			if (Mesh == null) Terrain.RebuildPatch(this);

			if (meshRenderer == null) meshRenderer = SgtHelper.GetOrAddComponent<MeshRenderer>(gameObject);

			if (meshFilter == null) meshFilter = SgtHelper.GetOrAddComponent<MeshFilter>(gameObject);

			SgtHelper.SetEnabled(meshRenderer, true);

			if (meshFilter.sharedMesh != Mesh)
			{
				SgtHelper.BeginStealthSet(meshFilter);
				{
					meshFilter.sharedMesh = Mesh;
				}
				SgtHelper.EndStealthSet();
			}

			UpdateMaterials();

			// Has colliders if it's under max depth
			UpdateCollider(Depth < Terrain.MaxColliderDepth);
		}
	}

	public void PoolMeshNow()
	{
		Mesh = SgtObjectPool<Mesh>.Add(Mesh, m => m.Clear());
	}

	public void RegenerateMeshes()
	{
		PoolMeshNow();
		UpdateState();

		if (ChildBL != null) ChildBL.RegenerateMeshes();
		if (ChildBR != null) ChildBR.RegenerateMeshes();
		if (ChildTL != null) ChildTL.RegenerateMeshes();
		if (ChildTR != null) ChildTR.RegenerateMeshes();
	}

	public void PoolMeshesNow()
	{
		PoolMeshNow();

		if (ChildBL != null) ChildBL.PoolMeshesNow();
		if (ChildBR != null) ChildBR.PoolMeshesNow();
		if (ChildTL != null) ChildTL.PoolMeshesNow();
		if (ChildTR != null) ChildTR.PoolMeshesNow();
	}

	public void UpdateSplitMerge()
	{
#if UNITY_EDITOR
		if (Application.isPlaying == false)
		{
			if (Depth + 1 > Terrain.MaxSplitsInEditMode)
			{
				Merge(); return;
			}
		}
#endif
		// Split distances reduced at runtime?
		if (Depth > Terrain.SplitDistances.Length)
		{
			Merge(); return;
		}

		if (Depth < Terrain.SplitDistances.Length)
		{
			var bestDistance  = float.PositiveInfinity;
			var splitDistance = Terrain.SplitDistances[Depth];

			// Go through all observers to find the closest
			for (var i = SgtObserver.AllObservers.Count - 1; i >= 0; i--)
			{
				var observer = SgtObserver.AllObservers[i];

				if (observer != null)
				{
					var localPosition = transform.InverseTransformPoint(observer.transform.position);
					var distance      = Vector3.Distance(MeshCenter, localPosition);

					if (distance < bestDistance)
					{
						bestDistance = distance;
					}
				}
			}

			// Too far?
			if (bestDistance > splitDistance * 1.1f)
			{
				Merge(); return;
			}

			// Too near?
			if (Depth < Terrain.SplitDistances.Length)
			{
				if (bestDistance < splitDistance * 0.9f)
				{
					Split();
				}
			}
		}
	}

	public static SgtPatch Create(string name, int layer, Transform parent)
	{
		var patch = SgtComponentPool<SgtPatch>.Pop(name, layer, parent);

		return patch;
	}

	public static SgtPatch Pool(SgtPatch patch)
	{
		if (patch != null)
		{
			patch.ChildBL = Pool(patch.ChildBL);
			patch.ChildBR = Pool(patch.ChildBR);
			patch.ChildTL = Pool(patch.ChildTL);
			patch.ChildTR = Pool(patch.ChildTR);

			patch.Terrain       = null;
			patch.Parent        = null;
			patch.Material      = null;
			patch.FinalMaterial = null;

			patch.PoolMeshNow();

			SgtComponentPool<SgtPatch>.Add(patch);
		}

		return null;
	}

	public static SgtPatch MarkForDestruction(SgtPatch patch)
	{
		if (patch != null)
		{
			patch.Terrain = null;

			patch.gameObject.SetActive(true);
		}

		return null;
	}

	protected virtual void Update()
	{
		if (Terrain != null)
		{
			if (SgtTerrain.TickOverbudget == true)
			{
				return;
			}

			UpdateMaterialDirty();

			updateTime -= Time.deltaTime;
#if UNITY_EDITOR
			if (Application.isPlaying == false)
			{
				updateTime = 0.0f;
			}
#endif
			if (updateTime <= 0.0f)
			{
				updateTime = Random.Range(0.5f, 1.0f);

				if (Terrain != null)
				{
					UpdateSplitMerge();
				}
				else
				{
					SgtHelper.Destroy(gameObject);
				}
			}
		}
		else
		{
			Pool(this);
		}
	}

	protected virtual void OnDestroy()
	{
		PoolMeshNow();
	}

	private void UpdateMaterialDirty()
	{
		if (materialDirty == true)
		{
			materialDirty = false;

			UpdateStates();
		}
	}

	private void Split()
	{
		if (ChildrenExist == false)
		{
			var PointCC = (PointBL + PointTR) * 0.5f;
			var PointBC = (PointBL + PointBR) * 0.5f;
			var PointTC = (PointTL + PointTR) * 0.5f;
			var PointCL = (PointTL + PointBL) * 0.5f;
			var PointCR = (PointTR + PointBR) * 0.5f;

			var CoordCC = (CoordBL + CoordTR) * 0.5f;
			var CoordBC = (CoordBL + CoordBR) * 0.5f;
			var CoordTC = (CoordTL + CoordTR) * 0.5f;
			var CoordCL = (CoordTL + CoordBL) * 0.5f;
			var CoordCR = (CoordTR + CoordBR) * 0.5f;

			if (ChildBL == null) ChildBL = Terrain.CreatePatch("Bottom Left" , this, PointBL, PointBC, PointCL, PointCC, CoordBL, CoordBC, CoordCL, CoordCC, Depth + 1);
			if (ChildBR == null) ChildBR = Terrain.CreatePatch("Bottom Right", this, PointBC, PointBR, PointCC, PointCR, CoordBC, CoordBR, CoordCC, CoordCR, Depth + 1);
			if (ChildTL == null) ChildTL = Terrain.CreatePatch("Top Left"    , this, PointCL, PointCC, PointTL, PointTC, CoordCL, CoordCC, CoordTL, CoordTC, Depth + 1);
			if (ChildTR == null) ChildTR = Terrain.CreatePatch("Top Right"   , this, PointCC, PointCR, PointTC, PointTR, CoordCC, CoordCR, CoordTC, CoordTR, Depth + 1);

			UpdateState();
		}
	}

	private void Merge()
	{
		if (ChildrenExist == true)
		{
			ChildBL = Pool(ChildBL);
			ChildBR = Pool(ChildBR);
			ChildTL = Pool(ChildTL);
			ChildTR = Pool(ChildTR);

			UpdateState();
		}
	}

	private void UpdateMaterials()
	{
		var coronaMaterial  = Terrain.CoronaMaterial;
		var sharedMaterials = meshRenderer.sharedMaterials;

		if (coronaMaterial != null)
		{
			if (sharedMaterials.Length != 2 || sharedMaterials[0] != FinalMaterial || sharedMaterials[1] != coronaMaterial)
			{
				sharedMaterials2[0] = FinalMaterial;
				sharedMaterials2[1] = coronaMaterial;

				SgtHelper.BeginStealthSet(meshRenderer);
				{
					meshRenderer.sharedMaterials = sharedMaterials2;
				}
				SgtHelper.EndStealthSet();
			}
		}
		else
		{
			if (sharedMaterials.Length != 1 || sharedMaterials[0] != FinalMaterial)
			{
				sharedMaterials1[0] = FinalMaterial;

				SgtHelper.BeginStealthSet(meshRenderer);
				{
					meshRenderer.sharedMaterials = sharedMaterials1;
				}
				SgtHelper.EndStealthSet();
			}
		}
	}

	private void UpdateCollider(bool enableColliders)
	{
		if (enableColliders == true)
		{
			if (Mesh == null) Terrain.RebuildPatch(this);

			if (meshCollider == null) meshCollider = SgtHelper.GetOrAddComponent<MeshCollider>(gameObject);

			if (meshCollider.sharedMesh != Mesh)
			{
				SgtHelper.BeginStealthSet(meshCollider);
				{
					meshCollider.sharedMesh = Mesh;
				}
				SgtHelper.EndStealthSet();
			}

			SgtHelper.SetEnabled(meshCollider, true);
		}
		else
		{
			SgtHelper.SetEnabled(meshCollider, false);
		}
	}
}

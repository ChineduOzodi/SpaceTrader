using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Terrain")]
public class SgtTerrain : MonoBehaviour
{
	public static List<SgtTerrain> AllTerrains = new List<SgtTerrain>();

	private static int ticksCount;

	private static bool ticksUsed;

	[Tooltip("The amount of rows & columns on each patch edge")]
	[SgtRange(1, 16)]
	public int Resolution = 5;

	#pragma warning disable 414
	[Tooltip("The amount of times the main patches can be split in edit mode")]
	public int MaxSplitsInEditMode = 0;

	[Tooltip("The maximum depth of the patches that get colliders")]
	public int MaxColliderDepth = 0;

	[Tooltip("The local distance between the patch and observer (camera) for the patch to split")]
	public float[] SplitDistances = new float[0];

	[Tooltip("The thickness of the patch skirt to hide LOD seams")]
	[SgtRange(0.0f, 1.0f)]
	public float SkirtThickness = 0.1f;

	[Tooltip("The inner radius of the terrain")]
	public float RadiusMin = 0.9f;

	[Tooltip("The outer radius of the terrain")]
	public float RadiusMax = 1.0f;

	[Tooltip("The base material applied to patches")]
	public Material Material;

	[Tooltip("The corona or atmosphere applied to the patches")]
	public SgtCorona Corona;

	public delegate void CalculateHeightDelegate(Vector3 localPosition, ref float height);

	public CalculateHeightDelegate OnCalculateHeight;

	public delegate void CalculateCoordDelegate(Vector3 localPosition, Vector3 center, ref Vector2 coord);

	public CalculateCoordDelegate OnCalculateCoord1;

	public CalculateCoordDelegate OnCalculateCoord2;

	[System.NonSerialized]
	private bool stateDirty;

	[System.NonSerialized]
	private bool meshDirty;

	[System.NonSerialized]
	private bool materialDirty;

	[System.NonSerialized]
	private Material expectedCoronaMaterial;

	[System.NonSerialized]
	private bool expectedCoronaMaterialSet;

	[SerializeField]
	private SgtPatch positiveX;

	[SerializeField]
	private SgtPatch positiveY;

	[SerializeField]
	private SgtPatch positiveZ;

	[SerializeField]
	private SgtPatch negativeX;

	[SerializeField]
	private SgtPatch negativeY;

	[SerializeField]
	private SgtPatch negativeZ;

	[System.NonSerialized]
	private Vector3[] positions;

	[System.NonSerialized]
	private Vector2[] coords1;

	[System.NonSerialized]
	private Vector2[] coords2;

	[System.NonSerialized]
	private Vector3[] normals;

	[System.NonSerialized]
	private Vector4[] tangents;

	[System.NonSerialized]
	private Vector3[] quadPoints;

	[System.NonSerialized]
	private Vector3[] quadNormals;

	[System.NonSerialized]
	private Vector3[] quadTangents;

	[System.NonSerialized]
	private int[] indices;

	public static bool TickOverbudget
	{
		get
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false)
			{
				return false;
			}
#endif
			if (ticksUsed == true)
			{
				var ticksElapsed = System.Environment.TickCount - ticksCount;

				if (ticksElapsed > 10)
				{
					return true;
				}
            }
			else
			{
				ticksUsed  = true;
				ticksCount = System.Environment.TickCount;
            }

			return false;
		}
	}

	public Material CoronaMaterial
	{
		get
		{
			if (Corona != null)
			{
				return Corona.InnerMaterial;
			}

			return null;
		}
	}

	public void MarkStateAsDirty()
	{
#if UNITY_EDITOR
		SgtHelper.SetDirty(this);
#endif
		stateDirty = true;
	}

	public void MarkMeshAsDirty()
	{
#if UNITY_EDITOR
		SgtHelper.SetDirty(this);
#endif
		meshDirty = true;
	}

	public void MarkMaterialAsDirty()
	{
#if UNITY_EDITOR
		SgtHelper.SetDirty(this);
#endif
		materialDirty = true;
	}

	// This will return the local surface position under the given local position
	public Vector3 GetSurfacePositionLocal(Vector3 localPosition, float offset = 0.0f)
	{
		var height = 0.5f;

		if (OnCalculateHeight != null) OnCalculateHeight(localPosition, ref height);

		height = RadiusMin + (RadiusMax - RadiusMin) * height + offset;

		return localPosition.normalized * height;
	}

	// This will return the world surface position under the given world position
	public Vector3 GetSurfacePositionWorld(Vector3 worldPosition, float offset = 0.0f)
	{
		var localPosition = transform.InverseTransformPoint(worldPosition);

		localPosition = GetSurfacePositionLocal(localPosition, offset);

		return transform.TransformPoint(localPosition);
	}

	// This will return the local surface normal under the given local position
	public Vector3 GetSurfaceNormalLocal(Vector3 localPosition, Vector3 localRight, Vector3 localForward)
	{
		var right       = GetSurfacePositionLocal(localPosition + localRight);
		var left        = GetSurfacePositionLocal(localPosition - localRight);
		var forward     = GetSurfacePositionLocal(localPosition + localForward);
		var back        = GetSurfacePositionLocal(localPosition - localForward);
		var rightLeft   = right   - left;
		var forwardBack = forward - back;

		return Vector3.Cross(forwardBack.normalized, rightLeft.normalized).normalized;
	}

	// This will return the world surface normal under the given world position, using 4 samples, whose distances are based on the right & forward vectors
	public Vector3 GetSurfaceNormalWorld(Vector3 worldPosition, Vector3 worldRight, Vector3 worldForward)
	{
		var localPosition = transform.InverseTransformPoint(worldPosition);
		var localRight    = transform.InverseTransformDirection(worldRight);
		var localForward  = transform.InverseTransformDirection(worldForward);
		var localNormal   = GetSurfaceNormalLocal(localPosition, localRight, localForward);

		return transform.TransformDirection(localNormal);
	}

	public Vector3 GetSurfaceNormalWorld(Vector3 worldPosition)
	{
		return (worldPosition - transform.position).normalized;
	}

	public SgtPatch CreatePatch(string name, SgtPatch parent, Vector3 pointBL, Vector3 pointBR, Vector3 pointTL, Vector3 pointTR, Vector3 coordBL, Vector3 coordBR, Vector3 coordTL, Vector3 coordTR, int depth)
	{
		var parentTransform = parent != null ? parent.transform : transform;
		var patch           = SgtPatch.Create(name, gameObject.layer, parentTransform);

		patch.Terrain = this;
		patch.Parent  = parent;
		patch.Depth   = depth;
		patch.PointBL = pointBL;
		patch.PointBR = pointBR;
		patch.PointTL = pointTL;
		patch.PointTR = pointTR;
		patch.CoordBL = coordBL;
		patch.CoordBR = coordBR;
		patch.CoordTL = coordTL;
		patch.CoordTR = coordTR;

		patch.UpdateState();

#if UNITY_EDITOR
		if (Application.isPlaying == false)
		{
			patch.UpdateSplitMerge();
		}
#endif

		return patch;
	}

	public void RebuildPatch(SgtPatch patch)
	{
		if (Resolution > 0)
		{
			var resAdd1      = Resolution + 1;
			var resAdd2      = Resolution + 2;
			var resAdd3      = Resolution + 3;
			var resRecip     = SgtHelper.Reciprocal(Resolution);
			var mainVerts    = resAdd1 * resAdd1;
			var skirtVerts   = resAdd1 * 4;
			var mainIndices  = Resolution * Resolution * 6;
			var skirtIndices = Resolution * 24;
			var mesh         = patch.Mesh;
			var vertex       = default(int);
			var vertex2      = default(int);
			var index        = default(int);

			if (   positions == null ||    positions.Length != mainVerts + skirtVerts ) positions    = new Vector3[mainVerts + skirtVerts];
			if (     coords1 == null ||      coords1.Length != mainVerts + skirtVerts ) coords1      = new Vector2[mainVerts + skirtVerts];
			if (     coords2 == null ||      coords2.Length != mainVerts + skirtVerts ) coords2      = new Vector2[mainVerts + skirtVerts];
			if (     normals == null ||      normals.Length != mainVerts + skirtVerts ) normals      = new Vector3[mainVerts + skirtVerts];
			if (    tangents == null ||     tangents.Length != mainVerts + skirtVerts ) tangents     = new Vector4[mainVerts + skirtVerts];
			if (  quadPoints == null ||   quadPoints.Length != resAdd3   * resAdd3    ) quadPoints   = new Vector3[resAdd3 * resAdd3];
			if ( quadNormals == null ||  quadNormals.Length != resAdd2   * resAdd2    ) quadNormals  = new Vector3[resAdd2 * resAdd2];
			if (quadTangents == null || quadTangents.Length != resAdd2   * resAdd2    ) quadTangents = new Vector3[resAdd2 * resAdd2];

			// Go through all vertices, but extend the borders by one
			for (var y = -1; y < resAdd2; y++)
			{
				for (var x = -1; x < resAdd2; x++)
				{
					var u      = x * resRecip;
					var v      = y * resRecip;
					var pointB = Lerp3(patch.PointBL, patch.PointBR, u);
					var pointT = Lerp3(patch.PointTL, patch.PointTR, u);
					var point  = GetSurfacePositionLocal(Lerp3(pointB, pointT, v));

					index = x + 1 + (y + 1) * resAdd3;

					quadPoints[index] = point;

					// Is this a main vertex?
					if (x >= 0 && x < resAdd1 && y >= 0 && y < resAdd1)
					{
						var coordB = Lerp2(patch.CoordBL, patch.CoordBR, u);
						var coordT = Lerp2(patch.CoordTL, patch.CoordTR, u);
						var coord1 = Lerp2(coordB, coordT, v);
						var coord2 = new Vector2(u, v);
						var center = (patch.PointBL + patch.PointBR + patch.PointTL + patch.PointTR) * 0.25f;

						if (OnCalculateCoord1 != null) OnCalculateCoord1(point, center, ref coord1);
						if (OnCalculateCoord2 != null) OnCalculateCoord2(point, center, ref coord2);

						vertex = x + y * resAdd1;

						positions[vertex] = point;

						coords1[vertex] = coord1;

						coords2[vertex] = coord2;
					}
				}
			}

			// Quad normals & tangents
			for (var y = 0; y < resAdd2; y++)
			{
				for (var x = 0; x < resAdd2; x++)
				{
					var bl = x + y * resAdd3;
					var br = x + 1 + y * resAdd3;
					var tl = x + (y + 1) * resAdd3;
					var tr = x + 1 + (y + 1) * resAdd3;

					var b = quadPoints[bl] - quadPoints[br];
					var t = quadPoints[tl] - quadPoints[tr];
					var l = quadPoints[bl] - quadPoints[tl];
					var r = quadPoints[br] - quadPoints[tr];

					var h = (b + t).normalized;
					var v = (l + r).normalized;
					var i = x + y * resAdd2;

					quadNormals[i] = Vector3.Cross(h, v);

					quadTangents[i] = v;
				}
			}

			// Normals & Tangents
			for (var y = 0; y < resAdd1; y++)
			{
				for (var x = 0; x < resAdd1; x++)
				{
					var bl = x + y * resAdd2;
					var br = x + 1 + y * resAdd2;
					var tl = x + (y + 1) * resAdd2;
					var tr = x + 1 + (y + 1) * resAdd2;

					var n = quadNormals[bl] + quadNormals[br] + quadNormals[tl] + quadNormals[tr];
					var t = quadTangents[bl] + quadTangents[br] + quadTangents[tl] + quadTangents[tr];
					var i = x + y * resAdd1;

					normals[i] = n.normalized;
					//normals[i] = positions[i].normalized;

					tangents[i] = SgtHelper.NewVector4(t.normalized, 1.0f);
				}
			}

			// Skirt vertices
			var scale = 1.0f - SgtHelper.Divide(SkirtThickness * Mathf.Pow(0.5f, patch.Depth), 1.0f);

			for (var i = 0; i < resAdd1; i++)
			{
				// Bottom
				vertex  = mainVerts + i;
				vertex2 = i;

				positions[vertex] = positions[vertex2] * scale; coords1[vertex] = coords1[vertex2]; coords2[vertex] = coords2[vertex2]; normals[vertex] = normals[vertex2]; tangents[vertex] = tangents[vertex2];

				// Top
				vertex  = mainVerts + i + resAdd1;
				vertex2 = resAdd1 * Resolution + i;

				positions[vertex] = positions[vertex2] * scale; coords1[vertex] = coords1[vertex2]; coords2[vertex] = coords2[vertex2]; normals[vertex] = normals[vertex2]; tangents[vertex] = tangents[vertex2];

				// Left
				vertex  = mainVerts + i + resAdd1 + resAdd1;
				vertex2 = resAdd1 * i;

				positions[vertex] = positions[vertex2] * scale; coords1[vertex] = coords1[vertex2]; coords2[vertex] = coords2[vertex2]; normals[vertex] = normals[vertex2]; tangents[vertex] = tangents[vertex2];

				// Right
				vertex  = mainVerts + i + resAdd1 + resAdd1 + resAdd1;
				vertex2 = resAdd1 * i + Resolution;

				positions[vertex] = positions[vertex2] * scale; coords1[vertex] = coords1[vertex2]; coords2[vertex] = coords2[vertex2]; normals[vertex] = normals[vertex2]; tangents[vertex] = tangents[vertex2];
			}

			// Indices
			if (indices== null || indices.Length != mainIndices + skirtIndices)
			{
				indices = new int[mainIndices + skirtIndices];

				// Main
				for (var y = 0; y < Resolution; y++)
				{
					for (var x = 0; x < Resolution; x++)
					{
						index  = (x + y * Resolution) * 6;
						vertex = x + y * resAdd1;

						indices[index + 0] = vertex;
						indices[index + 1] = vertex + 1;
						indices[index + 2] = vertex + resAdd1;
						indices[index + 3] = vertex + resAdd1 + 1;
						indices[index + 4] = vertex + resAdd1;
						indices[index + 5] = vertex + 1;
					}
				}

				// Skirt
				for (var i = 0; i < Resolution; i++)
				{
					// Bottom
					index   = mainIndices + (Resolution * 0 + i) * 6;
					vertex  = mainVerts + i;
					vertex2 = i;

					indices[index + 0] = vertex;
					indices[index + 1] = vertex + 1;
					indices[index + 2] = vertex2;
					indices[index + 3] = vertex2 + 1;
					indices[index + 4] = vertex2;
					indices[index + 5] = vertex + 1;

					// Top
					index   = mainIndices + (Resolution * 1 + i) * 6;
					vertex  = mainVerts + i + resAdd1;
					vertex2 = Resolution * resAdd1 + i;

					indices[index + 0] = vertex2;
					indices[index + 1] = vertex2 + 1;
					indices[index + 2] = vertex;
					indices[index + 3] = vertex + 1;
					indices[index + 4] = vertex;
					indices[index + 5] = vertex2 + 1;

					// Left
					index   = mainIndices + (Resolution * 2 + i) * 6;
					vertex  = mainVerts + i + resAdd1 + resAdd1;
					vertex2 = i * resAdd1;

					indices[index + 0] = vertex;
					indices[index + 1] = vertex2;
					indices[index + 2] = vertex + 1;
					indices[index + 3] = vertex2 + resAdd1;
					indices[index + 4] = vertex + 1;
					indices[index + 5] = vertex2;

					// Right
					index   = mainIndices + (Resolution * 3 + i) * 6;
					vertex  = mainVerts + i + resAdd1 + resAdd1 + resAdd1;
					vertex2 = i * resAdd1 + Resolution;

					indices[index + 0] = vertex2;
					indices[index + 1] = vertex;
					indices[index + 2] = vertex2 + resAdd1;
					indices[index + 3] = vertex + 1;
					indices[index + 4] = vertex2 + resAdd1;
					indices[index + 5] = vertex;
				}
			}

			if (mesh != null)
			{
				mesh.Clear();
			}
			else
			{
				mesh = patch.Mesh = SgtObjectPool<Mesh>.Pop() ?? new Mesh();

				mesh.name      = "Patch";
				mesh.hideFlags = HideFlags.DontSave;
			}

			mesh.vertices  = positions;
			mesh.uv        = coords1;
			mesh.uv2       = coords2;
			mesh.normals   = normals;
			mesh.tangents  = tangents;
			mesh.triangles = indices;
			mesh.RecalculateBounds();

			patch.MeshCenter = mesh.bounds.center;
		}
	}

	// Excludes clamp
	private static Vector2 Lerp2(Vector2 a, Vector2 b, float t)
	{
		a.x += (b.x - a.x) * t;
		a.y += (b.y - a.y) * t;

		return a;
	}

	private static Vector3 Lerp3(Vector3 a, Vector3 b, float t)
	{
		a.x += (b.x - a.x) * t;
		a.y += (b.y - a.y) * t;
		a.z += (b.z - a.z) * t;

		return a;
	}

	protected virtual void OnEnable()
	{
		AllTerrains.Add(this);

		MarkMaterialAsDirty();
	}

	protected virtual void OnDisable()
	{
		AllTerrains.Remove(this);
	}

	protected virtual void Update()
	{
		UpdateSplitDistances();
		UpdateDirtyState();
		UpdateDirtyMesh();
		UpdatePatches();
	}

	protected virtual void LateUpdate()
	{
		UpdateDirtyMaterials();

		ticksUsed = false;
	}

	protected virtual void OnDestroy()
	{
		negativeX = SgtPatch.MarkForDestruction(negativeX);
		negativeY = SgtPatch.MarkForDestruction(negativeY);
		negativeZ = SgtPatch.MarkForDestruction(negativeZ);
		positiveX = SgtPatch.MarkForDestruction(positiveX);
		positiveY = SgtPatch.MarkForDestruction(positiveY);
		positiveZ = SgtPatch.MarkForDestruction(positiveZ);
	}

	private void UpdateSplitDistances()
	{
		if (SplitDistances.Length > 0)
		{
			if (SplitDistances[0] <= 0.0f)
			{
				SplitDistances[0] = 1.0f;
			}

			for (var i = 1; i < SplitDistances.Length; i++)
			{
				var p = SplitDistances[i - 1];
				var c = SplitDistances[i];

				if (c <= 0.0f || c >= p)
				{
					SplitDistances[i] = p * 0.5f;
				}
			}
		}
	}

	private void UpdateDirtyState()
	{
		if (stateDirty == true)
		{
			stateDirty = false;

			if (positiveX != null) positiveX.UpdateStates();
			if (positiveY != null) positiveY.UpdateStates();
			if (positiveZ != null) positiveZ.UpdateStates();
			if (negativeX != null) negativeX.UpdateStates();
			if (negativeY != null) negativeY.UpdateStates();
			if (negativeZ != null) negativeZ.UpdateStates();
		}
	}

	private void UpdateDirtyMesh()
	{
		if (meshDirty == true)
		{
			meshDirty = false;

			if (positiveX != null) positiveX.RegenerateMeshes();
			if (positiveY != null) positiveY.RegenerateMeshes();
			if (positiveZ != null) positiveZ.RegenerateMeshes();
			if (negativeX != null) negativeX.RegenerateMeshes();
			if (negativeY != null) negativeY.RegenerateMeshes();
			if (negativeZ != null) negativeZ.RegenerateMeshes();
		}
	}

	private void UpdateDirtyMaterials()
	{
		if (expectedCoronaMaterial != CoronaMaterial || (expectedCoronaMaterialSet == true && expectedCoronaMaterial == null))
		{
			expectedCoronaMaterial    = CoronaMaterial;
			expectedCoronaMaterialSet = CoronaMaterial != null;
			materialDirty             = true;
		}

		if (materialDirty == true)
		{
			materialDirty = false;

			if (negativeX != null) negativeX.UpdateStates();
			if (negativeY != null) negativeY.UpdateStates();
			if (negativeZ != null) negativeZ.UpdateStates();
			if (positiveX != null) positiveX.UpdateStates();
			if (positiveY != null) positiveY.UpdateStates();
			if (positiveZ != null) positiveZ.UpdateStates();
		}
	}

	private void UpdatePatches()
	{
		if (negativeX == null) negativeX = CreatePatch("Negative X", Quaternion.Euler(  0.0f,  90.0f, 0.0f));
		if (negativeY == null) negativeY = CreatePatch("Negative Y", Quaternion.Euler( 90.0f,   0.0f, 0.0f));
		if (negativeZ == null) negativeZ = CreatePatch("Negative Z", Quaternion.Euler(  0.0f, 180.0f, 0.0f));
		if (positiveX == null) positiveX = CreatePatch("Positive X", Quaternion.Euler(  0.0f, 270.0f, 0.0f));
		if (positiveY == null) positiveY = CreatePatch("Positive Y", Quaternion.Euler(270.0f,   0.0f, 0.0f));
		if (positiveZ == null) positiveZ = CreatePatch("Positive Z", Quaternion.Euler(  0.0f,   0.0f, 0.0f));
	}

	private SgtPatch CreatePatch(string name, Quaternion rotation)
	{
		var pointBL = rotation * new Vector3(-1.0f, -1.0f, 1.0f);
		var pointBR = rotation * new Vector3( 1.0f, -1.0f, 1.0f);
		var pointTL = rotation * new Vector3(-1.0f,  1.0f, 1.0f);
		var pointTR = rotation * new Vector3( 1.0f,  1.0f, 1.0f);
		var coordBL = new Vector2(1.0f, 0.0f);
		var coordBR = new Vector2(0.0f, 0.0f);
		var coordTL = new Vector2(1.0f, 1.0f);
		var coordTR = new Vector2(0.0f, 1.0f);

		return CreatePatch(name, null, pointBL, pointBR, pointTL, pointTR, coordBL, coordBR, coordTL, coordTR, 0);
	}
}

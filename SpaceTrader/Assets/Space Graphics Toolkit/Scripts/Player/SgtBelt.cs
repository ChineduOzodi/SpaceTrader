using UnityEngine;
using System.Collections.Generic;

public abstract class SgtBelt : MonoBehaviour
{
	public static List<SgtBelt> AllBelts = new List<SgtBelt>();

	public List<Light> Lights = new List<Light>();

	public List<SgtShadow> Shadows = new List<SgtShadow>();

	public Color Color = Color.white;

	public float Brightness = 1.0f;

	public SgtRenderQueue RenderQueue = SgtRenderQueue.Geometry;

	public int RenderQueueOffset;

	public float Age;

	public float TimeScale = 1.0f;

	public bool AutoRegenerate = true;

	[SerializeField]
	protected List<SgtBeltGroup> groups = new List<SgtBeltGroup>();

	protected int lightCount;

	protected static List<string> keywords = new List<string>();

	[System.NonSerialized]
	private bool meshDirty = true;

	[System.NonSerialized]
	private bool meshGenerated;

	public void MarkMeshAsDirty()
	{
#if UNITY_EDITOR
		SgtHelper.SetDirty(this);
#endif
		meshDirty = true;
	}

	public SgtCustomBelt MakeEditableCopy(int layer = 0, Transform parent = null)
	{
		return MakeEditableCopy(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public SgtCustomBelt MakeEditableCopy(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
#if UNITY_EDITOR
		SgtHelper.BeginUndo("Create Editable Belt Copy");
#endif
		var gameObject = SgtHelper.CreateGameObject("Editable Belt Copy", layer, parent, localPosition, localRotation, localScale);
		var customBelt = SgtHelper.AddComponent<SgtCustomBelt>(gameObject, false);
		var asteroids  = default(List<SgtBeltAsteroid>);
		var pool       = default(bool);

		CalculateAsteroids(out asteroids, out pool);

		if (asteroids != null)
		{
			if (pool == true)
			{
				customBelt.Asteroids = asteroids;
			}
			else
			{
				for (var i = 0; i < asteroids.Count; i++)
				{
					var asteroid = asteroids[i];

					if (asteroid != null)
					{
						var newAsteroid = SgtClassPool<SgtBeltAsteroid>.Pop() ?? new SgtBeltAsteroid(); customBelt.Asteroids.Add(asteroid);

						newAsteroid.CopyFrom(asteroid);
					}
				}
			}
		}

		customBelt.Color      = Color;
		customBelt.Brightness = Brightness;

		return customBelt;
	}

	public void UpdateState()
	{
		UpdateDirty();
		UpdateGroups();
	}

	[ContextMenu("Regenerate")]
	public void Regenerate()
	{
		meshDirty     = false;
		meshGenerated = true;

		BeginRegeneration();
		{
			RegenerateMeshes();
		}
		EndRegeneration();
	}

#if UNITY_EDITOR
	[ContextMenu("Make Editable Copy")]
	public void MakeEditableCopyContext()
	{
		var customBelt = MakeEditableCopy(gameObject.layer, transform.parent, transform.localPosition, transform.localRotation, transform.localScale);

		SgtHelper.SelectAndPing(customBelt);
	}
#endif

	protected virtual void Update()
	{
		Age += Time.deltaTime * TimeScale;

		UpdateState();
	}

	protected virtual void OnEnable()
	{
#if UNITY_EDITOR
		if (AllBelts.Count == 0)
		{
			SgtHelper.RepaintAll();
		}
#endif
		AllBelts.Add(this);

		SgtObserver.OnObserverPreRender += ObserverPreRender;

		for (var i = groups.Count - 1; i >= 0; i--)
		{
			var group = groups[i];

			if (group != null)
			{
				group.gameObject.SetActive(true);
			}
		}
	}

	protected virtual void OnDisable()
	{
		AllBelts.Remove(this);

		SgtObserver.OnObserverPreRender -= ObserverPreRender;

		for (var i = groups.Count - 1; i >= 0; i--)
		{
			var group = groups[i];

			if (group != null)
			{
				group.gameObject.SetActive(false);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		for (var i = groups.Count - 1; i >= 0; i--)
		{
			SgtBeltGroup.MarkForDestruction(groups[i]);
		}

		groups.Clear();
	}

	protected abstract void CalculateAsteroids(out List<SgtBeltAsteroid> asteroids, out bool pool); // pool == true when they are temporary

	protected virtual void UpdateGroupMaterial(SgtBeltGroup group)
	{
		if (group.Material == null) group.Material = SgtHelper.CreateTempMaterial(SgtHelper.ShaderNamePrefix + "Belt");

		var scale       = transform.lossyScale.x;
		var color       = SgtHelper.Brighten(Color, Brightness);
		var renderQueue = (int)RenderQueue + RenderQueueOffset;
		var lightCount  = SgtHelper.WriteLights(Lights, 2, transform.position, null, null, group.Material);
		var shadowCount = SgtHelper.WriteShadows(Shadows, 2, group.Material);

		SgtHelper.WriteLightKeywords(Lights.Count > 0, lightCount, keywords);
		SgtHelper.WriteShadowKeywords(shadowCount, keywords);

		group.Material.renderQueue = renderQueue;
		group.Material.SetTexture("_MainTex", group.MainTex);
		group.Material.SetTexture("_HeightTex", group.HeightTex);
		group.Material.SetColor("_Color", color);
		group.Material.SetFloat("_Scale", scale);
		group.Material.SetFloat("_Age", Age);
	}

	private void UpdateDirty()
	{
		if (meshDirty == true)
		{
			if (AutoRegenerate == true || meshGenerated == false)
			{
				Regenerate();
			}
		}
	}

	private void UpdateGroups()
	{
		for (var i = groups.Count - 1; i >= 0; i--)
		{
			var group = groups[i];

			if (group != null)
			{
				UpdateGroupMaterial(group);

				SgtHelper.SetKeywords(group.Material, keywords); keywords.Clear();

				group.ManualUpdate();
			}
		}
	}

	private void BeginRegeneration()
	{
		groups.RemoveAll(g => g == null);

		for (var i = groups.Count - 1; i >= 0; i--)
		{
			var group = groups[i];

			group.Models.RemoveAll(m => m == null);

			group.Asteroids.Clear();

			for (var j = group.Models.Count - 1; j >= 0; j--)
			{
				group.Models[j].PoolMeshNow();
			}
		}
	}

	private void EndRegeneration()
	{
		for (var i = groups.Count - 1; i >= 0; i--)
		{
			var group = groups[i];

			if (group.Asteroids.Count > 0)
			{
				group.Asteroids.Clear(); // No longer needed, and they've already been pooled in RegenerateMeshes()

				for (var j = group.Models.Count - 1; j >= 0; j--)
				{
					var model = group.Models[j];

					if (model.Mesh == null)
					{
						SgtBeltModel.Pool(model);

						group.Models.RemoveAt(j);
					}
				}
			}
			else
			{
				SgtBeltGroup.Pool(group);

				groups.RemoveAt(i);
			}
		}
	}

	private void RegenerateMeshes()
	{
		var asteroids = default(List<SgtBeltAsteroid>);
		var pool      = default(bool);

		CalculateAsteroids(out asteroids, out pool);

		if (asteroids != null)
		{
			// Sort asteroids into groups
			for (var i = asteroids.Count - 1; i >= 0; i--)
			{
				var asteroid = asteroids[i];

				if (asteroid != null)
				{
					var group = GetGroup(asteroid.MainTex, asteroid.HeightTex);

					group.Asteroids.Add(asteroid);
				}
			}

			// Pool asteroids?
			if (pool == true)
			{
				SgtClassPool<SgtBeltAsteroid>.Add(asteroids);
			}

			// Build groups
			for (var i = groups.Count - 1; i >= 0; i--)
			{
				var group          = groups[i];
				var groupAsteroids = group.Asteroids;
				var maxWidth       = 0.0f;
				var maxHeight      = 0.0f;

				SgtProceduralMesh.Clear();

				for (var j = groupAsteroids.Count - 1; j >= 0; j--)
				{
					var asteroid = groupAsteroids[j];
					var radius   = asteroid.Radius;
					var distance = asteroid.OrbitDistance;
					var height   = asteroid.Height;
					var uv       = SgtHelper.CalculateSpriteUV(asteroid.MainTex);

					maxWidth  = Mathf.Max(maxWidth, distance + radius);
					maxHeight = Mathf.Max(maxHeight, height + radius);

					SgtProceduralMesh.PushPosition(asteroid.OrbitAngle, distance, asteroid.OrbitSpeed, 4);

					SgtProceduralMesh.PushColor(asteroid.Color, 4);

					SgtProceduralMesh.PushNormal(-1.0f,  1.0f, 0.0f);
					SgtProceduralMesh.PushNormal( 1.0f,  1.0f, 0.0f);
					SgtProceduralMesh.PushNormal(-1.0f, -1.0f, 0.0f);
					SgtProceduralMesh.PushNormal( 1.0f, -1.0f, 0.0f);

					SgtProceduralMesh.PushTangent(asteroid.Angle / Mathf.PI, asteroid.Spin / Mathf.PI, 0.0f, 0.0f, 4);

					SgtProceduralMesh.PushCoord1(uv.x, uv.y);
					SgtProceduralMesh.PushCoord1(uv.z, uv.y);
					SgtProceduralMesh.PushCoord1(uv.x, uv.w);
					SgtProceduralMesh.PushCoord1(uv.z, uv.w);

					SgtProceduralMesh.PushCoord2(radius, height, 4);
				}

				var bounds = new Bounds(Vector3.zero, new Vector3(maxWidth * 2.0f, maxHeight * 2.0f, maxWidth * 2.0f));

				SgtProceduralMesh.SplitQuads(HideFlags.DontSave);

				var meshCount = SgtProceduralMesh.Count;

				// Copy meshes
				for (var j = 0; j < meshCount; j++)
				{
					var mesh  = SgtProceduralMesh.Pop();
					var model = group.Models.Count > j ? group.Models[j] : SgtBeltModel.Create(group);

					mesh.bounds = bounds;

					model.Mesh  = mesh;
				}
			}
		}
	}

	private SgtBeltGroup GetGroup(Sprite diffuseSprite, Sprite depthSprite)
	{
		var diffuseTexture = diffuseSprite != null ? diffuseSprite.texture : null;
		var depthTexture   = depthSprite   != null ? depthSprite.texture   : null;

		for (var i = groups.Count - 1; i >= 0; i--)
		{
			var group = groups[i];

			if (group.MainTex == diffuseTexture && group.HeightTex == depthTexture)
			{
				return group;
			}
		}

		var newGroup = SgtBeltGroup.Create(this); groups.Add(newGroup);

		newGroup.MainTex   = diffuseTexture;
		newGroup.HeightTex = depthTexture;

		return newGroup;
	}

	private void ObserverPreRender(SgtObserver observer)
	{
		for (var i = groups.Count - 1; i >= 0; i--)
		{
			var group = groups[i];

			if (group != null && group.Material != null)
			{
				group.Material.SetFloat("_CameraRollAngle", observer.RollAngle * Mathf.Deg2Rad);
			}
		}
	}
}

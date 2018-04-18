using UnityEngine;
using System.Collections.Generic;

public abstract class SgtStarfield : MonoBehaviour
{
	public static List<SgtStarfield> AllStarfields = new List<SgtStarfield>();

	public Color Color = Color.white;

	public float Brightness = 1.0f;

	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	public int RenderQueueOffset;

	public float Age;

	public float TimeScale = 1.0f;

	[SgtRange(0.0f, 1000.0f)]
	public float Softness;

	public bool AutoRegenerate = true;

	public bool StretchToObservers;

	public bool StretchOverride;

	public Vector3 StretchVector;

	public float StretchScale = 1.0f;

	public bool FadeNear;

	public float FadeNearRadius = 1.0f;

	public float FadeNearThickness = 2.0f;

	public bool FadeFar;

	public float FadeFarRadius = 10.0f;

	public float FadeFarThickness = 2.0f;

	public bool FollowObservers;

	public bool AllowPulse;

	[SerializeField]
	protected List<SgtStarfieldGroup> groups = new List<SgtStarfieldGroup>();

	protected static List<string> keywords = new List<string>();

	[System.NonSerialized]
	private bool meshDirty = true;

	[System.NonSerialized]
	private bool meshGenerated;

	protected static List<SgtStarfieldStar> tempStars = new List<SgtStarfieldStar>();

	public void MarkMeshAsDirty()
	{
#if UNITY_EDITOR
		SgtHelper.SetDirty(this);
#endif
		meshDirty = true;
	}

	public SgtCustomStarfield MakeEditableCopy(int layer = 0, Transform parent = null)
	{
		return MakeEditableCopy(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public SgtCustomStarfield MakeEditableCopy(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
#if UNITY_EDITOR
		SgtHelper.BeginUndo("Create Editable Starfield Copy");
#endif

		var gameObject      = SgtHelper.CreateGameObject("Editable Starfield Copy", layer, parent, localPosition, localRotation, localScale);
		var customStarfield = SgtHelper.AddComponent<SgtCustomStarfield>(gameObject, false);
		var stars           = default(List<SgtStarfieldStar>);
		var pool            = default(bool);

		CalculateStars(out stars, out pool);

		if (stars != null)
		{
			if (pool == true)
			{
				customStarfield.Stars = stars;
			}
			else
			{
				for (var i = 0; i < stars.Count; i++)
				{
					var star = stars[i];

					if (star != null)
					{
						var newStar = SgtClassPool<SgtStarfieldStar>.Pop() ?? new SgtStarfieldStar(); customStarfield.Stars.Add(star);

						newStar.CopyFrom(star);
					}
				}
			}
		}

		customStarfield.Color              = Color;
		customStarfield.Brightness         = Brightness;
		customStarfield.RenderQueue        = RenderQueue;
		customStarfield.RenderQueueOffset  = RenderQueueOffset;
		customStarfield.Age                = Age;
		customStarfield.TimeScale          = TimeScale;
		customStarfield.Softness           = Softness;
		customStarfield.AutoRegenerate     = AutoRegenerate;
		customStarfield.StretchToObservers = StretchToObservers;
		customStarfield.StretchOverride    = StretchOverride;
		customStarfield.StretchVector      = StretchVector;
		customStarfield.StretchScale       = StretchScale;
		customStarfield.FadeNear           = FadeNear;
		customStarfield.FadeNearRadius     = FadeNearRadius;
		customStarfield.FadeNearThickness  = FadeNearThickness;
		customStarfield.FadeFar            = FadeFar;
		customStarfield.FadeFarRadius      = FadeFarRadius;
		customStarfield.FadeFarThickness   = FadeFarThickness;
		customStarfield.FollowObservers    = FollowObservers;
		customStarfield.AllowPulse         = AllowPulse;

		return customStarfield;
	}

	public void UpdateState()
	{
		UpdateDirty();
		UpdateGroups();
	}

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
		var customStarfield = MakeEditableCopy(gameObject.layer, transform.parent, transform.localPosition, transform.localRotation, transform.localScale);

		SgtHelper.SelectAndPing(customStarfield);
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
		if (AllStarfields.Count == 0)
		{
			SgtHelper.RepaintAll();
		}
#endif
		AllStarfields.Add(this);

		Camera.onPreCull    += CameraPreCull;
		Camera.onPostRender += CameraPostRender;

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
		AllStarfields.Remove(this);

		Camera.onPreCull    -= CameraPreCull;
		Camera.onPostRender -= CameraPostRender;

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
			SgtStarfieldGroup.MarkForDestruction(groups[i]);
		}

		groups.Clear();
	}

	protected virtual void CameraPreCull(Camera camera)
	{
		for (var i = groups.Count - 1; i >= 0; i--)
		{
			var group = groups[i];

			if (group != null)
			{
				if (group.Material != null)
				{
					var observer = SgtObserver.Find(camera);

					if (observer != null)
					{
						group.Material.SetFloat("_CameraRollAngle", observer.RollAngle * Mathf.Deg2Rad);

						if (StretchToObservers == true)
						{
							var velocity = (StretchOverride == true ? StretchVector : observer.Velocity) * StretchScale;

							group.Material.SetVector("_StretchVector", velocity);
							group.Material.SetVector("_StretchDirection", velocity.normalized);
							group.Material.SetFloat("_StretchLength", velocity.magnitude);
						}
					}
				}

				if (FollowObservers == true)
				{
					if (group.TempSet == false)
					{
						group.TempSet      = true;
						group.TempPosition = group.transform.position;
					}

					group.transform.position = camera.transform.position;
				}
			}
		}
	}

	protected void CameraPostRender(Camera camera)
	{
		for (var i = groups.Count - 1; i >= 0; i--)
		{
			var group = groups[i];

			if (group != null)
			{
				if (group.Material != null)
				{
					group.Material.SetFloat("_CameraRollAngle", 0.0f);
				}

				if (FollowObservers == true && group.TempSet == true)
				{
					group.TempSet = false;

					group.transform.position = group.TempPosition;
				}
			}
		}
	}

	protected abstract void CalculateStars(out List<SgtStarfieldStar> stars, out bool pool); // pool == true when they are temporary

	protected virtual void UpdateGroupMaterial(SgtStarfieldGroup group)
	{
		var color       = SgtHelper.Brighten(Color, Color.a * Brightness);
		var scale       = transform.lossyScale.x;
		var renderQueue = (int)RenderQueue + RenderQueueOffset;

		if (group.Material == null) group.Material = SgtHelper.CreateTempMaterial(SgtHelper.ShaderNamePrefix + "Starfield");

		group.Material.renderQueue = renderQueue;
		group.Material.SetTexture("_Texture", group.Texture);
		group.Material.SetColor("_Color", color);
		group.Material.SetFloat("_Scale", scale);

		if (AllowPulse == true)
		{
			keywords.Add("LIGHT_1");

			group.Material.SetFloat("_Age", Age);
		}

		if (Softness > 0.0f)
		{
			keywords.Add("LIGHT_2");

			group.Material.SetFloat("_InvFade", SgtHelper.Reciprocal(Softness));
		}

		if (StretchToObservers == true)
		{
			keywords.Add("SGT_C");
		}

		if (FadeNear == true)
		{
			keywords.Add("SGT_D");

			group.Material.SetFloat("_FadeNearRadius", FadeNearRadius);
			group.Material.SetFloat("_FadeNearScale", SgtHelper.Reciprocal(FadeNearThickness));
		}

		if (FadeFar == true)
		{
			keywords.Add("SGT_E");

			group.Material.SetFloat("_FadeFarRadius", FadeFarRadius);
			group.Material.SetFloat("_FadeFarScale", SgtHelper.Reciprocal(FadeFarThickness));
		}
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

				SgtHelper.SetKeywords(group.Material, keywords, ref group.LastKeywords); keywords.Clear();

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

			group.Stars.Clear();

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

			if (group.Stars.Count > 0)
			{
				group.Stars.Clear(); // No longer needed, and they've already been pooled in RegenerateMeshes()

				for (var j = group.Models.Count - 1; j >= 0; j--)
				{
					var model = group.Models[j];

					if (model.Mesh == null)
					{
						SgtStarfieldModel.Pool(model);

						group.Models.RemoveAt(j);
					}
				}
			}
			else
			{
				SgtStarfieldGroup.Pool(group);

				groups.RemoveAt(i);
			}
		}
	}

	protected virtual void RegenerateMeshes()
	{
		var stars = default(List<SgtStarfieldStar>);
		var pool  = default(bool);

		CalculateStars(out stars, out pool);

		if (stars != null)
		{
			// Sort stars into groups
			for (var i = stars.Count - 1; i >= 0; i--)
			{
				var star = stars[i];

				if (star != null)
				{
					var group = GetGroup(star.Sprite);

					group.Stars.Add(star);
				}
			}

			// Pool stars?
			if (pool == true)
			{
				SgtClassPool<SgtStarfieldStar>.Add(stars);
			}

			// Build groups
			for (var i = groups.Count - 1; i >= 0; i--)
			{
				var group = groups[i];

				if (group.Stars.Count > 0)
				{
					var groupStars = group.Stars;
					var minMaxSet  = false;
					var min        = default(Vector3);
					var max        = default(Vector3);

					SgtProceduralMesh.Clear();

					for (var j = groupStars.Count - 1; j >= 0; j--)
					{
						var star     = groupStars[j];
						var position = star.Position;
						var radius   = star.Radius;
						var uv       = SgtHelper.CalculateSpriteUV(star.Sprite);
						var angle    = star.Angle / Mathf.PI;

						ExpandBounds(ref minMaxSet, ref min, ref max, position, radius);

						SgtProceduralMesh.PushPosition(position, 4);

						SgtProceduralMesh.PushColor(star.Color, 4);

						SgtProceduralMesh.PushNormal(-1.0f,  1.0f, angle);
						SgtProceduralMesh.PushNormal( 1.0f,  1.0f, angle);
						SgtProceduralMesh.PushNormal(-1.0f, -1.0f, angle);
						SgtProceduralMesh.PushNormal( 1.0f, -1.0f, angle);

						SgtProceduralMesh.PushTangent(star.PulseOffset, star.PulseSpeed, star.PulseRange, 0.0f, 4);

						SgtProceduralMesh.PushCoord1(uv.x, uv.y);
						SgtProceduralMesh.PushCoord1(uv.z, uv.y);
						SgtProceduralMesh.PushCoord1(uv.x, uv.w);
						SgtProceduralMesh.PushCoord1(uv.z, uv.w);

						SgtProceduralMesh.PushCoord2(radius,  0.5f);
						SgtProceduralMesh.PushCoord2(radius, -0.5f);
						SgtProceduralMesh.PushCoord2(radius,  0.5f);
						SgtProceduralMesh.PushCoord2(radius, -0.5f);
					}

					var bounds = SgtHelper.NewBoundsFromMinMax(min, max);

					SgtProceduralMesh.SplitQuads(HideFlags.DontSave);

					var meshCount = SgtProceduralMesh.Count;

					// Copy meshes
					for (var j = 0; j < meshCount; j++)
					{
						var mesh  = SgtProceduralMesh.Pop();
						var model = group.Models.Count > j ? group.Models[j] : SgtStarfieldModel.Create(group);

						mesh.bounds = bounds;

						model.Mesh  = mesh;
					}
				}
			}
		}
	}

	protected SgtStarfieldGroup GetGroup(Sprite sprite)
	{
		var texture = sprite != null ? sprite.texture : null;

		for (var i = groups.Count - 1; i >= 0; i--)
		{
			var group = groups[i];

			if (group.Texture == texture)
			{
				return group;
			}
		}

		var newGroup = SgtStarfieldGroup.Create(this); groups.Add(newGroup);

		newGroup.Texture = texture;

		return newGroup;
	}

	protected static void ExpandBounds(ref bool minMaxSet, ref Vector3 min, ref Vector3 max, Vector3 position, float radius)
	{
		var radius3 = new Vector3(radius, radius, radius);

		if (minMaxSet == false)
		{
			minMaxSet = true;

			min = position - radius3;
			max = position + radius3;
		}

		min = Vector3.Min(min, position - radius3);
		max = Vector3.Max(max, position + radius3);
	}
}

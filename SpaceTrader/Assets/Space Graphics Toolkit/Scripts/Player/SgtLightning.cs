using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(SgtLightning))]
public class SgtLightning_Editor : SgtEditor<SgtLightning>
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.Sprite == null));
		{
			DrawDefault("Sprite");
		}
		EndError();

		DrawDefault("Color");

		DrawDefault("Brightness");

		BeginError(Any(t => t.Life > t.MaxLife));
		{
			DrawDefault("Life");
		}
		EndError();

		BeginError(Any(t => t.MaxLife <= 0.0f));
		{
			DrawDefault("MaxLife");
		}
		EndError();
	}
}
#endif

[AddComponentMenu("")]
public class SgtLightning : MonoBehaviour
{
	public static List<SgtLightning> AllLightnings = new List<SgtLightning>();

	[Tooltip("How many seconds remain until this lightning ends")]
	public float Life = 1.0f;

	[Tooltip("How many seconds the lightning will remain")]
	public float MaxLife = 1.0f;

	[Tooltip("The current sprite used by the lightning")]
	public Sprite Sprite;

	[Tooltip("The current color of the lightning")]
	public Color Color = Color.white;

	[Tooltip("The current brightness of the lightning")]
	public float Brightness = 1.0f;

	private Material material;

	private Mesh mesh;

	private MeshRenderer meshRenderer;

	private MeshFilter meshFilter;

	private static List<int> ringSides = new List<int>();

	private static MaterialPropertyBlock propertyBlock;

	public static SgtLightning Create(int layer, Transform parent, float size, float radius, float detail)
	{
		if (size > 0.0f && radius > 0.0f && detail > 0.0f)
		{
			if (size > 180.0f) size = 180.0f;

			var lightning = SgtComponentPool<SgtLightning>.Pop("Lightning", layer, parent);

			lightning.Init(size * Mathf.Deg2Rad, radius, detail);

			return lightning;
        }

		return null;
	}

	private void Init(float size, float radius, float detail)
	{
		if (material == null) material = SgtHelper.CreateTempMaterial(SgtHelper.ShaderNamePrefix + "Lightning");

		if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();

		if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();

		SgtProceduralMesh.Clear();
		{
			mesh = SgtObjectPool<Mesh>.Add(mesh);

			AddVertices(size, radius, detail);

			AddIndices();

			mesh = SgtProceduralMesh.SplitSingle(HideFlags.DontSave);
			mesh.RecalculateBounds();

			meshFilter.sharedMesh = mesh;

			meshRenderer.sharedMaterial = material;
		}
		SgtProceduralMesh.Discard();
	}

	protected virtual void OnWillRenderObject()
	{
		if (meshRenderer != null)
		{
			if (Sprite != null && Life > 0.0f && MaxLife > 0.0f)
			{
				var uv = SgtHelper.CalculateSpriteUV(Sprite);

                material.SetTexture("_MainTex", Sprite.texture);

				material.SetFloat("_Age", 1.0f - Life / MaxLife);

				material.SetColor("_Color", SgtHelper.Brighten(Color, Brightness));

				material.SetVector("_Offset", new Vector2(uv.x, uv.y));

				material.SetVector("_Scale", new Vector2(uv.z - uv.x, uv.w - uv.y));
            }
			else
			{
				SgtComponentPool<SgtLightning>.Add(this);
			}
		}
	}

	private void AddVertices(float size, float radius, float detail)
	{
        var columnCount = Mathf.CeilToInt(size * detail * 0.1f);
		var sizeStep    = size / (float)columnCount;

		ringSides.Clear();

		for (var i = 0; i <= columnCount; i++)
		{
			AddRing(i * sizeStep, size, radius, detail);
		}
	}

	private void AddRing(float size, float sizeMax, float radius, float resolution)
	{
		var sides = (int)(Mathf.Sin(size) * resolution);

		if (SgtProceduralMesh.PositionCount + sides < 65000)
		{
			var extents = (size / sizeMax) * 0.5f;

			if (sides < 5)
			{
				if (size > 0.0f)
				{
					sides = 5;
				}
				else
				{
					sides = 1;
				}
			}

			var angleStep = SgtHelper.Reciprocal(sides) * Mathf.PI * 2.0f;

			for (var i = 0; i < sides; i++)
			{
				var angle = i * angleStep;
				var x     = radius * Mathf.Sin(angle) * Mathf.Sin(size);
				var y     = radius * Mathf.Cos(angle) * Mathf.Sin(size);
				var z     = radius * Mathf.Cos(size);
				var u     = 0.5f + Mathf.Sin(angle) * extents;
				var v     = 0.5f + Mathf.Cos(angle) * extents;

				SgtProceduralMesh.PushPosition(x, y, z);
				SgtProceduralMesh.PushCoord1(u, v);
			}

			ringSides.Add(sides);
		}
	}

	private void AddIndices()
	{
		var totalSides = 0;

		for (var i = 1; i < ringSides.Count; i++)
		{
			var innerOffset = totalSides;
			var innerSides  = ringSides[i - 1];
			var outerOffset = totalSides + innerSides;
			var outerSides  = ringSides[i    ];

			AddIndices(innerOffset, innerSides, outerOffset, outerSides);

			totalSides += innerSides;
        }
	}

	private void AddIndices(int innerOffset, int innerSides, int outerOffset, int outerSides)
	{
		var stepInner      = 1.0f / innerSides;
		var stepOuter      = 1.0f / outerSides;
		var indexInner     = 0;
		var indexOuter     = 0;
		var progress       = 0.0f;
		var remainingInner = innerSides;
		var remainingOuter = outerSides;

		if (remainingInner == 1)
		{
			remainingInner = 0;
        }

		while (remainingInner > 0 || remainingOuter > 0)
		{
			var targetInner    = (indexInner + 1) * stepInner;
			var targetOuter    = (indexOuter + 1) * stepOuter;
			var remainderInner = targetInner - progress;
            var remainderOuter = targetOuter - progress;

			if (remainderInner <= remainderOuter && remainingInner > 0)
			{
				SgtProceduralMesh.PushIndex(innerOffset + indexInner % innerSides);

				SgtProceduralMesh.PushIndex(outerOffset + indexOuter % outerSides);

				SgtProceduralMesh.PushIndex(innerOffset + (indexInner + 1) % innerSides);

				progress        = targetInner;
				remainingInner -= 1;
				indexInner     += 1;
			}

			if (remainderInner >= remainderOuter && remainingOuter > 0)
			{
				SgtProceduralMesh.PushIndex(outerOffset + (indexOuter + 1) % outerSides);

				SgtProceduralMesh.PushIndex(innerOffset + indexInner % innerSides);

				SgtProceduralMesh.PushIndex(outerOffset + indexOuter % outerSides);

				progress        = targetOuter;
				remainingOuter -= 1;
				indexOuter     += 1;
			}
        }
	}

	protected virtual void OnEnable()
	{
		AllLightnings.Add(this);
	}

	protected virtual void OnDisable()
	{
		AllLightnings.Remove(this);
	}

	protected virtual void OnDestroy()
	{
		mesh = SgtHelper.Destroy(mesh);
	}

	protected virtual void Update()
	{
		Life -= Time.deltaTime;

		if (Life <= 0.0f)
		{
			SgtComponentPool<SgtLightning>.Add(this);
		}
	}
}

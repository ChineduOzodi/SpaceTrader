using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Ring")]
public class SgtRing : MonoBehaviour
{
	public static List<SgtRing> AllRings = new List<SgtRing>();

	public List<Light> Lights = new List<Light>();

	public List<SgtShadow> Shadows = new List<SgtShadow>();

	public Texture MainTex;

	public Color Color = Color.white;

	public float Brightness = 1.0f;

	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	public int RenderQueueOffset;

	public float InnerRadius = 1.0f;

	public float OuterRadius = 2.0f;

	public int SegmentCount = 8;

	public int SegmentDetail = 10;

	[SgtRange(-1.0f, 1.0f)]
	public float LightingBias = 0.5f;

	[SgtRange(0.0f, 1.0f)]
	public float LightingSharpness = 0.5f;

	public bool Scattering;

	[SgtRange(0.0f, 5.0f)]
	public float MieSharpness = 2.0f;

	[SgtRange(0.0f, 10.0f)]
	public float MieStrength = 1.0f;

	public float BoundsShift;

	protected bool meshDirty = true;

	protected Mesh mesh;

	protected Material material;

	[SerializeField]
	protected List<SgtRingSegment> segments = new List<SgtRingSegment>();

	protected static List<string> keywords = new List<string>();

	public float Width
	{
		get
		{
			return OuterRadius - InnerRadius;
		}
	}

	public void MarkMeshAsDirty()
	{
#if UNITY_EDITOR
		SgtHelper.SetDirty(this);
#endif
		meshDirty = true;
	}

	public void UpdateState()
	{
		UpdateDirty();
		UpdateMaterial();
		UpdateSegments();
	}

	public static SgtRing CreateRing(int layer = 0, Transform parent = null)
	{
		return CreateRing(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtRing CreateRing(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Ring", layer, parent, localPosition, localRotation, localScale);
		var ring       = gameObject.AddComponent<SgtRing>();

		return ring;
	}

	protected virtual void OnEnable()
	{
		AllRings.Add(this);

		for (var i = segments.Count - 1; i >= 0; i--)
		{
			var segment = segments[i];

			if (segment != null)
			{
				segment.gameObject.SetActive(true);
			}
		}
	}

	protected virtual void OnDisable()
	{
		AllRings.Remove(this);

		for (var i = segments.Count - 1; i >= 0; i--)
		{
			var segment = segments[i];

			if (segment != null)
			{
				segment.gameObject.SetActive(false);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		SgtHelper.Destroy(material);

		for (var i = segments.Count - 1; i >= 0; i--)
		{
			SgtRingSegment.MarkForDestruction(segments[i]);
		}

		segments.Clear();
	}

	protected virtual void Update()
	{
		UpdateState();
	}

#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;

		SgtHelper.DrawCircle(Vector3.zero, Vector3.up, InnerRadius);
		SgtHelper.DrawCircle(Vector3.zero, Vector3.up, OuterRadius);
	}
#endif

	protected void UpdateDirty()
	{
		if (mesh == null || mesh.vertexCount == 0)
		{
			meshDirty = true;
		}

		if (meshDirty == true)
		{
			meshDirty = false;

			RegenerateMesh();
		}
	}

	protected virtual void UpdateMaterial()
	{
		if (material == null) material = SgtHelper.CreateTempMaterial(SgtHelper.ShaderNamePrefix + "Ring");

		var color       = SgtHelper.Brighten(Color, Brightness);
		var renderQueue = (int)RenderQueue + RenderQueueOffset;
		var lightCount  = SgtHelper.WriteLights(Lights, 2, transform.position, null, null, material);
		var shadowCount = SgtHelper.WriteShadows(Shadows, 2, material);

		if (material.renderQueue != renderQueue)
		{
			material.renderQueue = renderQueue;
		}

		material.SetTexture("_MainTex", MainTex);
		material.SetColor("_Color", color);
		material.SetFloat("_LightingBias", LightingBias);
		material.SetFloat("_LightingSharpness", LightingSharpness);

		if (Scattering == true)
		{
			keywords.Add("SGT_A");

			SgtHelper.WriteMie(MieSharpness, MieStrength, material);
		}

		SgtHelper.WriteLightKeywords(Lights.Count > 0, lightCount, keywords);
		SgtHelper.WriteShadowKeywords(shadowCount, keywords);

		SgtHelper.SetKeywords(material, keywords); keywords.Clear();
	}

	protected void UpdateSegments()
	{
		segments.RemoveAll(s => s == null);

		if (SegmentCount != segments.Count)
		{
			SgtHelper.ResizeArrayTo(ref segments, SegmentCount, i => SgtRingSegment.Create(this), s => SgtRingSegment.Pool(s));
		}

		var angleStep = SgtHelper.Divide(360.0f, SegmentCount);

		for (var i = SegmentCount - 1; i >= 0; i--)
		{
			var angle    = angleStep * i;
			var rotation = Quaternion.Euler(0.0f, angle, 0.0f);

			segments[i].ManualUpdate(mesh, material, rotation);
		}
	}

	protected virtual void RegenerateMesh()
	{
		mesh = SgtObjectPool<Mesh>.Add(mesh, m => m.Clear());

		SgtProceduralMesh.Clear();
		{
			if (SegmentDetail >= 3)
			{
				var angleTotal = SgtHelper.Divide(Mathf.PI * 2.0f, SegmentCount);
				var angleStep  = SgtHelper.Divide(angleTotal, SegmentDetail);
				var coordStep  = SgtHelper.Reciprocal(SegmentDetail);

				for (var i = 0; i <= SegmentDetail; i++)
				{
					var coord = coordStep * i;
					var angle = angleStep * i;
					var sin   = Mathf.Sin(angle);
					var cos   = Mathf.Cos(angle);

					SgtProceduralMesh.PushPosition(sin * InnerRadius, 0.0f, cos * InnerRadius);
					SgtProceduralMesh.PushPosition(sin * OuterRadius, 0.0f, cos * OuterRadius);

					SgtProceduralMesh.PushNormal(Vector3.up);
					SgtProceduralMesh.PushNormal(Vector3.up);

					SgtProceduralMesh.PushCoord1(0.0f, coord);
					SgtProceduralMesh.PushCoord1(1.0f, coord);
				}
			}
		}
		SgtProceduralMesh.SplitStrip(HideFlags.DontSave);

		mesh = SgtProceduralMesh.Pop(); SgtProceduralMesh.Discard();

		if (mesh != null)
		{
			var bounds = mesh.bounds;

			mesh.bounds = SgtHelper.NewBoundsCenter(bounds, bounds.center + bounds.center.normalized * BoundsShift);
		}
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Ring", false, 10)]
	public static void CreateRingMenuItem()
	{
		var parent = SgtHelper.GetSelectedParent();
		var ring   = CreateRing(parent != null ? parent.gameObject.layer : 0, parent);

		SgtHelper.SelectAndPing(ring);
	}
#endif
}

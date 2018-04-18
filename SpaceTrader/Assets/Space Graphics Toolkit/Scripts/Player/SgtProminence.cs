using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Prominence")]
public class SgtProminence : MonoBehaviour
{
	public static List<SgtProminence> AllProminences = new List<SgtProminence>();

	public Texture MainTex;

	public Color Color = Color.white;

	public float Brightness = 1.0f;

	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	public int RenderQueueOffset;

	[SgtSeed]
	public int Seed;

	public float InnerRadius = 1.0f;

	public float OuterRadius = 2.0f;

	public int PlaneCount = 8;

	public int Detail = 10;

	public bool FadeEdge;

	public float FadePower = 2.0f;

	public bool ClipNear;

	public float ClipPower = 2.0f;

	public float ObserverOffset;

	public float Width
	{
		get
		{
			return OuterRadius - InnerRadius;
		}
	}

	private bool dirty = true;

	[System.NonSerialized]
	private Mesh mesh;

	[System.NonSerialized]
	private Material material;

	[SerializeField]
	private List<SgtProminencePlane> planes = new List<SgtProminencePlane>();

	private static List<string> keywords = new List<string>();

	public void MarkAsDirty()
	{
#if UNITY_EDITOR
		SgtHelper.SetDirty(this);
#endif
		dirty = true;
	}

	public void UpdateState()
	{
		UpdateDirty();
		UpdateMaterial();
		UpdatePlanes();
	}

	public static SgtProminence CreateProminence(int layer = 0, Transform parent = null)
	{
		return CreateProminence(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtProminence CreateProminence(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Prominence", layer, parent, localPosition, localRotation, localScale);
		var prominence = gameObject.AddComponent<SgtProminence>();

		return prominence;
	}

	protected virtual void OnEnable()
	{
		AllProminences.Add(this);

		Camera.onPreCull    += CameraPreCull;
		Camera.onPostRender += CameraPostRender;

		for (var i = planes.Count - 1; i >= 0; i--)
		{
			var plane = planes[i];

			if (plane != null)
			{
				plane.gameObject.SetActive(true);
			}
		}
	}

	protected virtual void OnDisable()
	{
		AllProminences.Remove(this);

		Camera.onPreCull    -= CameraPreCull;
		Camera.onPostRender -= CameraPostRender;

		for (var i = planes.Count - 1; i >= 0; i--)
		{
			var plane = planes[i];

			if (plane != null)
			{
				plane.gameObject.SetActive(false);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		SgtHelper.Destroy(material);

		for (var i = planes.Count - 1; i >= 0; i--)
		{
			SgtProminencePlane.MarkForDestruction(planes[i]);
		}

		planes.Clear();
	}

	protected virtual void Update()
	{
		UpdateState();
	}

#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;

		Gizmos.DrawWireSphere(Vector3.zero, InnerRadius);

		Gizmos.DrawWireSphere(Vector3.zero, OuterRadius);
	}
#endif

	private void UpdateDirty()
	{
		if (mesh == null || mesh.vertexCount == 0)
		{
			dirty = true;
		}

		if (dirty == true)
		{
			dirty = false;

			RegenerateMesh();
		}
	}

	private void UpdateMaterial()
	{
		if (material == null) material = SgtHelper.CreateTempMaterial(SgtHelper.ShaderNamePrefix + "Prominence");

		var color       = SgtHelper.Premultiply(SgtHelper.Brighten(Color, Brightness));
		var renderQueue = (int)RenderQueue + RenderQueueOffset;

		material.renderQueue = renderQueue;
		material.SetTexture("_MainTex", MainTex);
		material.SetColor("_Color", color);
		material.SetVector("_WorldPosition", transform.position);

		if (FadeEdge == true)
		{
			keywords.Add("SGT_A");

			material.SetFloat("_FadePower", FadePower);
		}

		if (ClipNear == true)
		{
			keywords.Add("SGT_B");

			material.SetFloat("_ClipPower", ClipPower);
		}

		SgtHelper.SetKeywords(material, keywords); keywords.Clear();
	}

	private void UpdatePlanes()
	{
		planes.RemoveAll(p => p == null);

		if (PlaneCount != planes.Count)
		{
			SgtHelper.ResizeArrayTo(ref planes, PlaneCount, i => SgtProminencePlane.Create(this), p => SgtProminencePlane.Pool(p));
		}

		SgtHelper.BeginRandomSeed(Seed);
		{
			for (var i = planes.Count - 1; i >= 0; i--)
			{
				planes[i].ManualUpdate(mesh, material, Random.rotationUniform);
			}
		}
		SgtHelper.EndRandomSeed();
	}

	private void RegenerateMesh()
	{
		mesh = SgtObjectPool<Mesh>.Add(mesh, m => m.Clear());

		SgtProceduralMesh.Clear();
		{
			if (Detail >= 3)
			{
				var angleStep  = SgtHelper.Divide(Mathf.PI * 2.0f, Detail);
				var coordStep  = SgtHelper.Reciprocal(Detail);

				for (var i = 0; i <= Detail; i++)
				{
					var coord = coordStep * i;
					var angle = angleStep * i;
					var sin   = Mathf.Sin(angle);
					var cos   = Mathf.Cos(angle);
					var iPos  = new Vector3(sin * InnerRadius, 0.0f, cos * InnerRadius);
					var oPos  = new Vector3(sin * OuterRadius, 0.0f, cos * OuterRadius);

					SgtProceduralMesh.PushPosition(iPos);
					SgtProceduralMesh.PushPosition(oPos);

					SgtProceduralMesh.PushNormal(Vector3.up);
					SgtProceduralMesh.PushNormal(Vector3.up);

					SgtProceduralMesh.PushCoord1(0.0f, coord * InnerRadius);
					SgtProceduralMesh.PushCoord1(1.0f, coord * OuterRadius);

					SgtProceduralMesh.PushCoord2(InnerRadius, 0.0f);
					SgtProceduralMesh.PushCoord2(OuterRadius, 0.0f);
				}
			}
		}
		SgtProceduralMesh.SplitStrip(HideFlags.DontSave);

		mesh = SgtProceduralMesh.Pop(); SgtProceduralMesh.Discard();
	}

	private void CameraPreCull(Camera camera)
	{
		if (ObserverOffset != 0.0f)
		{
			for (var i = planes.Count - 1; i >= 0; i--)
			{
				var plane = planes[i];

				if (plane != null)
				{
					var planeTransform = plane.transform;
					var oldPosition    = planeTransform.position;
					var observerDir    = (oldPosition - camera.transform.position).normalized;

					if (plane.TempSet == false)
					{
						plane.TempSet      = true;
						plane.TempPosition = oldPosition;
					}

					planeTransform.position += observerDir * ObserverOffset;
				}
			}
		}
	}

	private void CameraPostRender(Camera camera)
	{
		if (ObserverOffset != 0.0f)
		{
			for (var i = planes.Count - 1; i >= 0; i--)
			{
				var plane = planes[i];

				if (plane != null && plane.TempSet == true)
				{
					plane.TempSet = false;

					plane.transform.position = plane.TempPosition;
				}
			}
		}
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Prominence", false, 10)]
	public static void CreateProminenceMenuItem()
	{
		var parent     = SgtHelper.GetSelectedParent();
		var prominence = CreateProminence(parent != null ? parent.gameObject.layer : 0, parent);

		SgtHelper.SelectAndPing(prominence);
	}
#endif
}

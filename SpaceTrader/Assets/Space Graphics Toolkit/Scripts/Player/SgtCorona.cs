using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Corona")]
public class SgtCorona : MonoBehaviour
{
	public static List<SgtCorona> AllCoronas = new List<SgtCorona>();

	public Color Color = Color.white;

	public float Brightness = 1.0f;

	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	public int RenderQueueOffset;

	[SgtRange(0.0f, 1.0f)]
	public float Fog = 0.0f;

	public bool Smooth = true;

	public float InnerPower = 3.0f;

	public float InnerMeshRadius = 1.0f;

	public List<MeshRenderer> InnerRenderers = new List<MeshRenderer>();

	public float MiddlePower = 0.0f;

	[SgtRange(0.0f, 1.0f)]
	public float MiddleRatio = 0.5f;

	public float OuterPower = 3.0f;

	public float OuterMeshRadius = 1.0f;

	public List<Mesh> OuterMeshes = new List<Mesh>();

	public Gradient DensityColor = new Gradient();

	public float DensityScale = 1.0f;

	public float Height = 0.1f;

	protected bool lutDirty = true;

	[System.NonSerialized]
	protected Texture2D atmosphereLut;

	[System.NonSerialized]
	protected Material innerMaterial;

	[System.NonSerialized]
	protected Material outerMaterial;

	[SerializeField]
	protected bool awakeCalled;

	[SerializeField]
	protected List<SgtCoronaOuter> outers = new List<SgtCoronaOuter>();

	protected static List<string> innerKeywords = new List<string>();

	protected static List<string> outerKeywords = new List<string>();

	private static GradientColorKey[] defaultAtmosphereColor = new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.5f) };

	public Material InnerMaterial
	{
		get
		{
			if (innerMaterial == null)
			{
				UpdateMaterial();
			}

			return innerMaterial;
		}
	}

	public float OuterRadius
	{
		get
		{
			return InnerMeshRadius + Height;
		}
	}

	public float MiddleHeight
	{
		get
		{
			return MiddleRatio * Height;
		}
	}

	public float MiddleRadius
	{
		get
		{
			return InnerMeshRadius + MiddleRatio * Height;
		}
	}

	public void MarkLutAsDirty()
	{
#if UNITY_EDITOR
		if (lutDirty == false)
		{
			SgtHelper.SetDirty(this);
		}
#endif

		lutDirty = true;
	}

	public void UpdateState()
	{
		UpdateDirty();
		UpdateMaterial();
		UpdateInnerRenderers();
		UpdateOuters();
	}

	protected virtual float CalculateOuterPower(Vector3 cameraPosition, float clampedAltitude)
	{
		return Mathf.Lerp(MiddlePower, OuterPower, clampedAltitude);
	}

	protected virtual void Awake()
	{
		outers.RemoveAll(o => o == null || o.Corona != this);

		if (awakeCalled == false)
		{
			awakeCalled = true;

			AwakeOnce();
        }
	}

	protected virtual void AwakeOnce()
	{
		DensityColor.colorKeys = defaultAtmosphereColor;

		var terrain = GetComponent<SgtTerrain>();

		if (terrain != null)
		{
			terrain.Corona = this;
		}
		else
		{
			var meshRenderer = GetComponent<MeshRenderer>();

			if (meshRenderer != null)
			{
				var meshFilter = GetComponent<MeshFilter>();

				if (meshFilter != null)
				{
					var mesh = meshFilter.sharedMesh;

					if (mesh != null)
					{
						var min = mesh.bounds.min;
						var max = mesh.bounds.max;
						var avg = Mathf.Abs(min.x) + Mathf.Abs(min.y) + Mathf.Abs(min.z) + Mathf.Abs(max.x) + Mathf.Abs(max.y) + Mathf.Abs(max.z);

						InnerMeshRadius = avg / 6.0f;
					}
				}

				InnerRenderers.Add(meshRenderer);

#if UNITY_EDITOR
				var guids = UnityEditor.AssetDatabase.FindAssets("Geosphere40 t:mesh");

				if (guids.Length > 0)
				{
					var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
					var mesh = (Mesh)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));

					if (mesh != null)
					{
						OuterMeshes.Add(mesh);
					}
				}
#endif
			}
		}
	}

	protected virtual void OnEnable()
	{
#if UNITY_EDITOR
		if (AllCoronas.Count == 0)
		{
			SgtHelper.RepaintAll();
		}
#endif
		AllCoronas.Add(this);

		Camera.onPreRender += CameraPreRender;

		for (var i = outers.Count - 1; i >= 0; i--)
		{
			var outer = outers[i];

			if (outer != null)
			{
				outer.gameObject.SetActive(true);
			}
		}
	}

	protected virtual void OnDisable()
	{
		AllCoronas.Remove(this);

		Camera.onPreRender -= CameraPreRender;

		for (var i = InnerRenderers.Count - 1; i >= 0; i--)
		{
			var innerRenderer = InnerRenderers[i];

			SgtHelper.RemoveMaterial(innerRenderer, innerMaterial);
		}

		for (var i = outers.Count - 1; i >= 0; i--)
		{
			var outer = outers[i];

			if (outer != null)
			{
				outer.gameObject.SetActive(false);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		for (var i = InnerRenderers.Count - 1; i >= 0; i--)
		{
			var innerRenderer = InnerRenderers[i];

			SgtHelper.RemoveMaterial(innerRenderer, innerMaterial);
		}

		SgtHelper.Destroy(atmosphereLut);
		SgtHelper.Destroy(outerMaterial);
		SgtHelper.Destroy(innerMaterial);

		for (var i = outers.Count - 1; i >= 0; i--)
		{
			SgtCoronaOuter.MarkForDestruction(outers[i]);
		}

		outers.Clear();
	}

	protected virtual void Update()
	{
		UpdateState();
	}

	protected virtual void CameraPreRender(Camera camera)
	{
		UpdateMaterial();

		var cameraPosition       = camera.transform.position;
		var localCameraPosition  = transform.InverseTransformPoint(cameraPosition);
		var localDistance        = localCameraPosition.magnitude;
		var clampedAltitude      = Mathf.InverseLerp(MiddleRadius, OuterRadius, localDistance);
		var innerAtmosphereDepth = default(float);
		var outerAtmosphereDepth = default(float);
		var radiusRatio          = SgtHelper.Divide(InnerMeshRadius, OuterRadius);
		var innerHeightRatio     = SgtHelper.Divide(MiddleHeight, OuterRadius);
		var scaleDistance        = SgtHelper.Divide(localDistance, OuterRadius);
		var fog                  = 1.0f - Fog;

		SgtHelper.CalculateAtmosphereThicknessAtHorizon(radiusRatio, 1.0f, scaleDistance, out innerAtmosphereDepth, out outerAtmosphereDepth);

		// Make the fog level reduce when you get closer than the ground height
		innerAtmosphereDepth = Mathf.Max(innerAtmosphereDepth, innerHeightRatio);

		if (scaleDistance > 1.0f)
		{
			innerKeywords.Add("SGT_A"); outerKeywords.Add("SGT_A");
		}

		if (Smooth == true)
		{
			innerKeywords.Add("SGT_B"); outerKeywords.Add("SGT_B");
		}

		SgtHelper.SetKeywords(innerMaterial, innerKeywords); innerKeywords.Clear();
		SgtHelper.SetKeywords(outerMaterial, outerKeywords); outerKeywords.Clear();

		innerMaterial.SetFloat("_HorizonLengthRecip", SgtHelper.Reciprocal(innerAtmosphereDepth * fog));
		outerMaterial.SetFloat("_HorizonLengthRecip", SgtHelper.Reciprocal(outerAtmosphereDepth * fog));

		outerMaterial.SetFloat("_Power", CalculateOuterPower(cameraPosition, clampedAltitude));
	}

	protected virtual void RegenerateAtmosphereLut()
	{
		if (atmosphereLut == null || atmosphereLut.width != 1 || atmosphereLut.height != 64)
		{
			SgtHelper.Destroy(atmosphereLut);

			atmosphereLut = SgtHelper.CreateTempTeture2D(1, 64);
		}

		for (var y = 0; y < atmosphereLut.height; y++)
		{
			var t = y / (float)atmosphereLut.height;

			atmosphereLut.SetPixel(0, y, DensityColor.Evaluate(t));
		}

		atmosphereLut.wrapMode = TextureWrapMode.Clamp;
		atmosphereLut.Apply();
	}

	protected virtual void UpdateDirty()
	{
		if (atmosphereLut == null) lutDirty = true;

		if (lutDirty == true)
		{
			lutDirty = false;

			RegenerateAtmosphereLut();
		}
	}

	protected virtual void UpdateMaterial()
	{
		if (innerMaterial == null) innerMaterial = SgtHelper.CreateTempMaterial(SgtHelper.ShaderNamePrefix + "CoronaInner");
		if (outerMaterial == null) outerMaterial = SgtHelper.CreateTempMaterial(SgtHelper.ShaderNamePrefix + "CoronaOuter");

		var scale        = SgtHelper.Divide(OuterMeshRadius, OuterRadius);
		var worldToLocal = SgtHelper.Scaling(scale) * transform.worldToLocalMatrix;
		var color        = SgtHelper.Brighten(Color, Brightness);
		var renderQueue  = (int)RenderQueue + RenderQueueOffset;

		innerMaterial.renderQueue = renderQueue;
		innerMaterial.SetColor("_Color", color);
		innerMaterial.SetTexture("_AtmosphereLut", atmosphereLut);
		innerMaterial.SetFloat("_AtmosphereScale", DensityScale);
		innerMaterial.SetFloat("_Power", InnerPower);
		innerMaterial.SetFloat("_SkyRadius", OuterRadius);
		innerMaterial.SetFloat("_SkyRadiusRecip", SgtHelper.Reciprocal(OuterRadius));
		innerMaterial.SetMatrix("_WorldToLocal", worldToLocal);

		outerMaterial.renderQueue = renderQueue;
		outerMaterial.SetColor("_Color", color);
		outerMaterial.SetTexture("_AtmosphereLut", atmosphereLut);
		outerMaterial.SetFloat("_AtmosphereScale", DensityScale);
		outerMaterial.SetMatrix("_WorldToLocal", worldToLocal);
	}

	private void UpdateInnerRenderers()
	{
		for (var i = InnerRenderers.Count - 1; i >= 0; i--)
		{
			var innerRenderer = InnerRenderers[i];

			if (innerRenderer != null)
			{
				SgtHelper.BeginStealthSet(innerRenderer);
				{
					SgtHelper.ReplaceMaterial(innerRenderer, innerMaterial);
				}
				SgtHelper.EndStealthSet();
			}
		}
	}

	private void UpdateOuters()
	{
		outers.RemoveAll(o => o == null);

		if (OuterMeshes.Count != outers.Count)
		{
			SgtHelper.ResizeArrayTo(ref outers, OuterMeshes.Count, i => SgtCoronaOuter.Create(this), o => SgtCoronaOuter.Pool(o));
		}

		var outerScale = SgtHelper.Divide(OuterRadius, OuterMeshRadius);

		for (var i = OuterMeshes.Count - 1; i >= 0; i--)
		{
			outers[i].ManualUpdate(OuterMeshes[i], outerMaterial, outerScale);
		}
	}

#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		if (SgtHelper.Enabled(this) == true)
		{
			var r0 = InnerMeshRadius;
			var r1 = MiddleRadius;
			var r2 = OuterRadius;

			SgtHelper.DrawSphere(transform.position, transform.right * transform.lossyScale.x * r0, transform.up * transform.lossyScale.y * r0, transform.forward * transform.lossyScale.z * r0);
			SgtHelper.DrawSphere(transform.position, transform.right * transform.lossyScale.x * r1, transform.up * transform.lossyScale.y * r1, transform.forward * transform.lossyScale.z * r1);
			SgtHelper.DrawSphere(transform.position, transform.right * transform.lossyScale.x * r2, transform.up * transform.lossyScale.y * r2, transform.forward * transform.lossyScale.z * r2);
		}
	}
#endif
}

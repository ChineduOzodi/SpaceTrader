using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Cloudsphere")]
public class SgtCloudsphere : MonoBehaviour
{
	public static List<SgtCloudsphere> AllCloudspheres = new List<SgtCloudsphere>();

	public List<Light> Lights = new List<Light>();

	public List<SgtShadow> Shadows = new List<SgtShadow>();

	public float MeshRadius = 1.0f;

	public List<Mesh> Meshes = new List<Mesh>();

	public Color Color = Color.white;

	public float Brightness = 1.0f;

	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	public int RenderQueueOffset;

	public float Radius = 1.5f;

	public bool FadeNear;

	public float FadeInnerRadius = 0.25f;

	public float FadeOuterRadius = 0.5f;

	public float ObserverOffset;

	public Cubemap MainTex;

	public Gradient LightingBrightness = new Gradient();

	public Gradient LightingColor = new Gradient();

	public Gradient RimColor = new Gradient();

	private bool lutDirty = true;

	[SerializeField]
	private bool awakeCalled;

	[System.NonSerialized]
	private Material material;

	[System.NonSerialized]
	private Texture2D lightingLut;

	[System.NonSerialized]
	private Texture2D rimLut;

	[SerializeField]
	private List<SgtCloudsphereModel> models = new List<SgtCloudsphereModel>();

	private static List<string> keywords = new List<string>();

	private static GradientColorKey[] defaultLightingBrightness = new GradientColorKey[] { new GradientColorKey(Color.black, 0.4f), new GradientColorKey(Color.white, 0.6f) };

	private static GradientColorKey[] defaultLightingColor = new GradientColorKey[] { new GradientColorKey(Color.red, 0.25f), new GradientColorKey(Color.white, 0.5f) };

	private static GradientColorKey[] defaultRimColor = new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.white, 0.5f) };

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
		UpdateModels();
	}

	public static SgtCloudsphere CreateCloudsphere(int layer = 0, Transform parent = null)
	{
		return CreateCloudsphere(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtCloudsphere CreateCloudsphere(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject  = SgtHelper.CreateGameObject("Cloudsphere", layer, parent, localPosition, localRotation, localScale);
		var cloudsphere = gameObject.AddComponent<SgtCloudsphere>();

		return cloudsphere;
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Cloudsphere", false, 10)]
	public static void CreateCloudsphereMenuItem()
	{
		var parent      = SgtHelper.GetSelectedParent();
		var cloudsphere = CreateCloudsphere(parent != null ? parent.gameObject.layer : 0, parent);

		SgtHelper.SelectAndPing(cloudsphere);
	}
#endif

	protected virtual void Awake()
	{
		models.RemoveAll(m => m == null || m.Cloudsphere != this);

		if (awakeCalled == false)
		{
			awakeCalled = true;

			LightingBrightness.colorKeys = defaultLightingBrightness;

			LightingColor.colorKeys = defaultLightingColor;

			RimColor.colorKeys = defaultRimColor;
		}
	}

	protected virtual void OnEnable()
	{
		Camera.onPreCull    += CameraPreCull;
		Camera.onPostRender += CameraPostRender;

		AllCloudspheres.Add(this);

		for (var i = models.Count - 1; i >= 0; i--)
		{
			var model = models[i];

			if (model != null)
			{
				model.gameObject.SetActive(true);
			}
		}
	}

	protected virtual void OnDisable()
	{
		Camera.onPreCull    -= CameraPreCull;
		Camera.onPostRender -= CameraPostRender;

		AllCloudspheres.Remove(this);

		for (var i = models.Count - 1; i >= 0; i--)
		{
			var model = models[i];

			if (model != null)
			{
				model.gameObject.SetActive(false);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		SgtHelper.Destroy(material);

		for (var i = models.Count - 1; i >= 0; i--)
		{
			SgtCloudsphereModel.MarkForDestruction(models[i]);
		}

		models.Clear();
	}

	protected virtual void Update()
	{
		UpdateState();
	}

	private void UpdateDirty()
	{
		if (lightingLut == null || rimLut == null) lutDirty = true;

		if (lutDirty == true)
		{
			lutDirty = false;

			RegenerateRimLut();
			RegenerateLightingLut();
		}
	}

	private void RegenerateRimLut()
	{
		if (rimLut == null || rimLut.width != 1 || rimLut.height != 64)
		{
			SgtHelper.Destroy(rimLut);

			rimLut = SgtHelper.CreateTempTeture2D(1, 64);
		}

		for (var y = 0; y < rimLut.height; y++)
		{
			var t = y / (float)rimLut.height;

			rimLut.SetPixel(0, y, RimColor.Evaluate(t));
		}

		rimLut.wrapMode = TextureWrapMode.Clamp;
		rimLut.Apply();
	}

	private void RegenerateLightingLut()
	{
		if (lightingLut == null || lightingLut.width != 1 || lightingLut.height != 64)
		{
			SgtHelper.Destroy(lightingLut);

			lightingLut = SgtHelper.CreateTempTeture2D(1, 64);
		}

		for (var y = 0; y < lightingLut.height; y++)
		{
			var t = y / (float)lightingLut.height;

			lightingLut.SetPixel(0, y, LightingBrightness.Evaluate(t) * LightingColor.Evaluate(t));
		}

		lightingLut.wrapMode = TextureWrapMode.Clamp;
		lightingLut.Apply();
	}

	private void UpdateMaterial()
	{
		if (material == null) material = SgtHelper.CreateTempMaterial(SgtHelper.ShaderNamePrefix + "Cloudsphere");

		var color       = SgtHelper.Brighten(Color, Brightness);
		var renderQueue = (int)RenderQueue + RenderQueueOffset;

		if (FadeNear == true)
		{
			keywords.Add("SGT_A");

			material.SetFloat("_FadeRadius", FadeInnerRadius);
			material.SetFloat("_FadeScale", SgtHelper.Reciprocal(FadeOuterRadius - FadeInnerRadius));
		}

		material.renderQueue = renderQueue;
		material.SetTexture("_MainTex", MainTex);
		material.SetColor("_Color", color);
		material.SetTexture("_RimLut", rimLut);
		material.SetTexture("_LightingLut", lightingLut);

		var lightCount  = SgtHelper.WriteLights(Lights, 2, transform.position, null, null, material);
		var shadowCount = SgtHelper.WriteShadows(Shadows, 2, material);

		SgtHelper.WriteLightKeywords(Lights.Count > 0, lightCount, keywords);
		SgtHelper.WriteShadowKeywords(shadowCount, keywords);

		SgtHelper.SetKeywords(material, keywords); keywords.Clear();
	}

	private void UpdateModels()
	{
		models.RemoveAll(m => m == null);

		if (Meshes.Count != models.Count)
		{
			SgtHelper.ResizeArrayTo(ref models, Meshes.Count, i => SgtCloudsphereModel.Create(this), m => SgtCloudsphereModel.Pool(m));
		}

		var scale = SgtHelper.Divide(Radius, MeshRadius);

		for (var i = Meshes.Count - 1; i >= 0; i--)
		{
			models[i].ManualUpdate(Meshes[i], material, scale);
		}
	}

	private void CameraPreCull(Camera camera)
	{
		if (ObserverOffset != 0.0f)
		{
			for (var i = models.Count - 1; i >= 0; i--)
			{
				var model = models[i];

				if (model != null)
				{
					var modelTransform = model.transform;
					var oldPosition    = modelTransform.position;
					var observerDir    = (oldPosition - camera.transform.position).normalized;

					if (model.TempSet == false)
					{
						model.TempSet      = true;
						model.TempPosition = oldPosition;
					}

					modelTransform.position += observerDir * ObserverOffset;
				}
			}
		}
	}

	private void CameraPostRender(Camera camera)
	{
		if (ObserverOffset != 0.0f)
		{
			for (var i = models.Count - 1; i >= 0; i--)
			{
				var model = models[i];

				if (model != null && model.TempSet == true)
				{
					model.TempSet = false;

					model.transform.position = model.TempPosition;
				}
			}
		}
	}
}

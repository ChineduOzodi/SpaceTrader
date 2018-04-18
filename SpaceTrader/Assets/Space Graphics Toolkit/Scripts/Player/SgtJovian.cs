using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Jovian")]
public class SgtJovian : MonoBehaviour
{
	public static List<SgtJovian> AllJovians = new List<SgtJovian>();

	public List<Light> Lights = new List<Light>();

	public List<SgtShadow> Shadows = new List<SgtShadow>();

	public float MeshRadius = 1.0f;

	public List<Mesh> Meshes = new List<Mesh>();

	public Color Color = Color.white;

	public float Brightness = 1.0f;

	public SgtRenderQueue RenderQueue = SgtRenderQueue.Transparent;

	public int RenderQueueOffset;

	public bool Smooth = true;

	public bool Scattering;

	[SgtRange(0.0f, 5.0f)]
	public float MieSharpness = 2.0f;

	[SgtRange(0.0f, 10.0f)]
	public float MieStrength = 1.0f;

	public bool LimitAlpha = true;

	public Cubemap MainTex;

	public float Power = 3.0f;

	public float Density = 10.0f;

	public SgtOutputMode DensityMode;

	public Gradient LightingBrightness = new Gradient();

	public Gradient LightingColor = new Gradient();

	public Gradient RimColor = new Gradient();

	private bool lutDirty = true;

	private int lightCount;

	private int shadowCount;

	[SerializeField]
	private bool awakeCalled;

	[System.NonSerialized]
	private Material material;

	[System.NonSerialized]
	private Texture2D lightingLut;

	[System.NonSerialized]
	private Texture2D rimLut;

	[SerializeField]
	private List<SgtJovianModel> models = new List<SgtJovianModel>();

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

	public static SgtJovian CreateJovian(int layer = 0, Transform parent = null)
	{
		return CreateJovian(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtJovian CreateJovian(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Jovian", layer, parent, localPosition, localRotation, localScale);
		var jovian     = gameObject.AddComponent<SgtJovian>();

		return jovian;
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Jovian", false, 10)]
	public static void CreateJovianMenuItem()
	{
		var parent = SgtHelper.GetSelectedParent();
		var jovian = CreateJovian(parent != null ? parent.gameObject.layer : 0, parent);

		SgtHelper.SelectAndPing(jovian);
	}
#endif

	protected virtual void Awake()
	{
		models.RemoveAll(m => m == null || m.Jovian != this);

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
#if UNITY_EDITOR
		if (AllJovians.Count == 0)
		{
			SgtHelper.RepaintAll();
		}
#endif
		AllJovians.Add(this);

		Camera.onPreRender += CameraPreRender;

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
		AllJovians.Remove(this);

		Camera.onPreRender -= CameraPreRender;

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
			SgtJovianModel.MarkForDestruction(models[i]);
		}

		models.Clear();
	}

	protected virtual void Update()
	{
		UpdateState();
	}

#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		if (SgtHelper.Enabled(this) == true)
		{
			var r0 = transform.lossyScale;

			SgtHelper.DrawSphere(transform.position, transform.right * r0.x, transform.up * r0.y, transform.forward * r0.z);
		}
	}
#endif

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
		if (material == null) material = SgtHelper.CreateTempMaterial(SgtHelper.ShaderNamePrefix + "Jovian");

		var color        = SgtHelper.Brighten(Color, Brightness);
		var renderQueue  = (int)RenderQueue + RenderQueueOffset;
		var localToWorld = transform.localToWorldMatrix * SgtHelper.Scaling(MeshRadius * 2.0f); // Double mesh radius so the max thickness caps at 1.0

		material.renderQueue = renderQueue;
		material.SetTexture("_MainTex", MainTex);
		material.SetColor("_Color", color);
		material.SetFloat("_Power", Power);
		material.SetFloat("_Density", Density);
		material.SetMatrix("_WorldToLocal", localToWorld.inverse);
		material.SetMatrix("_LocalToWorld", localToWorld);
		material.SetTexture("_RimLut", rimLut);
		material.SetTexture("_LightingLut", lightingLut);

		lightCount  = SgtHelper.WriteLights(Lights, 2, transform.position, transform, null, material);
		shadowCount = SgtHelper.WriteShadows(Shadows, 2, material);
	}

	private void UpdateModels()
	{
		models.RemoveAll(m => m == null);

		if (Meshes.Count != models.Count)
		{
			SgtHelper.ResizeArrayTo(ref models, Meshes.Count, i => SgtJovianModel.Create(this), m => SgtJovianModel.Pool(m));
		}

		for (var i = Meshes.Count - 1; i >= 0; i--)
		{
			models[i].ManualUpdate(Meshes[i], material);
		}
	}

	private void CameraPreRender(Camera camera)
	{
		UpdateMaterial();

		var cameraPosition      = camera.transform.position;
		var localCameraPosition = transform.InverseTransformPoint(cameraPosition);
		var localDistance       = localCameraPosition.magnitude;

		if (localDistance > MeshRadius)
		{
			keywords.Add("SGT_A");
		}

		if (Smooth == true)
		{
			keywords.Add("SGT_B");
		}

		switch (DensityMode)
		{
		case SgtOutputMode.Linear:                             break;
		case SgtOutputMode.Logarithmic: keywords.Add("SGT_C"); break;
		}

		if (Scattering == true)
		{
			keywords.Add("SGT_D");

			SgtHelper.WriteMie(MieSharpness, MieStrength, material);

			if (LimitAlpha == true)
			{
				keywords.Add("SGT_E");
			}
		}

		SgtHelper.WriteLightKeywords(Lights.Count > 0, lightCount, keywords);
		SgtHelper.WriteShadowKeywords(shadowCount, keywords);

		SgtHelper.SetKeywords(material, keywords); keywords.Clear();
	}
}

using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Atmosphere")]
public class SgtAtmosphere : SgtCorona
{
	public static List<SgtAtmosphere> AllAtmospheres = new List<SgtAtmosphere>();

	public List<Light> Lights = new List<Light>();

	public List<SgtShadow> Shadows = new List<SgtShadow>();

	public bool Scattering;

	[SgtRange(0.0f, 5.0f)]
	public float MieSharpness = 2.0f;

	[SgtRange(0.0f, 10.0f)]
	public float MieStrength = 1.0f;

	[SgtRange(0.0f, 10.0f)]
	public float RayleighStrength = 0.1f;

	public bool GroundScattering;

	public float GroundPower = 2.0f;

	[SgtRange(0.0f, 5.0f)]
	public float GroundMieSharpness = 1.0f;

	[SgtRange(0.0f, 10.0f)]
	public float GroundMieStrength = 0.5f;

	public bool LimitBrightness = true;

	public Gradient LightingBrightness = new Gradient();

	public Gradient LightingColor = new Gradient();

	private Texture2D lightingLut;

	private int lightCount;

	private int shadowCount;

	private static GradientColorKey[] defaultLightingBrightness = new GradientColorKey[] { new GradientColorKey(Color.black, 0.4f), new GradientColorKey(Color.white, 0.6f) };

	private static GradientColorKey[] defaultLightingColor = new GradientColorKey[] { new GradientColorKey(Color.red, 0.25f), new GradientColorKey(Color.white, 0.5f) };

	private static GradientColorKey[] defaultAtmosphereColor = new GradientColorKey[] { new GradientColorKey(Color.cyan, 0.0f), new GradientColorKey(Color.white, 1.0f) };

	protected override void AwakeOnce()
	{
		base.AwakeOnce();

		LightingBrightness.colorKeys = defaultLightingBrightness;

		LightingColor.colorKeys = defaultLightingColor;

		DensityColor.colorKeys = defaultAtmosphereColor;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		AllAtmospheres.Add(this);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		AllAtmospheres.Remove(this);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		SgtHelper.Destroy(lightingLut);
	}

	protected override void CameraPreRender(Camera camera)
	{
		if (Scattering == true)
		{
			outerKeywords.Add("SGT_C");

			if (GroundScattering == true)
			{
				innerKeywords.Add("SGT_C");
			}

			if (LimitBrightness == true)
			{
				innerKeywords.Add("SGT_D");
				outerKeywords.Add("SGT_D");
			}
		}

		SgtHelper.WriteLightKeywords(Lights.Count > 0, lightCount, innerKeywords, outerKeywords);
		SgtHelper.WriteShadowKeywords(shadowCount, innerKeywords, outerKeywords);

		base.CameraPreRender(camera);
	}

	protected override float CalculateOuterPower(Vector3 cameraPosition, float clampedAltitude)
	{
		var cameraDir  = (cameraPosition - transform.position).normalized;
		var lightCount = 0;
		var maxLights  = 2;
		var strength   = 1.0f - clampedAltitude;

		for (var i = Lights.Count - 1; i >= 0; i--)
		{
			var light = Lights[i];

			if (SgtHelper.Enabled(light) == true && light.intensity > 0.0f && lightCount < maxLights)
			{
				var direction = default(Vector3);
				var position  = default(Vector3);
				var color     = default(Color);

				SgtHelper.CalculateLight(light, transform.position, null, null, ref position, ref direction, ref color);

				var dot      = Vector3.Dot(direction, cameraDir);
				var lighting = LightingBrightness.Evaluate(dot * 0.5f + 0.5f);

				clampedAltitude += (1.0f - lighting.a) * strength;
			}
		}

		return base.CalculateOuterPower(cameraPosition, clampedAltitude);
	}

	protected override void UpdateDirty()
	{
		if (lightingLut == null || atmosphereLut == null) lutDirty = true;

		if (lutDirty == true)
		{
			lutDirty = false;

			RegenerateLightingLut();
			RegenerateAtmosphereLut();
		}
	}

	protected override void UpdateMaterial()
	{
		if (innerMaterial == null) innerMaterial = SgtHelper.CreateTempMaterial(SgtHelper.ShaderNamePrefix + "AtmosphereInner");
		if (outerMaterial == null) outerMaterial = SgtHelper.CreateTempMaterial(SgtHelper.ShaderNamePrefix + "AtmosphereOuter");

		base.UpdateMaterial();

		innerMaterial.SetTexture("_LightingLut", lightingLut);

		outerMaterial.SetTexture("_LightingLut", lightingLut);

		innerMaterial.SetFloat("_ScatteringPower", GroundPower);

		if (Scattering == true)
		{
			SgtHelper.WriteMie(MieSharpness, MieStrength, outerMaterial);
			SgtHelper.WriteRayleigh(RayleighStrength, outerMaterial);

			if (GroundScattering == true)
			{
				SgtHelper.WriteMie(GroundMieSharpness, GroundMieStrength, innerMaterial);
				SgtHelper.WriteRayleigh(RayleighStrength, innerMaterial);
			}
		}

		lightCount  = SgtHelper.WriteLights(Lights, 2, transform.position, transform, null, innerMaterial, outerMaterial);
		shadowCount = SgtHelper.WriteShadows(Shadows, 2, innerMaterial, outerMaterial);
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
			var a = LightingBrightness.Evaluate(t);
			var b = LightingColor.Evaluate(t);
			var c = a * b;

			c.a = c.grayscale;

			lightingLut.SetPixel(0, y, c);
		}

		lightingLut.wrapMode = TextureWrapMode.Clamp;
		lightingLut.Apply();
	}
}

using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Nebula Starfield")]
public class SgtNebulaStarfield : SgtStarfield
{
	[SgtSeed]
	[Tooltip("This seed used when generating random values for this component")]
	public int Seed;

	[Tooltip("This texture used to color the nebula particles")]
	public Texture SourceTex;

	[SgtRange(0.0f, 1.0f)]
	[Tooltip("This amount of particles that get generated from the SourceTex")]
	public float Resolution = 0.1f;

	[SgtRange(0.0f, 1.0f)]
	[Tooltip("This brightness of the sampled SourceTex pixel for a particle to be spawned")]
	public float Threshold = 0.1f;

	[SgtRange(0.0f, 1.0f)]
	[Tooltip("This allows you to randomly offset each nebula particle position")]
	public float Jitter;

	[Tooltip("The calculation used to find the height offset of a particle in the nebula")]
	public SgtNebulaSource HeightSource = SgtNebulaSource.None;

	[Tooltip("The size of the generated nebula")]
	public Vector3 Size = new Vector3(100.0f, 100.0f, 100.0f);

	[Tooltip("The brightness of the nebula when viewed from the side (good for galaxies)")]
	public float HorizontalBrightness = 0.25f;

	[Tooltip("The relationship between the Brightness and HorizontalBrightness relative to the vieing angle")]
	public float HorizontalPower = 1.0f;

	[Tooltip("The minimum size of a generated nebula particle")]
	public float StarRadiusMin = 0.0f;

	[Tooltip("The maximum size of a generated nebula particle")]
	public float StarRadiusMax = 1.0f;

	[SgtRange(0.0f, 1.0f)]
	[Tooltip("The maximum pulse size of a generated nebula particle")]
	public float StarPulseMax = 1.0f;

	[Tooltip("The sprite textures applied to the stars")]
	public List<Sprite> StarSprites = new List<Sprite>();

	protected override void CameraPreCull(Camera camera)
	{
		base.CameraPreCull(camera);

		var dir    = (transform.position - camera.transform.position).normalized;
		var theta  = Mathf.Abs(Vector3.Dot(transform.up, dir));
		var bright = Mathf.Lerp(HorizontalBrightness, Brightness, Mathf.Pow(theta, HorizontalPower));
		var color  = SgtHelper.Brighten(Color, Color.a * bright);

		for (var i = groups.Count - 1; i >= 0; i--)
		{
			var group = groups[i];

			if (group != null)
			{
				if (group.Material != null)
				{
					group.Material.SetColor("_Color", color);
				}
			}
		}
	}

	public static SgtNebulaStarfield CreateNebulaStarfield(int layer = 0, Transform parent = null)
	{
		return CreateNebulaStarfield(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtNebulaStarfield CreateNebulaStarfield(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Nebula Starfield", layer, parent, localPosition, localRotation, localScale);
		var starfield  = gameObject.AddComponent<SgtNebulaStarfield>();

		return starfield;
	}

	protected override void CalculateStars(out List<SgtStarfieldStar> stars, out bool pool)
	{
		var texture = SourceTex as Texture2D;

		stars = tempStars; tempStars.Clear();
		pool  = true;

		if (texture != null && Resolution > 0.0f && Resolution <= 1.0f)
		{
#if UNITY_EDITOR
			SgtHelper.MakeTextureReadable(texture);
			SgtHelper.MakeTextureTruecolor(texture);
#endif
			var samplesX = Mathf.FloorToInt(SourceTex.width  * Resolution);
			var samplesY = Mathf.FloorToInt(SourceTex.height * Resolution);
			var stepX    = SgtHelper.Reciprocal(samplesX);
			var stepY    = SgtHelper.Reciprocal(samplesY);
			var halfSize = Size * 0.5f;
			var scale    = SgtHelper.Divide(SourceTex.width, samplesX);

			SgtHelper.BeginRandomSeed(Seed);
			{
				for (var y = 0; y < samplesY; y++)
				{
					for (var x = 0; x < samplesX; x++)
					{
						var fracX = x * stepX;
						var fracY = y * stepY;
						var pixel = texture.GetPixelBilinear(fracX, fracY);
						var gray  = pixel.grayscale;

						if (gray > Threshold)
						{
							var star      = SgtClassPool<SgtStarfieldStar>.Pop() ?? new SgtStarfieldStar(); stars.Add(star);
							var position  = -halfSize + Random.insideUnitSphere * Jitter * StarRadiusMax * scale;

							position.x += Size.x * fracX;
							position.y += Size.y * GetHeight(pixel);
							position.z += Size.z * fracY;

							star.Sprite      = GetRandomStarSprite();
							star.Color       = pixel;
							star.Radius      = Random.Range(StarRadiusMin, StarRadiusMax) * scale;
							star.Angle       = Random.Range(0.0f, Mathf.PI * 2.0f);
							star.Position    = position;
							star.PulseRange  = Random.value * StarPulseMax;
							star.PulseSpeed  = Random.value;
							star.PulseOffset = Random.value;
						}
					}
				}
			}
			SgtHelper.EndRandomSeed();
		}
	}

#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;

		Gizmos.DrawWireCube(Vector3.zero, Size);
	}
#endif

	private Sprite GetRandomStarSprite()
	{
		if (StarSprites != null && StarSprites.Count > 0)
		{
			var index = Random.Range(0, StarSprites.Count);

			return StarSprites[index];
		}

		return null;
	}

	private float GetHeight(Color pixel)
	{
		switch (HeightSource)
		{
			case SgtNebulaSource.None: return 0.5f;
			case SgtNebulaSource.Red: return pixel.r;
			case SgtNebulaSource.Green: return pixel.g;
			case SgtNebulaSource.Blue: return pixel.b;
			case SgtNebulaSource.Alpha: return pixel.a;
			case SgtNebulaSource.AverageRgb: return (pixel.r + pixel.g + pixel.b) / 3.0f;
			case SgtNebulaSource.MinRgb: return Mathf.Min(pixel.r, Mathf.Min(pixel.g, pixel.b));
			case SgtNebulaSource.MaxRgb: return Mathf.Max(pixel.r, Mathf.Max(pixel.g, pixel.b));
		}

		return 0.0f;
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Nebula Starfield", false, 10)]
	private static void CreateNebulaStarfieldMenuItem()
	{
		var parent    = SgtHelper.GetSelectedParent();
		var starfield = CreateNebulaStarfield(parent != null ? parent.gameObject.layer : 0, parent);

		SgtHelper.SelectAndPing(starfield);
	}
#endif
}

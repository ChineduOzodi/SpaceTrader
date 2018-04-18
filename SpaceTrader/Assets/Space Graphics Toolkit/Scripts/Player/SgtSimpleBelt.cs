using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Simple Belt")]
public class SgtSimpleBelt : SgtBelt
{
	[SgtSeed]
	public int Seed;

	public float Thickness;

	public float InnerRadius = 1.0f;

	public float InnerSpeed = 0.1f;

	public float OuterRadius = 2.0f;

	public float OuterSpeed = 0.05f;

	public int AsteroidCount = 1000;

	public float AsteroidSpin = 1.0f;

	public float AsteroidRadiusMin = 0.025f;

	public float AsteroidRadiusMax = 0.05f;

	public List<SgtBeltAsteroidVariant> AsteroidVariants = new List<SgtBeltAsteroidVariant>();

	public SgtBeltAsteroidVariant GetRandomAsteroidVariant()
	{
		if (AsteroidVariants != null && AsteroidVariants.Count > 0)
		{
			var index = Random.Range(0, AsteroidVariants.Count);

			return AsteroidVariants[index];
		}

		return default(SgtBeltAsteroidVariant);
	}

	public static SgtSimpleBelt CreateSimpleBelt(int layer = 0, Transform parent = null)
	{
		return CreateSimpleBelt(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtSimpleBelt CreateSimpleBelt(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Simple Belt", layer, parent, localPosition, localRotation, localScale);
		var belt       = gameObject.AddComponent<SgtSimpleBelt>();

		return belt;
	}

	protected override void CalculateAsteroids(out List<SgtBeltAsteroid> asteroids, out bool pool)
	{
		asteroids = new List<SgtBeltAsteroid>();
		pool      = true;

		SgtHelper.BeginRandomSeed(Seed);
		{
			for (var i = 0; i < AsteroidCount; i++)
			{
				var variant = GetRandomAsteroidVariant();

				if (variant != null)
				{
					var asteroid   = SgtClassPool<SgtBeltAsteroid>.Pop() ?? new SgtBeltAsteroid(); asteroids.Add(asteroid);
					var distance01 = Random.value;

					asteroid.MainTex       = variant.MainTex;
					asteroid.HeightTex     = variant.HeightTex;
					asteroid.Color         = Color.white;
					asteroid.Radius        = Random.Range(AsteroidRadiusMin, AsteroidRadiusMax);
					asteroid.Height        = Random.Range(-Thickness, Thickness);
					asteroid.Angle         = Random.Range(0.0f, Mathf.PI * 2.0f);
					asteroid.Spin          = Random.Range(-AsteroidSpin, AsteroidSpin);
					asteroid.OrbitAngle    = Random.Range(0.0f, Mathf.PI * 2.0f);
					asteroid.OrbitSpeed    = Mathf.Lerp(InnerSpeed, OuterSpeed, distance01);
					asteroid.OrbitDistance = Mathf.Lerp(InnerRadius, OuterRadius, distance01);
				}
			}
		}
		SgtHelper.EndRandomSeed();
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Simple Belt", false, 10)]
	public static void CreateSimpleBeltMenuItem()
	{
		var parent = SgtHelper.GetSelectedParent();
		var belt   = CreateSimpleBelt(parent != null ? parent.gameObject.layer : 0, parent);

		SgtHelper.SelectAndPing(belt);
	}
#endif
}

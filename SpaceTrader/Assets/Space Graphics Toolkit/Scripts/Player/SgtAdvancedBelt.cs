using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Advanced Belt")]
public class SgtAdvancedBelt : SgtBelt
{
	[SgtSeed]
	public int Seed;

	public AnimationCurve DistanceDistribution = new AnimationCurve();

	public AnimationCurve HeightDistribution = new AnimationCurve();

	public AnimationCurve SpeedDistribution = new AnimationCurve();

	public AnimationCurve SpeedNoiseDistribution = new AnimationCurve();

	public AnimationCurve RadiusDistribution = new AnimationCurve();

	public AnimationCurve SpinDistribution = new AnimationCurve();

	public int AsteroidCount = 1000;

	public List<SgtBeltAsteroidVariant> AsteroidVariants = new List<SgtBeltAsteroidVariant>();

	[SerializeField]
	private bool awakeCalled;

	private static Keyframe[] defaultDistanceKeyframes = new Keyframe[] { new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 2.0f) };

	private static Keyframe[] defaultHeightKeyframes = new Keyframe[] { new Keyframe(0.0f, -0.1f), new Keyframe(1.0f, 0.1f) };

	private static Keyframe[] defaultSpeedKeyframes = new Keyframe[] { new Keyframe(0.0f, 0.1f), new Keyframe(1.0f, 0.05f) };

	private static Keyframe[] defaultSpeedOffsetKeyframes = new Keyframe[] { new Keyframe(0.0f, 0.1f), new Keyframe(1.0f, 0.1f) };

	private static Keyframe[] defaultRadiusKeyframes = new Keyframe[] { new Keyframe(0.0f, 0.1f), new Keyframe(1.0f, 0.2f) };

	private static Keyframe[] defaultSpinKeyframes = new Keyframe[] { new Keyframe(0.0f, -0.1f), new Keyframe(1.0f, 0.1f) };

	public SgtBeltAsteroidVariant GetRandomAsteroidVariant()
	{
		if (AsteroidVariants != null && AsteroidVariants.Count > 0)
		{
			var index = Random.Range(0, AsteroidVariants.Count);

			return AsteroidVariants[index];
		}

		return default(SgtBeltAsteroidVariant);
	}

	public static SgtAdvancedBelt CreateAdvancedBelt(int layer = 0, Transform parent = null)
	{
		return CreateAdvancedBelt(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtAdvancedBelt CreateAdvancedBelt(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Advanced Belt", layer, parent, localPosition, localRotation, localScale);
		var belt       = gameObject.AddComponent<SgtAdvancedBelt>();

		return belt;
	}

	protected virtual void Awake()
	{
		if (awakeCalled == false)
		{
			awakeCalled = true;

			DistanceDistribution.keys = defaultDistanceKeyframes;
			HeightDistribution.keys = defaultHeightKeyframes;
			SpeedDistribution.keys = defaultSpeedKeyframes;
			SpeedNoiseDistribution.keys = defaultSpeedOffsetKeyframes;
			RadiusDistribution.keys = defaultRadiusKeyframes;
			SpinDistribution.keys = defaultSpinKeyframes;
		}
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
					var offset     = SpeedNoiseDistribution.Evaluate(distance01);

					asteroid.MainTex       = variant.MainTex;
					asteroid.HeightTex     = variant.HeightTex;
					asteroid.Color         = Color.white;
					asteroid.Radius        = RadiusDistribution.Evaluate(Random.value);
					asteroid.Height        = HeightDistribution.Evaluate(Random.value);
					asteroid.Angle         = Random.Range(0.0f, Mathf.PI * 2.0f);
					asteroid.Spin          = SpinDistribution.Evaluate(Random.value);
					asteroid.OrbitAngle    = Random.Range(0.0f, Mathf.PI * 2.0f);
					asteroid.OrbitSpeed    = SpeedDistribution.Evaluate(distance01) + Random.Range(-offset, offset);
					asteroid.OrbitDistance = DistanceDistribution.Evaluate(distance01);
				}
			}
		}
		SgtHelper.EndRandomSeed();
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Advanced Belt", false, 10)]
	public static void CreateAdvancedBeltMenuItem()
	{
		var belt = CreateAdvancedBelt(0, SgtHelper.GetSelectedParent());

		SgtHelper.SelectAndPing(belt);
	}
#endif
}

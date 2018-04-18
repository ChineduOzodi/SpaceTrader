using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Spiral Starfield")]
public class SgtSpiralStarfield : SgtStarfield
{
	public float Radius = 100.0f;

	[SgtSeed]
	public int Seed;

	public int ArmCount = 1;

	public float Twist = 1.0f;

	public AnimationCurve Thickness = new AnimationCurve();

	public int StarCount = 1000;

	public float StarRadiusMin = 0.0f;

	public float StarRadiusMax = 1.0f;

	[SgtRange(0.0f, 1.0f)]
	public float StarPulseMax = 1.0f;

	public List<Sprite> StarSprites = new List<Sprite>();

	[SerializeField]
	private bool awakeCalled;

	private static Keyframe[] defaultThicknessKeyframes = new Keyframe[] { new Keyframe(0.0f, 0.025f), new Keyframe(1.0f, 0.25f) };

	public static SgtSpiralStarfield CreateSpiralStarfield(int layer = 0, Transform parent = null)
	{
		return CreateSpiralStarfield(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtSpiralStarfield CreateSpiralStarfield(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Spiral Starfield", layer, parent, localPosition, localRotation, localScale);
		var starfield  = gameObject.AddComponent<SgtSpiralStarfield>();

		return starfield;
	}

	protected override void CalculateStars(out List<SgtStarfieldStar> stars, out bool pool)
	{
		stars = tempStars; tempStars.Clear();
		pool  = true;

		SgtHelper.BeginRandomSeed(Seed);
		{
			var armStep   = 360.0f * SgtHelper.Reciprocal(ArmCount);
			var twistStep = 360.0f * Twist;

			for (var i = 0; i < StarCount; i++)
			{
				var star      = SgtClassPool<SgtStarfieldStar>.Pop() ?? new SgtStarfieldStar(); stars.Add(star);
				var position  = Random.insideUnitSphere;
				var magnitude = 1 - (Random.insideUnitSphere).magnitude;

				position *= (1 - magnitude) * Thickness.Evaluate(Random.value);

				position += Quaternion.AngleAxis(i * armStep + magnitude * twistStep, Vector3.up) * Vector3.forward * magnitude;

				star.Sprite      = GetRandomStarSprite();
				star.Color       = Color.white;
				star.Radius      = Random.Range(StarRadiusMin, StarRadiusMax);
				star.Angle       = Random.Range(0.0f, Mathf.PI * 2.0f);
				star.Position    = position * Radius;
				star.PulseRange  = Random.value * StarPulseMax;
				star.PulseSpeed  = Random.value;
				star.PulseOffset = Random.value;
			}
		}
		SgtHelper.EndRandomSeed();
	}

	protected virtual void Awake()
	{
		if (awakeCalled == false)
		{
			awakeCalled = true;

			Thickness.keys = defaultThicknessKeyframes;
		}
	}

#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;

		Gizmos.DrawWireSphere(Vector3.zero, Radius);
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

#if UNITY_EDITOR
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Spiral Starfield", false, 10)]
	private static void CreateSpiralStarfieldMenuItem()
	{
		var parent    = SgtHelper.GetSelectedParent();
		var starfield = CreateSpiralStarfield(parent != null ? parent.gameObject.layer : 0, parent);

		SgtHelper.SelectAndPing(starfield);
	}
#endif
}

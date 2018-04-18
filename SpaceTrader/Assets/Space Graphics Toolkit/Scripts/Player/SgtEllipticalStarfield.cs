using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Elliptical Starfield")]
public class SgtEllipticalStarfield : SgtStarfield
{
	public float Radius = 1.0f;

	[SgtSeed]
	public int Seed;

	[SgtRange(0.0f, 1.0f)]
	public float Symmetry = 1.0f;

	[SgtRange(0.0f, 1.0f)]
	public float Offset = 0.0f;

	public bool Inverse;

	public int StarCount = 1000;

	public float StarRadiusMin = 0.0f;

	public float StarRadiusMax = 0.05f;

	[SgtRange(0.0f, 1.0f)]
	public float StarPulseMax = 1.0f;

	public List<Sprite> StarSprites = new List<Sprite>();

	public static SgtEllipticalStarfield CreateEllipticalStarfield(int layer = 0, Transform parent = null)
	{
		return CreateEllipticalStarfield(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtEllipticalStarfield CreateEllipticalStarfield(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Elliptical Starfield", layer, parent, localPosition, localRotation, localScale);
		var starfield  = gameObject.AddComponent<SgtEllipticalStarfield>();

		return starfield;
	}

#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;

		Gizmos.DrawWireSphere(Vector3.zero, Radius);

		Gizmos.DrawWireSphere(Vector3.zero, Radius * Offset);
	}
#endif

	protected override void CalculateStars(out List<SgtStarfieldStar> stars, out bool pool)
	{
		stars = tempStars; tempStars.Clear();
		pool  = true;

		SgtHelper.BeginRandomSeed(Seed);
		{
			for (var i = 0; i < StarCount; i++)
			{
				var star      = SgtClassPool<SgtStarfieldStar>.Pop() ?? new SgtStarfieldStar(); stars.Add(star);
				var position  = Random.insideUnitSphere;
				var magnitude = Offset;

				if (Inverse == true)
				{
					magnitude += (1.0f - position.magnitude) * (1.0f - Offset);
				}
				else
				{
					magnitude += position.magnitude * (1.0f - Offset);
				}

				position.y *= Symmetry;

				star.Sprite      = GetRandomStarSprite();
				star.Color       = Color.white;
				star.Radius      = Random.Range(StarRadiusMin, StarRadiusMax);
				star.Angle       = Random.Range(0.0f, Mathf.PI * 2.0f);
				star.Position    = position.normalized * magnitude * Radius;
				star.PulseRange  = Random.value * StarPulseMax;
				star.PulseSpeed  = Random.value;
				star.PulseOffset = Random.value;
			}
		}
		SgtHelper.EndRandomSeed();
	}

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
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Elliptical Starfield", false, 10)]
	private static void CreateEllipticalStarfieldMenuItem()
	{
		var parent    = SgtHelper.GetSelectedParent();
		var starfield = CreateEllipticalStarfield(parent != null ? parent.gameObject.layer : 0, parent);

		SgtHelper.SelectAndPing(starfield);
	}
#endif
}

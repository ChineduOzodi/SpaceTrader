using UnityEngine;
using System.Collections.Generic;

// This component allows you to make star distributions that are box/cube shaped
[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Box Starfield")]
public class SgtBoxStarfield : SgtStarfield
{
	// The random seed used when generating the star positions
	[SgtSeed]
	public int Seed;

	// The size of the starfield box
	public Vector3 Extents = Vector3.one;

	// How far from the center of the box the stars will spawn from. If this is 1, then the stars will only spawn at the edges
	[SgtRange(0.0f, 1.0f)]
	public float Offset = 0.0f;

	// The amount of stars that will be generated in the starfield
	public int StarCount = 1000;

	// The minimum radius of stars in the starfield
	public float StarRadiusMin = 0.0f;

	// The maximum radius of stars in the starfield
	public float StarRadiusMax = 0.05f;

	// The maximum amount a star's size can pulse over time. A value of 1 means the star can potentially pulse between its maximum size, and 0
	[SgtRange(0.0f, 1.0f)]
	public float StarPulseMax = 1.0f;

	// The sprites used by the stars in the starfield. Each star is assigned a random star from here
	public List<Sprite> StarSprites = new List<Sprite>();

	// This allows you to create a box starfield GameObject under the specified parent
	public static SgtBoxStarfield CreateBoxStarfield(int layer = 0, Transform parent = null)
	{
		return CreateBoxStarfield(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtBoxStarfield CreateBoxStarfield(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Box Starfield", layer, parent, localPosition, localRotation, localScale);
		var starfield  = gameObject.AddComponent<SgtBoxStarfield>();

		return starfield;
	}

	protected override void CalculateStars(out List<SgtStarfieldStar> stars, out bool pool)
	{
		stars = tempStars; tempStars.Clear();
		pool  = true;

		SgtHelper.BeginRandomSeed(Seed);
		{
			for (var i = 0; i < StarCount; i++)
			{
				var star     = SgtClassPool<SgtStarfieldStar>.Pop() ?? new SgtStarfieldStar(); stars.Add(star);
				var x        = Random.Range(-0.5f, 0.5f);
				var y        = Random.Range(-0.5f, 0.5f);
				var z        = Random.Range(Offset * 0.5f, 0.5f);
				var position = default(Vector3);

				if (Random.value >= 0.5f)
				{
					z = -z;
				}

				switch (Random.Range(0, 3))
				{
					case 0: position = new Vector3(z, x, y); break;
					case 1: position = new Vector3(x, z, y); break;
					case 2: position = new Vector3(x, y, z); break;
				}

				star.Sprite      = GetRandomStarSprite();
				star.Color       = Color.white;
				star.Radius      = Random.Range(StarRadiusMin, StarRadiusMax);
				star.Angle       = Random.Range(0.0f, Mathf.PI * 2.0f);
				star.Position    = Vector3.Scale(position, Extents);
				star.PulseRange  = Random.value * StarPulseMax;
				star.PulseSpeed  = Random.value;
				star.PulseOffset = Random.value;
			}
		}
		SgtHelper.EndRandomSeed();
	}

	// Returns a random sprite from the StarSprites list
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
	// Show the component gizmos
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;

		Gizmos.DrawWireCube(Vector3.zero, Extents * Offset);
		Gizmos.DrawWireCube(Vector3.zero, Extents);
	}

	// Show the editor-only GameObject menu option to make a box starfield
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Box Starfield", false, 10)]
	private static void CreateBoxStarfieldMenuItem()
	{
		var parent    = SgtHelper.GetSelectedParent();
		var starfield = CreateBoxStarfield(parent != null ? parent.gameObject.layer : 0, parent);

		SgtHelper.SelectAndPing(starfield);
	}
#endif
}

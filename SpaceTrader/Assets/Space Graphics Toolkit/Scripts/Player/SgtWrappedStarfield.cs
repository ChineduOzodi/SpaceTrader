using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Wrapped Starfield")]
public class SgtWrappedStarfield : SgtStarfield
{
	[SgtSeed]
	public int Seed;

	public Vector3 Size = new Vector3(100.0f, 100.0f, 100.0f);

	public bool Wrap3D = true;

	public int StarCount = 1000;

	public float StarRadiusMin = 0.0f;

	public float StarRadiusMax = 1.0f;

	[SgtRange(0.0f, 1.0f)]
	public float StarPulseMax = 1.0f;

	public List<Sprite> StarSprites = new List<Sprite>();

	public static SgtWrappedStarfield CreateWrappedStarfield(int layer = 0, Transform parent = null)
	{
		return CreateWrappedStarfield(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtWrappedStarfield CreateWrappedStarfield(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Wrapped Starfield", layer, parent, localPosition, localRotation, localScale);
		var starfield  = gameObject.AddComponent<SgtWrappedStarfield>();

		return starfield;
	}

	// Shift all bounds on top of the observer, so it never exits the view frustum
	protected override void CameraPreCull(Camera camera)
	{
		// Make sure this is disabled, else the wrapping will never be seen
		FollowObservers = false;

		base.CameraPreCull(camera);

		for (var i = groups.Count - 1; i >= 0; i--)
		{
			var group = groups[i];

			if (group != null)
			{
				var models = group.Models;

				for (var j = models.Count - 1; j >= 0; j--)
				{
					var model = models[j];

					if (model != null)
					{
						var modelMesh = model.Mesh;

						if (modelMesh != null)
						{
							var center = model.transform.InverseTransformPoint(camera.transform.position);

							modelMesh.bounds = new Bounds(center, Size);
						}
					}
				}
			}
		}
	}

	protected override void UpdateGroupMaterial(SgtStarfieldGroup group)
	{
		base.UpdateGroupMaterial(group);

		if (Wrap3D == true)
		{
			keywords.Add("SGT_B");
		}
		else
		{
			keywords.Add("SGT_A");
		}

		group.Material.SetVector("_WrapSize", Size);
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
				var position = default(Vector3);

				position.x = Random.Range(-Size.x, Size.x);
				position.y = Random.Range(-Size.y, Size.y);
				position.z = Random.Range(-Size.z, Size.z);

				star.Sprite      = GetRandomStarSprite();
				star.Color       = Color.white;
				star.Radius      = Random.Range(StarRadiusMin, StarRadiusMax);
				star.Angle       = Random.Range(0.0f, Mathf.PI * 2.0f);
				star.Position    = position;
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
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Wrapped Starfield", false, 10)]
	private static void CreateWrappedStarfieldMenuItem()
	{
		var parent    = SgtHelper.GetSelectedParent();
		var starfield = CreateWrappedStarfield(parent != null ? parent.gameObject.layer : 0, parent);

		SgtHelper.SelectAndPing(starfield);
	}
#endif
}

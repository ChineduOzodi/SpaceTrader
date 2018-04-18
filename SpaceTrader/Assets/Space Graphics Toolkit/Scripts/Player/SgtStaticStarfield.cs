using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Static Starfield")]
public class SgtStaticStarfield : SgtStarfield
{
	public float Radius = 1.0f;

	[SgtSeed]
	public int Seed;

	[SgtRange(0.0f, 1.0f)]
	public float Symmetry = 1.0f;

	public int StarCount = 1000;

	public float StarRadiusMin = 0.0f;

	public float StarRadiusMax = 0.05f;

	public List<Sprite> StarSprites = new List<Sprite>();

	public static SgtStaticStarfield CreateStaticStarfield(int layer = 0, Transform parent = null)
	{
		return CreateStaticStarfield(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtStaticStarfield CreateStaticStarfield(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Static Starfield", layer, parent, localPosition, localRotation, localScale);
		var starfield  = gameObject.AddComponent<SgtStaticStarfield>();

		return starfield;
	}

#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;

		Gizmos.DrawWireSphere(Vector3.zero, Radius);
	}
#endif

	protected override void CameraPreCull(Camera camera)
	{
		FollowObservers = true;

		base.CameraPreCull(camera);
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
				var position = Random.insideUnitSphere;

				position.y *= Symmetry;

				star.Sprite   = GetRandomStarSprite();
				star.Color    = Color.white;
				star.Radius   = Random.Range(StarRadiusMin, StarRadiusMax);
				star.Angle    = Random.Range(0.0f, Mathf.PI * 2.0f);
				star.Position = position.normalized * Radius;
			}
		}
		SgtHelper.EndRandomSeed();
	}

	protected override void UpdateGroupMaterial(SgtStarfieldGroup group)
	{
		if (group.Material == null) group.Material = SgtHelper.CreateTempMaterial(SgtHelper.ShaderNamePrefix + "StaticStarfield");

		base.UpdateGroupMaterial(group);
	}

	protected override void RegenerateMeshes()
	{
		var stars = default(List<SgtStarfieldStar>);
		var pool  = default(bool);

		CalculateStars(out stars, out pool);

		if (stars != null)
		{
			// Sort stars into groups
			for (var i = stars.Count - 1; i >= 0; i--)
			{
				var star = stars[i];

				if (star != null)
				{
					var group = GetGroup(star.Sprite);

					group.Stars.Add(star);
				}
			}

			// Pool stars?
			if (pool == true)
			{
				SgtClassPool<SgtStarfieldStar>.Add(stars);
			}

			// Build groups
			for (var i = groups.Count - 1; i >= 0; i--)
			{
				var group = groups[i];

				if (group.Stars.Count > 0)
				{
					var groupStars = group.Stars;
					var minMaxSet  = false;
					var min        = default(Vector3);
					var max        = default(Vector3);

					SgtProceduralMesh.Clear();

					for (var j = groupStars.Count - 1; j >= 0; j--)
					{
						var star     = groupStars[j];
						var position = star.Position;
						var radius   = star.Radius;
						var uv       = SgtHelper.CalculateSpriteUV(star.Sprite);
						var rotation = Quaternion.FromToRotation(Vector3.back, position.normalized) * Quaternion.Euler(0.0f, 0.0f, star.Angle);
						var up       = rotation * Vector3.up    * radius;
						var right    = rotation * Vector3.right * radius;

						ExpandBounds(ref minMaxSet, ref min, ref max, position, radius);

						SgtProceduralMesh.PushPosition(position - up - right);
						SgtProceduralMesh.PushPosition(position - up + right);
						SgtProceduralMesh.PushPosition(position + up - right);
						SgtProceduralMesh.PushPosition(position + up + right);

						SgtProceduralMesh.PushColor(star.Color, 4);

						SgtProceduralMesh.PushCoord1(uv.x, uv.y);
						SgtProceduralMesh.PushCoord1(uv.z, uv.y);
						SgtProceduralMesh.PushCoord1(uv.x, uv.w);
						SgtProceduralMesh.PushCoord1(uv.z, uv.w);
					}

					var bounds = SgtHelper.NewBoundsFromMinMax(min, max);

					SgtProceduralMesh.SplitQuads(HideFlags.DontSave);

					var meshCount = SgtProceduralMesh.Count;

					// Copy meshes
					for (var j = 0; j < meshCount; j++)
					{
						var mesh  = SgtProceduralMesh.Pop();
						var model = group.Models.Count > j ? group.Models[j] : SgtStarfieldModel.Create(group);

						mesh.bounds = bounds;

						model.Mesh  = mesh;
					}
				}
			}
		}
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
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Static Starfield", false, 10)]
	private static void CreateStaticStarfieldMenuItem()
	{
		var parent    = SgtHelper.GetSelectedParent();
		var starfield = CreateStaticStarfield(parent != null ? parent.gameObject.layer : 0, parent);

		SgtHelper.SelectAndPing(starfield);
	}
#endif
}

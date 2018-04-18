using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Terrain Displacer")]
public class SgtTerrainDisplacer : SgtTerrainModifier
{
	[Tooltip("The heightmap texture using a cylindrical (equirectangular) projection")]
	public Texture2D Heightmap;

	[Tooltip("The strength of the displacement")]
	[SgtRange(0.0f, 1.0f)]
	public float Strength = 1.0f;

	protected override void OnEnable()
	{
		base.OnEnable();

		terrain.OnCalculateHeight += OnCalculateHeight;
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		terrain.OnCalculateHeight -= OnCalculateHeight;
	}

#if UNITY_EDITOR
	protected override void OnValidate()
	{
		SgtHelper.MakeTextureReadable(Heightmap);

		base.OnValidate();
	}
#endif

	private void OnCalculateHeight(Vector3 localPosition, ref float height)
	{
		if (Heightmap != null)
		{
			var uv    = SgtHelper.CartesianToPolarUV(localPosition);
			var color = SampleBilinear(uv);

			height += (color.a - 0.5f) * Strength;
		}
	}

	private Color SampleBilinear(Vector2 uv)
	{
		return Heightmap.GetPixelBilinear(uv.x, uv.y);
	}
}

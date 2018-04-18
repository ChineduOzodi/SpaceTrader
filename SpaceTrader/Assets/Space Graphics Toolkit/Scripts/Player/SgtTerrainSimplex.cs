using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Terrain Simplex")]
public class SgtTerrainSimplex : SgtTerrainModifier
{
	[Tooltip("The scale of the displacement")]
	public float Scale = 10;

	[Tooltip("The strength of the displacement")]
	[SgtRange(0.0f, 0.5f)]
	public float Strength = 0.5f;

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

	private void OnCalculateHeight(Vector3 localPosition, ref float height)
	{
		localPosition = localPosition.normalized * Scale;

		height += SgtSimplex.Generate(localPosition.x, localPosition.y, localPosition.z) * Strength;
	}
}

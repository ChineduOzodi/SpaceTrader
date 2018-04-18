using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Terrain Cylindrical")]
public class SgtTerrainCylindrical : SgtTerrainModifier
{
	protected override void OnEnable()
	{
		base.OnEnable();

		terrain.OnCalculateCoord1 += OnCalculateCoord1;
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		terrain.OnCalculateCoord1 -= OnCalculateCoord1;
	}

	private void OnCalculateCoord1(Vector3 localPosition, Vector3 localCenter, ref Vector2 coord)
	{
		coord.x = Mathf.Atan2(localPosition.x, localPosition.z);
		coord.y = Mathf.Asin(localPosition.y / localPosition.magnitude);

		coord.x = 0.5f - coord.x / (Mathf.PI * 2.0f);
		coord.y = 0.5f + coord.y / Mathf.PI;

		coord = SgtHelper.CartesianToPolarUV(localPosition);

		if (coord.x < 0.001f)
		{
			if (localCenter.x < 0.0f)
			{
				coord.x = 1.0f;
			}
		}
		else if (coord.x > 0.999f)
		{
			if (localCenter.x > 0.0f)
			{
				coord.x = 0.0f;
			}
		}
	}
}

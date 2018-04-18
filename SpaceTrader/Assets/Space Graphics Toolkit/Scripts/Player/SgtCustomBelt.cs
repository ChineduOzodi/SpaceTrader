using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Custom Belt")]
public class SgtCustomBelt : SgtBelt
{
	public List<SgtBeltAsteroid> Asteroids = new List<SgtBeltAsteroid>();

	public static SgtCustomBelt CreateCustomBelt(int layer = 0, Transform parent = null)
	{
		return CreateCustomBelt(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtCustomBelt CreateCustomBelt(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Custom Belt", layer, parent, localPosition, localRotation, localScale);
		var belt       = gameObject.AddComponent<SgtCustomBelt>();

		return belt;
	}

	protected override void CalculateAsteroids(out List<SgtBeltAsteroid> asteroids, out bool pool)
	{
		asteroids = Asteroids;
		pool      = false;
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Custom Belt", false, 10)]
	public static void CreateCustomBeltMenuItem()
	{
		var parent = SgtHelper.GetSelectedParent();
		var belt   = CreateCustomBelt(parent != null ? parent.gameObject.layer : 0, parent);

		SgtHelper.SelectAndPing(belt);
	}
#endif
}

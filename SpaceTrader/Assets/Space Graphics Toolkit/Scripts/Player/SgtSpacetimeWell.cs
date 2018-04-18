using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Spacetime Well")]
public class SgtSpacetimeWell : MonoBehaviour
{
	public static List<SgtSpacetimeWell> AllWells = new List<SgtSpacetimeWell>();

	public float Radius = 1.0f;

	public float Age;

	public float Strength = 1.0f;

	public bool Oscillate;

	public float Amplitude = 0.2f;

	public float Frequency = 1.0f;

	public float Offset = 0.0f;

	public static SgtSpacetimeWell Create(SgtSpacetime spacetime)
	{
		var gameObject = SgtHelper.CreateGameObject("Well", spacetime.gameObject.layer, spacetime.transform);
		var well       = gameObject.AddComponent<SgtSpacetimeWell>();

		spacetime.Wells.Add(well);

		return well;
	}

	protected virtual void OnEnable()
	{
		AllWells.Add(this);
	}

	protected virtual void OnDisable()
	{
		AllWells.Remove(this);
	}

	protected virtual void Update()
	{
#if UNITY_EDITOR
		if (Application.isPlaying == false)
		{
			return;
		}
#endif
		Age += Time.deltaTime;

		if (Oscillate == true)
		{
			Strength = Offset + Mathf.Sin(Age * Frequency) * Amplitude;
		}
	}

#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, Radius);
	}
#endif
}

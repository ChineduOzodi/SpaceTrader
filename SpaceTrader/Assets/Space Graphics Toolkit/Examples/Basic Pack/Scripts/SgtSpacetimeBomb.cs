using UnityEngine;
using System.Collections.Generic;

// This component handles adding/removing itself from a spacetime's well list
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Spacetime Bomb")]
public class SgtSpacetimeBomb : MonoBehaviour
{
	public SgtSpacetime Spacetime;
	
	public SgtSpacetimeWell Well;
	
	public float Radius;
	
	public float ShrinkSpeed;
	
	protected virtual void Update()
	{
		if (Well != null)
		{
			Well.Radius = Radius;
		}
		
		Radius -= ShrinkSpeed * Time.deltaTime;
		
		if (Radius <= 0.0f)
		{
			SgtHelper.Destroy(gameObject);
		}
	}
	
	protected virtual void Start()
	{
		if (Well != null && Spacetime != null && Spacetime.Wells.Contains(Well) == false)
		{
			Spacetime.Wells.Add(Well);
		}
	}
	
	protected virtual void OnDestroy()
	{
		if (Well != null && Spacetime != null)
		{
			Spacetime.Wells.Remove(Well);
		}
	}
}
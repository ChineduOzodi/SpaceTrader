using UnityEngine;
using System.Collections.Generic;

// This component allows gravity receivers to get attracted to it
[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Gravity Source")]
public class SgtGravitySource : MonoBehaviour
{
	public static List<SgtGravitySource> AllGravitySources = new List<SgtGravitySource>();

	public float Mass = 100.0f;

	private Rigidbody thisRigidbody;

	protected virtual void OnEnable()
	{
		AllGravitySources.Add(this);
	}

	protected virtual void OnDisable()
	{
		AllGravitySources.Remove(this);
	}

	protected virtual void Update()
	{
		if (thisRigidbody == null) thisRigidbody = GetComponent<Rigidbody>();

		if (thisRigidbody != null)
		{
			Mass = thisRigidbody.mass;
		}
	}
}

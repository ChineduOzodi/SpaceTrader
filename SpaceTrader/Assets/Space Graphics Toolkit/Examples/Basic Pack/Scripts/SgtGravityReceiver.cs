using UnityEngine;

// This component causes the attached rigidbody to get pulled toward all gravity sources
[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Gravity Receiver")]
public class SgtGravityReceiver : MonoBehaviour
{
	private Rigidbody thisRigidbody;

	protected virtual void FixedUpdate()
	{
		if (thisRigidbody == null) thisRigidbody = GetComponent<Rigidbody>();

		for (var i = SgtGravitySource.AllGravitySources.Count - 1; i >= 0; i--)
		{
			var gravitySource = SgtGravitySource.AllGravitySources[i];

			if (gravitySource.transform != transform)
			{
				var totalMass  = thisRigidbody.mass * gravitySource.Mass;
				var vector     = gravitySource.transform.position - transform.position;
				var distanceSq = vector.sqrMagnitude;

				if (distanceSq > 0.0f)
				{
					var force = totalMass / distanceSq;

					thisRigidbody.AddForce(vector.normalized * force * Time.fixedDeltaTime, ForceMode.Acceleration);
				}
			}
		}
	}
}

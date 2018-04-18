using UnityEngine;

// This component handles basic orbiting around the parent GameObject
[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Simple Orbit")]
public class SgtSimpleOrbit : MonoBehaviour
{
	public float Radius = 1.0f;
	
	[Range(0.0f, 1.0f)]
	public float Oblateness;
	
	public Vector3 Center;
	
	public float Angle;
	
	public float DegreesPerSecond = 10.0f;
	
	protected virtual void Update()
	{
		Angle += DegreesPerSecond * Time.deltaTime;
		
		var r1 = Radius;
		var r2 = Radius * (1.0f - Oblateness);
		var lp = Center;
		
		lp.x += Mathf.Sin(Angle * Mathf.Deg2Rad) * r1;
		lp.z += Mathf.Cos(Angle * Mathf.Deg2Rad) * r2;
		
		SgtHelper.SetLocalPosition(transform, lp);
	}
	
#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		if (SgtHelper.Enabled(this) == true)
		{
			if (transform.parent != null)
			{
				Gizmos.matrix = transform.parent.localToWorldMatrix;
			}
			
			var r1 = Radius;
			var r2 = Radius * (1.0f - Oblateness);
			
			SgtHelper.DrawCircle(Center, Vector3.right * r1, Vector3.forward * r2);
			
			Gizmos.DrawLine(Vector3.zero, transform.localPosition);
		}
	}
#endif
}
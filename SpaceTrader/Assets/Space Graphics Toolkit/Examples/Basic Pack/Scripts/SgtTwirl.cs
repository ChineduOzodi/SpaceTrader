using UnityEngine;

// This object allows you to rotate an object around a randomly changing axis
public class SgtTwirl : MonoBehaviour
{
	public float DegreesPerSecond = 1.0f;
	
	public float Dampening = 2.0f;
	
	public Vector3 Axis = Vector3.up;
	
	private Vector3 targetAxis;
	
	protected virtual void Update()
	{
		if (targetAxis == Vector3.zero || Vector3.Distance(targetAxis, Axis) < 0.1f)
		{
			targetAxis.x = Random.Range(-1.0f, 1.0f);
			targetAxis.y = Random.Range(-1.0f, 1.0f);
			targetAxis.z = Random.Range(-1.0f, 1.0f);
			targetAxis = targetAxis.normalized;
		}
		
		Axis = SgtHelper.Dampen3(Axis, targetAxis, Dampening, Time.deltaTime, 0.1f);
		
		transform.Rotate(Axis.normalized, DegreesPerSecond * Time.deltaTime);
	}
}
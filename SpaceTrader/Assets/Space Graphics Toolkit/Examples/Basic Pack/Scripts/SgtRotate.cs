using UnityEngine;

// This component rotates the GameObject every frame
public class SgtRotate : MonoBehaviour
{
	public Vector3 DegreesPerSecond = new Vector3(0.0f, 100.0f, 0.0f);
	
	protected virtual void Update()
	{
		transform.Rotate(DegreesPerSecond * Time.deltaTime);
	}
}
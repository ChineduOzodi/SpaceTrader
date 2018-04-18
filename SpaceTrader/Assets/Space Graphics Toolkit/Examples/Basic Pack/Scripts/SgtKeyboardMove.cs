using UnityEngine;

// This component handles keyboard movement when attached to the camera
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Keyboard Move")]
public class SgtKeyboardMove : MonoBehaviour
{
	public KeyCode Require = KeyCode.None;
	
	public float Sensitivity = 1.0f;
	
	public float Dampening = 5.0f;
	
	private Vector3 targetPosition;
	
	protected virtual void Start()
	{
		targetPosition = transform.position;
	}
	
	protected virtual void Update()
	{
		if (Require == KeyCode.None || Input.GetKey(Require) == true)
		{
			targetPosition += transform.forward * Input.GetAxisRaw("Vertical") * Sensitivity * Time.deltaTime;
			
			targetPosition += transform.right * Input.GetAxisRaw("Horizontal") * Sensitivity * Time.deltaTime;
		}
		
		var currentPosition = SgtHelper.Dampen3(transform.position, targetPosition, Dampening, Time.deltaTime, 0.1f);
		
		SgtHelper.SetPosition(transform, currentPosition);
	}
}
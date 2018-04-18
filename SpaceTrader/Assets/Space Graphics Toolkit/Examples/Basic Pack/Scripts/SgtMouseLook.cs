using UnityEngine;

// This component handles mouselook when attached to the camera
[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Mouse Look")]
public class SgtMouseLook : MonoBehaviour
{
	public KeyCode Require = KeyCode.Mouse0;

	public float Sensitivity = 2.0f;

	public float TargetPitch;

	public float TargetYaw;

	public float Dampening = 10.0f;

	private float currentPitch;

	private float currentYaw;

	protected virtual void Awake()
	{
		currentPitch = TargetPitch;
		currentYaw   = TargetYaw;
	}

	protected virtual void Update()
	{
		TargetPitch = Mathf.Clamp(TargetPitch, -89.9f, 89.9f);

		if (Require == KeyCode.None || Input.GetKey(Require) == true)
		{
			TargetPitch -= Input.GetAxisRaw("Mouse Y") * Sensitivity;

			TargetYaw += Input.GetAxisRaw("Mouse X") * Sensitivity;
		}

		currentPitch = SgtHelper.Dampen(currentPitch, TargetPitch, Dampening, Time.deltaTime, 0.1f);
		currentYaw   = SgtHelper.Dampen(currentYaw  , TargetYaw  , Dampening, Time.deltaTime, 0.1f);

		var rotation = Quaternion.Euler(currentPitch, currentYaw, 0.0f);

		SgtHelper.SetLocalRotation(transform, rotation);
	}
}

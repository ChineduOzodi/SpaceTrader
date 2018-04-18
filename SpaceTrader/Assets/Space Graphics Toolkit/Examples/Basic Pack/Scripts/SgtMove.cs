using UnityEngine;

// This component will move the GameObject every frame
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Move")]
public class SgtMove : MonoBehaviour
{
	public Vector3 Speed = Vector3.forward;
	
	protected virtual void Update()
	{
		transform.Translate(Speed * Time.deltaTime);
	}
}
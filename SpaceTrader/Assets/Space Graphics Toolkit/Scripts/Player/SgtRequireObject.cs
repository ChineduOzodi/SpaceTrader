using UnityEngine;

public class SgtRequireObject : MonoBehaviour
{
	public Object Target;
	
	protected virtual void Update()
	{
		if (Target == null)
		{
			//SgtHelper.Destroy(gameObject);
		}
	}
}
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Observer")]
public class SgtObserver : MonoBehaviour
{
	public static List<SgtObserver> AllObservers = new List<SgtObserver>();
	
	public static System.Action<SgtObserver> OnObserverPreCull;
	
	public static System.Action<SgtObserver> OnObserverPreRender;
	
	public static System.Action<SgtObserver> OnObserverPostRender;
	
	public Camera Camera;
	
	public float RollAngle;
	
	public Quaternion RollQuataternion = Quaternion.identity;
	
	public Matrix4x4 RollMatrix = Matrix4x4.identity;
	
	public Vector3 DeltaPosition;
	
	public Vector3 Velocity;
	
	public Quaternion OldRotation = Quaternion.identity;
	
	public Vector3 OldPosition;
	
	public static SgtObserver Find(Camera camera)
	{
		return AllObservers.Find(o => o.Camera == camera);
	}
	
	protected virtual void OnPreCull()
	{
		if (Camera == null) Camera = GetComponent<Camera>();
		
		if (OnObserverPreCull != null) OnObserverPreCull(this);
	}
	
	protected virtual void OnPreRender()
	{
		if (Camera == null) Camera = GetComponent<Camera>();
		
		if (OnObserverPreRender != null) OnObserverPreRender(this);
		
#if UNITY_EDITOR
		for (var i = SgtRing.AllRings.Count - 1; i >= 0; i--)
		{
			SgtRing.AllRings[i].UpdateState();
		}
#endif
	}
	
	protected virtual void OnPostRender()
	{
		if (Camera == null) Camera = GetComponent<Camera>();
		
		if (OnObserverPostRender != null) OnObserverPostRender(this);
	}
	
	protected virtual void LateUpdate()
	{
		var newRotation   = transform.rotation;
		var newPosition   = transform.position;
		var deltaRotation = Quaternion.Inverse(OldRotation) * newRotation;
		var deltaPosition = OldPosition - newPosition;
		
		OldRotation = newRotation;
		OldPosition = newPosition;
		
		RollAngle        = (RollAngle - deltaRotation.eulerAngles.z) % 360.0f;
		RollQuataternion = Quaternion.Euler(new Vector3(0.0f, 0.0f, RollAngle));
		RollMatrix       = SgtHelper.Rotation(RollQuataternion);
		DeltaPosition    = deltaPosition;
		Velocity         = SgtHelper.Reciprocal(Time.deltaTime) * deltaPosition;
	}
	
	protected virtual void OnEnable()
	{
		AllObservers.Add(this);
		
		OldPosition = transform.position;
	}
	
	protected virtual void OnDisable()
	{
		AllObservers.Remove(this);
	}
}
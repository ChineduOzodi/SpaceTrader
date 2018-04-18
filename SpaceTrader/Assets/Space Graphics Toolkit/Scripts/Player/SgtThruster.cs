using UnityEngine;
using System.Collections.Generic;

// This component allows you to create simple thrusters that can apply forces to Rigidbodies based on their position. You can also use sprites to change the graphics
[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Thruster")]
public class SgtThruster : MonoBehaviour
{
	// Static list of all currently enabled thrusters in the scene
	public static List<SgtThruster> AllThrusters = new List<SgtThruster>();

	[Tooltip("How active is this thruster? 0 for off, 1 for max power, -1 for max reverse, etc")]
	public float Throttle;

	[Tooltip("How quickly the scale will move to the target value when the throttle value is changed")]
	public float Dampening = 10.0f;

	[Tooltip("This sets how much the flame & flare scale will randomly change")]
	[SgtRange(0.0f, 1.0f)]
	public float Flicker = 0.1f;

	[Tooltip("How many seconds the thruster has been running for")]
	public float Age;

	[Tooltip("How fast the thruster ages")]
	public float TimeScale = 20.0f;

	[Tooltip("The rigidbody you want to apply the thruster forces to")]
	public Rigidbody Rigidbody;

	[Tooltip("The type of force we want to apply to the Rigidbody")]
	public SgtForceType ForceType = SgtForceType.AddForceAtPosition;

	[Tooltip("The force mode used when ading force to the Rigidbody")]
	public ForceMode ForceMode = ForceMode.Acceleration;

	[Tooltip("The maximum amount of force applied to the rigidbody (when the throttle is -1 or 1)")]
	public float ForceMagnitude = 1.0f;

	[Tooltip("This allows you to set the sprite used by the thruster flame")]
	public Sprite FlameSprite;

	[Tooltip("The scale of the thruster flame when the throttle is at 1")]
	public Vector2 FlameScale = Vector2.one;

	[Tooltip("This allows you to set the sprite used by the thruster flare")]
	public Sprite FlareSprite;

	[Tooltip("The scale of the thruster flame when the throttle is at 1")]
	public Vector2 FlareScale = Vector2.one;

	[Tooltip("This allows you to set which layers the flare will get occluded by")]
	public LayerMask FlareMask = -5;

	[SerializeField]
	private SgtThrusterFlame flame;

	[SerializeField]
	private SgtThrusterFlare flare;

	// Create a child GameObject with a thruster attached
	public static SgtThruster CreateThruster(int layer = 0, Transform parent = null)
	{
		return CreateThruster(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtThruster CreateThruster(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Thruster", layer, parent, localPosition, localRotation, localScale);
		var thruster   = gameObject.AddComponent<SgtThruster>();

		return thruster;
	}

	protected virtual void OnEnable()
	{
		AllThrusters.Add(this);

		if (flame != null) flame.gameObject.SetActive(true);
		if (flare != null) flare.gameObject.SetActive(true);
	}

	protected virtual void OnDisable()
	{
		AllThrusters.Remove(this);

		if (flame != null) flame.gameObject.SetActive(false);
		if (flare != null) flare.gameObject.SetActive(false);
	}

	protected virtual void OnDestroy()
	{
		SgtThrusterFlame.MarkForDestruction(flame);
		SgtThrusterFlare.MarkForDestruction(flare);
	}

	protected virtual void FixedUpdate()
	{
#if UNITY_EDITOR
		if (Application.isPlaying == false)
		{
			return;
		}
#endif
		// Apply thruster force to rigidbody
		if (Rigidbody != null)
		{
			var force = transform.forward * ForceMagnitude * Throttle * Time.fixedDeltaTime;

			switch (ForceType)
			{
				case SgtForceType.AddForce: Rigidbody.AddForce(force, ForceMode); break;
				case SgtForceType.AddForceAtPosition: Rigidbody.AddForceAtPosition(force, transform.position, ForceMode); break;
			}
		}
	}

	protected virtual void LateUpdate()
	{
		if (flame == null) flame = SgtThrusterFlame.Create(this);
		if (flare == null) flare = SgtThrusterFlare.Create(this);

		Age += Time.deltaTime * TimeScale;

		var flameFlicker = Mathf.PerlinNoise(Age, Dampening) * Flicker;
		var flareFlicker = Mathf.PerlinNoise(Age, Dampening) * Flicker;

		flame.UpdateFlame(FlameSprite, FlameScale * Throttle, flameFlicker, Dampening);
		flare.UpdateFlare(FlareSprite, FlareScale * Throttle, flareFlicker, Dampening);
	}

#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		var a = transform.position;
		var b = transform.position + transform.forward * ForceMagnitude;

		Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
		Gizmos.DrawLine(a, b);

		Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		Gizmos.DrawLine(a, a + (b - a) * Throttle);
	}
#endif

#if UNITY_EDITOR
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Thruster", false, 10)]
	public static void CreateThrusterMenuItem()
	{
		var parent   = SgtHelper.GetSelectedParent();
		var thruster = CreateThruster(parent != null ? parent.gameObject.layer : 0, parent);

		SgtHelper.SelectAndPing(thruster);
	}
#endif
}

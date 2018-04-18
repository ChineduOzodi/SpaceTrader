using UnityEngine;

// This component is created and managed by the SgtTHruster component
[ExecuteInEditMode]
[AddComponentMenu("")]
public class SgtThrusterFlame : MonoBehaviour
{
	public SgtThruster Thruster;

	public Vector3 CurrentScale;

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	private static Material flameMaterial;

	[System.NonSerialized]
	private bool tempSet;

	[System.NonSerialized]
	private Quaternion tempRotation;

	// This returns the default shared flame material
	public static Material FlameMaterial
	{
		get
		{
			if (flameMaterial == null)
			{
				flameMaterial = SgtHelper.CreateMaterial(SgtHelper.ShaderNamePrefix + "ThrusterFlame", false);
				flameMaterial.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
			}

			return flameMaterial;
		}
	}

	public static SgtThrusterFlame Create(SgtThruster thruster)
	{
		var flame = SgtComponentPool<SgtThrusterFlame>.Pop("Flame", thruster.gameObject.layer, thruster.transform);

		flame.Thruster = thruster;

		return flame;
	}

	public static void Pool(SgtThrusterFlame flame)
	{
		if (flame != null)
		{
			flame.Thruster = null;

			SgtComponentPool<SgtThrusterFlame>.Add(flame);
		}
	}

	public static void MarkForDestruction(SgtThrusterFlame flame)
	{
		if (flame != null)
		{
			flame.Thruster = null;

			flame.gameObject.SetActive(true);
		}
	}

	public void UpdateFlame(Sprite sprite, Vector3 targetScale, float flicker, float dampening)
	{
		// Get or add SpriteRenderer?
		if (spriteRenderer == null)
		{
			spriteRenderer = SgtHelper.GetOrAddComponent<SpriteRenderer>(gameObject);

			spriteRenderer.sharedMaterial = FlameMaterial;
		}

		// Assign the default material?
		if (spriteRenderer.sharedMaterial == null)
		{
			SgtHelper.BeginStealthSet(spriteRenderer);
			{
				spriteRenderer.sharedMaterial = FlameMaterial;
			}
			SgtHelper.EndStealthSet();
		}

		// Assign the current sprite?
		if (spriteRenderer.sprite != sprite)
		{
			spriteRenderer.sprite = sprite;
		}

		// Transition scale
		CurrentScale = SgtHelper.Dampen3(CurrentScale, targetScale, dampening, Time.deltaTime, 0.1f);

		transform.localScale = CurrentScale * (1.0f - flicker);
	}

	protected virtual void OnEnable()
	{
		Camera.onPreCull    += CameraPreCull;
		Camera.onPostRender += CameraPostRender;
	}

	protected virtual void OnDisable()
	{
		Camera.onPreCull    -= CameraPreCull;
		Camera.onPostRender -= CameraPostRender;
	}

	protected virtual void Update()
	{
		if (Thruster == null)
		{
			Pool(this);
		}
	}

	private void CameraPreCull(Camera camera)
	{
		if (Thruster != null)
		{
			var thrusterTransform = Thruster.transform;
			var position          = thrusterTransform.position;
			var direction         = thrusterTransform.forward;
			var cross             = Vector3.Cross(direction, position - camera.transform.position);
			var rotation          = Quaternion.LookRotation(cross, direction) * Quaternion.Euler(0.0f, 90.0f, 90.0f);

			// Rotate flame to camera
			if (tempSet == false)
			{
				tempSet      = true;
				tempRotation = transform.rotation;
			}

			transform.rotation = rotation;
		}
	}

	private void CameraPostRender(Camera camera)
	{
		if (Thruster != null && tempSet == false)
		{
			tempSet = true;

			transform.rotation = tempRotation;
		}
	}
}

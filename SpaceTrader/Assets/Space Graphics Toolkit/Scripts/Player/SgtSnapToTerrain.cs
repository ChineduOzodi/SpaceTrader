using UnityEngine;

// This component allows you to snap a GameObject to the surface of an SgtTerrain
[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Snap To Terrain")]
public class SgtSnapToTerrain : MonoBehaviour
{
	[Tooltip("The terrain we want this GameObject to snap to")]
	public SgtTerrain Terrain;

	[Tooltip("Enable this if you want the position to be snapped")]
	public bool SnapPosition = true;

	[Tooltip("This allows you to set how far from the surface this GameObject will be snapped")]
	public float SnapOffset;

	[Tooltip("How fast the current GameObject moves to the target position (0 = instant)")]
	public float SnapMoveDampening;

	[Tooltip("Enable this if you want the rotation to be snapped")]
	public bool SnapRotation = true;

	[Tooltip("This allows you to set how far apart the right/left height samples will be. Increasing this can make the rotations smoother")]
	public float SnapRightDistance = 0.1f;

	[Tooltip("This allows you to set how far apart the forward/back height samples will be. Increasing this can make the rotations smoother")]
	public float SnapForwardDistance = 0.1f;

	[Tooltip("How fast the current GameObject rotates to the target position (0 = instant)")]
	public float SnapTurnDampening;

	// This static method will move the transform down to the surface of the terrain
	public static void SnapTransformPosition(SgtTerrain terrain, Transform transform, float offset = 0.0f, float dampening = 0.0f)
	{
		if (terrain != null && transform != null)
		{
			var oldPosition = transform.position;
			var newPosition = terrain.GetSurfacePositionWorld(oldPosition, offset);

			if (oldPosition != newPosition)
			{
				if (dampening > 0.0f)
				{
					transform.position = SgtHelper.Dampen3(transform.position, newPosition, dampening, Time.deltaTime);
				}
				else
				{
					transform.position = newPosition;
				}
			}
		}
	}

	// This static method will rotate the transform to the surface of the terrain below
	public static void SnapTransformRotation(SgtTerrain terrain, Transform transform, float rightDistance = 1.0f, float forwardDistance = 1.0f, float dampening = 0.0f)
	{
		if (terrain != null && transform != null)
		{
			var newNormal = default(Vector3);

			// Rotate to surface normal?
			if (rightDistance != 0.0f && forwardDistance != 0.0f)
			{
				var worldRight   = transform.right   * rightDistance;
				var worldForward = transform.forward * forwardDistance;

				newNormal = terrain.GetSurfaceNormalWorld(transform.position, worldRight, worldForward);
			}
			// Rotate to planet center?
			else
			{
				newNormal = terrain.GetSurfaceNormalWorld(transform.position);
			}

			var oldRotation = transform.rotation;
			var newRotation = Quaternion.FromToRotation(transform.up, newNormal) * oldRotation;

			//if (oldRotation != newRotation)
			{
				if (dampening > 0.0f)
				{
					transform.rotation = SgtHelper.Dampen(transform.rotation, newRotation, dampening, Time.deltaTime);
				}
				else
				{
					transform.rotation = newRotation;
				}
			}
		}
	}

	public void UpdateSnap()
	{
		// Snap the position?
		if (SnapPosition == true)
		{
			SnapTransformPosition(Terrain, transform, SnapOffset, SnapMoveDampening);
		}

		// Snap the rotation?
		if (SnapRotation == true)
		{
			SnapTransformRotation(Terrain, transform, SnapRightDistance, SnapForwardDistance, SnapTurnDampening);
		}
	}

	protected virtual void Update()
	{
		UpdateSnap();
	}
}

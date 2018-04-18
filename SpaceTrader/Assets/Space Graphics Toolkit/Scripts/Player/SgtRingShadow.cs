using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Ring Shadow")]
public class SgtRingShadow : SgtShadow
{
	public Texture Texture;
	
	public SgtRing Ring;
	
	public float InnerRadius = 1.0f;
	
	public float OuterRadius = 2.0f;
	
	public override Texture GetTexture()
	{
		return Texture;
	}
	
	public override bool CalculateShadow()
	{
		if (base.CalculateShadow() == true)
		{
			if (Texture != null)
			{
				if (SgtHelper.Enabled(Ring) == true)
				{
					InnerRadius = Ring.InnerRadius;
					OuterRadius = Ring.OuterRadius;
				}
				
				var direction = default(Vector3);
				var position  = default(Vector3);
				var color     = default(Color);
				
				SgtHelper.CalculateLight(Light, transform.position, null, null, ref position, ref direction, ref color);
				
				var rotation = Quaternion.FromToRotation(direction, Vector3.back);
				var squash   = Vector3.Dot(direction, transform.up); // Find how squashed the ellipse is based on light direction
				var width    = transform.lossyScale.x * OuterRadius;
				var length   = transform.lossyScale.z * OuterRadius;
				var axis     = rotation * transform.up; // Find the transformed up axis
				var spin     = Quaternion.LookRotation(Vector3.forward, new Vector2(-axis.x, axis.y)); // Orient the shadow ellipse
				var scale    = SgtHelper.Reciprocal3(new Vector3(width, length * Mathf.Abs(squash), 1.0f));
				var skew     = Mathf.Tan(SgtHelper.Acos(-squash));
				
				var shadowT = SgtHelper.Translation(-transform.position);
				var shadowR = SgtHelper.Rotation(spin * rotation); // Spin the shadow so lines up with its tilt
				var shadowS = SgtHelper.Scaling(scale); // Scale the ring into an oval
				var shadowK = SgtHelper.ShearingZ(new Vector2(0.0f, skew)); // Skew the shadow so it aligns with the ring plane
				
				Matrix = shadowS * shadowK * shadowR * shadowT;
				Ratio  = SgtHelper.Divide(OuterRadius, OuterRadius - InnerRadius);
				
				return true;
			}
		}
		
		return false;
	}
	
#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		if (SgtHelper.Enabled(this) == true)
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			
			SgtHelper.DrawCircle(Vector3.zero, Vector3.right * InnerRadius, Vector3.forward * InnerRadius);
			SgtHelper.DrawCircle(Vector3.zero, Vector3.right * OuterRadius, Vector3.forward * OuterRadius);
			
			if (CalculateShadow() == true)
			{
				Gizmos.matrix = Matrix.inverse;
				
				Gizmos.DrawWireCube(new Vector3(0,0,5), new Vector3(2,2,10));
			}
		}
	}
#endif
}
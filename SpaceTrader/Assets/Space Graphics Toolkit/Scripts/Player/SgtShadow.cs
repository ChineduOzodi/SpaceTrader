using UnityEngine;

public abstract class SgtShadow : MonoBehaviour
{
	public Light Light;
	
	public Matrix4x4 Matrix;
	
	public float Ratio;
	
	public abstract Texture GetTexture();
	
	// Show enable/disable checkbox
	protected virtual void Start()
	{
	}
	
	public virtual bool CalculateShadow()
	{
		if (SgtHelper.Enabled(Light) == true && Light.intensity > 0.0f)
		{
			return true;
		}
		
		return false;
	}
}
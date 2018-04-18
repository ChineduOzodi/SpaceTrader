using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Sphere Shadow")]
public class SgtSphereShadow : SgtShadow
{
	public float InnerRadius = 1.0f;
	
	public float OuterRadius = 2.0f;
	
	public Gradient PenumbraBrightness = new Gradient();
	
	public Gradient PenumbraColor = new Gradient();
	
	private Texture2D penumbraLut;
	
	private bool lutDirty = true;
	
	[SerializeField]
	private bool awakeCalled;
	
	private static GradientColorKey[] defaultLightingBrightness = new GradientColorKey[] { new GradientColorKey(Color.black, 0.0f), new GradientColorKey(Color.white, 1.0f) };
	
	private static GradientColorKey[] defaultLightingColor = new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.white, 1.0f) };
	
	private static Vector3[] vectors = new Vector3[3];
	
	private static float[] magnitudes = new float[3];
	
	public override Texture GetTexture()
	{
		UpdateDirty();
		
		return penumbraLut;
	}
	
	// TODO: Make this work correctly
	public override bool CalculateShadow()
	{
		if (base.CalculateShadow() == true)
		{
			var direction = default(Vector3);
			var position  = default(Vector3);
			var color     = default(Color);
			
			SgtHelper.CalculateLight(Light, transform.position, null, null, ref position, ref direction, ref color);
			
			var rotation = Quaternion.FromToRotation(direction, Vector3.back);
			
			SetVector(0, rotation * transform.right   * transform.lossyScale.x * OuterRadius);
			SetVector(1, rotation * transform.up      * transform.lossyScale.y * OuterRadius);
			SetVector(2, rotation * transform.forward * transform.lossyScale.z * OuterRadius);
			
			SortVectors();
			
			var spin  = Quaternion.LookRotation(Vector3.forward, new Vector2(-vectors[1].x, vectors[1].y)); // Orient the shadow ellipse
			var scale = SgtHelper.Reciprocal3(new Vector3(magnitudes[0], magnitudes[1], 1.0f));
			
			var shadowT = SgtHelper.Translation(-transform.position);
			var shadowR = SgtHelper.Rotation(spin * rotation);
			var shadowS = SgtHelper.Scaling(scale);
			
			Matrix = shadowS * shadowR * shadowT;
			Ratio  = SgtHelper.Divide(OuterRadius, OuterRadius - InnerRadius);
			
			return true;
		}
		
		return false;
	}
	
	public void MarkLutAsDirty()
	{
#if UNITY_EDITOR
		if (lutDirty == false)
		{
			SgtHelper.SetDirty(this);
		}
#endif
		
		lutDirty = true;
	}
	
	protected virtual void Awake()
	{
		if (awakeCalled == false)
		{
			awakeCalled = true;
			
			PenumbraBrightness.colorKeys = defaultLightingBrightness;
			
			PenumbraColor.colorKeys = defaultLightingColor;
		}
	}
	
	protected virtual void OnDestroy()
	{
		SgtHelper.Destroy(penumbraLut);
	}
	
#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		if (SgtHelper.Enabled(this) == true)
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			
			Gizmos.DrawWireSphere(Vector3.zero, InnerRadius);
			Gizmos.DrawWireSphere(Vector3.zero, OuterRadius);
			
			if (CalculateShadow() == true)
			{
				Gizmos.matrix = Matrix.inverse;
				
				Gizmos.DrawWireCube(new Vector3(0,0,5), new Vector3(2,2,10));
			}
		}
	}
#endif
	
	private void UpdateDirty()
	{
		if (penumbraLut == null) lutDirty = true;
		
		if (lutDirty == true)
		{
			lutDirty = false;
			
			RegenerateLightingLut();
		}
	}
	
	private void RegenerateLightingLut()
	{
		if (penumbraLut == null || penumbraLut.width != 1 || penumbraLut.height != 64)
		{
			SgtHelper.Destroy(penumbraLut);
			
			penumbraLut = SgtHelper.CreateTempTeture2D(1, 64);
		}
		
		for (var y = 0; y < penumbraLut.height; y++)
		{
			var t = y / (float)penumbraLut.height;
			var a = PenumbraBrightness.Evaluate(t);
			var b = PenumbraColor.Evaluate(t);
			var c = a * b;
			
			c.a = c.grayscale;
			
			penumbraLut.SetPixel(0, y, c);
		}
		
		// Make sure the last pixel is white
		penumbraLut.SetPixel(0, penumbraLut.height - 1, Color.white);
		
		penumbraLut.wrapMode = TextureWrapMode.Clamp;
		penumbraLut.Apply();
	}
	
	private void SetVector(int index, Vector3 vector)
	{
		vectors[index] = vector;
		
		magnitudes[index] = new Vector2(vector.x, vector.y).magnitude;
	}
	
	// Put the highest magnitude vectors in indices 0 & 1
	private void SortVectors()
	{
		// Lowest is 0 or 2
		if (magnitudes[0] < magnitudes[1])
		{
			// Lowest is 0
			if (magnitudes[0] < magnitudes[2])
			{
				vectors[0] = vectors[2]; magnitudes[0] = magnitudes[2];
			}
		}
		// Lowest is 1 or 2
		else
		{
			// Lowest is 1
			if (magnitudes[1] < magnitudes[2])
			{
				vectors[1] = vectors[2]; magnitudes[1] = magnitudes[2];
			}
		}
	}
}
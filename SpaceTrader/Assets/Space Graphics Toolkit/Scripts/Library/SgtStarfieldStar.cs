using UnityEngine;

[System.Serializable]
public class SgtStarfieldStar
{
	public Sprite Sprite;
	
	public Color Color = Color.white;
	
	public float Radius;
	
	[SgtRange(-1.0f, 1.0f)]
	public float Angle;
	
	public Vector3 Position;
	
	[SgtRange(0.0f, 1.0f)]
	public float PulseSpeed = 1.0f;
	
	[SgtRange(0.0f, 1.0f)]
	public float PulseRange;
	
	[SgtRange(0.0f, 1.0f)]
	public float PulseOffset;
	
	public void CopyFrom(SgtStarfieldStar other)
	{
		Sprite      = other.Sprite;
		Color       = other.Color;
		Radius      = other.Radius;
		Angle       = other.Angle;
		Position    = other.Position;
		PulseSpeed  = other.PulseSpeed;
		PulseRange  = other.PulseRange;
		PulseOffset = other.PulseOffset;
	}
}
using UnityEngine;

[System.Serializable]
public class SgtBeltAsteroid
{
	public Sprite MainTex;
	
	public Sprite HeightTex;
	
	public Color Color = Color.white;
	
	public float Radius;
	
	public float Height;
	
	public float Angle;
	
	public float Spin;
	
	public float OrbitAngle;
	
	public float OrbitSpeed;
	
	public float OrbitDistance;
	
	public void CopyFrom(SgtBeltAsteroid other)
	{
		MainTex       = other.MainTex;
		HeightTex     = other.HeightTex;
		Color         = other.Color;
		Radius        = other.Radius;
		Angle         = other.Angle;
		Spin          = other.Spin;
		OrbitAngle    = other.OrbitAngle;
		OrbitSpeed    = other.OrbitSpeed;
		Height        = other.Height;
		OrbitDistance = other.OrbitDistance;
	}
}
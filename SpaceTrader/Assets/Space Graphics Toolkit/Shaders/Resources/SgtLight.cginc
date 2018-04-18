#if LIGHT_1 || LIGHT_2
float4 _Light1Color;
float4 _Light1Position;
float3 _Light1Direction;

	#if LIGHT_2
float4 _Light2Color;
float4 _Light2Position;
float3 _Light2Direction;
	#endif

float MiePhase(float angle, float4 mie)
{
	return mie.y / pow(mie.z - mie.x * angle, mie.w);
	//return mie.y / pow(mie.z - mie.x * angle, mie.w);
}

float RayleighPhase(float angle, float rayleigh)
{
	return rayleigh * angle * angle;
}

float MieRayleighPhase(float angle, float4 mie, float rayleigh)
{
	return MiePhase(angle, mie) + RayleighPhase(angle, rayleigh);
}
#endif
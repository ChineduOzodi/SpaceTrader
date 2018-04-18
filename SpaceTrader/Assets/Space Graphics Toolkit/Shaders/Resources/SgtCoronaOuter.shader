// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Hidden/SgtCoronaOuter"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_AtmosphereLut("Atmosphere LUT", 2D) = "white" {}
		_AtmosphereScale("Atmosphere Scale", Float) = 1
		_HorizonLengthRecip("Horizon Length Recip", Float) = 0
		_Power("Power", Float) = 3
	}
	SubShader
	{
		Tags
		{
			"Queue"           = "Transparent"
			"RenderType"      = "Transparent"
			"IgnoreProjector" = "True"
		}
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Front
			Lighting Off
			ZWrite Off
			ZTest LEqual

			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag
				// Outside
				#pragma multi_compile DUMMY SGT_A
				// Smooth
				#pragma multi_compile DUMMY SGT_B

				sampler2D _AtmosphereLut;
				float     _AtmosphereScale;
				float     _HorizonLengthRecip;
				float     _Power;
				float4    _Color;
				float4x4  _WorldToLocal;

				struct a2v
				{
					float4 vertex : POSITION;
				};

				struct v2f
				{
					float4 vertex    : SV_POSITION;
					float2 texcoord0 : TEXCOORD0; // horizon UV
					float4 texcoord1 : TEXCOORD1; // color
				};

				struct f2g
				{
					float4 color : COLOR;
				};

				void Vert(a2v i, out v2f o)
				{
					float4 wPos = mul(unity_ObjectToWorld, i.vertex);
					float4 far  = mul(_WorldToLocal, wPos);
					float4 near = mul(_WorldToLocal, float4(_WorldSpaceCameraPos, 1.0f));
#if SGT_A
					near.xyz = reflect(far.xyz, normalize(far.xyz - near.xyz));
#endif
					float depth   = length(near.xyz - far.xyz);
					float horizon = depth * _HorizonLengthRecip;
					float density = pow(horizon, _Power);

					o.vertex        = UnityObjectToClipPos(i.vertex);
					o.texcoord0     = horizon * _AtmosphereScale;
					o.texcoord1.xyz = _Color.xyz;
					o.texcoord1.w   = _Color.w * density;
				}

				void Frag(v2f i, out f2g o)
				{
					o.color = i.texcoord1 * tex2D(_AtmosphereLut, i.texcoord0);
					o.color.a = saturate(o.color.a);
#if SGT_B
					o.color = smoothstep(float4(0,0,0,0), float4(1,1,1,1), o.color);
#endif
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Hidden/SgtAccretion"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		_DustTex("Dust Tex", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_Age("Age", Float) = 0.0
		_Twist("Twist", Float) = 0.0
		_TwistBias("Twist Bias", Float) = 0.0

		_Light1Color("Light 1 Color", Color) = (0,0,0)
		_Light2Color("Light 2 Color", Color) = (0,0,0)
		_Light1Position("Light 1 Position", Vector) = (0,0,0)
		_Light2Position("Light 2 Position", Vector) = (0,0,0)

		_Shadow1Texture("Shadow 1 Texture", 2D) = "white" {}
		_Shadow1Ratio("Shadow 1 Ratio", Float) = 1
		_Shadow2Texture("Shadow 2 Texture", 2D) = "white" {}
		_Shadow2Ratio("Shadow 2 Ratio", Float) = 1
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
			Cull Off
			Lighting Off
			ZWrite Off

			CGPROGRAM
				#include "SgtLight.cginc"
				#include "SgtShadow.cginc"
				#pragma vertex Vert
				#pragma fragment Frag
				#pragma multi_compile DUMMY LIGHT_0 LIGHT_1 LIGHT_2
				#pragma multi_compile DUMMY SHADOW_1 SHADOW_2
				// Scattering
				#pragma multi_compile DUMMY SGT_A

				// Keep under instruction limits
				#if SGT_A && LIGHT_2
					#undef LIGHT_1
					#undef LIGHT_2
					#define LIGHT_1 1
					#define LIGHT_2 0
				#endif

				sampler2D _MainTex;
				sampler2D _DustTex;
				float4    _Color;
				float4    _Mie;
				float     _Age;
				float     _Twist;
				float     _TwistBias;

				struct a2v
				{
					float4 vertex    : POSITION;
					float2 texcoord0 : TEXCOORD0; // x = 0..1 distance, y = inner/outer edge position
					float  texcoord1 : TEXCOORD1; // inner/outer radius
				};

				struct v2f
				{
					float4 vertex    : SV_POSITION;
					float3 texcoord0 : TEXCOORD0;
					float4 texcoord1 : TEXCOORD1;
#if LIGHT_1 || LIGHT_2
					float3 texcoord2 : TEXCOORD2; // world vertex/pixel to camera
					float3 texcoord3 : TEXCOORD3; // world vertex/pixel to light 1
	#if LIGHT_2
					float3 texcoord4 : TEXCOORD4; // world vertex/pixel to light 2
	#endif
#endif
#if SHADOW_1 || SHADOW_2
					float4 texcoord5 : TEXCOORD5; // world vertex/pixel
#endif
				};

				struct f2g
				{
					float4 color : COLOR;
				};

				void Vert(a2v i, out v2f o)
				{
					float4 wPos = mul(unity_ObjectToWorld, i.vertex);

					o.vertex       = UnityObjectToClipPos(i.vertex);
					o.texcoord0.xy = i.texcoord0;
					o.texcoord0.z  = i.texcoord1;
					o.texcoord1    = 1.0f;
#if LIGHT_0 || LIGHT_1 || LIGHT_2
					o.texcoord1.xyz *= UNITY_LIGHTMODEL_AMBIENT.xyz;
#endif
#if LIGHT_1 || LIGHT_2
					o.texcoord2 = _WorldSpaceCameraPos - wPos.xyz;
					o.texcoord3 = _Light1Position.xyz - wPos.xyz;
	#if LIGHT_2
					o.texcoord4 = _Light2Position.xyz - wPos.xyz;
	#endif
#endif
#if SHADOW_1 || SHADOW_2
					o.texcoord5 = mul(unity_ObjectToWorld, i.vertex);
#endif
				}

				void Frag(v2f i, out f2g o)
				{
					float  scaledDist  = i.texcoord0.x;
					float  scaledEdge  = i.texcoord0.y / i.texcoord0.z;
					float  shiftedDist = scaledDist + _Age;
					float  twistedEdge = scaledEdge + pow(scaledDist, _TwistBias) * _Twist;
					float2 mainUV      = float2(scaledDist, scaledEdge);
					float2 dustUV      = float2(shiftedDist, twistedEdge);

					o.color = i.texcoord1;
#if LIGHT_1 || LIGHT_2
					i.texcoord2 = normalize(i.texcoord2);
					i.texcoord3 = normalize(i.texcoord3);

					float3 lighting = saturate(dot(i.texcoord3, i.texcoord2)) * _Light1Color.xyz;
	#if LIGHT_2
					i.texcoord4 = normalize(i.texcoord4);

					lighting += saturate(dot(i.texcoord4, i.texcoord2)) * _Light2Color.xyz;
	#endif
	#if SGT_A
					lighting += MiePhase(dot(i.texcoord2, i.texcoord3), _Mie) * _Light1Color.xyz;
		#if LIGHT_2
					lighting += MiePhase(dot(i.texcoord2, i.texcoord4), _Mie) * _Light2Color.xyz;
		#endif
	#endif
	#if SHADOW_1 || SHADOW_2
					lighting *= ShadowColor(i.texcoord5).xyz;
	#endif
					o.color.xyz += lighting;
					o.color.a = saturate(o.color.a);
#else
	#if SHADOW_1 || SHADOW_2
					o.color.xyz *= ShadowColor(i.texcoord5).xyz;
	#endif
#endif
					o.color *= tex2D(_MainTex, mainUV) * tex2D(_DustTex, dustUV) * _Color;
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader

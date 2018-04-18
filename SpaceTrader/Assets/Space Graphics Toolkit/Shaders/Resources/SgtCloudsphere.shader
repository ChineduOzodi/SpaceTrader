// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Hidden/SgtCloudsphere"
{
	Properties
	{
		_MainTex("Main Tex", CUBE) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_RimLut("Rim LUT", 2D) = "white" {}
		_LightingLut("Lighting LUT", 2D) = "white" {}
		_FadeRadius("Fade Radius", Float) = 1
		_FadeScale("Fade Scale", Float) = 1

		_Light1Color("Light 1 Color", Color) = (0,0,0)
		_Light2Color("Light 2 Color", Color) = (0,0,0)
		_Light1Position("Light 1 Position", Vector) = (0,0,0)
		_Light2Position("Light 2 Position", Vector) = (0,0,0)
		_Light1Direction("Light 1 Direction", Vector) = (0,0,0)
		_Light2Direction("Light 2 Direction", Vector) = (0,0,0)

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
			Cull Back
			Lighting Off
			ZWrite Off

			CGPROGRAM
				#include "SgtLight.cginc"
				#include "SgtShadow.cginc"
				#pragma vertex Vert
				#pragma fragment Frag
				#pragma multi_compile DUMMY LIGHT_0 LIGHT_1 LIGHT_2
				#pragma multi_compile DUMMY SHADOW_1 SHADOW_2
				// Fade near
				#pragma multi_compile DUMMY SGT_A

				samplerCUBE _MainTex;
				float4      _Color;
				float       _FadeRadius;
				float       _FadeScale;
				sampler2D   _RimLut;
				sampler2D   _LightingLut;

				struct a2v
				{
					float4 vertex   : POSITION;
					float3 normal   : NORMAL;
				};

				struct v2f
				{
					float4 vertex    : SV_POSITION;
					float4 texcoord0 : TEXCOORD0; // color
					float3 texcoord1 : TEXCOORD1; // normal
					float4 texcoord2 : TEXCOORD2; // x = rim theta, y = light 1 theta, z = light 2 theta
#if SHADOW_1 || SHADOW_2
					float4 texcoord3 : TEXCOORD3; // wpos
#endif
					float3 texcoord4 : TEXCOORD4; // near
				};

				struct f2g
				{
					float4 color : COLOR;
				};

				void Vert(a2v i, out v2f o)
				{
					float4 wVertex = mul(unity_ObjectToWorld, i.vertex);
					float3 wNormal = normalize(mul((float3x3)unity_ObjectToWorld, i.normal));
					float3 wNear   = _WorldSpaceCameraPos - wVertex.xyz;

					o.vertex    = UnityObjectToClipPos(i.vertex);
					o.texcoord0 = 1.0f;
#if LIGHT_0 || LIGHT_1 || LIGHT_2
					o.texcoord0.xyz *= UNITY_LIGHTMODEL_AMBIENT.xyz;
#endif
					o.texcoord1 = i.normal;
					o.texcoord2 = dot(wNormal, normalize(wNear));
#if LIGHT_1 || LIGHT_2
					o.texcoord2.y = dot(wNormal, _Light1Direction) * 0.5f + 0.5f;
	#if LIGHT_2
					o.texcoord2.z = dot(wNormal, _Light2Direction) * 0.5f + 0.5f;
	#endif
#endif
#if SHADOW_1 || SHADOW_2
					o.texcoord3 = wVertex;
#endif
#if SGT_A
					o.texcoord4 = wNear;
#endif
				}

				void Frag(v2f i, out f2g o)
				{
					o.color = i.texcoord0;
#if LIGHT_1 || LIGHT_2
					float4 light1   = tex2D(_LightingLut, i.texcoord2.yy) * _Light1Color;
					float4 lighting = float4(light1.xyz, 0.0f);
	#if LIGHT_2
					float4 light2 = tex2D(_LightingLut, i.texcoord2.zz) * _Light2Color;

					lighting.xyz += light2.xyz;
	#endif
	#if SHADOW_1 || SHADOW_2
					lighting *= ShadowColor(i.texcoord3);
	#endif
					o.color += lighting;
#endif
					o.color.a = saturate(o.color.a);
#if !LIGHT_0 && !LIGHT_1 && !LIGHT_2
	#if SHADOW_1 || SHADOW_2
					o.color.xyz *= ShadowColor(i.texcoord3).xyz;
	#endif
#endif
#if SGT_A
					o.color.a *= saturate((length(i.texcoord4) - _FadeRadius) * _FadeScale);
#endif
					o.color *= tex2D(_RimLut, i.texcoord2.xx) * texCUBE(_MainTex, i.texcoord1) * _Color;
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader

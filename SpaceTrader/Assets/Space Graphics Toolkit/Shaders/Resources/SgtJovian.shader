// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Hidden/SgtJovian"
{
	Properties
	{
		_MainTex("Main Tex", CUBE) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_Mie("Mie", Vector) = (0,0,0)
		_Density("Density", Float) = 1
		_Power("Power", Float) = 3
		_RimLut("Rim LUT", 2D) = "white" {}
		_LightingLut("Lighting LUT", 2D) = "white" {}

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
			Cull Front
			Lighting Off
			ZWrite Off

			CGPROGRAM
				#include "SgtLight.cginc"
				#include "SgtShadow.cginc"
				#pragma vertex Vert
				#pragma fragment Frag
				#pragma multi_compile DUMMY LIGHT_0 LIGHT_1 LIGHT_2
				#pragma multi_compile DUMMY SHADOW_1 SHADOW_2
				// Outside
				#pragma multi_compile DUMMY SGT_A
				// Smooth
				#pragma multi_compile DUMMY SGT_B
				// Logathmic
				#pragma multi_compile DUMMY SGT_C
				// Scattering
				#pragma multi_compile DUMMY SGT_D
				// Limit
				#pragma multi_compile DUMMY SGT_E

				// Keep under instruction limits
				#if SGT_D && LIGHT_2
					#undef LIGHT_1
					#undef LIGHT_2
					#define LIGHT_1 1
					#define LIGHT_2 0
				#endif

				samplerCUBE _MainTex;
				float4      _Color;
				float4      _Mie;
				float       _Density;
				float       _Power;
				sampler2D   _RimLut;
				sampler2D   _LightingLut;
				float4x4    _WorldToLocal;
				float4x4    _LocalToWorld;

				struct a2v
				{
					float4 vertex : POSITION;
				};

				struct v2f
				{
					float4 vertex    : SV_POSITION;
					float2 texcoord0 : TEXCOORD0; // rim
					float3 texcoord1 : TEXCOORD1; // near model position
					float4 texcoord2 : TEXCOORD2; // color
#if LIGHT_1 || LIGHT_2
					float4 texcoord3 : TEXCOORD3; // xyz = light 1 to vertex/pixel, w = light 1 theta
	#if LIGHT_2
					float4 texcoord4 : TEXCOORD4; // xyz = light 2 to vertex/pixel, w = light 2 theta
	#endif
	#if SGT_D
					float3 texcoord5 : TEXCOORD5; // camera to vertex/pixel
	#endif
#endif
#if SHADOW_1 || SHADOW_2
					float4 texcoord6 : TEXCOORD6; // near world position
#endif
				};

				struct f2g
				{
					float4 color : COLOR;
				};

				float ScaleDown(float from)
				{
					return 1.0f - exp(-from);
				}

				void Vert(a2v i, out v2f o)
				{
					float4 wPos = mul(unity_ObjectToWorld, i.vertex);
					float4 far  = mul(_WorldToLocal, wPos);
					float4 near = mul(_WorldToLocal, float4(_WorldSpaceCameraPos, 1.0f));
					float4 lCam = near;
#if SGT_A
					near.xyz = reflect(far.xyz, normalize(far.xyz - near.xyz));
#endif
					float depth   = length(near.xyz - far.xyz);
					float density = pow(depth, _Power) * _Density;
#ifdef SGT_C
					density = ScaleDown(density);
#else
					density = saturate(density);
#endif
					o.vertex        = UnityObjectToClipPos(i.vertex);
					o.texcoord0     = dot(near.xyz, normalize(lCam.xyz - far.xyz));
					o.texcoord1     = near.xyz;
					o.texcoord2.xyz = 1.0f;
					o.texcoord2.w   = density;
#if LIGHT_0 || LIGHT_1 || LIGHT_2
					o.texcoord2.xyz *= UNITY_LIGHTMODEL_AMBIENT.xyz;
#endif
#if LIGHT_1 || LIGHT_2
					float3 nearD = normalize(near.xyz);

					o.texcoord3 = dot(nearD, _Light1Direction) * 0.5f + 0.5f;
	#if LIGHT_2
					o.texcoord4 = dot(nearD, _Light2Direction) * 0.5f + 0.5f;
	#endif
	#if SGT_D
					o.texcoord5     = _WorldSpaceCameraPos - wPos.xyz;
					o.texcoord3.xyz = _Light1Position.xyz - _WorldSpaceCameraPos;
		#if LIGHT_2
					o.texcoord4.xyz = _Light2Position.xyz - _WorldSpaceCameraPos;
		#endif
	#endif
#endif
#if SHADOW_1 || SHADOW_2
					o.texcoord6 = mul(_LocalToWorld, near);
#endif
				}

				void Frag(v2f i, out f2g o)
				{
					float4 atmosphere = tex2D(_RimLut, i.texcoord0) * texCUBE(_MainTex, i.texcoord1) * _Color;
					o.color = atmosphere * i.texcoord2;
#if LIGHT_1 || LIGHT_2
					float4 light1   = tex2D(_LightingLut, i.texcoord3.ww) * atmosphere * _Light1Color;
					float4 lighting = float4(light1.xyz, 0.0f);
	#if LIGHT_2
					float4 light2 = tex2D(_LightingLut, i.texcoord4.ww) * atmosphere * _Light2Color;

					lighting.xyz += light2.xyz;
	#endif
	#if SGT_D
					i.texcoord5 = normalize(i.texcoord5);

					float4 scattering = MiePhase(dot(i.texcoord5, normalize(i.texcoord3.xyz)), _Mie) * light1;
		#if LIGHT_2
					scattering += MiePhase(dot(i.texcoord5, normalize(i.texcoord4.xyz)), _Mie) * light2;
		#endif
		#if SGT_E
					scattering.w = max(scattering.x, max(scattering.y, scattering.z)); // Limit alpha
		#endif
					lighting += scattering * i.texcoord2.w * (1.0f - i.texcoord2.w); // Only scatter at the edges
	#endif
	#if SHADOW_1 || SHADOW_2
					lighting *= ShadowColor(i.texcoord6);
	#endif
					o.color += lighting;
#endif
					o.color.a = saturate(o.color.a);
#if !LIGHT_0 && !LIGHT_1 && !LIGHT_2
	#if SHADOW_1 || SHADOW_2
					o.color.xyz *= ShadowColor(i.texcoord6).xyz;
	#endif
#endif
#if SGT_B
					o.color = smoothstep(float4(0,0,0,0), float4(1,1,1,1), o.color);
#endif
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader

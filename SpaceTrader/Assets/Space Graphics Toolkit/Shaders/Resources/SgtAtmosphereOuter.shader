// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Hidden/SgtAtmosphereOuter"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_Mie("Mie", Vector) = (0,0,0)
		_Rayleigh("Rayleigh", Float) = 0
		_AtmosphereLut("Atmosphere LUT", 2D) = "white" {}
		_AtmosphereScale("Atmosphere Scale", Float) = 1
		_HorizonLengthRecip("Horizon Length Recip", Float) = 0
		_LightingLut("Lighting LUT", 2D) = "white" {}
		_Power("Power", Float) = 3

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
				// Scattering
				#pragma multi_compile DUMMY SGT_C
				// Limit
				#pragma multi_compile DUMMY SGT_D

				// Keep under instruction limits
				#if SGT_C && LIGHT_2
					#undef LIGHT_1
					#undef LIGHT_2
					#define LIGHT_1 1
					#define LIGHT_2 0
				#endif

				#if SGT_C && SHADOW_2
					#undef SHADOW_1
					#undef SHADOW_2
					#define SHADOW_1 1
					#define SHADOW_2 0
				#endif

				#if SGT_C && SGT_D && LIGHT_1 && SHADOW_1
					#undef SGT_D
					#define SGT_D 0
				#endif

				float4    _Color;
				float4    _Mie;
				float     _Rayleigh;
				float     _AtmosphereScale;
				float     _HorizonLengthRecip;
				float     _Power;
				sampler2D _AtmosphereLut;
				sampler2D _LightingLut;
				float4x4  _WorldToLocal;

				struct a2v
				{
					float4 vertex : POSITION;
				};

				struct v2f
				{
					float4 vertex    : SV_POSITION;
					float  texcoord0 : TEXCOORD0; // horizon UV
					float4 texcoord1 : TEXCOORD1; // xyz = color, w = opacity
#if LIGHT_1 || LIGHT_2
					float4 texcoord2 : TEXCOORD2; // xyz = light 1 to vertex/pixel, w = light 1 theta
	#if LIGHT_2
					float4 texcoord3 : TEXCOORD3; // xyz = light 2 to vertex/pixel, w = light 2 theta
	#endif
	#if SGT_C
					float3 texcoord4 : TEXCOORD4; // camera to vertex/pixel
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
					o.texcoord1.xyz = 1.0f;
					o.texcoord1.w   = density;
#if LIGHT_0 || LIGHT_1 || LIGHT_2
					o.texcoord1.xyz *= UNITY_LIGHTMODEL_AMBIENT.xyz;
#endif
#if LIGHT_1 || LIGHT_2
					float3 nearD = normalize(near.xyz);

					o.texcoord2 = dot(nearD, _Light1Direction) * 0.5f + 0.5f;
	#if LIGHT_2
					o.texcoord3 = dot(nearD, _Light2Direction) * 0.5f + 0.5f;
	#endif
	#if SGT_C
					o.texcoord4     = _WorldSpaceCameraPos - wPos.xyz;
					o.texcoord2.xyz = _Light1Position.xyz - _WorldSpaceCameraPos;
		#if LIGHT_2
					o.texcoord3.xyz = _Light2Position.xyz - _WorldSpaceCameraPos;
		#endif
	#endif
#endif
#if SHADOW_1 || SHADOW_2
					o.texcoord5 = wPos;
#endif
				}

				void Frag(v2f i, out f2g o)
				{
					float4 atmosphere = tex2D(_AtmosphereLut, i.texcoord0.xx) * _Color;

					o.color = i.texcoord1 * atmosphere;
#if LIGHT_1 || LIGHT_2
					float4 light1   = tex2D(_LightingLut, i.texcoord2.ww) * atmosphere * _Light1Color;
					float4 lighting = float4(light1.xyz, 0.0f);
	#if LIGHT_2
					float4 light2 = tex2D(_LightingLut, i.texcoord3.ww) * atmosphere * _Light2Color;

					lighting.xyz += light2.xyz;
	#endif
	#if SHADOW_1 || SHADOW_2
					float4 shadowColor = ShadowColor(i.texcoord5);
					lighting *= ShadowColor(i.texcoord5);
	#endif
	#if SGT_C
					i.texcoord4 = normalize(i.texcoord4);

					float4 scattering = MieRayleighPhase(dot(i.texcoord4, normalize(i.texcoord2.xyz)), _Mie, _Rayleigh) * light1;
		#if LIGHT_2
					scattering += MieRayleighPhase(dot(i.texcoord4, normalize(i.texcoord3.xyz)), _Mie, _Rayleigh) * light2;
		#endif
		#if SHADOW_1 || SHADOW_2
					scattering *= shadowColor.w;
		#endif
		#if SGT_D
					lighting += saturate(scattering * i.texcoord1.w) * saturate(1.0f - (o.color + lighting)); // Make the scattering fill the remaining color
		#else
					lighting += scattering * i.texcoord1.w; // Allow overflow
		#endif
	#endif
					o.color += lighting;
					o.color.a = saturate(o.color.a);
#endif
#if !LIGHT_0 && !LIGHT_1 && !LIGHT_2
	#if SHADOW_1 || SHADOW_2
					o.color.xyz *= ShadowColor(i.texcoord5).xyz;
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

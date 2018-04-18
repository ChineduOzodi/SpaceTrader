// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Hidden/SgtStarfield"
{
	Properties
	{
		_CameraRollAngle("Camera Roll Angle", Float) = 0
		_Texture("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_Scale("Scale", Float) = 1
		_Age("Age", Float) = 1
		_StretchDirection("Stretch Direction", Vector) = (0,0,0)
		_StretchLength("Stretch Length", Float) = 0
		_StretchVector("Stretch Vector", Float) = 0
		_WrapSize("Wrap Size", Vector) = (0,0,0)
		_FadeNearRadius("Fade Near Radius", Float) = 0
		_FadeNearScale("Fade Near Scale", Float) = 0
		_FadeFarRadius("Fade Far Radius", Float) = 0
		_FadeFarScale("Fade Far Scale", Float) = 0
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
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
			Blend One One
			Cull Off
			Lighting Off
			ZWrite Off

			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag
				// Wrap 2D or 3D
				#pragma multi_compile DUMMY SGT_A SGT_B
				// Stretch
				#pragma multi_compile DUMMY SGT_C
				// Fade near
				#pragma multi_compile DUMMY SGT_D
				// Fade far
				#pragma multi_compile DUMMY SGT_E
				// Pulse (avoid using SGT_F)
				#pragma multi_compile DUMMY LIGHT_1
				// Soft particles (avoid using SGT_G)
				#pragma multi_compile DUMMY LIGHT_2

#if LIGHT_2
				#include "UnityCG.cginc"
#endif

				sampler2D _Texture;
				float4    _Color;
				float     _Scale;
				float     _Age;
				float3    _WrapSize;
				float3    _StretchDirection;
				float     _StretchLength;
				float3    _StretchVector;
				float     _CameraRollAngle;
				float     _FadeNearRadius;
				float     _FadeNearScale;
				float     _FadeFarRadius;
				float     _FadeFarScale;

				sampler2D_float _CameraDepthTexture;
				float _InvFade;

				struct a2v
				{
					float4 vertex    : POSITION;
					float4 color     : COLOR;
					float3 normal    : NORMAL; // xy = corner offset, z = angle
					float3 tangent   : TANGENT; // x = pulse offset, y = pulse speed, z = pulse scale
					float2 texcoord0 : TEXCOORD0; // uv
					float2 texcoord1 : TEXCOORD1; // x = radius, y = back or front [-0.5 .. 0.5]
				};

				struct v2f
				{
					float4 vertex    : SV_POSITION;
					float4 color     : COLOR;
					float2 texcoord0 : TEXCOORD0;
#if LIGHT_2
					float4 projPos : TEXCOORD1;
#endif
				};

				struct f2g
				{
					float4 color : COLOR;
				};

				float2 Rotate(float2 v, float a)
				{
					float s = sin(a);
					float c = cos(a);
					return float2(c * v.x - s * v.y, s * v.x + c * v.y);
				}

				void Vert(a2v i, out v2f o)
				{
#if SGT_A || SGT_B
					float4 cameraO   = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0f)) / _Scale;
					float3 relativeO = i.vertex.xyz - cameraO.xyz;
	#if SGT_A
					i.vertex.xy = cameraO.xy + (frac(relativeO.xy / _WrapSize.xy) - 0.5f) * _WrapSize.xy;
	#elif SGT_B
					i.vertex.xyz = cameraO.xyz + (frac(relativeO.xyz / _WrapSize.xyz) - 0.5f) * _WrapSize.xyz;
	#endif
#endif
					float radius = i.texcoord1.x * _Scale;
#if SGT_C
					float4 vertexM  = mul(unity_ObjectToWorld, i.vertex);
					float4 vertexMV = mul(UNITY_MATRIX_MV, i.vertex);
					float3 up       = cross(_StretchDirection, normalize(vertexM.xyz - _WorldSpaceCameraPos));

					// Uncomment below if you want the stars to be stretched based on their size too
					vertexM.xyz += _StretchVector * i.texcoord1.y; // * radius;
					vertexM.xyz += up * i.normal.y * radius;

					o.vertex = mul(UNITY_MATRIX_VP, vertexM);
#else
	#if LIGHT_1
					radius *= 1.0f + sin(i.tangent.x * 3.141592654f + _Age * i.tangent.y) * i.tangent.z;
	#endif
					float4 vertexMV = mul(UNITY_MATRIX_MV, i.vertex);
					float4 cornerMV = vertexMV;
					float  angle    = _CameraRollAngle + i.normal.z * 3.141592654f;

					i.normal.xy = Rotate(i.normal.xy, angle);

					cornerMV.xy += i.normal.xy * radius;

					o.vertex = mul(UNITY_MATRIX_P, cornerMV);
#endif
#if SGT_D
					i.color *= saturate((length(vertexMV.xyz) - _FadeNearRadius) * _FadeNearScale);
#endif
#if SGT_E
					i.color *= saturate((_FadeFarRadius - length(vertexMV.xyz)) * _FadeFarScale);
#endif
					o.color     = i.color * _Color;
					o.texcoord0 = i.texcoord0;
#if LIGHT_2
					o.projPos = ComputeScreenPos(o.vertex);
					o.projPos.z = -mul(UNITY_MATRIX_MV, i.vertex).z;
#endif
				}

				void Frag(v2f i, out f2g o)
				{
#ifdef LIGHT_2
					float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
					float partZ  = i.projPos.z;
					float fade   = saturate(_InvFade * (sceneZ-partZ));

					i.color *= fade;
#endif

					o.color = tex2D(_Texture, i.texcoord0) * i.color;
					o.color.a = saturate(o.color.a);
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader

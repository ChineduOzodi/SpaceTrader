// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Hidden/SgtBelt"
{
	Properties
	{
		_CameraRollAngle("Camera Roll Angle", Float) = 0
		_MainTex("Main Tex", 2D) = "white" {}
		_HeightTex("Height Tex", 2D) = "black" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_Scale("Scale", Float) = 1
		_Age("Age", Float) = 0
		
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
			"Queue"           = "Geometry"
			"RenderType"      = "TransparentCutout"
			"IgnoreProjector" = "True"
		}
		Pass
		{
			Blend One Zero
			Cull Back
			Lighting Off
			ZTest LEqual
			
			CGPROGRAM
				#include "SgtLight.cginc"
				#include "SgtShadow.cginc"
				#pragma vertex Vert
				#pragma fragment Frag
				#pragma multi_compile DUMMY LIGHT_0 LIGHT_1 LIGHT_2
				#pragma multi_compile DUMMY SHADOW_1 SHADOW_2
				
				sampler2D _MainTex;
				sampler2D _HeightTex;
				float4    _Color;
				float     _Scale;
				float     _Age;
				float     _CameraRollAngle;
				
				struct a2v
				{
					float4 vertex    : POSITION; // x = orbit angle, y = orbit distance, z = orbit speed
					float4 color     : COLOR;
					float3 normal    : NORMAL;
					float2 tangent   : TANGENT; // x = angle, y = spin
					float2 texcoord0 : TEXCOORD0; // uv
					float2 texcoord1 : TEXCOORD1; // x = radius, y = height
				};
				
				struct v2f
				{
					float4 vertex    : SV_POSITION;
					float2 texcoord0 : TEXCOORD0; // uv
					float4 texcoord1 : TEXCOORD1; // color
#if LIGHT_1 || LIGHT_2
					float3 texcoord2 : TEXCOORD2; // scaled model view vertex/pixel offset
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
				
				float2 Rotate(float2 v, float a)
				{
					float s = sin(a);
					float c = cos(a);
					return float2(c * v.x - s * v.y, s * v.x + c * v.y);
				}
				
				void Vert(a2v i, out v2f o)
				{
					float orbitAngle    = i.vertex.x + i.vertex.z * _Age;
					float orbitDistance = i.vertex.y;
					float angle         = _CameraRollAngle + (i.tangent.x + i.tangent.y * _Age) * 3.141592654f;
					float radius        = i.texcoord1.x * _Scale;
					
					i.vertex.x = sin(orbitAngle) * orbitDistance;
					i.vertex.y = i.texcoord1.y;
					i.vertex.z = cos(orbitAngle) * orbitDistance;
					i.vertex.w = 1.0f;
					
					i.normal.xy = Rotate(i.normal.xy, angle);
					
					float4 wPos     = mul(unity_ObjectToWorld, i.vertex);
					float4 vertexMV = mul(UNITY_MATRIX_MV, i.vertex);
					float4 cornerMV = vertexMV;
					
					cornerMV.xyz += i.normal * radius;
					
					o.vertex    = mul(UNITY_MATRIX_P, cornerMV);
					o.texcoord0 = i.texcoord0;
					o.texcoord1 = 1.0f;
#if LIGHT_0 || LIGHT_1 || LIGHT_2
					o.texcoord1.xyz *= UNITY_LIGHTMODEL_AMBIENT.xyz;
#endif
#if LIGHT_1 || LIGHT_2
					o.texcoord2 = (cornerMV.xyz - vertexMV.xyz) / radius;
					
					float4 light1v = mul(UNITY_MATRIX_V, _Light1Position);
					o.texcoord3 = normalize(light1v.xyz - vertexMV.xyz);
	#if LIGHT_2
					float4 light2v = mul(UNITY_MATRIX_V, _Light2Position);
					o.texcoord4 = normalize(light2v.xyz - vertexMV.xyz);
	#endif
#endif
#if SHADOW_1 || SHADOW_2
					o.texcoord5 = wPos;
#endif
				}
				
				void Frag(v2f i, out f2g o)
				{
					o.color = i.texcoord1;
#if LIGHT_1 || LIGHT_2
					float3 offset = i.texcoord2; offset.z += tex2D(_HeightTex, i.texcoord0).a;
					float3 normal = normalize(offset);
					
					float3 lighting = saturate(dot(normal, i.texcoord3)) * _Light1Color;
	#if LIGHT_2
					lighting += saturate(dot(normal, i.texcoord4)) * _Light1Color;
	#endif
	#if SHADOW_1 || SHADOW_2
					lighting *= ShadowColor(i.texcoord5).xyz;
	#endif
					o.color.xyz += lighting;
#endif
#if !LIGHT_0 && !LIGHT_1 && !LIGHT_2
	#if SHADOW_1 || SHADOW_2
					o.color.xyz *= ShadowColor(i.texcoord5).xyz;
	#endif
#endif
					o.color *= tex2D(_MainTex, i.texcoord0) * _Color;
					
					clip(o.color.a - 0.5f);
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader
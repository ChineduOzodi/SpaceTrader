Shader "Hidden/SFSoftShadows/FogLayer" {
	Properties {
		_FogColor ("Fog color and alpha.", Color) = (1.0, 1.0, 1.0, 0.0)
		_Scatter ("Light scattering color (RGB), Hard/soft mix (A)", Color) = (1.0, 1.0, 1.0, 0.15)
	}
	
	SubShader {
		Pass {
			Blend One OneMinusSrcAlpha
			Cull Off
			Lighting Off
			ZTest Always
			ZWrite Off
			
			CGPROGRAM
				#include "UnityCG.cginc"
				#pragma vertex VShader
				#pragma fragment FShader
				
#if UNITY_UV_STARTS_AT_TOP
				float4x4 _SFProjection;
#endif
				
				sampler2D _SFLightMap;
				sampler2D _SFLightMapWithShadows;
				
				// Transmi
				half4 _FogColor;
				half4 _Scatter;
				
				struct VertexInput {
					float3 position : POSITION;
					float2 texVector2 : TEXCOORD0;
				};
				
				struct VertexOutput {
					float4 position : SV_POSITION;
					float4 lightVector2 : TEXCOORD1;
				};
				
				VertexOutput VShader(VertexInput v){
					float4 position = mul(UNITY_MATRIX_MVP, float4(v.position, 1.0));
					
#if !UNITY_UV_STARTS_AT_TOP
					float4 shadowVector2 = position;
#else
					// Unity does some magic crap to the projection matrix for DirectX
					// Can't trust it because you can't detect when it applies workarounds to it.
					float4 shadowVector2 = mul(mul(_SFProjection, UNITY_MATRIX_MV), float4(v.position, 1.0));
#endif
					
					VertexOutput o = {position, 0.5*(shadowVector2 + shadowVector2.w)};
					return o;
				}
				
				fixed4 FShader(VertexOutput v) : SV_Target {
					half3 l0 = tex2Dproj(_SFLightMap, UNITY_PROJ_COORD(v.lightVector2)).rgb;
					half3 l1 = tex2Dproj(_SFLightMapWithShadows, UNITY_PROJ_COORD(v.lightVector2)).rgb;
					half3 scatter = _Scatter.rgb*lerp(l0, l1, _Scatter.a);
					
					return half4(_FogColor.rgb*_FogColor.a + scatter, _FogColor.a);
				}
			ENDCG
		}
	}
}

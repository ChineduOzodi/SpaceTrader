Shader "Sprites/SFSoftShadow" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SoftHardMix ("Unshadowed/Shadowed Mix", Range(0.0, 1.0)) = 0.0
		_Glow ("Self Illumination", Color) = (0.0, 0.0, 0.0, 0.0)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}
	
	SubShader {
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
		
		Pass {
			Blend One OneMinusSrcAlpha
			Cull Off
			Lighting Off
			ZWrite Off
			
			Fog {
				Mode Off
			}
			
			CGPROGRAM
				#include "UnityCG.cginc"
				#pragma vertex VShader
				#pragma fragment FShader
				#pragma multi_compile _ PIXELSNAP_ON
				
#if UNITY_UV_STARTS_AT_TOP
				float4x4 _SFProjection;
#endif
				
				sampler2D _MainTex;
				float4 _MainTex_ST;
				
				sampler2D _SFLightMap;
				sampler2D _SFLightMapWithShadows;
				float _SFGlobalDynamicRange;
				
				float _SoftHardMix;
				float4 _Glow;
				
				struct VertexInput {
					float3 position : POSITION;
					float2 texVector2 : TEXCOORD0;
					float4 color : COLOR;
				};
				
				struct VertexOutput {
					float4 position : SV_POSITION;
					float2 texVector2 : TEXCOORD0;
					float4 lightVector2 : TEXCOORD1;
					float4 color : COLOR;
				};
				
				VertexOutput VShader(VertexInput v){
					float4 position = mul(UNITY_MATRIX_MVP, float4(v.position, 1.0));
					
					#if defined(PIXELSNAP_ON)
					position = UnityPixelSnap(position);
					#endif
					
#if !UNITY_UV_STARTS_AT_TOP
					float4 shadowVector2 = position;
#else
					// Unity does some magic crap to the projection matrix for DirectX
					// Can't trust it because you can't detect when it applies workarounds to it.
					float4 shadowVector2 = mul(mul(_SFProjection, UNITY_MATRIX_MV), float4(v.position, 1.0));
					
					#if defined(PIXELSNAP_ON)
					shadowVector2 = UnityPixelSnap(shadowVector2);
					#endif
#endif
					
					VertexOutput o = {
						position,
						TRANSFORM_TEX(v.texVector2, _MainTex),
						0.5*(shadowVector2 + shadowVector2.w),
						v.color,
					};

					// If you're on DirectX, using Full Screen Image Effects, AND have certain anti-aliasing settings,
					// you may need to manually flip your UV coordinates to account for DirectX flipping issues.
					// If so, uncomment this line:
					//o.texVector2.y = 1.0 - o.texVector2.y;

					return o;
				}
				
				fixed4 FShader(VertexOutput v) : SV_Target {
					fixed4 color = v.color*tex2D(_MainTex, v.texVector2);
					
					fixed3 l0 = tex2Dproj(_SFLightMap, UNITY_PROJ_COORD(v.lightVector2)).rgb;
					fixed3 l1 = tex2Dproj(_SFLightMapWithShadows, UNITY_PROJ_COORD(v.lightVector2)).rgb;
					fixed3 light = lerp(l0, l1, _SoftHardMix) + _Glow;
					color.rgb *= light*_SFGlobalDynamicRange*color.a;
					
					return color;
				}
			ENDCG
		}
	}
}

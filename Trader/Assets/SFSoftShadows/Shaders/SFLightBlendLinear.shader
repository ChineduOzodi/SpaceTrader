Shader "Hidden/SFSoftShadows/LightBlendLinear" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader {
		Pass {
			Blend DstAlpha One, One Zero
			Cull Off
			Lighting Off
			ZTest Always
			ZWrite Off
			
			CGPROGRAM
				#pragma vertex VShader
				#pragma fragment FShader
				
				sampler2D _MainTex;
				
				struct VertexInput {
					float3 position : POSITION;
					float4 color : COLOR;
				};
				
				struct VertexOutput {
					float4 position : SV_POSITION;
					float2 texCoord : TEXCOORD0;
					float4 color : COLOR;
				};
				
				VertexOutput VShader(VertexInput v){
					float4 p = float4(v.position, 1.0);
					float4 cc = mul(UNITY_MATRIX_P, p);
					
					VertexOutput o = {p, 0.5*cc.xy/cc.w + 0.5, v.color};
					return o;
				}
				
				fixed4 FShader(VertexOutput v) : SV_Target {
					return fixed4(v.color.rgb*tex2D(_MainTex, v.texCoord).aaa, 1.0);
				}
			ENDCG
		}
	}
}

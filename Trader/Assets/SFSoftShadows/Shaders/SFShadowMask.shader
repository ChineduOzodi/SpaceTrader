Shader "Hidden/SFSoftShadows/ShadowMask" {
	SubShader {
		Pass {
			BlendOp RevSub
			Blend One One
			Cull Back
			Lighting Off
			ZTest Always
			ZWrite Off
			
			CGPROGRAM
				#pragma vertex VShader
				#pragma fragment FShader
				
				float _GlobalSoftening;
				
				struct VertexInput {
					float3 occluderCoord_radius : POSITION;
					float3 segmentA_soften : TEXCOORD0;
					float2 segmentB : TEXCOORD1;
				};
				
				struct VertexOutput {
					float4 position : SV_POSITION;
//					float4 projectedPosition : TEXCOORD0;
					float4 penumbra : TEXCOORD1;
					float clipValue : TEXCOORD2;
				};
				
				float2x2 penumbraMatrix(float2 basisX, float2 basisY){
					float2x2 m = transpose(float2x2(basisX, basisY));
					return float2x2(m._m11, -m._m01, -m._m10, m._m00)/determinant(m);
				}
				
				VertexOutput VShader(VertexInput v){
					// The radius value is full of garbage if not multiplied by something.
					// Seems to be a Cg bug looking at the shader assembly output, but hard to tell.
					// Can't multiply by 1.0 because it is optimized away by the Cg compiler. -_-
					const float BIZARRE_BUG_WORKAROUND = 1.0 + 1e-5;
					
					float2 occluderCoord = v.occluderCoord_radius.xy;
					float radius = max(1e-5, v.occluderCoord_radius.z)*BIZARRE_BUG_WORKAROUND;
					
					float2 segmentA = v.segmentA_soften.xy;
					float2 segmentB = v.segmentB;
					
					// Derived values.
					float2 lightOffsetA = float2(-radius,  radius)*normalize(segmentA).yx;
					float2 lightOffsetB = float2( radius, -radius)*normalize(segmentB).yx;
					
					float2 position = lerp(segmentA, segmentB, occluderCoord.x);
					float2 projectionOffset = lerp(lightOffsetA, lightOffsetB, occluderCoord.x);
					float4 projected = float4(position - projectionOffset*occluderCoord.y, 0.0, 1.0 - occluderCoord.y);
					
					// Output values
					float4 clipPosition = mul(UNITY_MATRIX_MVP, projected);
					
					float2 penumbraA = mul(penumbraMatrix(lightOffsetA, segmentA), projected.xy - segmentA*projected.w);
					float2 penumbraB = mul(penumbraMatrix(lightOffsetB, segmentB), projected.xy - segmentB*projected.w);
					
					float2 clipNormal = normalize(segmentB - segmentA).yx*float2(-1.0, 1.0);
//					float clipDist = 0.5*dot(clipNormal, segmentA + segmentB);
//					float soften = max(_GlobalSoftening, v.segmentA_soften.z);
					float clipValue = dot(clipNormal, projected.xy - projected.w*position);
//					float clipValues = float4(clipNormal/clipDist, 1.0/soften, clipValue);
					
					VertexOutput o = {clipPosition, float4(penumbraA, penumbraB), clipValue};
					return o;
				}
				
				fixed4 FShader(VertexOutput v) : SV_Target {
					float2 p = clamp(v.penumbra.xz/v.penumbra.yw, -1.0, 1.0);
					float2 value = lerp(p*(3.0 - p*p)*0.25 + 0.5, 1.0, step(v.penumbra.yw, 0.0));
					float occlusion = (value[0] + value[1] - 1.0);
					
//					float2 position = v.projectedPosition.xy/v.projectedPosition.w;
//					float soften = length(position)*(1.0 - 1.0/min(0.0, dot(v.clipValues.xy, position)))*v.clipValues.z;
					
					return fixed4(0.0, 0.0, 0.0, occlusion*step(v.clipValue, 0.0));
				}
			ENDCG
		}
	}
}

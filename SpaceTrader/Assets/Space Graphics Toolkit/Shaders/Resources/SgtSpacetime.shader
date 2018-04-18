// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Hidden/SgtSpacetime"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_Tile("Tile", Float) = 1
		_Power("Power", Float) = 1
		_Effect("Effect", Vector) = (0, 0, 0)
		_Well1_Pos("Well 1 Pos", Vector) = (0, 0, 0)
		_Well1_Dat("Well 1 Dat", Vector) = (0, 0, 0, 0)
		_Well2_Pos("Well 2 Pos", Vector) = (0, 0, 0)
		_Well2_Dat("Well 2 Dat", Vector) = (0, 0, 0, 0)
		_Well3_Pos("Well 3 Pos", Vector) = (0, 0, 0)
		_Well3_Dat("Well 3 Dat", Vector) = (0, 0, 0, 0)
		_Well4_Pos("Well 4 Pos", Vector) = (0, 0, 0)
		_Well4_Dat("Well 4 Dat", Vector) = (0, 0, 0, 0)
		_Well5_Pos("Well 5 Pos", Vector) = (0, 0, 0)
		_Well5_Dat("Well 5 Dat", Vector) = (0, 0, 0, 0)
		_Well6_Pos("Well 6 Pos", Vector) = (0, 0, 0)
		_Well6_Dat("Well 6 Dat", Vector) = (0, 0, 0, 0)
		_Well7_Pos("Well 7 Pos", Vector) = (0, 0, 0)
		_Well7_Dat("Well 7 Dat", Vector) = (0, 0, 0, 0)
		_Well8_Pos("Well 8 Pos", Vector) = (0, 0, 0)
		_Well8_Dat("Well 8 Dat", Vector) = (0, 0, 0, 0)
		_Well9_Pos("Well 9 Pos", Vector) = (0, 0, 0)
		_Well9_Dat("Well 9 Dat", Vector) = (0, 0, 0, 0)
		_Well10_Pos("Well 10 Pos", Vector) = (0, 0, 0)
		_Well10_Dat("Well 10 Dat", Vector) = (0, 0, 0, 0)
		_Well11_Pos("Well 11 Pos", Vector) = (0, 0, 0)
		_Well11_Dat("Well 11 Dat", Vector) = (0, 0, 0, 0)
		_Well12_Pos("Well 12 Pos", Vector) = (0, 0, 0)
		_Well12_Dat("Well 12 Dat", Vector) = (0, 0, 0, 0)
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
				// Pinch, Offset
				#pragma multi_compile DUMMY SGT_A
				// Accumulate
				#pragma multi_compile DUMMY SGT_B
				// Well count + 1
				#pragma multi_compile DUMMY SGT_C
				// Well count + 2
				#pragma multi_compile DUMMY SGT_D
				// Well count + 4
				#pragma multi_compile DUMMY SGT_E
				// Well count + 8
				#pragma multi_compile DUMMY LIGHT_0
				
				#define WELL_COUNT (SGT_C * 1 + SGT_D * 2 + SGT_E * 4 + LIGHT_0 * 8)
				
				sampler2D _MainTex;
				float4    _Color;
				float     _Tile;
				float     _Power;
				float3    _Offset;
				float3    _Well1_Pos; float4 _Well1_Dat; // x = radius, y = age, z = strength
				float3    _Well2_Pos; float4 _Well2_Dat;
				float4    _Well3_Pos; float4 _Well3_Dat;
				float4    _Well4_Pos; float4 _Well4_Dat;
				float4    _Well5_Pos; float4 _Well5_Dat;
				float4    _Well6_Pos; float4 _Well6_Dat;
				float4    _Well7_Pos; float4 _Well7_Dat;
				float4    _Well8_Pos; float4 _Well8_Dat;
				float4    _Well9_Pos; float4 _Well9_Dat;
				float4    _Well10_Pos; float4 _Well10_Dat;
				float4    _Well11_Pos; float4 _Well11_Dat;
				float4    _Well12_Pos; float4 _Well12_Dat;
				
				struct a2v
				{
					float4 vertex    : POSITION;
					float4 color     : COLOR;
					float2 texcoord0 : TEXCOORD0;
				};
				
				struct v2f
				{
					float4 vertex    : SV_POSITION;
					float4 color     : COLOR;
					float2 texcoord0 : TEXCOORD0;
				};
				
				struct f2g
				{
					float4 color : COLOR;
				};
				
				void UpdateWell(inout float4 modifiedWPos, inout float4 originalWPos, float3 wellPosition, float4 wellData)
				{
#if SGT_B
					float3 vec = wellPosition.xyz - originalWPos.xyz;
#else
					float3 vec = wellPosition.xyz - modifiedWPos.xyz;
#endif
					float mag = saturate(length(vec) / wellData.x);
#if SGT_A
					modifiedWPos.xyz += _Offset * smoothstep(1.0f, 0.0f, mag) * wellData.x * wellData.z;
#else 
					mag = 1.0f - pow(smoothstep(0.0f, 1.0f, (mag)), _Power);
					
					modifiedWPos.xyz += vec * mag * wellData.z;
#endif
				}
				
				void Vert(a2v i, out v2f o)
				{
					float4 originalWPos = mul(unity_ObjectToWorld, i.vertex);
					float4 modifiedWPos = originalWPos;
#if WELL_COUNT >= 1
					UpdateWell(modifiedWPos, originalWPos, _Well1_Pos, _Well1_Dat);
#endif
#if WELL_COUNT >= 2
					UpdateWell(modifiedWPos, originalWPos, _Well2_Pos, _Well2_Dat);
#endif
#if WELL_COUNT >= 3
					UpdateWell(modifiedWPos, originalWPos, _Well3_Pos, _Well3_Dat);
#endif
#if WELL_COUNT >= 4
					UpdateWell(modifiedWPos, originalWPos, _Well4_Pos, _Well4_Dat);
#endif
#if WELL_COUNT >= 5
					UpdateWell(modifiedWPos, originalWPos, _Well5_Pos, _Well5_Dat);
#endif
#if WELL_COUNT >= 6
					UpdateWell(modifiedWPos, originalWPos, _Well6_Pos, _Well6_Dat);
#endif
#if WELL_COUNT >= 7
					UpdateWell(modifiedWPos, originalWPos, _Well7_Pos, _Well7_Dat);
#endif

#if WELL_COUNT >= 8
					UpdateWell(modifiedWPos, originalWPos, _Well8_Pos, _Well8_Dat);
#endif
#if WELL_COUNT >= 9
					UpdateWell(modifiedWPos, originalWPos, _Well9_Pos, _Well9_Dat);
#endif
#if WELL_COUNT >= 10
					UpdateWell(modifiedWPos, originalWPos, _Well10_Pos, _Well10_Dat);
#endif
#if WELL_COUNT >= 11
					UpdateWell(modifiedWPos, originalWPos, _Well11_Pos, _Well11_Dat);
#endif
#if WELL_COUNT >= 12
					UpdateWell(modifiedWPos, originalWPos, _Well12_Pos, _Well12_Dat);
#endif
					o.vertex    = mul(UNITY_MATRIX_VP, modifiedWPos);
					o.color     = i.color * _Color;
					o.texcoord0 = i.texcoord0 * _Tile;
				}
				
				void Frag(v2f i, out f2g o)
				{
					o.color = tex2D(_MainTex, i.texcoord0) * i.color;
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader
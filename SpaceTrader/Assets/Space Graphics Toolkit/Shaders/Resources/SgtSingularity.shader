// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Hidden/SgtSingularity"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_Center("Center", Vector) = (0, 0, 0)
		_Power("Power", Float) = 10
		_EdgePower("Edge Power", Float) = 10
		_HoleSize("Hole Size", Float) = 1
		_HolePower("Hole Power", Float) = 1
	}
	SubShader
	{
		Tags
		{
			"Queue"           = "Transparent"
			"RenderType"      = "Opaque"
			"IgnoreProjector" = "True"
		}
		GrabPass
		{
			Name "BASE"
			Tags { "LightMode" = "Always" }
 		}
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Front
			Lighting Off
			ZWrite Off
			Name "BASE"
			Tags { "LightMode" = "Always" }
			
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag
				// Hole
				#pragma multi_compile DUMMY SGT_A
				
				sampler2D _GrabTexture;
				float4    _Color;
				float4    _Center;
				float     _Power;
				float     _EdgePower;
				float     _HoleSize;
				float     _HolePower;
				
				struct a2v
				{
					float4 vertex    : POSITION;
					float3 normal    : NORMAL; // corners
					float2 texcoord0 : TEXCOORD0; // uv
				};
				
				struct v2f
				{
					float4 vertex    : SV_POSITION;
					float2 texcoord0 : TEXCOORD0;
					float2 texcoord1 : TEXCOORD1;
					float  texcoord2 : TEXCOORD2;
					float  texcoord3 : TEXCOORD3;
				};
				
				struct f2g
				{
					fixed4 color : COLOR;
				};
				
				void Vert(a2v i, out v2f o)
				{
					float4 vertexMVP = UnityObjectToClipPos(i.vertex);
					float4 centerMVP = mul(UNITY_MATRIX_VP, _Center);
					float4 vertM     = mul(unity_ObjectToWorld, i.vertex);
					float3 cam2vertM = normalize(_WorldSpaceCameraPos - vertM.xyz);
					float3 normalM   = normalize(mul((float3x3)unity_ObjectToWorld, i.normal));
					float  rim       = abs(dot(cam2vertM, normalM));
#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
#else
					float scale = 1.0;
#endif
					o.vertex    = vertexMVP;
					o.texcoord0 = (float2(vertexMVP.x, vertexMVP.y*scale) + vertexMVP.w) * 0.5 / vertexMVP.w;
					o.texcoord1 = (float2(centerMVP.x, centerMVP.y*scale) + centerMVP.w) * 0.5 / centerMVP.w;
					o.texcoord2 = pow(rim, _Power);
					o.texcoord3 = rim;
				}
				
				void Frag(v2f i, out f2g o)
				{
					float2 coord = lerp(i.texcoord0, i.texcoord1, i.texcoord2);
					
					float2 shift = abs(coord.xy - 0.5f) * 2.0f;
					float warp = saturate(max(shift.x, shift.y));
					
					warp = 1.0f - pow(warp, _EdgePower);
					
					o.color = tex2D(_GrabTexture, coord);
					o.color.a = warp;
					
					o.color.xyz = lerp(o.color.xyz, o.color.xyz * _Color.xyz, i.texcoord2);
#if SGT_A
					o.color.xyz *= 1.0f - pow(saturate(i.texcoord3 + _HoleSize), _HolePower);
#endif
				}
			ENDCG
		} // Pass
	} // SubShader
	SubShader {
		Blend DstColor Zero
		Pass {
			Name "BASE"
			SetTexture [_MainTex] {	combine texture }
		}
	}
} // Shader
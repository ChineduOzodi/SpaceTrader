// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SgtTerrainDebug"
{
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
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag
				
				struct a2v
				{
					float4 vertex    : POSITION;
					float2 texcoord0 : TEXCOORD0;
					float2 texcoord1 : TEXCOORD1;
				};
				
				struct v2f
				{
					float4 vertex    : SV_POSITION;
					float2 texcoord0 : TEXCOORD0;
					float2 texcoord1 : TEXCOORD1;
				};
				
				struct f2g
				{
					float4 color : COLOR;
				};
				
				void Vert(a2v i, out v2f o)
				{
					o.vertex    = UnityObjectToClipPos(i.vertex);
					o.texcoord0 = i.texcoord0;
					o.texcoord1 = i.texcoord1;
				}
				
				void Frag(v2f i, out f2g o)
				{
					float2 a = abs(i.texcoord0 - 0.5f) * 2.0f;
					float2 b = abs(i.texcoord1 - 0.5f) * 2.0f;
					
					o.color.x = max(a.x, a.y);
					o.color.y = max(b.x, b.y);
					o.color.zw = 1.0f;
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/SgtStaticStarfield"
{
	Properties
	{
		_Texture("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
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
				// Pulse (avoid using SGT_F)
				#pragma multi_compile DUMMY LIGHT_1

				sampler2D _Texture;
				float4    _Color;

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

				void Vert(a2v i, out v2f o)
				{
					o.vertex    = UnityObjectToClipPos(i.vertex);
					o.color     = i.color * _Color;
					o.texcoord0 = i.texcoord0;
				}

				void Frag(v2f i, out f2g o)
				{
					o.color = tex2D(_Texture, i.texcoord0) * i.color;
					o.color.a = saturate(o.color.a);
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader

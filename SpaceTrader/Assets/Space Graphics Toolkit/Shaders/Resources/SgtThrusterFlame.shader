// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/SgtThrusterFlame"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags
		{
			"Queue"             = "Transparent"
			"RenderType"        = "Transparent"
			"PreviewType"       = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Pass
		{
			Blend One One
			Cull Off
			ZWrite Off

			CGPROGRAM
				#pragma vertex   Vert
				#pragma fragment Frag

				sampler2D _MainTex;
				fixed4    _Color;

				struct a2v
				{
					float4 vertex   : POSITION;
					float2 texcoord : TEXCOORD0;
					float4 color    : COLOR;
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					float2 texcoord : TEXCOORD0;
					float4 color    : COLOR;
				};

				struct f2g
				{
					float4 color : COLOR;
				};

				void Vert(a2v i, out v2f o)
				{
					o.vertex   = UnityObjectToClipPos(i.vertex);
					o.texcoord = i.texcoord;
					o.color    = i.color * _Color;
				}

				void Frag(v2f i, out f2g o)
				{
					o.color = tex2D(_MainTex, i.texcoord) * i.color;
					o.color.a = saturate(o.color.a);
				}
		ENDCG
		} // Pass
	} // SubShader
} // Shader

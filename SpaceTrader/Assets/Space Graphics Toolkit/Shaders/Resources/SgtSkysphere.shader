// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/SgtSkysphere"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
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
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Front
			Lighting Off
			ZWrite Off
			
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag
				
				sampler2D _MainTex;
				float4    _Color;
				
				struct a2v
				{
					float4 vertex    : POSITION;
					float2 texcoord0 : TEXCOORD0;
				};
				
				struct v2f
				{
					float4 vertex    : SV_POSITION;
					float2 texcoord0 : TEXCOORD0;
				};
				
				struct f2g
				{
					float4 color : COLOR;
				};
				
				void Vert(a2v i, out v2f o)
				{
					o.vertex    = UnityObjectToClipPos(i.vertex);
					o.texcoord0 = i.texcoord0;
				}
				
				void Frag(v2f i, out f2g o)
				{
					o.color = tex2D(_MainTex, i.texcoord0) * _Color;
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Hidden/SgtWarpStarfield"
{
	Properties
	{
		_Texture("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_Width("Width", Float) = 1
		_Length("Length", Float) = 1
		_Scroll("Scroll", Float) = 1
		_Near("Near", Float) = 1
		_Range("Range", Float) = 1
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

				sampler2D _Texture;
				float4    _Color;
				float     _Width;
				float     _Length;
				float     _Scroll;
				float     _Near;
				float     _Range;

				struct a2v
				{
					float4 vertex    : POSITION;
					float4 color     : COLOR;
					float2 normal    : NORMAL;
					float2 texcoord0 : TEXCOORD0;
					float2 texcoord1 : TEXCOORD1;
					float2 texcoord2 : TEXCOORD3;
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
					float4 wPos = mul(unity_ObjectToWorld, i.vertex);

					wPos.z += _Scroll;

					wPos.z = (wPos.z - _Range * 0.5f) / _Range;
					wPos.z = frac(wPos.z);

					float alpha = 1.0f - wPos.z;

					wPos.z = wPos.z * _Range - _Range * 0.5f;

					wPos.xy += i.texcoord2.y * i.texcoord1.xy * _Width;
					wPos.z  += i.texcoord2.x * _Length;

					o.vertex    = mul(UNITY_MATRIX_VP, wPos);
					o.color     = alpha * _Color;
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

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Solar/BackgroundEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BackGroundTex ("Texture", 2D) = "black" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _BackGroundTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 backCol = tex2D(_BackGroundTex, float2((_WorldSpaceCameraPos.x / 1000 + i.uv.x * .5 + .25), (_WorldSpaceCameraPos.y / 1000 + i.uv.y * .5 + .25)));
				return col + (backCol * (1 - col));
			}
			ENDCG
		}
	}
}

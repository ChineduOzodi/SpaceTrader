// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Solar/StarBrightnessEffect"
{
	Properties
	{
		_MainTex ("Particle Texture", 2D) = "white" {}
		_TintColor ("Tint Color", Color) = (1,1,1,1)
		_StarRadius ("Star Radius", float) = 1
		_CutoffLight ("Cutoff Light", float) = .01
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend One OneMinusSrcColor
		ColorMask RGB
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
				fixed4 col: COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				fixed4 color: COLOR;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.color = v.col;
				return o;
			}

			sampler2D _MainTex;
			float _StarRadius;
			float4 _TintColor;
			float _CutoffLight;

			fixed4 frag (v2f i) : SV_Target
			{
				float texPosX = i.uv.x;
				float texPosY = i.uv.y;
				float4 starColor = float4(0,0,0,0);

				float realDistance = sqrt( pow((texPosX - .5) * 2, 2)
						+ pow((texPosY - .5) * 2, 2));

				starColor += _TintColor * i.color * _StarRadius / (pow((realDistance + .001),1.5));
				clip(starColor[3] - _CutoffLight);
				//float4 randomCol = float4(1,1,0,1);
				//return col + randomCol;
				return starColor;
			}
			ENDCG
		}
	}
}

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Solar/StarTemperatureEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_StarPosition ("Star Position", Vector) = (0,0,0,0)
		_StarTemperature ("Star Temperature", float) = 0.0
		_StarLum ("Star Luminosity", float) = 0.0
		_BondAlebo ("Bond Alebo", float) = 0.0
		_Greenhouse ("Greenhouse Effect", float) = 0.0
		_DistanceMod ("Distance Modifier", float) = 0.0
		_HotColor ("Too Hot Color", color) = (1,0,0,1)
		_SafeColor ("Safe Temp Color", color) = (0,1,0,1)
		_ColdColor ("Too Cold Color", color) = (0,0,1,1)
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
			float4 _StarPosition;
			float _StarTemperature;
			float _StarLum;
			float _BondAlebo;
			float _Greenhouse;
			float _DistanceMod;
			float4 _HotColor;
			float4 _SafeColor;
			float4 _ColdColor;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float distance = sqrt(
				pow((_StarPosition.x - (_WorldSpaceCameraPos.x + ((i.uv.x - .5) * unity_OrthoParams.x * 2))), 2)
				+ pow((_StarPosition.y - (_WorldSpaceCameraPos.y + ((i.uv.y - .5) * unity_OrthoParams.y * 2))), 2));

				float temp = pow(((1 - _BondAlebo) * _StarLum) / ((16 * 3.14159 * 5.6705e-8) * pow(distance * _DistanceMod, 2)), .25) * pow((1 + .438 * _Greenhouse * .9), .25) - 273.15;
				if (temp > 120){
					return col + (_HotColor * (1 - col));
				}
				else if (temp > -50){
					return col + (_SafeColor * (1 - col));
				}
				else{
					return col + (_ColdColor * (1 - col));
				}				
			}
			ENDCG
		}
	}
}

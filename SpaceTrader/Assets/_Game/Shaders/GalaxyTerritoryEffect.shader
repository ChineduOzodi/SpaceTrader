// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Solar/GalaxyTerritoryEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_StarPositionLength ("Star Position Lenght", int) = 1
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
			int _StarPositionLength;
			uniform float2 _StarPositionArray[1000];
			uniform float4 _StarColorArray[1000];
			uniform float _StarRadi[1000];

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float texPosX = i.uv.x;
				float texPosY = i.uv.y;
				float4 starColor = float4(0,0,0,0);
				for (int i = 0; i < _StarPositionLength; i++)
				{
					float2 _StarPosition = _StarPositionArray[i];
					float4 _StarColor = _StarColorArray[i];
					float _StarRadius = _StarRadi[i];
					float distance = sqrt(
						pow((_StarPosition.x - (_WorldSpaceCameraPos.x + ((texPosX - .5) * unity_OrthoParams.x * 2))), 2)
						+ pow((_StarPosition.y - (_WorldSpaceCameraPos.y + ((texPosY - .5) * unity_OrthoParams.y * 2))), 2));

					starColor += _StarColor * pow(_StarRadius, .1) / (pow((distance + .001),1.5) * 5);
				}
				return col + (starColor * (1 - col));
			}
			ENDCG
		}
	}
}

Shader "SgtEarthSurface"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Illum ("Illumin (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_SpecMap ("Specmap (A)", 2D) = "white" {}
		_EmissionLM ("Emission (Lightmapper)", Float) = 0
		_Divide ("Divide", Range(-1,1)) = 1
		_Sharpness ("Sharpness", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 400
		CGPROGRAM
			#pragma surface surf BlinnPhong2
			
			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _SpecMap;
			sampler2D _Illum;
			fixed4    _Color;
			half      _Divide;
			half      _Sharpness;
			half      _Shininess;
			
			struct Input
			{
				float2 uv_MainTex;
			};
			
			inline fixed4 LightingBlinnPhong2 (inout SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				fixed dif = dot (s.Normal, lightDir);
				fixed diff = max (0, dif);
				
				// Alter the emission based on lighting
				s.Emission *= saturate(1.0f - (dif - _Divide) * _Sharpness);
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0) * s.Gloss;
				
				fixed4 c;
				c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2);
				c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;
				return c;
			}
			
			// Copied from Illum-BumpSpec
			void surf (Input IN, inout SurfaceOutput o)
			{
				fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 spec = tex2D(_SpecMap, IN.uv_MainTex);
				fixed4 c = tex * _Color;
				
				o.Albedo = c.rgb;
				o.Emission = tex2D(_Illum, IN.uv_MainTex).rgb;
				o.Gloss = spec.a;
				o.Alpha = c.a;
				o.Specular = _Shininess;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			}
		ENDCG
	}
	FallBack "Self-Illumin/Specular"
}
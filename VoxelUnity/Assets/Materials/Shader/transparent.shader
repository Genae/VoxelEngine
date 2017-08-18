Shader "OwnShaders/transparent" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		// Render into depth buffer only
		Pass{
			ColorMask 0
		}

		// Render normally with depth buffer off
		ZWrite Off

		CGPROGRAM
		#pragma surface surf Lambert alpha fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.xyz;
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
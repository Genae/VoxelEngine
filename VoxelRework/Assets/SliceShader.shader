Shader "Example/Slices" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_BumpMap("Bumpmap", 2D) = "bump" {}
		_Slice("Slice", Float) = 0.0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		Cull Off
		CGPROGRAM
#pragma surface surf Lambert
		struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
		float3 worldPos;
	};
	sampler2D _MainTex;
	sampler2D _BumpMap;
	float _Slice;
	void surf(Input IN, inout SurfaceOutput o) {
		clip(_Slice - IN.worldPos.y);
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	}
	ENDCG
	}
		Fallback "Diffuse"
}
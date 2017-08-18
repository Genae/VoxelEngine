Shader "OwnShaders/windy" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		//_Glossiness("Smoothness", Range(0,1)) = 0.5
		//_Metallic("Metallic", Range(0,1)) = 0.0
		_Intensity("WindIntensity", Range(0,1)) = 0.2
		_Speed("WaveSpeed", Range(0,300)) = 50.0
		_WindDir("WindDirection", Vector) = (0,0,0)
	}
	SubShader {
		Tags {"Queue" = "Transparent" "RenderingMode" = "Opaque" } //fixes shadow artifacts queue = transparent renders objects after geometry -> what we need
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert fullforwardshadows
		#pragma vertex vert
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		struct appdata {
			float4 vertex : POSITION;
			float4 color : COLOR; 
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _Intensity;
		half _Speed;
		float4 _WindDir;

		void vert(inout appdata data) {
			float4 worldPosition = mul(unity_ObjectToWorld, data.vertex);
			data.vertex.x += sin((_Time * _Speed) + worldPosition.x) * data.color.r  * _Intensity * _WindDir.x;
			data.vertex.z += sin((_Time * _Speed) + worldPosition.z) * data.color.r  * _Intensity * _WindDir.z;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.xyz;
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

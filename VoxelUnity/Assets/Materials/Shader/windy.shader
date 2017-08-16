Shader "OwnShaders/windy" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Intensity("WindIntensity", Range(0,1)) = 0.2
		_Speed("WaveSpeed", Range(0,300)) = 50.0
		_WindDir("WindDirection", Vector) = (0,0,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard //fullforwardshadows
		#pragma vertex vert
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		struct appdata {
			float4 vertex : POSITION;
			float4 color : COLOR; //vertex color
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _Intensity;
		half _Speed;
		float4 _WindDir;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END


		void vert(inout appdata data) {
			float4 worldPosition = mul(unity_ObjectToWorld, data.vertex);
			float worldOffset = (worldPosition.x + worldPosition.z);
			float windvalue = sin((_Time * _Speed) + worldOffset) * data.color.r  * _Intensity;
			data.vertex.x += windvalue * _WindDir.x;
			data.vertex.z += windvalue * _WindDir.z;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.xyz;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

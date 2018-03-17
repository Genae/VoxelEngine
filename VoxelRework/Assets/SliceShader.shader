Shader "Own/T1"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Slice("Slice", Float) = 10000.0
	}

	SubShader
	{
		Tags{"Queue" = "Transparent" "RenderType" = "Transparent"}
		LOD 200

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float2 uv3 : TEXCOORD2;
			};

			struct v2f {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float2 uv3 : TEXCOORD2;
				float4 worldSpacePos : TEXCOORD3;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Slice;

			v2f vert(appdata vertex) 
			{
				v2f output;
				output.position = UnityObjectToClipPos(vertex.position);
				output.worldSpacePos = mul(unity_ObjectToWorld, vertex.position);
				output.uv = TRANSFORM_TEX(vertex.uv, _MainTex); 
				output.uv2 = TRANSFORM_TEX(vertex.uv2, _MainTex);
				output.uv3 = TRANSFORM_TEX(vertex.uv3, _MainTex);
				return output;
			}

			fixed4 frag(v2f input) : SV_TARGET
			{
				fixed4 col = tex2D(_MainTex, float2(frac(((((input.uv.x) * input.uv3.x) % 1) + input.uv2.x) / 8), frac(((((input.uv.y) * input.uv3.y) % 1) + input.uv2.y) / 8)));
				//fixed4 col = tex2D(_MainTex, input.uv);
				//clip(_Slice - input.position.y);
				if(_Slice < input.worldSpacePos.y){
					col.a = 0;
				}
				
				return col;
			}
			ENDCG
		}
	}
}





























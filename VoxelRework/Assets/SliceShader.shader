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
			};

			struct v2f {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 worldSpacePos : TEXCOORD1;
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
				return output;
			}

			fixed4 frag(v2f input) : SV_TARGET
			{
				
				fixed4 col = tex2D(_MainTex, input.uv);
				
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





























// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/FilmGrain"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		_SceneTex("Scene Texture", 2D) = "black"{}
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

			sampler2D _SceneTex;
			float4 _SceneTex_TexelSize;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				//if (_SceneTex_TexelSize.y < 0)
					o.uv.y = 1.0f - o.uv.y;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_SceneTex, i.uv);
				float strength = 7.0f;
				float x = (i.uv.x + 4.0f) * (i.uv.y + 4.0f) * (_Time * 10.0f);
				half g = half(fmod((fmod(x, 13.0f) + 1.0f) * (fmod(x, 123.0f) + 1.0f), 0.01f) - 0.005f) * strength;

				return col + g;
			}
			ENDCG
		}
	}
}

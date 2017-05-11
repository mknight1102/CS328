// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Blood/GravityMap"
{
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		// No culling or depth
		//Cull Off ZWrite Off ZTest Always

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
				float4 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct v2f
			{
				float4 color : COLOR;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				float4 gravVector = mul(unity_ObjectToWorld, v.tangent);
				float4 dirVector;
				dirVector.x = gravVector.x + v.normal.x;
				dirVector.y = gravVector.y + v.normal.y;
				dirVector.x = .5f * dirVector.x + 0.5f;
				dirVector.y = .5f * dirVector.y + 0.5f;
				o.color.xy = dirVector.xy;
				o.color.w = 1.0f;
				return o;
			}
			
			fixed4 frag (v2f IN) : SV_Target
			{
				return IN.color;
			}

			ENDCG
		}
	}
}

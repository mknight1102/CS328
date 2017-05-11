// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/BloodShader"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_GravityMapTex("Gravity Map", 2D) = "bump" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_BloodColor("Blood Color", Color) = (1, .1, .1, 1)
		//_BloodPoint("")
	}
	SubShader
	{
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
			float4 normal : NORMAL;
			float4 tangent : TANGENT;
		};

		struct v2f
		{
			float4 color : TEXCOORD1;
			float4 vertex : SV_POSITION;
		};

		v2f vert(appdata v)
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


		sampler2D _GravityMapTex;
		sampler2D _MainTex;
		float4 _BloodColor;
		float4 _GravityMapTex_TexelSize;

		fixed4 frag(v2f IN) : SV_Target
		{
			float blood = 0;
			float maxBlood = 5;

			fixed4 up = tex2D(_GravityMapTex, IN.color + fixed2(0, _GravityMapTex_TexelSize.y));
			fixed4 down = tex2D(_GravityMapTex, IN.color - fixed2(0, _GravityMapTex_TexelSize.y));
			fixed4 left = tex2D(_GravityMapTex, IN.color - fixed2(_GravityMapTex_TexelSize.x, 0));
			fixed4 right = tex2D(_GravityMapTex, IN.color + fixed2(_GravityMapTex_TexelSize.x, 0));

			// add blood if the other's gravity vector is pointing toward us
			if (up.y <= 0 && up.y >= -1) {
				blood += abs(up.y) * up.z;
				
			}
			if (down.y <= 1 && down.y >= 0) {
				blood += abs(down.y) * down.z;
			}
			if (left.x <= 0 && left.x >= -1) {
				blood += abs(left.x) * left.z;
			}
			if (right.x <= 1 && right.x >= 0) {
				blood += abs(right.x) * right.z;
			}
			blood += 1.0f;
			// re-adjust blood to max blood size
			if (blood > maxBlood)
				blood = maxBlood;

			fixed4 col = tex2D(_GravityMapTex, IN.color);
			col.z = blood;
			if (blood > 1)
				col = tex2D(_MainTex, _BloodColor);
			return col;
		}

			ENDCG
		}

		// Generate blood animation
		/*Pass
			{
				CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

				struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD1;
			};

			struct v2f
			{
				float4 uv : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _GravityMapTex;
			float4 _GravityMapTex_TexelSize;

			fixed4 frag(v2f IN) : SV_Target
			{
				// Init blood vars
				float blood = IN.uv.z;
				float maxBlood = 5;

				// find adjacent pixels
				fixed4 up = tex2D(_GravityMapTex, IN.uv + fixed2(0, _GravityMapTex_TexelSize.y));
				fixed4 down = tex2D(_GravityMapTex, IN.uv - fixed2(0, _GravityMapTex_TexelSize.y));
				fixed4 left = tex2D(_GravityMapTex, IN.uv - fixed2(_GravityMapTex_TexelSize.x, 0));
				fixed4 right = tex2D(_GravityMapTex, IN.uv + fixed2(_GravityMapTex_TexelSize.x, 0));

				// add blood if the other's gravity vector is pointing toward us
				if (up.y <= 0 && up.y >= -1) {
					blood += abs(up.y) * up.z;
					blood += 1.0f;
				}
				if (down.y <= 1 && down.y >= 0) {
					blood += abs(down.y) * down.z;
				}
				if (left.x <= 0 && left.x >= -1) {
					blood += abs(left.x) * left.z;
				}
				if (right.x <= 1 && right.x >= 0) {
					blood += abs(right.x) * right.z;
				}

				// re-adjust blood to max blood size
				if (blood > maxBlood)
					blood = maxBlood;

				fixed4 col = tex2D(_GravityMapTex, IN.uv);
				col.z = blood;

				return col;
			}
				ENDCG
		}

			// This pass Render's the character's blood
		Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

					struct appdata
				{
					float4 vertex : POSITION;
					float4 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 uv : TEXCOORD0;
					float4 map : TEXCOORD1;
					float4 vertex : SV_POSITION;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = v.uv;
					return o;
				}

				sampler2D _MainTex;

				fixed4 frag(v2f IN) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, IN.uv);
					
				if (IN.map.z < 0.0f)
					col = float4(1, 0, 0, 1);
				else
					col = float4(1, 1, 0, 1);

					return col;
				}
			ENDCG
		}*/
	}	
}

Shader "Unlit/AtmosphereFresnel"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TintColor("TintColor", Color) = (1, 1, 1, 1)
		_InnerPower("Inner Power", Float) = 1
		_EdgePower("Soft Edge Power", Float) = 2
		_InnerMultiplier("Inner Multiplier", Float) = 2
		_EdgeMultiplier("Soft Edge Multiplier", Float) = 2
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : POSITION;
				float4 localNormal: TEXCOORD3;
				float4 eye: TEXCOORD4;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColor;
			float1 _InnerPower;
			float1 _EdgePower;
			float1 _InnerMultiplier;
			float1 _EdgeMultiplier;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o, o.vertex);

				o.localNormal = mul(UNITY_MATRIX_MVP, float4(v.normal, 0));
				o.eye = normalize(-o.vertex);

				return o;
			}
						
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) * _TintColor;

				UNITY_APPLY_FOG(i.fogCoord, col);

				float dotToEye = dot(normalize(i.localNormal), normalize(i.eye));

				float innerBase = clamp(1 - (dotToEye * _InnerMultiplier), 0, 1);
				float inner = pow(innerBase, _InnerPower);

				float softEdgeBase = clamp(1 - (dotToEye * _EdgeMultiplier), 0, 1);
				float softEdge = pow(softEdgeBase, _EdgePower);

				float power = clamp(inner - softEdge, 0, 1);
								
				return fixed4(col.rgb, power * col.a);
				//return fixed4(power, power, power, 1);
				//return fixed4(inner, inner, inner, 1);
				//return fixed4(softEdge, softEdge, softEdge, 1);
			}
			ENDCG
		}
	}
}

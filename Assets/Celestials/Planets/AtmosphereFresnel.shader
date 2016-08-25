Shader "Unlit/AtmosphereFresnel"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}		
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

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
				//float3 normal : TEXCOORD2;
				float4 localNormal: TEXCOORD3;
				float4 eye: TEXCOORD4;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
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
				fixed4 col = tex2D(_MainTex, i.uv);

				UNITY_APPLY_FOG(i.fogCoord, col);

				float dotToEye = dot(normalize(i.localNormal), normalize(i.eye));

				float base = clamp(1 - dotToEye, 0, 1);

				float inner = pow(base, 1);
				float softEdge = pow(base * 1.1, 2);

				float power = (inner - softEdge);
				
				//return dotToEye > 1 ? fixed4(1, 0, 0, 1) : fixed4(0, 0, 0, 0);
				return fixed4(col.xyz, power);
				return fixed4(power, power, power, 1);
				return fixed4(softEdge, softEdge, softEdge, 1);
			}
			ENDCG
		}
	}
}

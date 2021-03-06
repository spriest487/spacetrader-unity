﻿Shader "Unlit/AtmosphereFresnel"{
    Properties{
        _Diffuse("Diffuse Texture (RGB)", 2D) = "white"{}
        _Wrapping("Light Wrapping", Range(0.0, 1.0)) = 0.5
        _Color("Fresnel Color (RGBA)", Color) = (1.0, 1.0, 1.0, 1.0)
        _Factor("Fresnel Factor", float) = 0.5
        _FPow("Fresnel Power", float) = 2.0
        _Soften("Soften Power", float) = 1.0
    }

    SubShader {
        Tags{ "RenderType" = "Transparent" "Queue"="Transparent" }

        Blend SrcAlpha One
        ZWrite Off
        ZTest LEqual
        
    CGPROGRAM
        #pragma surface surf AtmosphereFresnel approxview

        struct Input {
            half2 uv_Diffuse;
            half3 viewDir;
        };

        sampler2D _Diffuse;
        half4 _Color;
        half _Factor;
        half _FPow;
        half _Soften;
        fixed _Wrapping;

        half4 LightingAtmosphereFresnel(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
            half3 wrap = atten * 2.0 * dot(s.Normal, lightDir + (viewDir * _Wrapping));
            half diff = dot(s.Normal, lightDir) * 0.5 + 0.5;

            half4 c;
            c.rgb = wrap * s.Albedo * _LightColor0.rgb * diff;
            c.a = s.Alpha;
            return c;
        }

        void surf(Input IN, inout SurfaceOutput o) {
            half fresnel = _Factor * pow(1.0 - dot(normalize(IN.viewDir), o.Normal), _FPow);

            half soften = pow(fresnel, _Soften);

            fresnel -= soften;

            half3 diffuse = tex2D(_Diffuse, IN.uv_Diffuse).rgb;
        
            //o.Albedo = tex2D(_Diffuse, IN.uv_Diffuse).rgb * fresnel;
            //o.Albedo = half4(fresnel, fresnel, fresnel, 1);
            //o.Emission = lerp(o.Albedo, _Color.rgb, _Color.a) * fresnel;
            o.Albedo = _Color.rgb;
            o.Emission = lerp(diffuse * _Color.rgb, _Color.rgb, _Color.a) * fresnel;
        }

    ENDCG
    }
}
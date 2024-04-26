   Shader "Custom/StandardSpecularWithTransparentCircle" {
    Properties {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SpecularColor ("Specular Color", Color) = (0.2, 0.2, 0.2, 1)
        _Glossiness ("Smoothness", Range(0, 1)) = 0.5
        _Radius ("Radius", Range(0, 1)) = 0.5
        _MaskCenter ("Mask Center", Vector) = (0, 0, 0)
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Specular
        #pragma target 3.0
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        float _Glossiness;
        float3 _SpecularColor;
        float4 _Color;
        float _Radius;
        float3 _MaskCenter;

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Gloss = _Glossiness;
            o.Specular = _SpecularColor;
            o.Alpha = c.a;

            float dist = distance(_MaskCenter, IN.worldPos);
            float alpha = smoothstep(_Radius, _Radius + 0.01, dist);

            o.Alpha *= alpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

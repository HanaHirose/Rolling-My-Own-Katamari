Shader "Unlit/TrasparentCircleShader"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Radius", Range(0, 1)) = 0.5
        _Transparency ("Transparency", Range(0, 1)) = 0
        _WorldOrigin ("World Origin", Vector) = (0, 0, 0)
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
    
        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        float _Radius;
        float _Transparency;
        float3 _WorldOrigin;

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            half4 col = tex2D(_MainTex, IN.uv_MainTex);

            float distanceToOrigin = distance(_WorldOrigin, IN.worldPos);
            float circle = 1.0 - smoothstep(_Radius, _Radius + 0.01, distanceToOrigin);


            //塊が隠れている位置を赤くするテスト
            if(circle > 0){
                col.rbg = float3(1,0,0);
            }


            
            //col.a =_Transparency;

            o.Albedo = col.rgb;
            o.Alpha = col.a;
        }
        ENDCG

    }

    FallBack "Transparent/Diffuse"
}

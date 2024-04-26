Shader "Unlit/TrasparentCircleShader2"{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Radius", Range(0, 1)) = 0.5
        _MaskCenter ("Mask Center", Vector) = (0, 0, 0)
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        Pass{
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
            float3 worldPos : TEXCOORD1;
        };

        sampler2D _MainTex;
        float _Radius;
        float3 _MaskCenter;

        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            fixed4 col = tex2D(_MainTex, i.uv);

            float dist = distance(_MaskCenter, i.worldPos);
            float alpha = smoothstep(_Radius, _Radius + 0.01, dist);

            col.a *= alpha;

            return col;
        }
        ENDCG
        }
    }
    FallBack "Diffuse"
}


Shader "Custom/TransparentBlackWithMask"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _Stencil ("Stencil Reference", Float) = 1
        _StencilComp ("Stencil Comparison", Float) = 8.000000
        _StencilOp ("Stencil Operation", Float) = 0.000000
        _StencilWriteMask ("Stencil Write Mask", Float) = 255.000000
        _StencilReadMask ("Stencil Read Mask", Float) = 255.000000
        _ColorMask ("Color Mask", Float) = 15.000000
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        
        // 親のMaskの適応のため、テストを追加
        Stencil
        {
            Ref 1
            Comp Equal
        }
        
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

            sampler2D _MainTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                if(col.r < 0.1 && col.g < 0.1 && col.b < 0.1)
                    discard;
                return col;
            }
            ENDCG
        }
    }
}


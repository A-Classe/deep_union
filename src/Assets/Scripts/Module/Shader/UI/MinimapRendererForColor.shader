Shader "Custom/MinimapRenderer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HeightThreshold ("Height Threshold", Range(0,1)) = 0.5
        _LowColor ("Low Color", Color) = (1,0,0,1)
        _HighColor ("High Color", Color) = (0,0,1,1)
        _Stencil ("Stencil Reference", Float) = 1
        _StencilComp ("Stencil Comparison", Float) = 8.000000
        _StencilOp ("Stencil Operation", Float) = 0.000000
        _StencilWriteMask ("Stencil Write Mask", Float) = 255.000000
        _StencilReadMask ("Stencil Read Mask", Float) = 255.000000
        _ColorMask ("Color Mask", Float) = 15.000000
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
         // mask適応
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
            float4 _MainTex_ST;
            float _HeightThreshold;
            float4 _LowColor;
            float4 _HighColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                const float height = col.b; // Green channel as height
                
                // Change color based on height
                if (height < _HeightThreshold)
                {
                    return _LowColor;
                }
                return _HighColor;
            }
            ENDCG
        }
    }
}
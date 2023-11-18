Shader "Custom/SimpleMultipleFadingRipples"
{
    Properties
    {
        _RippleColor ("Ripple Color", Color) = (1,1,1,1)
        _CircleColor ("Circle Color", Color) = (0,1,0,1)
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
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

            float4 _RippleColor;
            float4 _CircleColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float calcRipple(float2 uv, float4 center, float startTime, float time)
            {
                const float rippleSpeed = 0.3;
                const float rippleWidth = 0.004;
                const float fadeOutStrength = 0.5;

                const float dist = distance(uv, center.xy);
                const float ripplePhase = (time - startTime) * rippleSpeed - dist;
                const float rippleEffect = smoothstep(rippleWidth, 0.0, abs(frac(ripplePhase) - 0.5));
                const float fadeOut = 1.0 - smoothstep(0.0, fadeOutStrength, dist);
                return rippleEffect * fadeOut;
            }

            float4 frag(v2f i) : SV_Target
            {
                const float time = _Time.y;

                // Define ripple centers and start times
                const float4 rippleCenters = float4(0.5, 0.5, 0, 0);
                const float startTimes[3] = {0, 0.3, 0.5};

                // Calculate ripples
                float rippleSum = 0;
                for (int j = 0; j < 3; j++)
                {
                    rippleSum += calcRipple(i.uv, rippleCenters, startTimes[j], time);
                }

                 // 円の計算
                const float circleRadius = 0.02; // 円の半径
                const float edgeSoftness = 0.01; // 円の端の柔らかさ
                float distToCenter = distance(i.uv, rippleCenters.xy);
                float circle = smoothstep(circleRadius, circleRadius - edgeSoftness, distToCenter);


                // 波紋と円の合成
                float4 rippleColor = _RippleColor;
                rippleColor.a = rippleSum;

                float4 circleColor = _CircleColor;
                circleColor.a *= circle; // 円の透明度を設定

                return lerp(rippleColor, circleColor, circle); // 円が存在する場合、円の色を使用
            }
            ENDCG
        }
    }
}
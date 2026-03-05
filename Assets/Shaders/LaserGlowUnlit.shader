Shader "Custom/LaserGlowUnlit"
{
    Properties
    {
        [HDR] _GlowColor("Glow Color", Color) = (1, 1, 1, 1)
        _GlowIntensity("Glow Intensity", Range(0, 20)) = 8
        _Alpha("Alpha", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode"="SRPDefaultUnlit" }

            Blend SrcAlpha One
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                half4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                half4 color : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _GlowColor;
                half _GlowIntensity;
                half _Alpha;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.color = input.color;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half3 glow = input.color.rgb * _GlowColor.rgb * _GlowIntensity;
                half alpha = input.color.a * _Alpha;
                return half4(glow * alpha, alpha);
            }
            ENDHLSL
        }
    }
}

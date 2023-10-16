Shader "Custom/combineRenderTextures"
{
    Properties
    {
        [MainColor] _BaseColor("BaseColor", Color) = (1,1,1,1)
        _SecondaryColor("SecondaryColor", Color) = (1,0,0,1)
        [MainTexture] _TopTexture("TopTexture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline"}

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float4 positionHCS  : SV_POSITION;
                
            };

            TEXTURE2D(_TopTexture);
            SAMPLER(sampler_TopTexture);
            
            CBUFFER_START(UnityPerMaterial)
            float4 _TopTexture_ST;
            half4 _BaseColor;
            half4 _SecondaryColor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _TopTexture);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 topUV = IN.uv;
                // Color the top half of the screen with baseColor
                if (IN.uv.y < 0.5)
                {
                    return SAMPLE_TEXTURE2D(_TopTexture, sampler_TopTexture, topUV) * _SecondaryColor;
                }
                else
                {
                    return SAMPLE_TEXTURE2D(_TopTexture, sampler_TopTexture, topUV) * _BaseColor;
                }
            }
            ENDHLSL
        }
    }
}
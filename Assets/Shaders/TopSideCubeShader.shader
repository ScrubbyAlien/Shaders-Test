Shader "Custom/TopSideCubeShader"
{
    Properties
    {
        _BaseColor("BaseColor", Color) = (1, 1, 1, 1)
        _MainTex("Top Texture", 2D) = "white" {}
        _SideTexture("Side Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            Tags
            {
                "LightMode" = "SRPDefaultUnlit"
            }

            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _MainTex_ST;
                float4 _SideTexture_ST;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            TEXTURE2D(_SideTexture);
            SAMPLER(sampler_MainTex);
            SAMPLER(sampler_SideTexture);

            struct VertexInput { // geometry vertex attributes: normal, color, uv, etc.
                // vertex position in local space
                // POSITION semantic tells cpu where to look for data
                float3 positionLocal : POSITION;
                float3 normalLocal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct VertexOutput { // transformed data
                float4 positionClip : SV_POSITION;
                float3 normalLocal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            VertexOutput Vertex(VertexInput input) {
                VertexOutput output = (VertexOutput)0;
                output.positionClip = TransformObjectToHClip(input.positionLocal);
                output.normalLocal = input.normalLocal;
                if (output.normalLocal.y > 0.5) {
                    output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                }
                else {
                    output.uv = TRANSFORM_TEX(input.uv, _SideTexture);
                }
                return output;
            }

            half4 Fragment(VertexOutput output) : SV_Target {
                float4 textureColor;
                if (output.normalLocal.y > 0.5) {
                    textureColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, output.uv);
                }
                else {
                    textureColor = SAMPLE_TEXTURE2D(_SideTexture, sampler_SideTexture, output.uv);
                }
                return textureColor * _BaseColor;
            }
            ENDHLSL
        }


    }


}
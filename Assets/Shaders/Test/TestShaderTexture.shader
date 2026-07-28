Shader "Custom/TestShaderTexture"
{
    Properties
    {
        _BaseColor("BaseColor", Color) = (1, 1, 1, 1)
        _BaseTexture("Base Texture", 2D) = "white" {}
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
                float4 _BaseTexture_ST;
            CBUFFER_END

            TEXTURE2D(_BaseTexture);
            SAMPLER(sampler_BaseTexture);

            struct VertexInput { // geometry vertex attributes: normal, color, uv, etc.
                // vertex position in local space
                // POSITION semantic tells cpu where to look for data
                float3 positionLocal : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VertexOutput { // transformed data
                float4 positionClip : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            VertexOutput Vertex(VertexInput input) {
                VertexOutput output = (VertexOutput)0;
                output.positionClip = TransformObjectToHClip(input.positionLocal);
                output.uv = TRANSFORM_TEX(input.uv, _BaseTexture);
                return output;
            }

            half4 Fragment(VertexOutput output) : SV_Target {
                float4 textureColor = SAMPLE_TEXTURE2D(_BaseTexture, sampler_BaseTexture, output.uv);
                return textureColor * _BaseColor;
            }
            ENDHLSL
        }

        Pass
        {
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            ZWrite On
            ColorMask R

            HLSLPROGRAM
            #pragma vertex depthOnlyVert
            #pragma fragment depthOnlyFrag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct VertexInput {
                float4 positionLocal : POSITION;
            };

            struct VertexOutput {
                float4 positionClip : SV_POSITION;
            };

            VertexOutput depthOnlyVert(VertexInput input) {
                VertexOutput output;
                output.positionClip = TransformObjectToHClip(input.positionLocal.xyz);
                return output;
            }

            float depthOnlyFrag(VertexOutput output) {
                return output.positionClip.z;
            }
            ENDHLSL
        }

        Pass
        {
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            ZWrite On

            HLSLPROGRAM
            #pragma vertex depthNormalsVertex
            #pragma fragment depthNormalsFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct VertexInput {
                float4 positionLocal : POSITION;
                float3 normalLocal : NORMAL;
            };

            struct VertexOutput {
                float4 positionClip : SV_POSITION;
                float3 normalWorld : TEXCOORD0;
            };

            VertexOutput depthNormalsVertex(VertexInput input) {
                VertexOutput output;
                output.positionClip = TransformObjectToHClip(input.positionLocal.xyz);
                float3 normalWorld = TransformObjectToWorldNormal(input.normalLocal);
                output.normalWorld = NormalizeNormalPerVertex(normalWorld);
                return output;
            }

            float depthNormalsFragment(VertexOutput output) {
                float3 normalWorld = NormalizeNormalPerPixel(output.normalWorld);
                return float4(normalWorld, .0);
            }
            ENDHLSL
        }

    }


}
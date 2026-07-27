Shader "Custom/TestShaderSilhouette"
{
    Properties
    {
        _ForegroundColor("Foreground Color", Color) = (1, 1, 1, 1)
        _BackgroundColor("Background Color", Color) = (0, 0, 0, 1)
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

            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _ForegroundColor;
                float4 _BackgroundColor;
            CBUFFER_END

            struct VertexInput { // geometry vertex attributes: normal, color, uv, etc.
                // vertex position in local space
                // POSITION semantic tells cpu where to look for data
                float4 positionLocal : POSITION;
            };

            struct VertexOutput { // transformed data
                float4 positionClip : SV_POSITION;
                float4 positionScreen : TEXCOORD0;
            };

            VertexOutput Vertex(VertexInput input) {
                VertexOutput output = (VertexOutput)0;
                output.positionClip = TransformObjectToHClip(input.positionLocal);
                output.positionScreen = ComputeScreenPos(input.positionLocal);
                return output;
            }

            half4 Fragment(VertexOutput output) : SV_Target {
                float2 screenUV = output.positionScreen.xy / output.positionScreen.w;
                float rawDepth = SampleSceneDepth(screenUV);

                return lerp(_ForegroundColor, _BackgroundColor, rawDepth);
            }
            ENDHLSL
        }
    }


}
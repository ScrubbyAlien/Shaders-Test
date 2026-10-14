Shader "Custom/TopSideCubeShaderBufferRipple"
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

            #pragma target 5.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "effects.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _MainTex_ST;
                float4 _SideTexture_ST;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            TEXTURE2D(_SideTexture);
            SAMPLER(sampler_MainTex);

            uniform float Amplitude;
            uniform float2 GridSize;
            uniform StructuredBuffer<float> GridBuffer;

            int LinearCoord(float2 cellCoord) {
                return cellCoord.y * GridSize.x + cellCoord.x;
            }

            float GetGridBufferElement(float2 cellCoord) {
                return GridBuffer[LinearCoord(cellCoord)];
            }

            struct VertexInput { // geometry vertex attributes: normal, color, uv, etc.
                // vertex position in local space
                // POSITION semantic tells cpu where to look for data
                float3 positionLocal : POSITION;
                float3 normalLocal : NORMAL;
                float2 uv : TEXCOORD0;
                float2 cellCoord : TEXCOORD1;
            };

            struct VertexOutput { // transformed data
                float4 positionClip : SV_POSITION;
                float3 normalLocal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            VertexOutput Vertex(VertexInput input) {
                VertexOutput output = (VertexOutput)0;

                float3 worldPos = TransformObjectToWorld(input.positionLocal);
                float gridState = GetGridBufferElement(input.cellCoord);
                float belowZero = step(0, gridState);
                float abovePI = step(gridState, PI);
                float modifier = Amplitude * sin(gridState) * belowZero * abovePI;
                float3 newWorldPos = float3(worldPos.x, worldPos.y + modifier, worldPos.z);

                output.positionClip = TransformWorldToHClip(newWorldPos);
                output.normalLocal = input.normalLocal;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            half4 Fragment(VertexOutput output) : SV_Target {
                float4 textureColor;
                if (output.normalLocal.y > 0.5) {
                    textureColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, output.uv);
                }
                else {
                    textureColor = SAMPLE_TEXTURE2D(_SideTexture, sampler_MainTex, output.uv);
                }
                return textureColor * _BaseColor;
            }
            ENDHLSL
        }


    }


}
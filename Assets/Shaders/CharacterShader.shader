Shader "Custom/CharacterShader"
{
    Properties
    {
        _BaseColor("BaseColor", Color) = (1, 1, 1, 1)
        _BaseTexture("Base Texture", 2DArray) = "white" {}
        _Index("Index", Float) = 0
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
                float _Index;
            CBUFFER_END

            TEXTURE2D_ARRAY(_BaseTexture);
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

                // https://gist.github.com/kaiware007/8ebad2d28638ff83b6b74970a4f70c9a 
                // billboard mesh towards camera
				float3 vpos = mul((float3x3)unity_ObjectToWorld, input.positionLocal.xyz);
				float4 worldCoord = float4(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, unity_ObjectToWorld._m23, 1);
				float4 viewPos = mul(UNITY_MATRIX_V, worldCoord) + float4(vpos, 0);
				float4 outPos = mul(UNITY_MATRIX_P, viewPos);
                
                output.uv = TRANSFORM_TEX(input.uv, _BaseTexture);
                return output;
            }

            half4 Fragment(VertexOutput output) : SV_Target {
                float4 textureColor = SAMPLE_TEXTURE2D_ARRAY(_BaseTexture, sampler_BaseTexture, output.uv, _Index);
                if (textureColor.a == 0) discard;
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

            float depthOnlyFrag(VertexOutput output) : SV_Target {
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

            float depthNormalsFragment(VertexOutput output) : SV_Target {
                float3 normalWorld = NormalizeNormalPerPixel(output.normalWorld);
                return float4(normalWorld, .0);
            }
            ENDHLSL
        }

    }


}
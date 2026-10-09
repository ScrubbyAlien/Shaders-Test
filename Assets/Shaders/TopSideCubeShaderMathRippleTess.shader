Shader "Custom/TopSideCubeShaderMathRippleTess"
{
    Properties
    {
        _BaseColor("BaseColor", Color) = (1, 1, 1, 1)
        [MainTexture]
        _MainTexture("Top Texture", 2D) = "white" {}
        _SideTexture("Side Texture", 2D) = "white" {}

        [Header(Ripple Settings)] [Space]
        _Amplitude("Amplitude", Float) = 0.5
        _Frequency("Frequency", Float) = 1
        _Range("Range", Float) = 1
        _PropagationSpeed("Propagation Speed", Float) = 1

        [Space]
        [Header(Tessallation Settings)] [Space]
        _TessellationAmount("Tessellation Amount", Range(1, 64)) = 1
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
            #pragma hull Hull
            #pragma domain Domain

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "effects.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _MainTexture_ST;
                float4 _SideTexture_ST;
                float _Range;
                float _PropagationSpeed;
                float _Amplitude;
                float _Frequency;
                float _TessellationAmount;
            CBUFFER_END

            uniform float4 Origin;
            uniform float StartTime;

            TEXTURE2D(_MainTexture);
            TEXTURE2D(_SideTexture);
            SAMPLER(sampler_MainTexture);

            struct VertexInput { // geometry vertex attributes: normal, color, uv, etc.
                // vertex position in local space
                // POSITION semantic tells cpu where to look for data
                float3 positionLocal : POSITION;
                float3 normalLocal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct TessControlPoint {
                float3 positionWorld : INTERNALTESSPOS;
                float3 normalLocal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct TessFactors {
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
            };

            struct TessellationOutput { // transformed data
                float4 positionClip : SV_POSITION;
                float3 normalLocal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            TessControlPoint Vertex(VertexInput input) {
                TessControlPoint control_point = (TessControlPoint)0;

                control_point.positionWorld = TransformObjectToWorld(input.positionLocal.xyz);
                control_point.normalLocal = input.normalLocal;
                control_point.uv = TRANSFORM_TEX(input.uv, _MainTexture);

                return control_point;
            }

            [domain("tri")]
            [outputcontrolpoints(3)]
            [outputtopology("triangle_cw")]
            [partitioning("integer")]
            [patchconstantfunc("PatchConstantFunc")]
            TessControlPoint Hull(InputPatch<TessControlPoint, 3> patch, uint id : SV_OutputControlPointID) {
                return patch[id];
            }

            TessFactors PatchConstantFunc(InputPatch<TessControlPoint, 3> patch) {
                TessFactors factors = (TessFactors)0;

                // float3 triPos0 = patch[0].positionWorld;
                // float3 triPos1 = patch[1].positionWorld;
                // float3 triPos2 = patch[2].positionWorld;
                //
                // // calculate halfway points along edges
                // float3 edgePos0 = 0.5f * (triPos1 + triPos2);
                // float3 edgePos1 = 0.5f * (triPos0 + triPos2);
                // float3 edgePos2 = 0.5f * (triPos0 + triPos1);
                //
                // float3 camPos = _WorldSpaceCameraPos;

                // float dist0 = distance(edgePos0, camPos);
                // float dist1 = distance(edgePos1, camPos);
                // float dist2 = distance(edgePos2, camPos);
                //
                // float fadeDist = _TessellationFadeEnd - _TessellationFadeStart;
                //
                // // clamp tesselation factor between 1 and 0 inside tesselation fade range
                // float edgeFactor0 = saturate(1.0f - (dist0 - _TessellationFadeStart) / fadeDist);
                // float edgeFactor1 = saturate(1.0f - (dist1 - _TessellationFadeStart) / fadeDist);
                // float edgeFactor2 = saturate(1.0f - (dist2 - _TessellationFadeStart) / fadeDist);

                // ensure tesselation factor is at least 1, otherwise trianlge will disappear
                float effectEndTime = StartTime + _Range / _PropagationSpeed;
                float inProgress = step(_Time.y, effectEndTime);

                // ensure tesselation factor is at least 1, otherwise trianlge will disappear
                factors.edge[0] = max(inProgress * _TessellationAmount, 1);
                factors.edge[1] = max(inProgress * _TessellationAmount, 1);
                factors.edge[2] = max(inProgress * _TessellationAmount, 1);

                factors.inside = ((factors.edge[0] + factors.edge[1] + factors.edge[2]) / 3.0f);

                return factors;
            }

            [domain("tri")]
            TessellationOutput Domain(
                TessFactors factors,
                OutputPatch<TessControlPoint, 3> patch,
                float3 barycentricCoords : SV_DomainLocation
            ) {
                TessellationOutput output = (TessellationOutput)0;

                float3 positionWorld =
                    patch[0].positionWorld * barycentricCoords.x +
                    patch[1].positionWorld * barycentricCoords.y +
                    patch[2].positionWorld * barycentricCoords.z;

                float2 uv =
                    patch[0].uv * barycentricCoords.x +
                    patch[1].uv * barycentricCoords.y +
                    patch[2].uv * barycentricCoords.z;

                float3 normalLocal =
                    patch[0].normalLocal * barycentricCoords.x +
                    patch[1].normalLocal * barycentricCoords.y +
                    patch[2].normalLocal * barycentricCoords.z;

                float height = CalculateRippleHeight(StartTime, _PropagationSpeed, _Range, positionWorld.xz, Origin.xz,
                                                     _Frequency, _Amplitude, 0);
                float3 newPositionWorld = float3(positionWorld.x, positionWorld.y + height, positionWorld.z);

                output.positionClip = TransformWorldToHClip(newPositionWorld);
                output.normalLocal = normalLocal;
                output.uv = TRANSFORM_TEX(uv, _MainTexture);
                return output;
            }

            half4 Fragment(TessellationOutput output) : SV_Target {
                float4 textureColor;
                if (output.normalLocal.y > 0.5) {
                    textureColor = SAMPLE_TEXTURE2D(_MainTexture, sampler_MainTexture, output.uv);
                }
                else {
                    textureColor = SAMPLE_TEXTURE2D(_SideTexture, sampler_MainTexture, output.uv);
                }
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

            float4 depthNormalsFragment(VertexOutput output) : SV_Target {
                float3 normalWorld = NormalizeNormalPerPixel(output.normalWorld);
                return float4(normalWorld, .0);
            }
            ENDHLSL
        }

    }


}
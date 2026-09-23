#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

float CalculateRippleHeight(
    float startTime, float propSpeed, float range, float settle,
    float2 pos, float2 origin,
    float wl, float freq, float amp
) {
    float realTime = _Time.y - startTime;
    float propagationDistance = realTime * propSpeed;
    float distToOrigin = distance(pos, origin);
    float outerDampening = 1 - saturate(distToOrigin / range);
    float innerDampening = 1 - saturate(abs(propagationDistance - distToOrigin) / (settle * wl));
    float dampening = outerDampening * innerDampening;
    float height = sin((distToOrigin + realTime * freq) / wl) * amp * dampening;
    return height;
}

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

float CalculateRippleHeight(
    float startTime, float propSpeed, float range,
    float2 pos, float2 origin, float freq, float amp
) {
    float realTime = _Time.y - startTime;
    float distToOrigin = distance(pos, origin);
    float rangeCutoff = step(distToOrigin, range);
    float propagation = realTime * propSpeed + 0.7;
    float half_wl = PI / freq;
    float innerRadius = max(0, propagation - half_wl);
    float waveInterval = step(distToOrigin, propagation) * step(innerRadius, distToOrigin);
    float height = waveInterval * rangeCutoff * amp * -sin(freq * (distToOrigin - propagation));
    return height;
}

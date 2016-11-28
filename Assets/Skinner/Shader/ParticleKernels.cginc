#include "Common.cginc"
#include "SimplexNoiseGrad3D.cginc"

sampler2D _SourcePositionBuffer0;
sampler2D _SourcePositionBuffer1;
sampler2D _PositionBuffer;
sampler2D _VelocityBuffer;
sampler2D _RotationBuffer;

float2 _LifeParams;   // 1/min, 1/max
float4 _Acceleration; // x, y, z, drag
float3 _SpinParams;   // spin*2, speed-to-spin*2, randomness
float2 _NoiseParams;  // freq, amp
float3 _NoiseOffset;
float4 _Config;       // throttle, dT, time

// Particle generator functions
float4 NewParticlePosition(float2 uv)
{
    float3 p0 = tex2D(_SourcePositionBuffer0, uv).xyz;
    float3 p1 = tex2D(_SourcePositionBuffer1, uv).xyz;
    float3 v = (p1 - p0) / _Config.y;
    return float4(p1, min(length(v) * 4 - 0.5, 0.5));

/*
    float3 p1 = tex2D(_SourcePositionBuffer1, uv).xyz;
    return float4(p1, 0.5);
    */
}

float4 NewParticleVelocity(float2 uv)
{
    float3 p0 = tex2D(_SourcePositionBuffer0, uv).xyz;
    float3 p1 = tex2D(_SourcePositionBuffer1, uv).xyz;
    float3 v = (p1 - p0) / _Config.y;
    return float4(v, 0);
}

float4 NewParticleRotation(float2 uv)
{
    // Uniform random unit quaternion
    // http://www.realtimerendering.com/resources/GraphicsGems/gemsiii/urot.c
    float r = UVRandom(uv, 7);
    float r1 = sqrt(1 - r);
    float r2 = sqrt(r);
    float t1 = UNITY_PI * 2 * UVRandom(uv, 8);
    float t2 = UNITY_PI * 2 * UVRandom(uv, 9);
    return float4(sin(t1) * r1, cos(t1) * r1, sin(t2) * r2, cos(t2) * r2);
}

// Deterministic random rotation axis
float3 RotationAxis(float2 uv)
{
    // Uniformaly distributed points
    // http://mathworld.wolfram.com/SpherePointPicking.html
    float u = UVRandom(uv, 10) * 2 - 1;
    float u2 = sqrt(1 - u * u);
    float sn, cs;
    sincos(UVRandom(uv, 11) * UNITY_PI * 2, sn, cs);
    return float3(u2 * cs, u2 * sn, u);
}

// Fragment shaders for initialization

float4 InitializePositionFragment(v2f_img i) : SV_Target
{
    // A far point and a random life
    return float4(1e+6, 1e+6, 1e+6, UVRandom(i.uv, 0));
}

float4 InitializeVelocityFragment(v2f_img i) : SV_Target
{
    // Zero velocity
    return 0;
}

float4 InitializeRotationFragment(v2f_img i) : SV_Target
{
    // Zero rotation
    return float4(0, 0, 0, 1);
}

// Fragment shaders for update

float4 UpdatePositionFragment(v2f_img i) : SV_Target
{
    float4 p = tex2D(_PositionBuffer, i.uv);
    float3 v = tex2D(_VelocityBuffer, i.uv).xyz;

    float lv = length(v);
    if (lv > 0.1) v = normalize(v) * min(lv, 1);

    // Decaying
    float dt = _Config.y;
    p.w -= lerp(_LifeParams.x, _LifeParams.y, UVRandom(i.uv, 12)) * dt;

    if (p.w > -0.5)
    {
        // Applying the velocity
        p.xyz += v * dt;
        return p;
    }
    else
    {
        // Respawn
        return NewParticlePosition(i.uv);
    }
}

float4 UpdateVelocityFragment(v2f_img i) : SV_Target
{
    float4 p = tex2D(_PositionBuffer, i.uv);
    float3 v = tex2D(_VelocityBuffer, i.uv).xyz;

    if (p.w < 0.5)
    {
        // Drag
        v *= _Acceleration.w; // dt is pre-applied in script

        // Constant acceleration
        float dt = _Config.y;
        v += _Acceleration.xyz * dt;

        // Acceleration by turbulent noise
        float3 np = (p.xyz + _NoiseOffset) * _NoiseParams.x;
        float3 n1 = snoise_grad(np);
        float3 n2 = snoise_grad(np + float3(21.83, 13.28, 7.32));
        v += cross(n1, n2) * _NoiseParams.y * dt;

        return float4(v, 0);
    }
    else
    {
        // Respawn
        return NewParticleVelocity(i.uv);
    }
}

float4 UpdateRotationFragment(v2f_img i) : SV_Target
{
    float4 r = tex2D(_RotationBuffer, i.uv);
    float3 v = tex2D(_VelocityBuffer, i.uv).xyz;

    // Delta angle
    float dt = _Config.y;
    float theta = (_SpinParams.x + length(v) * _SpinParams.y) * dt;

    // Randomness
    theta *= 1.0 - UVRandom(i.uv, 13) * _SpinParams.z;

    // Spin quaternion
    float sn, cs;
    sincos(theta, sn, cs);
    float4 dq = float4(RotationAxis(i.uv) * sn, cs);

    // Applying the quaternion and normalize the result.
    return normalize(QMult(dq, r));
}

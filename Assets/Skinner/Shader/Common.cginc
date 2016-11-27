#include "UnityCG.cginc"

// Seed for PRNG
float _RandomSeed;

// PRNG function
float UVRandom(float2 uv, float salt)
{
    uv += float2(salt, _RandomSeed);
    return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
}

// Quaternion multiplication
// http://mathworld.wolfram.com/Quaternion.html
float4 QMult(float4 q1, float4 q2)
{
    float3 ijk = q2.xyz * q1.w + q1.xyz * q2.w + cross(q1.xyz, q2.xyz);
    return float4(ijk, q1.w * q2.w - dot(q1.xyz, q2.xyz));
}

// Vector rotation with a quaternion
// http://mathworld.wolfram.com/Quaternion.html
float3 RotateVector(float3 v, float4 r)
{
    float4 r_c = r * float4(-1, -1, -1, 1);
    return QMult(r, QMult(float4(v, 0), r_c)).xyz;
}

// Stereographic projection and inverse projection
float2 StereoProjection(float3 n)
{
    return n.xy / (1 - n.z);
}

float3 StereoInverseProjection(float2 p)
{
    float d = 2 / (dot(p.xy, p.xy) + 1);
    return float3(p.xy * d, 1 - d);
}

// Hue to RGB convertion
half3 HueToRGB(half h)
{
    h = frac(h);
    half r = abs(h * 6 - 3) - 1;
    half g = 2 - abs(h * 6 - 2);
    half b = 2 - abs(h * 6 - 4);
    half3 rgb = saturate(half3(r, g, b));
#if UNITY_COLORSPACE_GAMMA
    return rgb;
#else
    return GammaToLinearSpace(rgb);
#endif
}

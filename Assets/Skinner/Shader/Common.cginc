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
half2 StereoProjection(half3 n)
{
    return n.xy / (1 - n.z);
}

half3 StereoInverseProjection(half2 p)
{
    float d = 2 / (dot(p.xy, p.xy) + 1);
    return float3(p.xy * d, 1 - d);
}

// Calculate the area of a triangle from the length of the sides.
half TriangleArea(half a, half b, half c)
{
    // Heron's formula
    half s = 0.5 * (a + b + c);
    return sqrt(s * (s - a) * (s - b) * (s - c));
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

// Common color animation
half _BaseHue;
half _HueRandomness;
half _Saturation;
half _Brightness;
half _EmissionProb;
half _HueShift;
half _BrightnessOffs;

half3 ColorAnimation(float id, half intensity)
{
    // Low frequency oscillation with half-wave rectified sinusoid.
    half phase = UVRandom(id, 30) * 32 + _Time.y * 4;
    half lfo = abs(sin(phase * UNITY_PI));

    // Switch LFO randomly at zero-cross points.
    lfo *= UVRandom(id + floor(phase), 31) < _EmissionProb;

    // Hue animation.
    half hue = _BaseHue + UVRandom(id, 32) * _HueRandomness + _HueShift * intensity;

    // Convert to RGB.
    half3 rgb = lerp(1, HueToRGB(hue), _Saturation);

    // Apply brightness.
    return rgb * (_Brightness * lfo + _BrightnessOffs * intensity);
}

// Scale animation used in the particle shaders.
float ParticleScale(float id, half life, half speed, half2 params)
{
    // Start/End
    half s = min((1 - life) * 20, min(life * 3, 1));
    // Scale by the initial speed.
    s *= min(speed * params.y, params.x);
    // 50% randomization
    s *= 1 - 0.5 * UVRandom(id, 20);
    return s;
}

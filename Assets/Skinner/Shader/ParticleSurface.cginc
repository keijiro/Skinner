// Surface shader for Skinner Particle
#include "Common.cginc"

// Buffers from the animation kernels
sampler2D _PositionBuffer;
sampler2D _VelocityBuffer;
sampler2D _RotationBuffer;

// Base material properties
half3 _Albedo;
half _Smoothness;
half _Metallic;

#if defined(SKINNER_TEXTURED)
sampler2D _AlbedoMap;
sampler2D _NormalMap;
half _NormalScale;
#endif

// Scale modifier
float2 _Scale; // (min, max)

// Self illumination
half _BaseHue;
half _HueRandomness;
half _Saturation;
half _Brightness;
half _EmissionProb;

// Color animation
half _SpeedToLightMin;
half _SpeedToLightMax;
half _HueShift;
half _BrightnessOffs;

struct Input
{
#if defined(SKINNER_TEXTURED)
    float2 uv_AlbedoMap;
#endif
#if defined(SKINNER_TWO_SIDED)
    fixed facing : VFACE;
#endif
    fixed4 color : COLOR;
};

void vert(inout appdata_full data)
{
    // Particle ID
    float id = data.texcoord1.x;

    // Fetch the particle position/velocity/rotation.
    float4 p = tex2Dlod(_PositionBuffer, float4(id, 0.5, 0, 0));
    float4 v = tex2Dlod(_VelocityBuffer, float4(id, 0.5, 0, 0));
    float4 r = tex2Dlod(_RotationBuffer, float4(id, 0.5, 0, 0));

    // Speed/Scale
    half speed = length(v.xyz);
    half scale = ParticleScale(id, p.w + 0.5, v.w, _Scale);

    // Low frequency oscillation with half-wave rectified sinusoid.
    half phase = UVRandom(id, 21) * 32 + _Time.y * 4;
    half lfo = abs(sin(phase * UNITY_PI));

    // Switch LFO randomly at zero-cross points.
    lfo *= UVRandom(id + floor(phase), 22) < _EmissionProb;

    // Calculate HSB and convert it to RGB.
    half intensity = smoothstep(_SpeedToLightMin, _SpeedToLightMax, speed);
    half hue = _BaseHue + UVRandom(id, 2) * _HueRandomness + _HueShift * intensity;
    half3 rgb = lerp(1, HueToRGB(hue), _Saturation);
    rgb *= _Brightness * lfo + _BrightnessOffs * intensity;

    // Modify the vertex attributes.
    data.vertex.xyz = RotateVector(data.vertex.xyz, r) * scale + p.xyz;
    data.normal = RotateVector(data.normal, r);
#if defined(SKINNER_TEXTURED)
    data.tangent.xyz = RotateVector(data.tangent.xyz, r);
#endif
    data.color.rgb = rgb;
}

void surf(Input IN, inout SurfaceOutputStandard o)
{
#if defined(SKINNER_TEXTURED)
    o.Albedo = tex2D(_AlbedoMap, IN.uv_AlbedoMap).rgb * _Albedo;
#else
    o.Albedo = _Albedo;
#endif

#if defined(SKINNER_TEXTURED)
    half3 n = UnpackScaleNormal(tex2D(_NormalMap, IN.uv_AlbedoMap), _NormalScale);
    #if defined(SKINNER_TWO_SIDED)
    o.Normal = n * (IN.facing > 0 ? 1 : -1);
    #else
    o.Normal = n;
    #endif
#else
    #if defined(SKINNER_TWO_SIDED)
    o.Normal = half3(0, 0, IN.facing > 0 ? 1 : -1);
    #endif
#endif

    o.Smoothness = _Smoothness;
    o.Metallic = _Metallic;
    o.Emission = IN.color.rgb;
}

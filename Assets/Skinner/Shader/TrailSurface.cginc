// Surface shader for Skinner Trail
#include "Common.cginc"

// Buffers from the animation kernels
sampler2D _PositionBuffer;
sampler2D _VelocityBuffer;
sampler2D _OrthnormBuffer;

// Base material properties
half3 _Albedo;
half _Smoothness;
half _Metallic;

// Line width modifier
half _SpeedToWidthMin;
half _SpeedToWidthMax;
half _Width;

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
    half3 color : COLOR;
    fixed facing : VFACE;
};

void vert(inout appdata_full v)
{
    // Line hash ID
    half hash = v.vertex.x;

    // Position/Velocity
    float4 texc = float4(v.vertex.xy, 0, 0);
    float3 pos = tex2Dlod(_PositionBuffer, texc);
    half speed = length(tex2Dlod(_VelocityBuffer, texc));

    // Normal/Binormal
    float4 basis = tex2Dlod(_OrthnormBuffer, texc);
    half3 normal = StereoInverseProjection(basis.xy);
    half3 binormal = StereoInverseProjection(basis.zw);

    // Dilate the line.
    half width = smoothstep(_SpeedToWidthMin, _SpeedToWidthMax, speed);
    pos += binormal * (width * _Width * v.vertex.z * (1 - v.vertex.y));

    // Low frequency oscillation with half-wave rectified sinusoid.
    half phase = UVRandom(hash, 0) * 32 + _Time.y * 4;
    half lfo = abs(sin(phase * UNITY_PI));

    // Switch LFO randomly at zero-cross points.
    lfo *= UVRandom(hash + floor(phase), 1) < _EmissionProb;

    // Calculate HSB and convert it to RGB.
    half intensity = smoothstep(_SpeedToLightMin, _SpeedToLightMax, speed);
    half hue = _BaseHue + UVRandom(hash, 2) * _HueRandomness + _HueShift * intensity;
    half3 rgb = lerp(1, HueToRGB(hue), _Saturation);
    rgb *= _Brightness * lfo + _BrightnessOffs * intensity;

    // Output
    v.vertex = float4(pos, v.vertex.w);
    v.normal = normal;
    v.color = half4(rgb, 1);
}

void surf(Input IN, inout SurfaceOutputStandard o)
{
    o.Albedo = _Albedo;
    o.Smoothness = _Smoothness;
    o.Metallic = _Metallic;
    o.Emission = IN.color;
    o.Normal = float3(0, 0, IN.facing > 0 ? 1 : -1);
}

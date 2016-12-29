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

void vert(inout appdata_full data)
{
    // Line ID
    float id = data.vertex.x;

    // Fetch the vertex position/velocity/orthnormal.
    float4 texcoord = float4(data.vertex.xy, 0, 0);
    float3 P = tex2Dlod(_PositionBuffer, texcoord).xyz;
    float3 V = tex2Dlod(_VelocityBuffer, texcoord).xyz;
    float4 B = tex2Dlod(_OrthnormBuffer, texcoord);

    // Vertex speed
    half speed = length(V);

    // Normal/Binormal
    half3 normal = StereoInverseProjection(B.xy);
    half3 binormal = StereoInverseProjection(B.zw);

    // Line width
    half width = _Width * data.vertex.z * (1 - data.vertex.y);
    width *= smoothstep(_SpeedToWidthMin, _SpeedToWidthMax, speed);

    // Low frequency oscillation with half-wave rectified sinusoid.
    half phase = UVRandom(id, 0) * 32 + _Time.y * 4;
    half lfo = abs(sin(phase * UNITY_PI));

    // Switch LFO randomly at zero-cross points.
    lfo *= UVRandom(id + floor(phase), 1) < _EmissionProb;

    // Calculate HSB and convert it to RGB.
    half intensity = smoothstep(_SpeedToLightMin, _SpeedToLightMax, speed);
    half hue = _BaseHue + UVRandom(id, 2) * _HueRandomness + _HueShift * intensity;
    half3 rgb = lerp(1, HueToRGB(hue), _Saturation);
    rgb *= _Brightness * lfo + _BrightnessOffs * intensity;

    // Modify the vertex attributes.
    data.vertex = float4(P + binormal * width, data.vertex.w);
    data.normal = normal;
    data.color = half4(rgb, 1);
}

void surf(Input IN, inout SurfaceOutputStandard o)
{
    o.Albedo = _Albedo;
    o.Smoothness = _Smoothness;
    o.Metallic = _Metallic;
    o.Emission = IN.color;
    o.Normal = float3(0, 0, IN.facing > 0 ? 1 : -1);
}

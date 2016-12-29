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
half3 _LineWidth; // (max width, cutoff, speed-to-width / max width)

// Color modifier
half _CutoffSpeed;
half _SpeedToIntensity;

struct Input
{
    half3 color : COLOR;
    fixed facing : VFACE;
};

void vert(inout appdata_full data)
{
    // Line ID
    float id = data.vertex.x;

    // Fetch samples from the animation kernel.
    float4 texcoord = float4(data.vertex.xy, 0, 0);
    float3 P = tex2Dlod(_PositionBuffer, texcoord).xyz;
    float3 V = tex2Dlod(_VelocityBuffer, texcoord).xyz;
    float4 B = tex2Dlod(_OrthnormBuffer, texcoord);

    // Extract normal/binormal vector from the orthnormal sample.
    half3 normal = StereoInverseProjection(B.xy);
    half3 binormal = StereoInverseProjection(B.zw);

    // Attribute modifiers
    half speed = length(V);

    half width = _LineWidth.x * data.vertex.z * (1 - data.vertex.y);
    width *= saturate((speed - _LineWidth.y) * _LineWidth.z);

    half intensity = saturate((speed - _CutoffSpeed) * _SpeedToIntensity);

    // Modify the vertex attributes.
    data.vertex = float4(P + binormal * width, data.vertex.w);
    data.normal = normal;
    data.color.rgb = ColorAnimation(id, intensity);
}

void surf(Input IN, inout SurfaceOutputStandard o)
{
    o.Albedo = _Albedo;
    o.Smoothness = _Smoothness;
    o.Metallic = _Metallic;
    o.Emission = IN.color;
    o.Normal = float3(0, 0, IN.facing > 0 ? 1 : -1);
}

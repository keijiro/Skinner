#include "Common.cginc"

sampler2D _PositionBuffer;
sampler2D _VelocityBuffer;
sampler2D _BasisBuffer;

struct Input
{
    half3 color : COLOR;
};

void vert(inout appdata_full v, out Input data)
{
    UNITY_INITIALIZE_OUTPUT(Input, data);

    float4 uv = float4(v.vertex.xy, 0, 0);

    // position
    float3 pos = tex2Dlod(_PositionBuffer, uv);

    // velocity
    float3 vel = tex2Dlod(_VelocityBuffer, uv);

    // Normal/Binormal
    float4 basis = tex2Dlod(_BasisBuffer, uv);
    float3 normal = StereoInverseProjection(basis.xy);
    float3 binormal = StereoInverseProjection(basis.zw);

    float intensity = saturate(length(vel) * 0.3);

#if defined(NORMAL_FLIP)
    normal = -normal;
#endif

    // Modify the vertex.
    v.vertex = float4(pos + binormal * (0.04 * intensity) * v.vertex.z * (1 - v.vertex.y), v.vertex.w);
    v.normal = normal;

    //v.color.rgb = half3(saturate(intensity * 2 - 1), saturate(2 - intensity * 2), 3.2) * pow(intensity, 4) * smoothstep(0.5, 1, intensity);
    v.color.rgb = HueToRGB(saturate(intensity * 2 - 1) * 0.5 + 0.6) * 1.2 * pow(intensity, 4) * smoothstep(0.5, 1, intensity);
}

void surf(Input IN, inout SurfaceOutputStandard o)
{
    o.Albedo = 0.4;//float3(0.05, 0.05, 0.05);
    o.Smoothness = 0.5;
    o.Metallic = 0.5;
    o.Emission = IN.color;
}

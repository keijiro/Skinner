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

    float intensity = pow(min(length(vel) * 0.3, 1), 4);

#if defined(NORMAL_FLIP)
    normal = -normal;
#endif

    // Modify the vertex.
    v.vertex = float4(pos + binormal * (0.01 + intensity * 0.02) * v.vertex.z * (1 - v.vertex.y), v.vertex.w);
    v.normal = normal;

    v.color.rgb = half3(0.5, 0.5, 3.5) * intensity;
}

void surf(Input IN, inout SurfaceOutputStandard o)
{
    o.Albedo = float3(0.05, 0.05, 0.05);
    o.Smoothness = 0.5;
    o.Metallic = 0.5;
    o.Emission = IN.color;
}

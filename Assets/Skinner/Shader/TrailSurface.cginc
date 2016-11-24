#include "Common.cginc"

sampler2D _PositionBuffer;
sampler2D _BasisBuffer;

struct Input
{
    half dummy;
};

void vert(inout appdata_full v, out Input data)
{
    UNITY_INITIALIZE_OUTPUT(Input, data);

    float4 uv = float4(v.vertex.xy, 0, 0);

    // position
    float3 pos = tex2Dlod(_PositionBuffer, uv);

    // Normal/Binormal
    float4 basis = tex2Dlod(_BasisBuffer, uv);
    float3 normal = StereoInverseProjection(basis.xy);
    float3 binormal = StereoInverseProjection(basis.zw);

#if defined(NORMAL_FLIP)
    normal = -normal;
#endif

    // Modify the vertex.
    v.vertex = float4(pos + binormal * 0.02 * v.vertex.z * (1 - v.vertex.y), v.vertex.w);
    v.normal = normal;
}

void surf(Input IN, inout SurfaceOutputStandard o)
{
    o.Albedo = 1;
    o.Smoothness = 0;
    o.Metallic = 0;
}

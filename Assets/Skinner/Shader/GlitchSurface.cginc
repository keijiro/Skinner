#include "Common.cginc"

sampler2D _PositionBuffer;

struct Input
{
    half3 color : COLOR;
};

void vert(inout appdata_full v)
{
    float t = _Time.y;

    float offs = UVRandom(t, 0);
    float4 uv0 = float4(v.vertex.x + offs + 0.005 * (UVRandom(v.vertex.x, t) - 0.5), 0.5, 0, 0);
    float4 uv1 = float4(v.vertex.y + offs + 0.005 * (UVRandom(v.vertex.y, t) - 0.5), 0.5, 0, 0);
    float4 uv2 = float4(v.vertex.z + offs + 0.005 * (UVRandom(v.vertex.z, t) - 0.5), 0.5, 0, 0);

    float3 p0 = tex2Dlod(_PositionBuffer, uv0).xyz;
    float3 p1 = tex2Dlod(_PositionBuffer, uv1).xyz;
    float3 p2 = tex2Dlod(_PositionBuffer, uv2).xyz;

    v.vertex.xyz = p0;
    v.normal = normalize(cross(p2 - p0, p1 - p0));
    v.color = 0;
}

void surf(Input IN, inout SurfaceOutputStandard o)
{
    o.Albedo = 0.4;
    o.Smoothness = 0.5;
    o.Metallic = 0.5;
}

#include "Common.cginc"

sampler2D _PositionBuffer;
float4 _PositionBuffer_TexelSize;
float _Offset;

struct Input
{
    half3 color : COLOR;
};

// Calculate the area of a triangle from the length of the sides
float TriangleArea(float a, float b, float c)
{
    // Heron's formula
    float s = 0.5 * (a + b + c);
    // Actually this returns the square of the area!
    return s * (s - a) * (s - b) * (s - c);
}

void vert(inout appdata_full v)
{
    float3 cv = v.texcoord.w;
    float delta = UVRandom(cv, 0) + _Offset * _PositionBuffer_TexelSize.y;

    v.color.rgb = HueToRGB(UVRandom(cv, 1) * 0.4 + 0.6) * 1.1 * (UVRandom(cv, floor(_Time.y * 2)) < 0.1) * abs(sin(_Time.y));

    float4 uv0 = float4(v.vertex.x, delta, 0, 0);
    float4 uv1 = float4(v.vertex.y, delta, 0, 0);
    float4 uv2 = float4(v.vertex.z, delta, 0, 0);

    float3 p0 = tex2Dlod(_PositionBuffer, uv0).xyz;
    float3 p1 = tex2Dlod(_PositionBuffer, uv1).xyz;
    float3 p2 = tex2Dlod(_PositionBuffer, uv2).xyz;

    float3 c = (p0 + p1 + p2) / 3;
    float3 n = normalize(cross(p1 - p0, p2 - p0));
#if defined(NORMAL_FLIP)
    n = -n;
#endif

    float s0 = length(p1 - p0);
    float s1 = length(p2 - p1);
    float s2 = length(p0 - p2);

    delta = 1 - pow(1 - frac(delta), 3);

    float th1 = 0.28;
    float th2 = 0.00004;

    float cond1 = (s0 < th1) * (s1 < th1) * (s2 < th1);
    float cond2 = TriangleArea(s0, s1, s2) < th2;

    //v.vertex.xyz = cond1 + cond2 > 0 ? lerp(p0, c, 0.2 * smoothstep(0.2, 0.3, delta) + 0.8 * smoothstep(0.9, 1, delta)) : c;
    v.vertex.xyz = cond1 + cond2 > 0 ? lerp(p0, c, smoothstep(0.2, 1.0, delta)) : c;
    v.normal = n;

    v.color += pow(1 - delta, 18) * float4(1, 0, 0, 0);
}

void surf(Input IN, inout SurfaceOutputStandard o)
{
    o.Albedo = 0.2;
    o.Smoothness = 0.5;
    o.Metallic = 0.5;
    o.Emission = IN.color;
}

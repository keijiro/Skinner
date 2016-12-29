// Surface shader for Skinner Glitch
#include "Common.cginc"

// Buffers from the animation kernels
sampler2D _PositionBuffer;
float4 _PositionBuffer_TexelSize;
float _BufferOffset;

// Base material properties
half3 _Albedo;
half _Smoothness;
half _Metallic;

// Glitch thresholds
half _EdgeThreshold;
half _AreaThreshold;

// Color modifier
half _ModDuration;

struct Input
{
    half3 color : COLOR;
    fixed facing : VFACE;
};

void vert(inout appdata_full data)
{
    // Triangle ID
    float id = data.texcoord.w;

    // V-coodinate offset for the position buffer.
    float voffs = UVRandom(id, 0) + _BufferOffset * _PositionBuffer_TexelSize.y;

    // U-coodinate offset: change randomly when V-offs wraps around.
    float uoffs = frac(UVRandom(id + floor(voffs), 1));

    // Actually only the fractional part of V-offs is important.
    voffs = frac(voffs);

    // Fetch samples from the animation kernel.
    // p0: current vertex, p1: left-hand neighbor, p2: right-hand neighbor
    float3 p0 = tex2Dlod(_PositionBuffer, float4(frac(data.texcoord.x + uoffs), voffs, 0, 0)).xyz;
    float3 p1 = tex2Dlod(_PositionBuffer, float4(frac(data.texcoord.y + uoffs), voffs, 0, 0)).xyz;
    float3 p2 = tex2Dlod(_PositionBuffer, float4(frac(data.texcoord.z + uoffs), voffs, 0, 0)).xyz;

    // Centroid of the triangle
    float3 center = (p0 + p1 + p2) / 3;

    // Lengths of the triangle edges
    half3 edges = half3(length(p1 - p0), length(p2 - p1), length(p0 - p2));

    // Soft thresholding by the edge lengths
    half3 ecull3 = saturate((edges - _EdgeThreshold) / _EdgeThreshold);
    half ecull = max(max(ecull3.x, ecull3.y), ecull3.z);

    // Soft thresholding by the triangle area
    half area = TriangleArea(edges.x, edges.y, edges.z);
    half acull = saturate((area - _AreaThreshold) / _AreaThreshold);

    // Finally, we can do something fun!
    float decay = pow(1 - voffs, 6);
    half scale = saturate(1 - max(ecull, acull)) * decay;
    half intensity = (1 - smoothstep(_ModDuration * 0.5, _ModDuration, voffs)) * decay;

    // Modify the vertex attributes.
    data.vertex.xyz = lerp(center, p0, scale);
    data.normal = normalize(cross(p1 - p0, p2 - p0));
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

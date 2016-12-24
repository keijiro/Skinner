// Surface shader for Skinner Glitch
#include "Common.cginc"

// Base material properties.
half3 _Albedo;
half _Smoothness;
half _Metallic;

// Buffers from the animation kernels.
sampler2D _PositionBuffer;
float4 _PositionBuffer_TexelSize;
float _BufferOffset;

// Parameters for the vertex modifier.
half _Threshold;

struct Input
{
    half3 color : COLOR;
};

// Calculate the area of a triangle from the length of the sides
half TriangleArea(half a, half b, half c)
{
    // Heron's formula
    half s = 0.5 * (a + b + c);
    // Actually this returns the square of the area!
    return s * (s - a) * (s - b) * (s - c);
}

void vert(inout appdata_full v)
{
    // Hash ID of the triangle.
    float hash = v.texcoord.w;

    // 0 to 1 time parameter (can be used as a V-coordinate value).
    float time01 = UVRandom(hash, 0) + _BufferOffset * _PositionBuffer_TexelSize.y;

    // Decaying animation parameter.
    half decay = 1 - pow(1 - frac(time01), 3);

    // Fetch the world space positions.
    // p0: current vertex, p1: left-hand neighbor, p2: right-hand neighbor
    float3 p0 = tex2Dlod(_PositionBuffer, float4(v.texcoord.x, time01, 0, 0)).xyz;
    float3 p1 = tex2Dlod(_PositionBuffer, float4(v.texcoord.y, time01, 0, 0)).xyz;
    float3 p2 = tex2Dlod(_PositionBuffer, float4(v.texcoord.z, time01, 0, 0)).xyz;

    // Centroid of the triangle.
    float3 center = (p0 + p1 + p2) / 3;

    // Length of the triangle edges.
    half3 edges = half3(length(p1 - p0), length(p2 - p1), length(p0 - p2));

    // Soft thresholding by edge length.
    half thresh1 = 0.28 * _Threshold;
    half3 econds = saturate((edges - thresh1) * (1.0 / thresh1));
    half econd = max(max(econds.x, econds.y), econds.z);

    // Soft thresholding by triangle area.
    half thresh2 = 0.00004 * _Threshold;
    half area = TriangleArea(edges.x, edges.y, edges.z);
    half acond = saturate((area - thresh2) / thresh2);

    // Scale factor.
    half scale = saturate(2 - econd - acond) * (1 - decay);

    // Modify the vertex position.
    v.vertex.xyz = lerp(center, p0, scale);

    // Calculate the normal vector.
#if !defined(NORMAL_FLIP)
    v.normal = normalize(cross(p1 - p0, p2 - p0));
#else
    v.normal = normalize(cross(p2 - p0, p1 - p0));
#endif

    // Animate the triangle color.
    //v.color.rgb = HueToRGB(UVRandom(hash, 1) * 0.4 + 0.6) * 1.1 * (UVRandom(hash, floor(_Time.y * 2)) < 0.1) * abs(sin(_Time.y));
    //v.color += pow(1 - decay, 18) * float4(1, 0, 0, 0);
    v.color = 0;
}

void surf(Input IN, inout SurfaceOutputStandard o)
{
    o.Albedo = _Albedo;
    o.Smoothness = _Smoothness;
    o.Metallic = _Metallic;
    o.Emission = IN.color;
}

#include "Common.cginc"

sampler2D _PositionBuffer;
float4 _PositionBuffer_TexelSize;

sampler2D _NewPositionBuffer;
float4 _NewPositionBuffer_TexelSize;

sampler2D _VelocityBuffer;
float4 _VelocityBuffer_TexelSize;

sampler2D _BasisBuffer;
float4 _BasisBuffer_TexelSize;

float4 InitializeVelocityFragment(v2f_img i) : SV_Target
{
    // Initialize with zero velocity.
    return 0;
}

float4 InitializePositionFragment(v2f_img i) : SV_Target
{
    // Simply copy the current vertex position.
    float3 p = tex2D(_NewPositionBuffer, i.uv.xy).xyz;
    return float4(p, 0);
}

float4 InitializeBasisFragment(v2f_img i) : SV_Target
{
    return 0;
}

float4 UpdateVelocityFragment(v2f_img i) : SV_Target
{
    float dv = _PositionBuffer_TexelSize.y;

    if (i.uv.y <= dv)
    {
        // The first row: calculate the vertex velocity.
        float3 p0 = tex2D(_PositionBuffer, i.uv.xy).xyz;
        float3 p1 = tex2D(_NewPositionBuffer, i.uv.xy).xyz;
        float3 v = (p1 - p0) * unity_DeltaTime.y;
        return float4(v, 0);
    }
    else
    {
        // Retrieve the velocity from the previous row and dampen it.
        float2 uv = float2(i.uv.x, i.uv.y - dv);
        float3 v = tex2D(_VelocityBuffer, uv).xyz;
        v *= exp(-2.0 * unity_DeltaTime.x);
        return float4(v, 0);
    }
}

float4 UpdatePositionFragment(v2f_img i) : SV_Target
{
    float dv = _PositionBuffer_TexelSize.y;

    if (i.uv.y <= dv)
    {
        // The first row: copy the current vertex position.
        float3 p = tex2D(_NewPositionBuffer, i.uv.xy).xyz;
        return float4(p, 0);
    }
    else
    {
        // Retrieve the position from the previous row.
        float2 uvp = float2(i.uv.x, i.uv.y - dv);
        float3 p = tex2D(_PositionBuffer, uvp).xyz;

        // Retrieve the vertex velocity and apply it.
        float3 v = tex2D(_VelocityBuffer, i.uv.xy).xyz;
        float lv = length(v);
        if (lv > 0.1) v = normalize(v) * min(lv, 0.5);
        p += v * unity_DeltaTime.x;

        return float4(p, 0);
    }
}

float4 UpdateBasisFragment(v2f_img i) : SV_Target
{
    float dv = _PositionBuffer_TexelSize.y;

    float2 uv0 = float2(i.uv.x, i.uv.y - dv * 2);
    float2 uv1 = float2(i.uv.x, i.uv.y - dv);
    float2 uv2 = float2(i.uv.x, i.uv.y + dv * 2);

    // Use the parent normal vector from the previous frame.
    float4 b1 = tex2D(_BasisBuffer, uv1);
    float3 ax = StereoInverseProjection(b1.zw);

    // Tangent vector
    float3 p0 = tex2D(_PositionBuffer, uv0);
    float3 p1 = tex2D(_PositionBuffer, uv2);
    float3 az = normalize(p1 - p0);

    // Reconstruct the orthonormal basis.
    float3 ay = normalize(cross(az, ax));
    ax = normalize(cross(ay, az));

    return float4(StereoProjection(ay), StereoProjection(ax));
}

#include "UnityCG.cginc"

sampler2D _PositionBuffer0;
float4 _PositionBuffer0_TexelSize;

sampler2D _PositionBuffer1;
float4 _PositionBuffer1_TexelSize;

sampler2D _VelocityBuffer;
float4 _VelocityBuffer_TexelSize;

float4 InitializeVelocityFragment(v2f_img i) : SV_Target
{
    // Initialize with zero velocity.
    return 0;
}

float4 InitializePositionFragment(v2f_img i) : SV_Target
{
    // Simply copy the current vertex position.
    float2 uv = float2(i.uv.x, 0.5);
    float3 p = tex2D(_PositionBuffer1, uv).xyz;
    return float4(p, 0);
}

float4 UpdateVelocityFragment(v2f_img i) : SV_Target
{
    float dv = _VelocityBuffer_TexelSize.y;

    if (i.uv.y <= dv)
    {
        // The first row: calculate the vertex velocity.
        float2 uv0 = i.uv.xy;
        float2 uv1 = float2(i.uv.x, 0.5);

        float3 p0 = tex2D(_PositionBuffer0, uv0).xyz;
        float3 p1 = tex2D(_PositionBuffer1, uv1).xyz;

        float3 v = (p1 - p0) * unity_DeltaTime.y;

        return float4(v, 0);
    }
    else
    {
        // Retrieve the velocity from the previous row.
        float2 uv = float2(i.uv.x, i.uv.y - dv);
        float3 v = tex2D(_VelocityBuffer, uv).xyz;

        // Dampening
        v *= exp(-3.0 * unity_DeltaTime.x);

        return float4(v, 0);
    }
}

float4 UpdatePositionFragment(v2f_img i) : SV_Target
{
    float dv = _PositionBuffer0_TexelSize.y;

    if (i.uv.y <= dv)
    {
        // The first row: copy the current vertex position.
        float2 uv = float2(i.uv.x, 0.5);
        float3 p = tex2D(_PositionBuffer1, uv).xyz;
        return float4(p, 0);
    }
    else
    {
        // Retrieve the position from the previous row.
        float2 uvp = float2(i.uv.x, i.uv.y - dv);
        float3 p = tex2D(_PositionBuffer0, uvp).xyz;

        // Retrieve the vertex velocity.
        float3 v = tex2D(_VelocityBuffer, i.uv.xy).xyz;

        // Calculate difference from the previous frame.
        float3 delta = v * unity_DeltaTime.x;// * 0.001;

        // Limit the velocity.
        //delta = normalize(delta) * min(length(delta), 0.01);

        return float4(p + delta, 0);
    }
}

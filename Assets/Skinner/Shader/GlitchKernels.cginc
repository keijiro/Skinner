// Animation kernels for Skinner Glitch
#include "Common.cginc"

sampler2D _SourcePositionBuffer0;
sampler2D _SourcePositionBuffer1;

sampler2D _PositionBuffer;
float4 _PositionBuffer_TexelSize;

sampler2D _VelocityBuffer;
float4 _VelocityBuffer_TexelSize;

float _VelocityScale;

float4 InitializePositionFragment(v2f_img i) : SV_Target
{
    return 0;
}

float4 InitializeVelocityFragment(v2f_img i) : SV_Target
{
    return 0;
}

float4 UpdatePositionFragment(v2f_img i) : SV_Target
{
    const float texelHeight = _PositionBuffer_TexelSize.y;

    float2 uv = i.uv.xy;

    if (uv.y < texelHeight)
    {
        // First row: just copy the source position.
        return tex2D(_SourcePositionBuffer1, uv);
    }
    else
    {
        // Fetch the position and the velocity from the previous row.
        uv.y -= texelHeight;
        float3 p = tex2D(_PositionBuffer, uv).xyz;
        float3 v = tex2D(_VelocityBuffer, uv).xyz;

        // Apply the velocity.
        p += v * unity_DeltaTime.x;

        return half4(p, 0);
    }
}

float4 UpdateVelocityFragment(v2f_img i) : SV_Target
{
    const float texelHeight = _VelocityBuffer_TexelSize.y;

    float2 uv = i.uv.xy;

    if (uv.y < texelHeight)
    {
        // First row: calculate the velocity and set it.
        float3 p0 = tex2D(_SourcePositionBuffer0, uv).xyz;
        float3 p1 = tex2D(_SourcePositionBuffer1, uv).xyz;
        return half4((p1 - p0) * unity_DeltaTime.y, 0) * _VelocityScale;
    }
    else
    {
        uv.y -= texelHeight;
        return tex2D(_VelocityBuffer, uv);
    }
}

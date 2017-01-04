// Animation kernels for Skinner Trail
#include "Common.cginc"

sampler2D _SourcePositionBuffer0;
sampler2D _SourcePositionBuffer1;

sampler2D _PositionBuffer;
float4 _PositionBuffer_TexelSize;

sampler2D _VelocityBuffer;
float4 _VelocityBuffer_TexelSize;

sampler2D _OrthnormBuffer;
float4 _OrthnormBuffer_TexelSize;

float _SpeedLimit;
float _Drag;

float4 InitializePositionFragment(v2f_img i) : SV_Target
{
    // Simply copy the source position.
    return tex2D(_SourcePositionBuffer1, i.uv.xy);
}

float4 InitializeVelocityFragment(v2f_img i) : SV_Target
{
    return 0;
}

half4 InitializeOrthnormFragment(v2f_img i) : SV_Target
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

        // Apply the velocity cap.
        float lv = max(length(v), 0.001);
        v = v * min(lv, _SpeedLimit) / lv;

        // Update the position with the velocity.
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
        // The first row: calculate the vertex velocity.
        // Get the average with the previous frame for low-pass filtering.
        float3 p0 = tex2D(_SourcePositionBuffer0, uv).xyz;
        float3 p1 = tex2D(_SourcePositionBuffer1, uv).xyz;
        float3 v0 = tex2D(_VelocityBuffer, uv).xyz;
        float3 v1 = (p1 - p0) * unity_DeltaTime.y;
        return float4((v0 + v1) * 0.5, 0);
    }
    else
    {
        // Retrieve the velocity from the previous row and dampen it.
        uv.y -= texelHeight;
        float3 v = tex2D(_VelocityBuffer, uv).xyz;
        return float4(v * _Drag, 0);
    }
}

half4 UpdateOrthnormFragment(v2f_img i) : SV_Target
{
    const float texelHeight = _OrthnormBuffer_TexelSize.y;

    float2 uv = i.uv.xy;

    float2 uv0 = float2(uv.x, uv.y - texelHeight * 2);
    float2 uv1 = float2(uv.x, uv.y - texelHeight);
    float2 uv2 = float2(uv.x, uv.y + texelHeight * 2);

    // Use the parent normal vector from the previous frame.
    half4 b1 = tex2D(_OrthnormBuffer, uv1);
    half3 ax = StereoInverseProjection(b1.zw);

    // Tangent vector
    float3 p0 = tex2D(_PositionBuffer, uv0);
    float3 p1 = tex2D(_PositionBuffer, uv2);
    half3 az = p1 - p0 + float3(1e-6, 0, 0); // zero-div guard

    // Reconstruct the orthonormal basis.
    half3 ay = normalize(cross(az, ax));
    ax = normalize(cross(ay, az));

    // Twisting
    half tw = frac(uv.x * 327.7289) * (1 - uv.y) * 0.2;
    ax = normalize(ax + ay * tw);

    return half4(StereoProjection(ay), StereoProjection(ax));
}

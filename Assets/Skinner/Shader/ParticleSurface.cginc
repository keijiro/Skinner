#include "Common.cginc"

sampler2D _MainTex;

half _Metallic;
half _Smoothness;

sampler2D _NormalMap;
half _NormalScale;

half _Emission;

sampler2D _PositionBuffer;
sampler2D _VelocityBuffer;
sampler2D _RotationBuffer;

// Scale factor
float2 _Scale; // (min, max)

// Scaling animation function
float ScaleAnimation(float2 uv, float time01)
{
    float s = lerp(_Scale.x, _Scale.y, UVRandom(uv, 14));
    // Linear scaling animation with life time
    // (0, 0) -> (0.1, 1) -> (0.9, 1) -> (1, 0)
    return s * min(1, 5 - abs(5 - time01 * 10));
}

struct Input
{
    float2 uv_MainTex;
    fixed4 color : COLOR;
};

void vert(inout appdata_full v)
{
    float4 uv = float4(v.texcoord1.xy, 0, 0);

    float4 pos = tex2Dlod(_PositionBuffer, uv);
    float3 vel = tex2Dlod(_VelocityBuffer, uv).xyz;
    float4 rot = tex2Dlod(_RotationBuffer, uv);

    float intensity = saturate(length(vel) * 0.3);

    float life = pos.w + 0.5;
    float sc = ScaleAnimation(uv, life) * (0.2 + intensity);

    v.vertex.xyz = RotateVector(v.vertex.xyz, rot) * sc + pos.xyz;
    v.normal = RotateVector(v.normal, rot);
#if _NORMALMAP
    v.tangent.xyz = RotateVector(v.tangent.xyz, rot);
#endif
    //v.color.xyz = lerp(half3(1.04, 0.04, 0.0), half3(3.4, 2.0, 1), length(vel) * 0.5);
    v.color.rgb = HueToRGB(intensity * 0.5 + 0.6) * 1.2 * pow(intensity, 3);
}

void surf(Input IN, inout SurfaceOutputStandard o)
{
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
    o.Albedo = 0.4;//IN.color.rgb * c.rgb;

    fixed4 n = tex2D(_NormalMap, IN.uv_MainTex);
    o.Normal = UnpackScaleNormal(n, _NormalScale);

    o.Emission = IN.color.rgb;

    o.Metallic = _Metallic;
    o.Smoothness = _Smoothness;
}

// Motion vector shader for Skinner Particle
#include "Common.cginc"

float4x4 _NonJitteredVP;
float4x4 _PreviousVP;
float4x4 _PreviousM;

sampler2D _PositionBuffer;
sampler2D _VelocityBuffer;
sampler2D _RotationBuffer;

sampler2D _PreviousPositionBuffer;
sampler2D _PreviousRotationBuffer;

float2 _Scale; // (min, max)

struct appdata
{
    float4 vertex : POSITION;
    float2 texcoord1 : TEXCOORD1;
};

struct v2f
{
    float4 vertex : SV_POSITION;
    float4 transfer0 : TEXCOORD0;
    float4 transfer1 : TEXCOORD1;
};

v2f vert(appdata data)
{
    // Particle ID
    float id = data.texcoord1.x;

    // Position/Rotation/Scale
    float4 p0 = tex2Dlod(_PreviousPositionBuffer, float4(id, 0.5, 0, 0));
    float4 r0 = tex2Dlod(_PreviousRotationBuffer, float4(id, 0.5, 0, 0));
    float4 p1 = tex2Dlod(_PositionBuffer, float4(id, 0.5, 0, 0));
    float4 v1 = tex2Dlod(_VelocityBuffer, float4(id, 0.5, 0, 0));
    float4 r1 = tex2Dlod(_RotationBuffer, float4(id, 0.5, 0, 0));
    half s0 = ParticleScale(id, p0.w + 0.5, v1.w, _Scale);
    half s1 = ParticleScale(id, p1.w + 0.5, v1.w, _Scale);

    // Apply the local transformation.
    float3 vp0 = RotateVector(data.vertex.xyz, r0) * s0 + p0.xyz;
    float3 vp1 = RotateVector(data.vertex.xyz, r1) * s1 + p1.xyz;

    // Transfer the data to the pixel shader.
    v2f o;
    o.vertex = UnityObjectToClipPos(float4(vp1, 1));
    o.transfer0 = mul(_PreviousVP, mul(_PreviousM,  float4(vp0, 1)));
    o.transfer1 = mul(_NonJitteredVP, mul(unity_ObjectToWorld, float4(vp1, 1)));
    return o;
}

half4 frag(v2f i) : SV_Target
{
    float3 hp0 = i.transfer0.xyz / i.transfer0.w;
    float3 hp1 = i.transfer1.xyz / i.transfer1.w;

    float2 vp0 = (hp0.xy + 1) / 2;
    float2 vp1 = (hp1.xy + 1) / 2;

#if UNITY_UV_STARTS_AT_TOP
    vp0.y = 1 - vp0.y;
    vp1.y = 1 - vp1.y;
#endif

    return half4(vp1 - vp0, 0, 1);
}

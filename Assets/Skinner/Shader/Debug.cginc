// Skinner/Debug - Debug visualization shader

#include "Common.cginc"

sampler2D _PreviousPositionBuffer;
sampler2D _PositionBuffer;
sampler2D _NormalBuffer;
sampler2D _TangentBuffer;

float _BufferOffset;

struct appdata
{
    float4 position : POSITION;
};

struct v2f
{
    float4 position : SV_POSITION;
    fixed3 color : COLOR;
};

v2f vert(appdata v)
{
    const float len = 0.05;

    float3 uvw = v.position.xyz;
    float4 texcoord = float4(uvw.x + _BufferOffset, 0.5, 0, 0);

    float3 prev = tex2Dlod(_PreviousPositionBuffer, texcoord).xyz;
    float3 position = tex2Dlod(_PositionBuffer, texcoord).xyz;
    half3 normal = tex2Dlod(_NormalBuffer, texcoord).xyz;
    half3 tangent = tex2Dlod(_TangentBuffer, texcoord).xyz;

    fixed3 color;

    if (uvw.y < 0.5)
    {
        // Line group #0 (red) - Velocity vector
        float3 delta = (position - prev) * unity_DeltaTime.y;
        position = position - delta * (uvw.z * len * 2);
        color = lerp(fixed3(1, 0, 0), 0.5, uvw.z);
    }
    else if (uvw.y < 1.5)
    {
        // Line group #1 (green) - Normal vector
        position += normal * uvw.z * len;
        color = lerp(fixed3(0, 1, 0), 0.5, uvw.z);
    }
    else
    {
        // Line group #2 (blue) - Tangent vector
        position += tangent * uvw.z * len;
        color = lerp(fixed3(0, 0, 1), 0.5, uvw.z);
    }

    v2f o;
    o.position = UnityObjectToClipPos(float4(position, 1));
    o.color = color;
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    return fixed4(i.color, 1);
}

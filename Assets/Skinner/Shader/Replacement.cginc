// Skinner/Replacement - Replacement shader used for vertex baking

#include "Common.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float2 texcoord : TEXCOORD0;
    half3 normal : NORMAL;
    half4 tangent : TANGENT;
};

struct v2f
{
    float4 position : SV_POSITION;
    float3 texcoord : TEXCOORD0;
    half3 normal : NORMAL;
    half4 tangent : TANGENT;
#if defined(SHADER_API_METAL) || defined(SHADER_API_VULKAN)
    float psize : PSIZE;
#endif
};

struct FragmentOutput
{
    float4 position : SV_Target0;
    half4 normal : SV_Target1;
    half4 tangent : SV_Target2;
};

v2f vert(appdata v)
{
    v2f o;
    // POSITION <= UV on the attribute buffer
    o.position = float4(v.texcoord.x * 2 - 1, 0, 0, 1);
    // TEXCOORD <= World position
    o.texcoord = mul(unity_ObjectToWorld, v.vertex).xyz;
    // NORMAL <= World normal
    o.normal = UnityObjectToWorldNormal(v.normal);
    // TANGENT <= World tangent
    o.tangent = half4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
#if defined(SHADER_API_METAL) || defined(SHADER_API_VULKAN)
    // Metal/Vulkan: Point size should be explicitly given.
    o.psize = 1;
#endif
    return o;
}

FragmentOutput frag(v2f i)
{
    FragmentOutput o;
    o.position = float4(i.texcoord, 0);
    o.normal = half4(i.normal, 0);
    o.tangent = i.tangent;
    return o;
}

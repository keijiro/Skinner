// Skinner/Replacement - Replacement shader used for vertex baking

#include "Common.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float2 texcoord : TEXCOORD0;
#if defined(SKINNER_NORMAL)
    half3 normal : NORMAL;
#endif
#if defined(SKINNER_TANGENT)
    half4 tangent : TANGENT;
#endif
};

struct v2f
{
    float4 position : SV_POSITION;
#if defined(SKINNER_POSITION)
    float3 texcoord : TEXCOORD0;
#endif
#if defined(SKINNER_NORMAL)
    half3 normal : NORMAL;
#endif
#if defined(SKINNER_TANGENT)
    half4 tangent : TANGENT;
#endif
#if defined(SHADER_API_METAL) || defined(SHADER_API_VULKAN)
    float psize : PSIZE;
#endif
};

#if defined(SKINNER_MRT)
struct FragmentOutput
{
    float4 position : SV_Target0;
    half4 normal : SV_Target1;
    half4 tangent : SV_Target2;
};
#endif

v2f vert(appdata v)
{
    v2f o;
    // POSITION <= UV on the attribute buffer
    o.position = float4(v.texcoord.x * 2 - 1, 0, 0, 1);
#if defined(SKINNER_POSITION)
    // TEXCOORD <= World position
    o.texcoord = mul(unity_ObjectToWorld, v.vertex).xyz;
#endif
#if defined(SKINNER_NORMAL)
    // NORMAL <= World normal
    o.normal = UnityObjectToWorldNormal(v.normal);
#endif
#if defined(SKINNER_TANGENT)
    // TANGENT <= World tangent
    o.tangent = half4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
#endif
#if defined(SHADER_API_METAL) || defined(SHADER_API_VULKAN)
    // Metal/Vulkan: Point size should be explicitly given.
    o.psize = 1;
#endif
    return o;
}

#if defined(SKINNER_MRT)
FragmentOutput frag(v2f i)
{
    FragmentOutput o;
    o.position = float4(i.texcoord, 0);
    o.normal = half4(i.normal, 0);
    o.tangent = i.tangent;
    return o;
}
#else
half4 frag(v2f i) : SV_Target
{
#if defined(SKINNER_POSITION)
    return float4(i.texcoord, 0);
#elif defined(SKINNER_NORMAL)
    return half4(i.normal, 0);
#elif defined(SKINNER_TANGENT)
    return i.tangent;
#endif
}
#endif

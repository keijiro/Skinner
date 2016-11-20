Shader "Hidden/Skinner/Replacement"
{
    SubShader
    {
        Tags { "Skinner" = "Source" }
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float3 uvw : TEXCOORD0;
            };

            struct FragmentOutput
            {
                float4 position : SV_Target0;
                float4 normal : SV_Target1;
                float4 tangent : SV_Target2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.position = float4(v.texcoord.xy * 2 - 1, 0, 1);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
                o.uvw = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            FragmentOutput frag(v2f i)
            {
                FragmentOutput o;
                o.position = float4(i.uvw, 0);
                o.normal = float4(i.normal, 0);
                o.tangent = i.tangent;
                return o;
            }

            ENDCG
        }
    }
}

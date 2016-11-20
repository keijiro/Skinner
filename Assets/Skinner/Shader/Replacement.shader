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
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 uvw : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.position = float4(v.texcoord.xy * 2 - 1, 0, 1);
                o.uvw = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return float4(i.uvw, 0);
            }

            ENDCG
        }
    }
}

Shader "Hidden/Skinner/Debug"
{
    Properties
    {
        _PositionBuffer("", 2D) = "black"{}
    }
    SubShader
    {
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 position : POSITION;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
            };

            sampler2D _PositionBuffer;

            float4 RetrievePosition(float u)
            {
                float4 texcoord = float4(u, 0.5, 0, 0);
                return float4(tex2Dlod(_PositionBuffer, texcoord).xyz, 1);
            }

            v2f vert(appdata v)
            {
                float3 uvw = v.position.xyz;
                float4 pos = UnityObjectToClipPos(RetrievePosition(uvw.x));
                pos.xy += uvw.yz * _ScreenParams.yx * 0.0001;

                v2f o;
                o.position = pos;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return half4(1, 0, 0, 0.2);
            }

            ENDCG
        }
    }
}

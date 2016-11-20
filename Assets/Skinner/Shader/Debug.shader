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

            v2f vert(appdata v)
            {
                float3 uvw = v.position.xyz;

                float3 p0 = tex2Dlod(_PositionBuffer, float4(uvw.x, 0.5, 0, 0)).xyz;
                float4 p1 = UnityObjectToClipPos(float4(p0, 1));
                p1.xy += uvw.yz * _ScreenParams.yx * 0.0001;

                v2f o;
                o.position = p1;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return half4(1, 0, 0, 0.4);
            }

            ENDCG
        }
    }
}

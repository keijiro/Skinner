Shader "Hidden/Skinner/Debug"
{
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

            sampler2D _PreviousPositionBuffer;
            sampler2D _PositionBuffer;
            sampler2D _NormalBuffer;
            sampler2D _TangentBuffer;

            struct appdata
            {
                float4 position : POSITION;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 color : COLOR;
            };

            v2f vert(appdata v)
            {
                float3 uvw = v.position.xyz;
                float4 texcoord = float4(uvw.x, 0.5, 0, 0);

                float3 prev = tex2Dlod(_PreviousPositionBuffer, texcoord).xyz;
                float3 position = tex2Dlod(_PositionBuffer, texcoord).xyz;
                float3 normal = tex2Dlod(_NormalBuffer, texcoord).xyz;
                float3 tangent = tex2Dlod(_TangentBuffer, texcoord).xyz;

                float3 color;

                if (uvw.y < 0.5)
                {
                    // line 0 - Velocity
                    position = position + (position - prev) * uvw.z * 5;
                    color = float3(1, 0, 0);
                }
                else if (uvw.y < 1.5)
                {
                    // line 1 - normal
                    position += normal * uvw.z * 0.05;
                    color = float3(0, 1, 0);
                }
                else
                {
                    // line 2 - tangent
                    position += tangent * uvw.z * 0.05;
                    color = float3(0, 0, 1);
                }

                v2f o;
                o.position = UnityObjectToClipPos(float4(position, 1));
                o.color = color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return half4(i.color, 0.2);
            }

            ENDCG
        }
    }
}

#include "UnityCG.cginc"

sampler2D _MainTex;
float4 _MainTex_TexelSize;

sampler2D _PositionBuffer;
float4 _PositionTex_TexelSize;

float4 UpdatePositionFragment(v2f_img i) : SV_Target
{
    float dv = _MainTex_TexelSize.y;
    if (i.uv.y <= dv)
    {
        return tex2D(_PositionBuffer, float2(i.uv.x, 0.5));
    }
    else
    {
        return tex2D(_MainTex, float2(i.uv.x, i.uv.y - dv));
    }
}

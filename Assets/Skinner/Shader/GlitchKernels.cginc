#include "Common.cginc"

sampler2D _MainTex;
float4 _MainTex_TexelSize;

sampler2D _SourcePositionBuffer;

float4 InitializePositionFragment(v2f_img i) : SV_Target
{
    return (float4)0;
}

float4 UpdatePositionFragment(v2f_img i) : SV_Target
{
    if (i.uv.y < _MainTex_TexelSize.y)
    {
        return tex2D(_SourcePositionBuffer, i.uv);
    }
    else
    {
        return tex2D(_MainTex, i.uv - float2(0, _MainTex_TexelSize.y)) + float4(unity_DeltaTime.x * 0.0, 0, 0, 0);
    }
}

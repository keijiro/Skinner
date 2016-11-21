sampler2D _PositionBuffer;
half3 _Color;

struct Input
{
    half alpha;
};

void vert(inout appdata_full v, out Input data)
{
    UNITY_INITIALIZE_OUTPUT(Input, data);

    float2 uv = v.vertex.xy;

    float3 pos = tex2Dlod(_PositionBuffer, float4(uv, 0, 0));

    v.vertex.xyz = pos;
    data.alpha = 1 - uv.y;
}

void surf(Input IN, inout SurfaceOutputStandard o)
{
    o.Alpha = IN.alpha * 0.5;
    o.Albedo = 0;
    o.Smoothness = 0;
    o.Metallic = 0;
    o.Emission = _Color;
}

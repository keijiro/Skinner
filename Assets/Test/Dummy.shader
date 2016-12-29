Shader "Test/Dummy"
{
    Properties
    {
        _Albedo("Albedo", Color) = (1, 1, 1, 1)
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
        _Metallic("Metallic", Range(0, 1)) = 0.0

        [Space]
        [HDR] _Emission("Emission", Color) = (1, 1, 1)

        [Space]
        _Voxelize("Voxelize", Float) = 0.2
        _Cutoff("Cutoff", Range(0, 1)) = 0.1
    }

    CGINCLUDE

    half4 _Albedo;
    half _Smoothness;
    half _Metallic;
    half3 _Emission;
    float _Voxelize;
    half _Cutoff;

    struct Input
    {
        float3 worldPos;
        fixed facing : VFACE;
    };

    float UVRandom(float2 uv, float salt)
    {
        uv += float2(salt, 0);
        return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
    }

    void vert(inout appdata_full data)
    {
        data.vertex.xyz = floor(data.vertex.xyz / _Voxelize) * _Voxelize;
    }

    void surf(Input IN, inout SurfaceOutputStandard o)
    {
        float3 vp = floor(IN.worldPos.xyz / _Voxelize) * _Voxelize;
        float rnd = UVRandom(vp.xy, vp.z + floor(_Time.y * 5));

        clip(_Cutoff - rnd);

        o.Albedo = _Albedo;
        o.Smoothness = _Smoothness;
        o.Metallic = _Metallic;
        o.Normal = float3(0, 0, IN.facing > 0 ? 1 : -1);
        o.Emission = _Emission;
    }

    ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Standard vertex:vert nolightmap addshadow
        #pragma target 3.0
        ENDCG
    }
}

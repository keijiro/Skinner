// Surface shader for Skinner Glitch
Shader "Skinner/Glitch"
{
    Properties
    {
        _Albedo("Albedo", Color) = (1, 1, 1)
        _Smoothness("Smoothness", Range(0, 1)) = 0
        _Metallic("Metallic", Range(0, 1)) = 0

        [Space]
        _Threshold("Glitch Threshold", Range(0, 5)) = 1

        [HideInInspector] _PositionBuffer("", 2D) = ""{}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Cull Back
        CGPROGRAM
        #pragma surface surf Standard vertex:vert nolightmap addshadow
        #pragma target 3.0
        #include "GlitchSurface.cginc"
        ENDCG

        Cull Front
        CGPROGRAM
        #pragma surface surf Standard vertex:vert nolightmap addshadow
        #pragma target 3.0
        #define NORMAL_FLIP
        #include "GlitchSurface.cginc"
        ENDCG
    }
}

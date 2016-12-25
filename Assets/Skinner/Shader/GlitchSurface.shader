// Surface shader for Skinner Glitch
Shader "Skinner/Glitch"
{
    Properties
    {
        _Albedo("Albedo", Color) = (0.5, 0.5, 0.5)
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
        _Metallic("Metallic", Range(0, 1)) = 0

        [Header(Glitch Thresholds)]
        _EdgeThreshold("Edge Length", Range(0.001, 2)) = 1
        _AreaThreshold("Triangle Area", Range(0.001, 0.1)) = 1

        [Header(Self Illumination)]
        _BaseHue("Base Hue", Range(0, 1)) = 0
        _HueRandomness("Hue Randomness", Range(0, 1)) = 0.2
        _Saturation("Saturation", Range(0, 1)) = 1
        _Brightness("Brightness", Range(0, 6)) = 0.8
        _EmissionProb("Probability", Range(0, 1)) = 0.2

        [Header(Color Animation)]
        _BrightnessOffs("Brightness Offset", Range(0, 6)) = 1.0
        _HueShift("Hue Shift", Range(-1, 1)) = 0.2
        _AttackLength("Duration", Range(0, 1)) = 0.5

        [HideInInspector] _PositionBuffer("", 2D) = ""{}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Cull Back
        CGPROGRAM
        #pragma surface surf Standard vertex:vert nolightmap addshadow
        #pragma target 3.0
        #pragma multi_compile __ UNITY_COLORSPACE_GAMMA
        #include "GlitchSurface.cginc"
        ENDCG

        Cull Front
        CGPROGRAM
        #pragma surface surf Standard vertex:vert nolightmap addshadow
        #pragma target 3.0
        #define NORMAL_FLIP
        #pragma multi_compile __ UNITY_COLORSPACE_GAMMA
        #include "GlitchSurface.cginc"
        ENDCG
    }
}

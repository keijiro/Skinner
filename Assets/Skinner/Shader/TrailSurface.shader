// Surface shader for Skinner Trail
Shader "Skinner/Trail"
{
    Properties
    {
        _Albedo("Albedo", Color) = (0.5, 0.5, 0.5)
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
        _Metallic("Metallic", Range(0, 1)) = 0

        [Header(Line Width Modifier)]
        _SpeedToWidthMin("Min Speed", Float) = 0
        _SpeedToWidthMax("Max Speed", Float) = 4
        _Width("Width At Max Speed", Float) = 0.05

        [Header(Self Illumination)]
        _BaseHue("Base Hue", Range(0, 1)) = 0
        _HueRandomness("Hue Randomness", Range(0, 1)) = 0.2
        _Saturation("Saturation", Range(0, 1)) = 1
        _Brightness("Brightness", Range(0, 6)) = 0.8
        _EmissionProb("Probability", Range(0, 1)) = 0.2

        [Header(Illumination Modifier)]
        _SpeedToLightMin("Min Speed", Float) = 1.5
        _SpeedToLightMax("Max Speed", Float) = 3
        _BrightnessOffs("Brightness Offset", Range(0, 6)) = 1.0
        _HueShift("Hue Shift", Range(-1, 1)) = 0.2

        [HideInInspector] _PositionBuffer("", 2D) = ""{}
        [HideInInspector] _VelocityBuffer("", 2D) = ""{}
        [HideInInspector] _OrthnormBuffer("", 2D) = ""{}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off
        CGPROGRAM
        #pragma surface surf Standard vertex:vert nolightmap addshadow
        #pragma target 3.0
        #pragma multi_compile __ UNITY_COLORSPACE_GAMMA
        #include "TrailSurface.cginc"
        ENDCG
    }
}

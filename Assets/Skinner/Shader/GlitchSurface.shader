Shader "Hidden/Skinner/Glitch/Surface"
{
    Properties
    {
        _PositionBuffer("", 2D) = ""{}
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

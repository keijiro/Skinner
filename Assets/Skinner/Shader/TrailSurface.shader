Shader "Skinner/Trail"
{
    Properties
    {
        _PositionBuffer("", 2D) = ""{}
        _VelocityTex("", 2D) = ""{}
        _BasisBuffer("", 2D) = ""{}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Cull Back
        CGPROGRAM
        #pragma surface surf Standard vertex:vert nolightmap addshadow
        #pragma target 3.0
        #include "TrailSurface.cginc"
        ENDCG

        Cull Front
        CGPROGRAM
        #pragma surface surf Standard vertex:vert nolightmap addshadow
        #pragma target 3.0
        #define NORMAL_FLIP
        #include "TrailSurface.cginc"
        ENDCG
    }
}

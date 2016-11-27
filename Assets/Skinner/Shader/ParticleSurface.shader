Shader "Skinner/Particle"
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
        CGPROGRAM
        #pragma surface surf Standard vertex:vert nolightmap addshadow
        #pragma target 3.0
        #include "ParticleSurface.cginc"
        ENDCG
    }
}

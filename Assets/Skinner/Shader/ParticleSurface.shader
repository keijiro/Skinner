Shader "Skinner/Particle"
{
    Properties
    {
        [HideInInspector] _PositionBuffer("", 2D) = ""{}
        [HDR] _Color("Color", Color) = (1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        CGPROGRAM
        #pragma surface surf Standard vertex:vert nolightmap alpha:fade
        #pragma target 3.0
        #include "ParticleSurface.cginc"
        ENDCG
    }
}

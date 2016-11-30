Shader "Hidden/Skinner/Glitch/Kernels"
{
    Properties
    {
        _MainTex("", 2D) = ""{}
        _SourcePositionBuffer("", 2D) = ""{}
    }
    CGINCLUDE
    #include "GlitchKernels.cginc"
    ENDCG
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment InitializePositionFragment
            #pragma target 3.0
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment UpdatePositionFragment
            #pragma target 3.0
            ENDCG
        }
    }
}

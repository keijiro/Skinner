// Animation kernels for Skinner Glitch
Shader "Hidden/Skinner/Glitch/Kernels"
{
    Properties
    {
        _SourcePositionBuffer0("", 2D) = ""{}
        _SourcePositionBuffer1("", 2D) = ""{}
        _PositionBuffer("", 2D) = ""{}
        _VelocityBuffer("", 2D) = ""{}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment InitializePositionFragment
            #pragma target 3.0
            #include "GlitchKernels.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment InitializeVelocityFragment
            #pragma target 3.0
            #include "GlitchKernels.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment UpdatePositionFragment
            #pragma target 3.0
            #include "GlitchKernels.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment UpdateVelocityFragment
            #pragma target 3.0
            #include "GlitchKernels.cginc"
            ENDCG
        }
    }
}

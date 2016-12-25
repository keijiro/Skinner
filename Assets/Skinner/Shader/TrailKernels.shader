// Animation kernels for Skinner Trail
Shader "Hidden/Skinner/Trail/Kernels"
{
    Properties
    {
        _SourcePositionBuffer0("", 2D) = ""{}
        _SourcePositionBuffer1("", 2D) = ""{}
        _PositionBuffer("", 2D) = ""{}
        _VelocityBuffer("", 2D) = ""{}
        _OrthnormBuffer("", 2D) = ""{}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment InitializePositionFragment
            #pragma target 3.0
            #include "TrailKernels.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment InitializeVelocityFragment
            #pragma target 3.0
            #include "TrailKernels.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment InitializeOrthnormFragment
            #pragma target 3.0
            #include "TrailKernels.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment UpdatePositionFragment
            #pragma target 3.0
            #include "TrailKernels.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment UpdateVelocityFragment
            #pragma target 3.0
            #include "TrailKernels.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment UpdateOrthnormFragment
            #pragma target 3.0
            #include "TrailKernels.cginc"
            ENDCG
        }
    }
}

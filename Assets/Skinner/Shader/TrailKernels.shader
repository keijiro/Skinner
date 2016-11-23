Shader "Hidden/Skinner/Trail/Kernels"
{
    Properties
    {
        _PositionBuffer0("", 2D) = ""{}
        _PositionBuffer1("", 2D) = ""{}
        _VelocityTex("", 2D) = ""{}
    }
    SubShader
    {
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
            #pragma fragment InitializePositionFragment
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
            #pragma fragment UpdatePositionFragment
            #pragma target 3.0
            #include "TrailKernels.cginc"
            ENDCG
        }
    }
}

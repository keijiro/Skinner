// Animation kernels for Skinner Particle
Shader "Hidden/Skinner/Particle/Kernels"
{
    Properties
    {
        _SourcePositionBuffer0("", 2D) = ""{}
        _SourcePositionBuffer1("", 2D) = ""{}
        _PositionBuffer("", 2D) = ""{}
        _VelocityBuffer("", 2D) = ""{}
        _RotationBuffer("", 2D) = ""{}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment InitializePositionFragment
            #pragma target 3.0
            #include "ParticleKernels.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment InitializeVelocityFragment
            #pragma target 3.0
            #include "ParticleKernels.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment InitializeRotationFragment
            #pragma target 3.0
            #include "ParticleKernels.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment UpdatePositionFragment
            #pragma target 3.0
            #include "ParticleKernels.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment UpdateVelocityFragment
            #pragma target 3.0
            #include "ParticleKernels.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment UpdateRotationFragment
            #pragma target 3.0
            #include "ParticleKernels.cginc"
            ENDCG
        }
    }
}

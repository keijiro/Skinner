Shader "Hidden/Skinner/Trail/Kernels"
{
    Properties
    {
        _MainTex("", 2D) = ""{}
        _PositionBuffer("", 2D) = ""{}
    }
    SubShader
    {
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

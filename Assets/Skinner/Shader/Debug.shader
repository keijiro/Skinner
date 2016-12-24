// Skinner/Debug - Debug visualization shader
Shader "Hidden/Skinner/Debug"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "Debug.cginc"
            ENDCG
        }
    }
}

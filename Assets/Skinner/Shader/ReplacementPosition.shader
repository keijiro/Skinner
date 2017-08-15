// Skinner/Replacement - Replacement shader used for vertex baking
Shader "Hidden/Skinner/ReplacementPosition"
{
    SubShader
    {
        Tags { "Skinner" = "Source" }
        Pass
        {
            ZTest Always ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #define SKINNER_POSITION
            #include "Replacement.cginc"
            ENDCG
        }
    }
}

// Skinner/Replacement - Replacement shader used for vertex baking
Shader "Hidden/Skinner/Replacement"
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
            #include "Replacement.cginc"
            ENDCG
        }
    }
}

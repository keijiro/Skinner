Shader "Custom/Body" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

// PRNG function
float UVRandom(float2 uv, float salt)
{
    uv += float2(salt, 0);
    return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
}

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
            clip(UVRandom(floor(o.Normal.xy * 20), floor(o.Normal.z * 20)) - 0.88);
			// Albedo comes from a texture tinted by color
			o.Albedo = 0;
			// Metallic and smoothness come from slider variables
			o.Metallic = 0;
			o.Smoothness = 0;
            o.Emission = float3(0.3, 0.4, 0.9);
		}
		ENDCG
	}
	FallBack "Diffuse"
}

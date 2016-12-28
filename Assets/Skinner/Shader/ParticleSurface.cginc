// Surface shader for Skinner Particle
#include "Common.cginc"

// Buffers from the animation kernels
sampler2D _PositionBuffer;
sampler2D _VelocityBuffer;
sampler2D _RotationBuffer;

// Base material properties
sampler2D _AlbedoMap;
half3 _Albedo;
sampler2D _NormalMap;
half _NormalScale;
half _Smoothness;
half _Metallic;

// Scale modifier
float2 _Scale; // (min, max)

// Self illumination
half _BaseHue;
half _HueRandomness;
half _Saturation;
half _Brightness;
half _EmissionProb;

// Color animation
half _SpeedToLightMin;
half _SpeedToLightMax;
half _HueShift;
half _BrightnessOffs;

struct Input
{
    float2 uv_AlbedoMap;
    fixed4 color : COLOR;
};

void vert(inout appdata_full data)
{
    // Particle ID
    half id = data.texcoord1.x;

    // Fetch the particle position/velocity/rotation.
    float4 p = tex2Dlod(_PositionBuffer, float4(id, 0.5, 0, 0));
    float4 v = tex2Dlod(_VelocityBuffer, float4(id, 0.5, 0, 0));
    float4 r = tex2Dlod(_RotationBuffer, float4(id, 0.5, 0, 0));

    // Life/Speed
    half life = p.w + 0.5;
    half speed = length(v.xyz);

    // Scale animation
    float scale = min((1 - life) * 10, min(life * 3, 1));
    // Scale by the initial speed.
    scale *= min(v.w * _Scale.y, _Scale.x);
    // 50% randomization
    scale *= 1 - 0.5 * UVRandom(id, 20);

    // Low frequency oscillation with half-wave rectified sinusoid.
    half phase = UVRandom(id, 21) * 32 + _Time.y * 4;
    half lfo = abs(sin(phase * UNITY_PI));

    // Switch LFO randomly at zero-cross points.
    lfo *= UVRandom(id + floor(phase), 22) < _EmissionProb;

    // Calculate HSB and convert it to RGB.
    half intensity = smoothstep(_SpeedToLightMin, _SpeedToLightMax, speed);
    half hue = _BaseHue + UVRandom(id, 2) * _HueRandomness + _HueShift * intensity;
    half3 rgb = lerp(1, HueToRGB(hue), _Saturation);
    rgb *= _Brightness * lfo + _BrightnessOffs * intensity;

    // Modify the vertex attributes.
    data.vertex.xyz = RotateVector(data.vertex.xyz, r) * scale + p.xyz;
    data.normal = RotateVector(data.normal, r);
    data.tangent.xyz = RotateVector(data.tangent.xyz, r);
    data.color.rgb = rgb;
}

void surf(Input IN, inout SurfaceOutputStandard o)
{
    o.Albedo = tex2D(_AlbedoMap, IN.uv_AlbedoMap).rgb * _Albedo;
    o.Normal = UnpackScaleNormal(tex2D(_NormalMap, IN.uv_AlbedoMap), _NormalScale);
    o.Smoothness = _Smoothness;
    o.Metallic = _Metallic;
    o.Emission = IN.color.rgb;
}

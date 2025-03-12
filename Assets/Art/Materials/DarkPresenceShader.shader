Shader "Custom/DarkPresenceShader"
{
    Properties
    {
        _Color ("Color", Color) = (0, 0, 0, 0.95) // Black color with 95% opacity
        _NoiseScale ("Noise Scale", Float) = 10.0 // Controls the "swirling" effect
        _NoiseSpeed ("Noise Speed", Float) = 1.0 // Controls how fast the noise moves
        _DarknessIntensity ("Darkness Intensity", Range(0.5, 1.0)) = 0.98 // Controls how dark the presence is
        _EdgeTransparency ("Edge Transparency", Range(0.0, 1.0)) = 0.1 // Controls transparency at the edges
        _FogDensity ("Fog Density", Range(0.0, 1.0)) = 0.5 // Controls the density of the fog
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha // Enable transparency
        ZWrite Off // Disable depth writing for transparency
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1; // World position for fog effect
            };

            float4 _Color;
            float _NoiseScale;
            float _NoiseSpeed;
            float _DarknessIntensity;
            float _EdgeTransparency;
            float _FogDensity;

            // Simple noise function for swirling effect
            float noise(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // Calculate world position
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Add swirling noise effect
                float2 uv = i.uv * _NoiseScale;
                uv.x += _Time.y * _NoiseSpeed; // Animate over time
                uv.y += _Time.y * _NoiseSpeed;

                float n = noise(uv);

                // Increase density by reducing transparency
                float alpha = _Color.a * (0.95 + 0.05 * n); // Less variation, more opaque

                // Add a gradient effect to make it feel thicker in the center
                float distanceFromCenter = length(i.uv - float2(0.5, 0.5)); // Distance from center
                float edgeFade = smoothstep(0.8, 1.0, distanceFromCenter); // Fade only at the very edges
                alpha *= 1.0 - edgeFade * _EdgeTransparency; // Apply edge transparency

                // Darken the color based on intensity
                float3 darkColor = _Color.rgb * _DarknessIntensity;

                // Add fog effect
                float fog = saturate(length(i.worldPos - _WorldSpaceCameraPos) * _FogDensity); // Fog based on distance from camera
                darkColor = lerp(darkColor, _Color.rgb, fog); // Blend with fog color

                return float4(darkColor, alpha);
            }
            ENDCG
        }
    }
}
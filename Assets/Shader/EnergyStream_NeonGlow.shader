Shader "UI/EnergyStream_NeonGlow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        _FillAmount ("Fill Amount", Range(0, 1)) = 1.0
        
        [Header(Glow Intensity)]
        _GlowBoost ("Light Emission", Range(1, 5)) = 2.5 // Boosts brightness > 1
        
        [Header(HDR Colors)]
        // Added [HDR] to allow values higher than 1.0 (Real light brightness)
        [HDR] _ColorBackground ("Background Dark", Color) = (0.0, 0.2, 0.0, 1)
        [HDR] _ColorDark ("Stream Dark", Color) = (0.1, 0.8, 0.1, 1)
        [HDR] _ColorBright ("Stream Bright", Color) = (0.5, 3.0, 0.5, 1) // High Green
        [HDR] _ColorCore ("Core Hot", Color) = (2.0, 2.0, 1.5, 1)       // Very bright White/Yellow
        [HDR] _TipColor ("Tip Flash", Color) = (3.0, 3.0, 3.0, 1)       // Pure White Light
        
        [Header(Flow Settings)]
        _Speed ("Flow Speed", Float) = 3.0
        _StreamScale ("Stream Density", Float) = 7.0
        _WarpStrength ("Wavy Distortion", Range(0, 0.5)) = 0.2
        _WarpFrequency ("Warp Frequency", Float) = 8.0
        
        [Header(Shape Settings)]
        _CoreWidth ("Core Width", Range(0, 1)) = 0.5
        _TipWidth ("Tip Glow Width", Range(0, 0.2)) = 0.1

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            fixed4 _Color;
            float _FillAmount;
            
            float _GlowBoost;
            fixed4 _ColorBackground;
            fixed4 _ColorDark;
            fixed4 _ColorBright;
            fixed4 _ColorCore;
            fixed4 _TipColor;
            
            float _Speed;
            float _StreamScale;
            float _WarpStrength;
            float _WarpFrequency;
            float _CoreWidth;
            float _TipWidth;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            float random (float2 uv) {
                return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123);
            }

            float noise (float2 uv) {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);
                float a = random(i);
                float b = random(i + float2(1.0, 0.0));
                float c = random(i + float2(0.0, 1.0));
                float d = random(i + float2(1.0, 1.0));
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            float fbm(float2 uv) {
                float v = 0.0;
                float a = 0.5;
                for (int i = 0; i < 2; i++) {
                    v += a * noise(uv);
                    uv *= 2.0;
                    a *= 0.5;
                }
                return v;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                
                // 1. FILL MASK
                float fillMask = step(uv.x, _FillAmount);
                if (fillMask <= 0.001) discard;

                // 2. FLOW PHYSICS
                // Use sine wave to distort Y
                float wave = sin(uv.x * _WarpFrequency - _Time.y * 5.0) * _WarpStrength;
                
                float2 streamUV = uv;
                streamUV.y += wave; 
                streamUV.x *= 1.5;         
                streamUV.y *= _StreamScale; 
                streamUV.x -= _Time.y * _Speed;
                
                float flowPattern = fbm(streamUV);
                // Contrast boost for sharper lightning lines
                flowPattern = smoothstep(0.3, 0.9, flowPattern);

                // 3. CORE BEAM
                float centerDist = abs((uv.y + wave * 0.5) - 0.5) * 2.0;
                float coreBeam = 1.0 - centerDist;
                // Make core wider for more glow
                coreBeam = smoothstep(1.0 - _CoreWidth, 1.0, coreBeam);
                
                // 4. COLOR MIXING
                fixed4 finalColor = _ColorBackground;
                
                // Mix background with stream colors
                fixed4 streamColor = lerp(_ColorDark, _ColorBright, flowPattern);
                
                // Add Glow Boost Multiplier here
                streamColor *= _GlowBoost; 
                
                float tubeFade = smoothstep(0.0, 0.3, 1.0 - centerDist);
                finalColor = lerp(finalColor, streamColor, tubeFade);
                
                // Add the Hot Core (Additive Mode for Glow)
                // We add (+) instead of lerp to simulate light accumulation
                finalColor += _ColorCore * coreBeam * flowPattern * _GlowBoost;

                // 5. TIP FLASH
                float tip = 1.0 - abs(uv.x - _FillAmount);
                tip = smoothstep(1.0 - _TipWidth, 1.0, tip);
                finalColor += _TipColor * tip * fillMask * _GlowBoost;

                // 6. OUTPUT
                finalColor.a = 1.0; 
                finalColor *= fillMask; 
                finalColor *= IN.color;

                return finalColor;
            }
            ENDCG
        }
    }
}
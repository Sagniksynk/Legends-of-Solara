Shader "UI/ArmorBar_Flow_Maskable"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        _FillAmount ("Fill Amount", Range(0, 1)) = 1.0
        
        [Header(Glow Settings)]
        _GlowBoost ("Light Emission", Range(1, 5)) = 2.0 
        
        [Header(Blue Armor Colors)]
        [HDR] _ColorBackground ("Background Dark", Color) = (0.0, 0.05, 0.2, 1)
        [HDR] _ColorDark ("Flow Dark", Color) = (0.0, 0.2, 0.8, 1)
        [HDR] _ColorBright ("Flow Bright", Color) = (0.0, 0.8, 1.0, 1)
        [HDR] _ColorCore ("Core Highlight", Color) = (0.8, 0.9, 1.0, 1)
        [HDR] _TipColor ("Tip Flash", Color) = (1.0, 1.0, 1.0, 1)
        
        [Header(Flow Physics)]
        _Speed ("Flow Speed", Float) = 1.5
        _StreamScale ("Stream Smoothness", Float) = 3.0
        _WarpStrength ("Wave Curvature", Range(0, 0.5)) = 0.2
        _WarpFrequency ("Wave Frequency", Float) = 3.0
        
        [Header(Shape Settings)]
        _CoreWidth ("Core Width", Range(0, 1)) = 0.6
        _TipWidth ("Tip Glow Width", Range(0, 0.2)) = 0.05

        // --- MASK SUPPORT PROPERTIES ---
        // These are required for the Mask component to control this shader
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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

        // --- STENCIL BLOCK (Critical for Masks) ---
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
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            // Enable clipping support
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

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

            // Mask Clipping Variables (Auto-filled by Unity)
            float4 _ClipRect;

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
                float wave = sin(uv.x * _WarpFrequency - _Time.y * 3.0) * _WarpStrength;
                
                float2 streamUV = uv;
                streamUV.y += wave; 
                streamUV.x *= 1.0;          
                streamUV.y *= _StreamScale; 
                streamUV.x -= _Time.y * _Speed;
                
                float flowPattern = fbm(streamUV);
                flowPattern = smoothstep(0.1, 0.9, flowPattern);

                // 3. CORE SWOOSH
                float centerDist = abs((uv.y + wave * 0.5) - 0.5) * 2.0;
                float coreBeam = 1.0 - centerDist;
                coreBeam = smoothstep(1.0 - _CoreWidth, 1.0, coreBeam);
                
                // 4. COLOR MIXING
                fixed4 finalColor = _ColorBackground;
                
                fixed4 streamColor = lerp(_ColorDark, _ColorBright, flowPattern);
                streamColor *= _GlowBoost; 
                
                float tubeFade = smoothstep(0.0, 0.4, 1.0 - centerDist);
                finalColor = lerp(finalColor, streamColor, tubeFade);
                
                finalColor += _ColorCore * coreBeam * flowPattern * _GlowBoost * 0.8;

                // 5. TIP FLASH
                float tip = 1.0 - abs(uv.x - _FillAmount);
                tip = smoothstep(1.0 - _TipWidth, 1.0, tip);
                finalColor += _TipColor * tip * fillMask * _GlowBoost;

                // 6. FINAL OUTPUT & MASKING
                finalColor.a = 1.0; 
                finalColor *= fillMask; 
                finalColor *= IN.color;

                // --- CRITICAL MASK CHECK ---
                // This function checks if the pixel is inside the Mask's rectangle
                #ifdef UNITY_UI_CLIP_RECT
                finalColor.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                // Additional Clipping for Alpha
                #ifdef UNITY_UI_ALPHACLIP
                clip (finalColor.a - 0.001);
                #endif

                return finalColor;
            }
            ENDCG
        }
    }
}
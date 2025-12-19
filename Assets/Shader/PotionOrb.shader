Shader "UI/PotionOrb_Complete"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Liquid Colors)]
        _FillAmount ("Fill Amount", Range(0, 1)) = 0.5
        _LiquidColorBottom ("Liquid Bright Bottom", Color) = (1, 0.2, 0, 1) 
        _LiquidColorTop ("Liquid Dark Top", Color) = (0.5, 0, 0, 1)      
        _SurfaceLineColor ("Surface Line Color", Color) = (1, 0.4, 0.4, 0.5)
        
        [Header(Glass and Glow)]
        _EmptyGlassColor ("Empty Glass Dark", Color) = (0.05, 0.05, 0.05, 0.8) 
        _RimGlowColor ("Bottom Glow Color", Color) = (1, 0.6, 0.2, 1) 
        _HighLightColor ("Reflection Spot", Color) = (1, 1, 1, 0.9)
        
        [Header(Wave Settings)]
        _WaveFrequency ("Wave Frequency", Range(0, 20)) = 8
        _WaveAmplitude ("Wave Amplitude", Range(0, 0.1)) = 0.03
        _WaveSpeed ("Wave Speed", Range(0, 5)) = 1.5
        
        [Header(Bubble Settings)]
        _BubbleColor ("Bubble Color", Color) = (1, 0.5, 0.5, 0.2)
        _BubbleDensity ("Bubble Density", Range(0, 1)) = 0.25
        _BubbleSize ("Bubble Size", Range(0, 0.5)) = 0.25
        _BubbleSpeed ("Bubble Speed", Float) = 0.2
        
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
            fixed4 _LiquidColorTop;
            fixed4 _LiquidColorBottom;
            fixed4 _SurfaceLineColor;
            
            fixed4 _EmptyGlassColor;
            fixed4 _RimGlowColor;
            fixed4 _HighLightColor;

            float _WaveFrequency;
            float _WaveAmplitude;
            float _WaveSpeed;
            
            fixed4 _BubbleColor;
            float _BubbleDensity;
            float _BubbleSize;
            float _BubbleSpeed;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            float random (float2 uv)
            {
                return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord - 0.5;
                float dist = length(uv);
                
                float circleAlpha = smoothstep(0.5, 0.495, dist);
                if(circleAlpha <= 0) discard;

                float wave = sin(uv.x * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveAmplitude;
                float currentFill = (_FillAmount - 0.5);
                float surfaceHeight = currentFill + wave;
                
                float liquidMask = smoothstep(surfaceHeight, surfaceHeight - 0.01, uv.y);
                float surfaceLine = smoothstep(surfaceHeight + 0.015, surfaceHeight, uv.y) 
                                  - smoothstep(surfaceHeight, surfaceHeight - 0.01, uv.y);

                float gradientFactor = smoothstep(-0.5, 0.5, uv.y); 
                fixed4 liquidRender = lerp(_LiquidColorBottom, _LiquidColorTop, gradientFactor);

                // Bottom Glow
                float bottomGlow = smoothstep(0.35, 0.5, dist) * liquidMask;
                float verticalFade = smoothstep(0.2, -0.4, uv.y); 
                liquidRender += _RimGlowColor * bottomGlow * verticalFade * 0.8;

                // --- BUBBLES (Big & Adjustable) ---
                float2 bubbleUV = IN.texcoord;
                bubbleUV.y -= _Time.y * _BubbleSpeed;
                bubbleUV.x += sin(IN.texcoord.y * 10.0 + _Time.y) * 0.01;
                
                // Low grid scale = Bigger Bubbles
                float gridScale = 8.0; 
                float2 gridID = floor(bubbleUV * gridScale);
                float2 gridUV = frac(bubbleUV * gridScale);
                
                float rand = random(gridID);
                float bubbleShape = 0;
                
                if(rand < _BubbleDensity) 
                {
                    float bDist = length(gridUV - 0.5);
                    float bSize = _BubbleSize + (rand * 0.1); 
                    bubbleShape = smoothstep(bSize, bSize - 0.05, bDist);
                }
                
                liquidRender += _BubbleColor * bubbleShape * liquidMask;

                fixed4 finalColor = _EmptyGlassColor;
                finalColor = lerp(finalColor, liquidRender, liquidMask);
                finalColor = lerp(finalColor, _SurfaceLineColor, surfaceLine);

                // --- SIDE HIGHLIGHT (Rotated & Thin) ---
                float2 spotCenter = uv - float2(-0.35, 0.25);
                
                float2 rotUV;
                rotUV.x = spotCenter.x * 0.866 - spotCenter.y * 0.5;
                rotUV.y = spotCenter.x * 0.5 + spotCenter.y * 0.866;
                
                rotUV.x *= 5.0; // Very thin
                rotUV.y *= 1.5; // Tall
                
                float glossDist = length(rotUV);
                float highlight = smoothstep(0.12, 0.0, glossDist); 
                
                float rim = smoothstep(0.45, 0.5, dist) * 0.3; 

                finalColor.rgb += _HighLightColor.rgb * highlight;
                finalColor.rgb += rim;

                finalColor.a *= circleAlpha; 
                finalColor *= IN.color;
                
                return finalColor;
            }
            ENDCG
        }
    }
}
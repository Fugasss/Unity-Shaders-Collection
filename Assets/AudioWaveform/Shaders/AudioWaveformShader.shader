Shader "Custom/UI/Unlit/AudioWaveformShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _WaveformTex("Waveform Texture", 2D) = "white" {}
        _WaveformNum("Waveform Number", Float) = 64
        _Progress("Progress", Range(0,1)) = 0.0

        _FilledColor ("Filled Color", Color) = (0.1, 0.8, 0.2, 1.0)
        _UnfilledColor ("Unfilled Color", Color) = (0.22, 0.62, 0.2, 1)
        
        
        // Min-Max should be between [0, 1]
        _MinSampleHeight("Min Sample Height", Float) = 0.0025 
        _MaxSampleHeight("Max Sample Height", Float) = 0.95
        
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
        LOD 100

        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType"="Plane"
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

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _WaveformTex;
            float4 _WaveformTex_ST;

            fixed4 _UnfilledColor, _FilledColor;
            float _Progress;
            float _WaveformNum;

            float _MinSampleHeight;
            float _MaxSampleHeight;
            
            bool _UseClipRect;
            float4 _ClipRect;

            bool _UseAlphaClip;

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            inline fixed4 capsule(in const float2 uv, in const float2 position, in const float2 size)
            {
                float2 halfSize = size / 2.0;
                float2 delta = abs(uv - position);
                float insideBox = step(delta.x, halfSize.x) * step(delta.y, halfSize.y);
                float2 bottom = float2(position.x, position.y - halfSize.y);
                float2 top = float2(position.x, position.y + halfSize.y);
                float inCaps = step(distance(uv, bottom), halfSize.x) + step(distance(uv, top), halfSize.x);

                float alpha = saturate(insideBox + inCaps);
                return fixed4(1, 1, 1, alpha);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color = _UnfilledColor;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif

                float num = _WaveformNum;
                float spacing = 1.0 / (num + 1.0);
                float sizeX = spacing - 0.005;

                float x = i.texcoord.x;

                float capsuleIndex = round(x * (num + 1.0) - 1.0); // rounds to nearest capsule
                capsuleIndex = clamp(capsuleIndex, 0.0, num - 1.0); // ensure within bounds

                if (capsuleIndex / _WaveformNum < _Progress)
                {
                    color.rgb = _FilledColor.rgb;
                }

                float offset = (capsuleIndex + 1.0) / (num + 1.0);
                float2 capsulePos = float2(offset, 0.5);

                float sample = tex2D(_WaveformTex, float2(offset, 0)).r;
                float height = lerp(_MinSampleHeight, _MaxSampleHeight, sample);
                float2 capsuleSize = float2(sizeX, height);

                fixed4 mask = capsule(i.texcoord, capsulePos, capsuleSize);

                return color * mask;
            }
            ENDCG
        }
    }
    Fallback "UI/Default"
}
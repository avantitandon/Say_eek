Shader "UI/ApertureShot"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (0,0,0,1)

        Radius   ("Radius", Float) = 1.2
        Softness ("Softness", Float) = 0.03
        Center   ("Center (UV)", Vector) = (0.5, 0.5, 0, 0)

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
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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
                float2 uv       : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float4 _ClipRect;

            float Radius;
            float Softness;
            float4 Center;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color * _Color;
                o.worldPos = v.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                #ifdef UNITY_UI_CLIP_RECT
                i.color.a *= UnityGet2DClipping(i.worldPos.xy, _ClipRect);
                #endif

                fixed4 tex = tex2D(_MainTex, i.uv);
                float baseA = tex.a * i.color.a;

                float2 d = i.uv - Center.xy;
                float aspect = _ScreenParams.x / _ScreenParams.y;
                d.x *= aspect;

                float dist = length(d);

                float mask = smoothstep(Radius - Softness, Radius + Softness, dist);

                return fixed4(0, 0, 0, baseA * mask);
            }
            ENDHLSL
        }
    }
}

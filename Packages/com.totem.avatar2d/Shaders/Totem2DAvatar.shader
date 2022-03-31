// Based on Unity’s built in renderer’s sprite shader
Shader "Totem/2D Avatar (Unlit Sprite)"
{
    Properties
    {
        [PerRendererData][HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData][HideInInspector] _MaskTex ("Mask Texture", 2D) = "white" {}
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData][HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData][HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0

        [PerRendererData][HideInInspector] _EyesColor ("Eyes Color", Color) = (1,1,1,1)
        [PerRendererData][HideInInspector] _HairColor ("Hair Color", Color) = (1,1,1,1)
        [PerRendererData][HideInInspector] _BodyColor ("Body Color", Color) = (1,1,1,1)

        _Saturation ("Shadow Saturation", Range(0.0, 1.0)) = .2
        _Brightness ("Shadow Brightness", Range(0.0, 1.0)) = .65
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

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA

            #include "UnityCG.cginc"
            #include "Colors.cginc"

            #ifdef UNITY_INSTANCING_ENABLED

            UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
                // SpriteRenderer.Color while Non-Batched/Instanced.
                UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
                // this could be smaller but that's how bit each entry is regardless of type
                UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
            UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

            #define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
            #define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)

            #endif // instancing

            CBUFFER_START(UnityPerDrawSprite)
            #ifndef UNITY_INSTANCING_ENABLED
            fixed4 _RendererColor;
            fixed2 _Flip;
            #endif
            float _EnableExternalAlpha;
            CBUFFER_END

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
            {
                return float4(pos.xy * flip, pos.z, 1.0);
            }

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _RendererColor;

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            fixed3 _EyesColor;
            fixed3 _HairColor;
            fixed3 _BodyColor;

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            sampler2D _MaskTex;

            fixed _Saturation;
            fixed _Brightness;

            fixed4 SampleSpriteTexture(float2 uv)
            {
                fixed4 color = tex2D(_MainTex, uv);

                #if ETC1_EXTERNAL_ALPHA
                fixed4 alpha = tex2D (_AlphaTex, uv);
                color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
                #endif

                return color;
            }

            fixed4 SampleMaskTexture(float2 uv)
            {
                fixed4 color = tex2D(_MaskTex, uv);

                #if ETC1_EXTERNAL_ALPHA
                fixed4 alpha = tex2D (_AlphaTex, uv);
                color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
                #endif

                return color;
            }

            fixed3 ShadeColor(fixed3 baseColor, fixed shadowAmount)
            {
                fixed3 hsv = RGBtoHSV(baseColor);
                hsv.g += _Saturation; // Saturation
                hsv.b -= (1 - _Brightness) * hsv.b; // Value
                hsv.gb = saturate(hsv.gb);

                fixed3 shadowColor = HSVtoRGB(hsv);
                return lerp(baseColor, shadowColor, smoothstep(0, 1, shadowAmount));
            }

            fixed4 SpriteFrag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture(IN.texcoord);
                fixed4 mask = SampleMaskTexture(IN.texcoord);

                // Use texture’s color when not masked
                c.rgb = step(mask.r + mask.g + mask.b, 0) * ShadeColor(c.rgb, mask.a)
                    // Eyes
                    + step(.01, mask.r) * _EyesColor
                    // Hair
                    + step(.01, mask.g) * ShadeColor(_HairColor, mask.a)
                    // Body
                    + step(.01, mask.b) * ShadeColor(_BodyColor, mask.a);

                c *= IN.color; // Tint
                c.rgb *= c.a; // Pre-mult

                return c;
            }
            ENDCG
        }
    }
}
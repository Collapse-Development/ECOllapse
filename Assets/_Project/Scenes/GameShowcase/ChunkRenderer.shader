Shader "Custom/ChunkRenderer"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white"
    }

    SubShader
    {
        // Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        Tags { "LightMode"="SRPDefaultUnlit" }

        Pass
        {
            // ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest Always
            ZWrite Off


            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;

                float4 _Tile;
                sampler2D _Atlas;
                sampler2D _Shapes;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 c = tex2D(_Shapes, IN.uv);
                if (c.a == 0) {return float4(0,0,0,0);}
                float2 a_uv = c.rg;
                a_uv.x = a_uv.x / _Tile.y + _Tile.x;
                return tex2D(_Atlas, a_uv); 
            }
            ENDHLSL
        }

        Pass
        {
            // ColorMask RGB
            // Blend SrcAlpha OneMinusSrcAlpha
            ZTest Always
            ZWrite Off
            Blend Off



            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;

                float2 _TileDepth;
                sampler2D _Shapes;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 c = tex2D(_Shapes, IN.uv);
                clip(c.a - 0.001);
                return float4(
                    tex2D(_Shapes, IN.uv - float2(0, 50.0/75.0)).rgb,
                    tex2D(_Shapes, IN.uv - float2(0, 25.0/75.0)).r * _TileDepth.x  + _TileDepth.y
                );
            }
            ENDHLSL
        }
    }
}

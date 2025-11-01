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

            struct SdfScene {
                uint shapeIdx;
                float4 A;
                float4 B;
                float3 pos;
                float3 viewportSize;
                float2 depthRange;
            };

            struct PlayfieldEnv {
                float3 sun;
                float4 cam;
                float2 projectedCam;
                float3 sceneSize;
                float3 screen;
                float4 rtXY;
                float4 eXUVpx;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;

                StructuredBuffer<SdfScene> _Scene;
                StructuredBuffer<PlayfieldEnv> _Env;
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
                return float4(sin(_Time.x * 40.0) * 0.5 + 0.5,cos(_Time.x * 40.0) * 0.5 + 0.5,0,1);
            }
            ENDHLSL
        }
    }
}

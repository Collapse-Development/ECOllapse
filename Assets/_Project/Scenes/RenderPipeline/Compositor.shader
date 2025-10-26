Shader "Custom/Compositor"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white"
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

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
                sampler2D _Buffer;
                sampler2D _Texture;
                sampler2D _HeightmapTexture;
                sampler2D _HeightmapMipTexture;
                sampler2D _Normals;
                float3 _CameraPos;
                float3 _CameraForward;
                float3 _CameraUp;
                float3 _CameraRight;
                float _CameraZoom;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }


            // todo! use mip octrees
            float hmDist(float3 p) {
                float d4 = p.y - tex2D(_HeightmapMipTexture, p.xz).a;
                if (d4 > 0.0) {return d4;}
                float d3 = p.y - tex2D(_HeightmapMipTexture, p.xz).b;
                if (d3 > 0.0) {return d3;}
                float d2 = p.y - tex2D(_HeightmapMipTexture, p.xz).g;
                if (d2 > 0.0) {return d2;}
                float d1 = p.y - tex2D(_HeightmapMipTexture, p.xz).r;
                if (d1 > 0.0) {return d1;}
                float d0 = p.y - tex2D(_HeightmapTexture, p.xz).r;
                return d0;
            }
            float shadowHm(float3 p, float3 dir) {
                float it = 0;
                float2 _TextureSize = float2(256, 256);
                [loop]
                while (
                    it < 512 
                    && p.x >= -0.00001 && p.x <= _TextureSize.x
                    && p.z >= -0.00001 && p.z <= _TextureSize.y
                ) {
                    float h = tex2D(_HeightmapMipTexture, p.xz / 256.0).r * 256.0;
                    float d = h - p.y;
                    if (d > 0) {
                        return 0.0;
                    }
                    it += 1;
                    p += dir;
                }
                return 1.0;
            }


            float sphereSDF(float3 p, float3 center, float radius) {
                return length(p - center) - radius;
            }

            float sampleScene(float3 p) {
                return sphereSDF(p, float3(128, 20, 128), 30);
            }

            float3 norm(float3 p) {
                float eps = 0.001;
                return normalize(float3(
                    sampleScene(p + float3(eps,0,0)) - sampleScene(p - float3(eps,0,0)),
                    sampleScene(p + float3(0,eps,0)) - sampleScene(p - float3(0,eps,0)),
                    sampleScene(p + float3(0,0,eps)) - sampleScene(p - float3(0,0,eps))
                ));
            }
            
            float rayMarch(float3 ro, float3 rd, int maxSteps, float stepScale) {
                float t = 0.0;
                for(int i=0; i < maxSteps; i++) {
                    float3 p = ro + rd * t;
                    float d = sampleScene(p);
                    if(d < 1.0) return t;
                    t += d * stepScale;
                    if(t > 300.0) break;
                }
                return -1.0;
            }

            half4 frag(Varyings i) : SV_Target
            {
            //     float2 s = _ScreenSize;
            //     float z = _CameraZoom;
            //     s = float2(1920, 1080);
            //     z = 0.1;

            //     float2 px = s * i.uv;
            //     float2 hId = px.xy - s * 0.5;
            //     float3 start =
            //         _CameraPos + 
            //         _CameraRight * hId.x * z
            //         + _CameraUp * hId.y * z;

            //     float3 ray = _CameraForward;
            //     float t = rayMarch(start, ray, 512, 1);
            //     if(t < 0) return float4(0,0,0,1);

            //     float3 pos = start + ray * t;
            //     float3 normal = norm(pos);

            //     float3 lightDir = normalize(float3(-0.5, 1.0, 0.5));
            //     float diff = max(dot(normal, lightDir), 0.1);

            //     return float4(diff, diff, diff, 1.0);

                float2 uv = i.uv * 2.0 - 1.0;
                uv.x *= _ScreenSize.x/_ScreenSize.y;

                float4 buffer = tex2D(_Buffer, i.uv);
                float2 b_uv = buffer.xz * 2.0;
                if (buffer.a == 0.0)
                    return float4(0,0,0,0);

                float3 pos = buffer.xyz * 2.0;           // world-space intersection
                float x = sin(_Time.x * 40.0);
                float y = cos(_Time.x * 120.0) * 0.5 + 0.5 + 0.1;
                float z = cos(_Time.x * 40.0);
                float3 sun = normalize(float3(x, -y, z));

                float3 normal = (tex2D(_Normals, b_uv).rgb - 0.5) * 2.0 * float3(1, 1, 1);
                float shadow = shadowHm(pos * 256.0 + normal * 2, -sun);

                float n = max(dot(normal, sun), 0.0);
                float diff = n * shadow;
                float amb = 0.2;
                float sdw = shadow * (1.0 - amb);
                float3 res = tex2D(_Texture, b_uv).rgb;
                // float3 res = float3(1, 1, 1);
                float3 col = res * amb + res * sdw * float3(1.0, 1.0, 1.0); // white sun
                return float4(col, 1.0);
            }
            ENDHLSL
        }
    }
}

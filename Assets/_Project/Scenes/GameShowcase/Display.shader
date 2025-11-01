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

            struct PlayfieldEnv {
                float4 sun;
                float4 cam;
                float4 projectedCam;
                float4 sceneSize;
                float4 screen;
                float4 rtXY;
                float4 eXUVpx;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;

                float4 _CameraPos;
                float4 _Screen;

                float4 _Tiles;
                
                sampler2D _SceneTexture;
                sampler2D _SceneExtras;
                sampler2D _SdfTexture;
                sampler2D _SdfExtras;

                StructuredBuffer<PlayfieldEnv> _Env;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }
            
            float remap(float x, float a, float b, float c, float d)
            {
                return c + (x - a) * ((d - c) / (b - a));
            }

            float sdSphere(float3 p, float r) {
                return length(p) - r;
            }

            float sdRoundBox( float3 p, float3 b, float r)
            {
                float3 q = abs(p) - b + r;
                return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0) - r;
            }

            float sdCapsule( float3 p, float3 a, float3 b, float r )
            {
                float3 pa = p - a, ba = b - a;
                float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
                return length( pa - ba*h ) - r;
            }

            float sampleScene(float3 p) {
                // return sdRoundBox(p + float3(0.1, 0.1, 0.0), float3(0.2, 0.2, 0.2), 0.0);
                // return sdSphere(p, 0.5);
                return sdCapsule(
                    p,
                    float3(0.0, 0.0, 0.0),
                    float3(0.0, -0.5, 0.0),
                    0.22
                );
            }

            float raymarch(float3 ro, float3 rd, float maxDist, int maxSteps) {
                float t = 0.0;
                for (int i = 0; i < maxSteps; i++) {
                    float3 pos = ro + rd * t;
                    float d = sampleScene(pos);
                    if (d < 0.001) return t;
                    t += d;
                    if (t > maxDist) break;
                }
                return -1.0;
            }

            float3 calcNormal(float3 p) {
                float eps = 0.001;
                float dx = sampleScene(p + float3(eps,0,0)) - sampleScene(p - float3(eps,0,0));
                float dy = sampleScene(p + float3(0,eps,0)) - sampleScene(p - float3(0,eps,0));
                float dz = sampleScene(p + float3(0,0,eps)) - sampleScene(p - float3(0,0,eps));
                return normalize(float3(dx, dy, dz));
            }




            float2 projectToScene(float3 p, float eX, float tY) {
                p.x = remap(p.x, 0.0, 1.0, eX, 1.0);
                p.y = remap(p.y, 0.0, 1.0, 0, tY);
                p.z = remap(p.z, 0.0, 1.0, 0, eX);
                return float2(p.x - p.z, p.y + p.z);
            }

            half4 frag(Varyings i) : SV_Target
            {
                // return tex2D(_SdfTexture, i.uv);
                // return tex2D(_SdfExtras, i.uv);
                // Strength of inner shadows. 1 is off
                float innerEdgeMul = 1.0;
                // Strength of outlines. 0 is off
                float factorMul = 1.0;

                PlayfieldEnv env = _Env[0];

                float t = _Time.x * 40;
                float3 sun = normalize(env.sun);

                float2 uv = (i.uv * 2.0 - 1.0) 
                    // * 0.9;
                    * _CameraPos.w;
                
                // Todo: move constant calcs to CPU.
                float aspect = _Screen.x / _Screen.y;
                uv.x *= aspect;
                
                // float rX = _Tiles.w * _Tiles.x + _Tiles.w * _Tiles.z;
                // float tX = (_Tiles.w * _Tiles.x) / rX;
                // float eX = 1.0 - tX;
                float eX = env.eXUVpx.x;
                float tY = env.rtXY.w;
                // float rY = _Tiles.w * _Tiles.y + _Tiles.w * _Tiles.z;
                // float tY = (_Tiles.w * _Tiles.y) / rY;

                float2 px = env.eXUVpx.yz; // float2(1.0 / rX, 1.0 /rY);

                float2 cam = env.projectedCam;// projectToScene(_CameraPos.xyz, eX, tY);
                
                uv.x = uv.x + cam.x;
                uv.y = uv.y + cam.y;

                // get scene (chunks) info
                float depth = -1.0;
                float3 norm = float3(0.0, 0.0, 0.0);
                float4 scene_col = float4(0.0, 0.0, 0.0, 0.0);




                float nearerFactor = 0;
                float4 nearerCol = float4(0,0,0,0);

                float thresholdCut = 0.027; // 0.027;
                float thresholdMin = 0.009; // 0.009
                float thresholdMax = 0.1;
                float neighborDepth = 0;
                float depthMax = depth;

                float innerScore = 0.0;
                float innerCount = 0.0;
                float innerCut = 0.4;

                if (
                    uv.x <= 1.0
                    && uv.x > 0
                    && uv.y <= 1.0
                    && uv.y > 0
                )  {
                    scene_col = tex2D(_SceneTexture, uv);
                    float4 extras = tex2D(_SceneExtras, uv);
                    depth = extras.a;
                    norm = extras.rgb * 2.0 - 1.0;

                    // KERNELS

                    // int innerOffsetsSize = 4;
                    // float2 innerOffsets[4] = {
                    //     float2(-1, -1), float2(1, -1),
                    //     float2(-1,  1), float2(1,  1)
                    // };

                    // int innerOffsetsSize = 8;
                    // float2 innerOffsets[8] = {
                    //     float2(-1, -1), float2(0, -1), float2(1, -1),
                    //     float2(-1,  0),             float2(1,  0),
                    //     float2(-1,  1), float2(0,  1), float2(1,  1)
                    // };

                    // int innerOffsetsSize = 4;
                    // float2 innerOffsets[4] = {
                    //     float2(-1, -1), float2(0, -1), float2(1, -1),
                    //     /*float2(-1,  0),*/             float2(1,  0),
                    //     //float2(-1,  1), float2(0,  1), float2(1,  1)
                    // };

                    int innerOffsetsSize = 2;
                    float2 innerOffsets[2] = {
                        float2(-1, -1), float2(0, -1), //float2(1, -1),
                        /*float2(-1,  0),*/             //float2(1,  0),
                        //float2(-1,  1), float2(0,  1), float2(1,  1)
                    };

                    // int innerOffsetsSize = 1;
                    // float2 innerOffsets[1] = {
                    //     /*float2(-1, -1), float2(0, -1),*/ /*float2(1, -1),*/
                    //     /*float2(-1,  0),*/             float2(1,  0),
                    //     //float2(-1,  1), float2(0,  1), float2(1,  1)
                    // };

                    for (int i = 0; i < innerOffsetsSize; i++) {
                        float2 o = innerOffsets[i];
                        float2 uvNeighbor = uv + o * px;

                        if (uvNeighbor.x > 0 && uvNeighbor.x < 1 && uvNeighbor.y > 0 && uvNeighbor.y < 1)
                        {
                            float3 neighborNorm = tex2D(_SceneExtras, uvNeighbor).rgb * 2.0 - 1.0;
                            float dp = dot(norm, neighborNorm);
                            innerScore += dp; // todo: norm edges and lights
                            innerCount += 1.0;
                        }
                    }
                    if (innerCount > 0){
                        innerScore /= innerCount;
                        innerScore = 1 - innerScore;
                        if (innerScore < innerCut) {
                            innerScore = 0.0;
                        }
                    }

                    // int innerOffsetsSize = 8;
                    // float2 innerOffsets[8] = {
                    //     float2(-1, -1), float2(0, -1), float2(1, -1),
                    //     float2(-1,  0),             float2(1,  0),
                    //     float2(-1,  1), float2(0,  1), float2(1,  1)
      
                    // };
                    // int innerOffsetsSize = 4;
                    // float2 innerOffsets[4] = {
                    //      float2(-1, -1),                float2(1, -1),
                         
                    //      float2(-1,  1),                float2(1,  1)
                    // };
                    // float depthMin = 0;
                    // for (int i = 0; i < innerOffsetsSize; i++) {
                    //     float2 uvNeighbor = uv + innerOffsets[i] * px;
                    //     float neighborDepth = 0;
                    //     if (
                    //         uvNeighbor.x <= 1.0
                    //         && uvNeighbor.x > 0
                    //         && uvNeighbor.y <= 1.0
                    //         && uvNeighbor.y > 0
                    //     ) {
                    //         float3 neighborNorm = tex2D(_SceneExtras, uvNeighbor).rgb * 2.0 - 1.0;
                    //         int n = 0;
                    //         n += dot(neighborNorm.x, norm.x) > 0.3 ? 0 : 1;
                    //         n += dot(neighborNorm.y, norm.y) > 0.3 ? 0 : 1;
                    //         n += dot(neighborNorm.z, norm.z) > 0.3 ? 0 : 1;
                    //         depthMin += pow(2, n);
                    //     }
                    // }



                    // int offsetsSize = 8;
                    // float2 offsets[8] = {
                    //     float2(-1, -1), float2(0, -1), float2(1, -1),
                    //     float2(-1,  0),             float2(1,  0),
                    //     float2(-1,  1), float2(0,  1), float2(1,  1)
                    // };
                    int offsetsSize = 4;
                    float2 offsets[4] = {
                                       float2(0, -1),
                        float2(-1,  0),             float2(1,  0),
                                       float2(0,  1),
                    };
                    for (int i = 0; i < offsetsSize; i++)
                    {
                        float2 uvNeighbor = uv + offsets[i] * px;
                        if (
                            uvNeighbor.x <= 1.0
                            && uvNeighbor.x > 0
                            && uvNeighbor.y <= 1.0
                            && uvNeighbor.y > 0
                        ) {
                            neighborDepth = tex2D(_SceneExtras, uvNeighbor).a;
                            if (neighborDepth > depthMax)
                            {
                                depthMax = neighborDepth;
                                nearerCol = tex2D(_SceneTexture, uvNeighbor);
                            }
                        }
                    }
                    float d = (depthMax - depth) * factorMul;
                    if (d > thresholdCut)
                        nearerFactor = smoothstep(thresholdMin, thresholdMax, d);
                };

                // return float4(innerScore, 0, 0, 1);


                nearerFactor = sqrt(nearerFactor);

                float2 monster_viewport_half = float2(32, 32);

                float3 monster_pos = float3(0, px.y, 0) * 2.0;
                monster_pos = _CameraPos.xyz + float3(0.0 + px.x * 8, 0.0 - px.y * monster_viewport_half.y * 0.65, 0.0);
                float2 monster_screen = projectToScene(monster_pos, eX, tY);
                float monster_depth = 1.0 - monster_pos.z - 0.02;

                float2 monster_viewport = monster_viewport_half * 2;
                float2 rect_min = 
                    float2(
                        monster_screen.x - px.x * monster_viewport_half.x,
                        monster_screen.y
                    );
                float2 rect_max = float2(
                        monster_screen.x + px.x * monster_viewport_half.x,
                        monster_screen.y + px.y * monster_viewport_half.y * 2.0
                    );

                if (uv.x >= rect_min.x && uv.x <= rect_max.x
                    && uv.y >= rect_min.y && uv.y <= rect_max.y
                    && depth < monster_depth)
                {
                    float2 local_uv = (uv - rect_min) / (rect_max - rect_min);
                    
                    float4x4 eye4 = float4x4( 
                        1., 0., 0., 0.,
                        0., 1., 0., 0.,
                        0., 0., 1., 0.,
                        0., 0., 0, 1.
                    );
                    float4x4 view_mat = float4x4( 
                        1., 0., 0., 0.,
                        0., 1., 0., 0.,
                        0.241, 0.241, 0.5, 0.,
                        0., 0., 0, 1.
                    );
                    // view_mat = float4x4( 
                    //     1., 0., 0., 0.,
                    //     0., 1., 0., 0.,
                    //     0.0, 0.0, 1., 0.,
                    //     0., 0., 0, 1.
                    // );
                    float zd = (1.0 / 32.0) * 32;
                    float far = monster_pos.z + zd;
                    float near = monster_pos.z - zd;
                    float3 start = mul(view_mat, float4(
                        local_uv * 2.0 - 1.0, 
                        0, //near,
                        0
                    )).xyz + float3(0, 0, zd);

                    
                    

                    float3 direction = mul(view_mat, float4(0., 0., -1., 0.)).xyz;

                    float t = raymarch(start, direction, zd * 2.0, 64);

                    if (t > 0.0 && (depthMax < monster_depth)) {
                        return float4(1, 1, 1, 1);
                    }
                    // return float4(depthMax, 0,0,1);
                    // else {
                    //     return float4(0, 0, 0, 0);
                    // }
                }
                


                if (depth < 0.0) {return nearerCol * nearerFactor;}
                
                float amb = 0.1;
                float d = max(0, dot(norm, -sun)) * (1.0 - amb) + amb;
                float3 col = scene_col.rgb * d;
                col = 
                    col * (1.0 - nearerFactor - innerScore) 
                    + (nearerCol * 0.3 * (nearerFactor) * d) 
                    + (col * innerEdgeMul * (innerScore));
                return float4(col, 1.0);
            }
            ENDHLSL
        }
    }
}

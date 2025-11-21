using System;
using System.IO;
using System.Runtime.InteropServices;
using RenderStructs;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.Rendering;



namespace RenderStructs
{
    public struct TileData
    {
        // ShapeIdx 0 is air, shapes start from 1
        public uint shapeIdx;
        public uint textureIdx;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public struct SdfScene
    {
        // for now this is just shapes,
        // in the future it will be scene with primitives and operations

        // ShapeIdx:
        // A.z is offset
        // 0 is Point (sphere defined by offset)
        // 1 is Line, A.xyz is start, B.xyz is end
        // 2 is Box, A.xyz is dims
        // unimplemented:
        // 3 is Cyl
        // 4 is Triangle

        // PrimitiveOp:
        // unimplemented:
        // 0 is Soft add
        // 1 is Soft subtract
        // 2 is Intersection
        // 3 is Interpolation



        public uint shapeIdx;
        public Vector4 A;
        public Vector4 B;
        public Vector3 pos;
        public Vector3 viewportSize;
        public Vector2 depthRange;
    }

    public struct SdfRenderObject
    {
        public SdfScene scene;
        public Mesh mesh;
        public ComputeBuffer buffer;

        public void InitBuffer()
        {
            buffer = new ComputeBuffer(1, System.Runtime.InteropServices.Marshal.SizeOf(typeof(SdfScene)));
        }
        public void Recompute(
            PlayfieldEnv env
        )
        {
            if (buffer == null) InitBuffer();
            Vector2 sceneCenter = RenderUtils.projectToScene(scene.pos, env.eXUVpx.x, env.rtXY.w) * 2f - Vector2.one;
            if (mesh == null)
            {
                mesh = RenderUtils.GenMeshUV(
                    sceneCenter.x + Mathf.Sin(Time.time * 0.1f) * 0.5f,
                    sceneCenter.y,
                    env.eXUVpx.y * scene.viewportSize.x / 2,
                    env.eXUVpx.z * scene.viewportSize.y / 2
                );
            } else {
                var dx = sceneCenter.x + Mathf.Sin(Time.time * 0.1f) * 0.5f;
                var dy = sceneCenter.y;
                var dwH = env.eXUVpx.y * scene.viewportSize.x / 2;
                var dhH = env.eXUVpx.z * scene.viewportSize.y / 2;
                mesh.vertices = new Vector3[]
                {
                    new Vector3(dx - dwH, dy - dhH , 0.5f),
                    new Vector3(dx + dwH, dy - dhH, 0.5f),
                    new Vector3(dx + dwH, dy + dhH, 0.5f),
                    new Vector3(dx - dwH, dy + dhH, 0.5f)
                };
            }
            buffer.SetData(new SdfScene[]{ scene });
        }
    }



    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public struct PlayfieldEnv
    {
        public Vector4 sun;
        public Vector4 cam;
        public Vector4 projectedCam;
        public Vector4 sceneSize; 
        public Vector4 screen; // w h aspect
        public Vector4 rtXY; // rX tX rY tY
        public Vector4 eXUVpx; // eX UVpx

        public PlayfieldEnv(
            Vector3 sun,
            Vector4 cam,
            Vector3 sceneSize,
            Vector2 screenSize,
            Vector2 tileSize,
            uint tileDepth
        )
        {
            this.sun = sun;
            this.cam = cam;
            this.sceneSize = sceneSize;
            // Debug.Log($"Got sun: {sun.x} {sun.y} {sun.z}");
            this.screen = new Vector3(screenSize.x, screenSize.y, screenSize.x / screenSize.y);
            // Debug.Log($"Got scene: {sceneSize.x} {sceneSize.y} {sceneSize.z}");
            // Debug.Log($"Got tile: {tileSize.x} {tileSize.y} {tileDepth}");
            float rX = sceneSize.x * tileSize.x + sceneSize.z * tileDepth;
            float tX = (sceneSize.x * tileSize.x) / rX;
            float eX = 1f - tX;
            float rY = sceneSize.y * tileSize.y + sceneSize.z * tileDepth;
            float tY = (sceneSize.y * tileSize.y) / rY;
            // Debug.Log($"rtXY: {rX} {rY} {tX} {tY}");
            this.rtXY = new Vector4(rX, rY, tX, tY);
            this.eXUVpx = new Vector4(eX, 1f/ rX, 1f/ rY, 0);
            this.projectedCam = RenderUtils.projectToScene(
                new Vector3(cam.x, cam.y, cam.z),
                eX,
                tY
            );
            // this.Print("Inited");
        }

        public void Print(
            string prefix
        )
        {
            Debug.Log($"{prefix}:\n- Sun: {sun.x} {sun.y} {sun.z}\n- Cam: {cam.x} {cam.y} {cam.z} {cam.w}\n- Screen: {screen.x} {screen.y} {screen.z}\n- RT: {rtXY.x} {rtXY.y} {rtXY.z} {rtXY.w}\n- eXUVpx: {eXUVpx.x} {eXUVpx.y} {eXUVpx.z} {eXUVpx.w}");
        }

        public void Update(
            Vector4 cam,
            Vector2 screenSize
        ){
            this.projectedCam = RenderUtils.projectToScene(
                new Vector3(cam.x, cam.y, cam.z),
                this.eXUVpx.x,
                this.rtXY.w
            );
            this.cam = cam;
            this.screen = new Vector3(screenSize.x, screenSize.y, screenSize.x / screenSize.y);
            // this.Print("Updated");
        }
    }
}

public static class RenderUtils
{
    public static void GenerateUVTexture()
    {
        int width = 128;
        int height = 128;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float u = (float)x / (width - 1);
                float v = (float)y / (height - 1);
                tex.SetPixel(x, y, new Color(u, v, 0f, 1f));
            }
        }
        tex.Apply();
        byte[] bytes = tex.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, "_Project/Scenes/GameShowcase/Textures/UV.png");
        File.WriteAllBytes(path, bytes);

        Debug.Log($"Saved UV texture to: {path}");
    }
    

    public static Mesh GenMeshPx(
        // Src
        float sx,
        float sy,
        float sw,
        float sh,
        float srcW,
        float srcH,

        // Dst
        float dx,
        float dy,
        float dw,
        float dh,
        float dstW,
        float dstH
    ){
        var quad = new Mesh();

        float dX = (dx / dstW) * 2f - 1f;
        float dY = 1f - (dy / dstH) * 2f - (dh / dstH) * 2f; // flip Y and offset for height
        float dW = (dw / dstW) * 2f;
        float dH = (dh / dstH) * 2f;
        quad.vertices = new Vector3[]
        {
            new Vector3(dX, dY, 0.5f), // ld
            new Vector3(dX + dW, dY, 0.5f), // rd
            new Vector3(dX + dW, dY + dH, 0.5f), // ru
            new Vector3(dX, dY + dH, 0.5f) // lu
        };

        float sX = sx / srcW;
        float sH = sh / srcH;
        float sY = 1f - sy / srcH - sH;
        float sW = sw / srcW;
        quad.uv = new Vector2[]
        {
            new Vector2(sX, sY),
            new Vector2(sX + sW, sY),
            new Vector2(sX + sW, sY + sH),
            new Vector2(sX, sY + sH)
        };

        // Debug.Log($"Tiles target: {dX}, {dY}, {dW}, {dH}\n{sX} {sY} {sW} {sH}");
        quad.triangles = new int[]
        {
            0, 2, 1,
            0, 3, 2
        };
        return quad;
    }

    public static Mesh GenMeshUV(
        float dx,
        float dy,
        float dwH,
        float dhH
    ){
        var quad = new Mesh();

        quad.vertices = new Vector3[]
        {
            new Vector3(dx - dwH, dy - dhH , 0.5f),
            new Vector3(dx + dwH, dy - dhH, 0.5f),
            new Vector3(dx + dwH, dy + dhH, 0.5f),
            new Vector3(dx - dwH, dy + dhH, 0.5f)
        };

        // Debug.Log($"Sdf target: {dx - dwH}, {dy - dhH} {dx + dwH} {dy + dhH}");
        quad.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };

        quad.triangles = new int[]
        {
            0, 2, 1,
            0, 3, 2
        };
        return quad;
    }


    public static void RenderTiles(
        CommandBuffer cmd,
        RenderTexture target, 
        RenderTexture targetExtras, 
        Material mat, 

        TileData[,,] tiles,

        Texture2D shapes,
        int chunkDim,
        Vector2 tileSize,
        int tileDepth,
        int tilePad,

        ComputeBuffer envBuffer,

        Texture2D atlas,
        int tileTextureSize,
        int tileTexPad,
        uint tileTextureCount
    )
    {   
        mat.SetBuffer("_Env", envBuffer);
        cmd.BeginSample("RenderTilesGPU");
        mat.SetTexture("_Shapes", shapes);
        mat.SetTexture("_Atlas", atlas);
        cmd.Clear();
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        for (int y = chunkDim - 1; y >= 0; y--)
        {
            for (int z = 0; z < chunkDim; z++)
            {
                for (int x = chunkDim - 1; x >= 0; x--)
                {
                    TileData t = tiles[x, chunkDim - y - 1, chunkDim - z - 1];

                    if (t.shapeIdx == 0) continue;

                    float dX = x * tileSize.x + z * tileDepth;
                    float dY = y * tileSize.y + z * tileDepth;
                    Mesh m = GenMeshPx(
                        (tileSize.x + tileDepth + tilePad) * (t.shapeIdx - 1), 0,
                        tileSize.x + tileDepth, tileSize.y + tileDepth,
                        shapes.width, shapes.height,

                        dX, dY,
                        tileSize.x + tileDepth, tileSize.y + tileDepth,
                        target.width, target.height
                    );
                    mpb.SetVector("_Tile", new Vector4(
                        (((float)tileTextureSize + (float)tileTexPad * t.textureIdx)/(float)atlas.width) * t.textureIdx, 
                        tileTextureCount
                    ));
                    cmd.SetRenderTarget(target);
                    cmd.DrawMesh(m, Matrix4x4.identity, mat, 0, 0, mpb);
                    mpb.SetVector("_TileDepth", new Vector4((1f/(float)chunkDim), ((float)z/(float)chunkDim)));
                    cmd.SetRenderTarget(targetExtras);
                    cmd.DrawMesh(m, Matrix4x4.identity, mat, 0, 1, mpb);
                }
            }
        }
        Graphics.ExecuteCommandBuffer(cmd);
        cmd.EndSample("RenderTilesGPU");
    }

    public static float remap(float x, float a, float b, float c, float d)
    {
        return c + (x - a) * ((d - c) / (b - a));
    }

    public static Vector2 projectToScene(
        Vector3 p,
        float eX,
        float tY
    )
    {
        p.x = remap(p.x, 0, 1, eX, 1);
        p.y = remap(p.y, 0, 1, 0, tY);
        p.z = remap(p.z, 0, 1, 0, eX);
        return new Vector2(p.x - p.z, p.y + p.z);
    }

    public static void RenderSdfScenes(
        CommandBuffer cmd,
        RenderTexture target, 
        RenderTexture targetExtras, 
        Material mat, 

        SdfRenderObject[] sdfScenes,

        PlayfieldEnv env,
        ComputeBuffer envBuffer
    ){
        // i've tried almost everything. and this still not rerendering meshes to buffer
        // i spend at least 6 hours on this.

        mat.SetBuffer("_Env", envBuffer);
        cmd.SetRenderTarget(target);
        // cmd.ClearRenderTarget(true, true, Color.black);

        var mpb = new MaterialPropertyBlock();
        foreach (var scene in sdfScenes)
        {
            scene.Recompute(env);
            Mesh m = scene.mesh;
            // m.bounds = new Bounds(Vector3.zero, Vector3.one * 10f);
            mpb.SetBuffer("_Scene", scene.buffer);
            cmd.DrawMesh(m, Matrix4x4.identity, mat, 0, 0, mpb);
        }

        Graphics.ExecuteCommandBuffer(cmd);
        // cmd.Clear();
        // CommandBufferPool.Release(cmd);
    }
}
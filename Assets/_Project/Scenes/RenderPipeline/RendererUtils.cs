using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public static class RendererUtils
{
    public static RenderTexture CreateNormTexture(
        int width, int height
    )
    {
        return CreateMipTexture(width, height);
    }

    public static RenderTexture CreateMipTexture(
        int width, int height
    )
    {
        var desc = new RenderTextureDescriptor(
            width, height,
            GraphicsFormat.R16G16B16A16_SFloat,
            0
        );
        desc.sRGB = false;
        desc.autoGenerateMips = false;
        var tex = new RenderTexture(desc);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.anisoLevel = 1;
        tex.enableRandomWrite = true;
        tex.Create();
        return tex;
    }

    // Todo: array of textures and ?parallel? computation with larger kernels. (maybe2-3 steps with 4 par each)
    public static void ProcessHeightMap(
        Texture2D heightMap,
        RenderTexture tex,
        RenderTexture norm
    ) {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        ComputeShader n = RendererStore.Instance.normalCompute;
        n.SetVector("_FullSize", new Vector4(heightMap.width, heightMap.height, 0, 0));
        n.SetTexture(0, "_NormalTexture", norm);
        n.SetTexture(0, "_HeightmapTexture", heightMap);
        n.Dispatch(
            0,
            heightMap.width / 8,
            heightMap.height / 8,
            1
        );


        ComputeShader s = RendererStore.Instance.mipMaxCompute;
        s.SetVector("_FullSize", new Vector4(heightMap.width, heightMap.height, 0, 0));
        s.SetVector("_FirstMul", new Vector4(0.25f, 0, 0, 0));
        for (int m = 0; m <= 3; m++)
        {
            int k = s.FindKernel($"MipLevel{m}");
            s.SetTexture(k, "_MipMapTexture", tex);
            s.SetTexture(k, "_HeightmapTexture", heightMap);
            s.Dispatch(
                k,
                heightMap.width / 8,
                heightMap.height / 8,
                1
            );
        }

        RenderTexture.active = tex;
        GL.Flush();
        RenderTexture.active = null;

        sw.Stop();
        Debug.Log($"Processing HM took {sw.ElapsedMilliseconds} ms");
    }
}


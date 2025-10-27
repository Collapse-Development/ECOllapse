using System;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RendererCore : MonoBehaviour
{
    RenderTexture mipMaxTexture;
    RenderTexture normalTexture;
    void Start()
    {
        mipMaxTexture = RendererUtils.CreateMipTexture(RendererStore.Instance.heightMap.width, RendererStore.Instance.heightMap.height);
        normalTexture = RendererUtils.CreateNormTexture(RendererStore.Instance.heightMap.width, RendererStore.Instance.heightMap.height);
        RendererUtils.ProcessHeightMap(RendererStore.Instance.heightMap, mipMaxTexture, normalTexture);
        var desc = new RenderTextureDescriptor(Screen.width, Screen.height, GraphicsFormat.R16G16B16A16_SFloat, 0);
        desc.sRGB = false;
        desc.autoGenerateMips = false;
        desc.enableRandomWrite = true;
        RendererStore.Instance.renderTarget = new RenderTexture(desc);
        RendererStore.Instance.renderTarget.filterMode = FilterMode.Point;
        RendererStore.Instance.renderTarget.wrapMode = TextureWrapMode.Clamp;
        RendererStore.Instance.renderTarget.anisoLevel = 1;
        RendererStore.Instance.renderTarget.enableRandomWrite = true;
        RendererStore.Instance.renderTarget.Create();
        keyboard = Keyboard.current;
    }
    public Text debugText;
    Vector3 cameraPos = new Vector3();
    Vector3 cameraForward = new Vector3();
    Vector3 cameraRight = new Vector3();
    Vector3 cameraUp = new Vector3();

    private Keyboard keyboard;
    float t = 0;
    float zoom = 0.4f;
    bool anim = true;
    void Update()
    {
        RendererUtils.ProcessHeightMap(RendererStore.Instance.heightMap, mipMaxTexture, normalTexture);
        if (anim)
            t+= Time.deltaTime;
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            anim = !anim;
        }

        float zoomDt = 0.01f;
        if (keyboard.upArrowKey.wasPressedThisFrame)
            zoom += zoomDt;
        if (keyboard.downArrowKey.wasPressedThisFrame)
            zoom -= zoomDt;

        // t = 0.2f;
        cameraPos.x = Mathf.Sin(t) * 64 + 128;
        cameraPos.y = 128.0f;
        cameraPos.z = Mathf.Cos(t) * 64 + 128;
        cameraForward = (new Vector3(128, 0, 128) - cameraPos).normalized;
        cameraRight = Vector3.Cross(Vector3.up, cameraForward).normalized;
        cameraUp = Vector3.Cross(cameraForward, cameraRight).normalized;

        var w = RendererStore.Instance.renderTarget.width;
        var h = RendererStore.Instance.renderTarget.height;

        var cs = RendererStore.Instance.heightMapDepthCompute;
        cs.SetTexture(0, "_HeightmapTexture", RendererStore.Instance.heightMap);
        cs.SetTexture(0, "_HeightmapMipTexture", mipMaxTexture);
        cs.SetVector(
            "_TextureSize", 
            new Vector4(mipMaxTexture.width, mipMaxTexture.height, 0, 0)
        );
        cs.SetVector("_ScreenSize", new Vector4(w, h, 0, 0));
        cs.SetVector("_CameraPos", cameraPos);
        cs.SetVector("_CameraForward", cameraForward);
        cs.SetVector("_CameraZoom", new Vector4(zoom, 0.0f));
        cs.SetVector("_CameraRight", cameraRight);
        cs.SetVector("_CameraUp", cameraUp);
        cs.SetTexture(0, "_RenderTarget", RendererStore.Instance.renderTarget);
        
        cs.Dispatch(0, w / 8, h / 8, 1);

        var s = RendererStore.Instance.compositorMat;
        s.SetVector("_CameraZoom", new Vector4(zoom, zoom, zoom, zoom));
        s.SetTexture("_Buffer", RendererStore.Instance.renderTarget);
        s.SetTexture("_Texture", RendererStore.Instance.texture);
        s.SetVector("_ScreenSize", new Vector4(w, h, 0, 0));
        s.SetTexture("_Normals", normalTexture);
        s.SetVector("_CameraPos", cameraPos);
        s.SetVector("_CameraForward", cameraForward);
        s.SetVector("_CameraRight", cameraRight);
        s.SetVector("_CameraUp", cameraUp);
        s.SetTexture("_HeightmapTexture", RendererStore.Instance.heightMap);
        s.SetTexture("_HeightmapMipTexture", mipMaxTexture);

        UpdateDbg();
    }

    private int frameSample = 10;
    private float totalDeltaTime = 0f;
    private int frameCount = 0;
    void UpdateDbg(){
        totalDeltaTime += Time.deltaTime;
        frameCount++;
        if (frameCount >= frameSample)
        {
            float avgDeltaTime = totalDeltaTime / frameCount;
            float avgFPS = 1f / avgDeltaTime;
            debugText.text = $"Frame Time: {avgDeltaTime * 1000f:F2} ms\nFPS: {avgFPS:F1}";
            totalDeltaTime = 0f;
            frameCount = 0;
        }
    }
}

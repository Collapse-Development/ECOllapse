using UnityEngine;

public class RendererStore : MonoBehaviour
{
    public static RendererStore Instance { get; private set; }
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Texture2D heightMap;
    public Texture2D texture;
    public RenderTexture renderTarget;
    public ComputeShader mipMaxCompute;
    public ComputeShader normalCompute;
    public ComputeShader heightMapDepthCompute;
    public Shader compositor;
    public Material compositorMat;
}

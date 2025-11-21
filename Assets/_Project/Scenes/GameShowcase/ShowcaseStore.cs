using UnityEngine;
using System.IO;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using RenderStructs;

public class ShowcaseStore : MonoBehaviour
{
    public static ShowcaseStore S { get; private set; }
    void Awake()
    {
        if (S != null && S != this)
        {
            Destroy(gameObject);
            return;
        }

        S = this;
        DontDestroyOnLoad(gameObject);
    }


    public Texture2D tileShapes;
    public static int tileSizeX = 16;
    public static int tileSizeY = 16;
    public static int tileDepth = 8;
    public static uint tileTextureCount = 4;
    public Texture2D tileTextureAtlas;
    public int tileShapePad = 1;
    public Material chunkRendererMat;
    public Material sdfRendererMat;


    private int tileTextureSize = 32;
    int tileTexPad = 0;


    // since we are using 8 bits for float - 32 is maximum depth ( 2^8 color depth / 8 depth px for tile = 32 tiles without loosing precision )
    public static int chunkDim = 12; 
    RenderTexture chunkCol;
    RenderTexture chunkExtras;

    public RenderTexture sdfCol;
    public RenderTexture sdfExtras;

    public Material compositor;

    public TileData[,,] tiles = new TileData[chunkDim, chunkDim, chunkDim];
    public CommandBuffer cmd;
    public CommandBuffer cmdSdf;
    
    public PlayfieldEnv env;
    public ComputeBuffer envBuffer;
    public ComputeBuffer sdfBuffer;

    public UnityEngine.UI.RawImage target;
    public void SetEnv(
        PlayfieldEnv e
    )
    {
        env = e;
        envBuffer = new ComputeBuffer(1, System.Runtime.InteropServices.Marshal.SizeOf(typeof(PlayfieldEnv)));
        envBuffer.SetData(new PlayfieldEnv[]{ e });
    }
    public void UpdateEnv(
        Vector4 cam,
        Vector2 screenSize
    )
    {
        env.Update(
            cam,
            screenSize
        );
        envBuffer.SetData(new PlayfieldEnv[]{ env });
    }

    public void Init()
    {
        int sizeX = tileSizeX * chunkDim + tileDepth * chunkDim;
        int sizeY = tileSizeY * chunkDim + tileDepth * chunkDim;
        var desc = new RenderTextureDescriptor(sizeX, sizeY, GraphicsFormat.R8G8B8A8_SNorm, 0);
        desc.sRGB = false;
        desc.autoGenerateMips = false;
        desc.enableRandomWrite = true;

        chunkCol = new RenderTexture(desc);
        chunkCol.filterMode = FilterMode.Point;
        chunkCol.Create();
        chunkExtras = new RenderTexture(desc);
        chunkExtras.filterMode = FilterMode.Point;
        chunkExtras.Create();

        SetEnv(new PlayfieldEnv(
            new Vector3(-0.25f, -1f, -0.5f),
            new Vector4(),
            new Vector3(chunkDim, chunkDim, chunkDim),
            new Vector3(Screen.width, Screen.height),
            new Vector2(tileSizeX, tileSizeY),
            (uint)tileDepth
        ));

        sdfCol = new RenderTexture(desc);
        sdfCol.filterMode = FilterMode.Point;
        sdfCol.Create();
        sdfExtras = new RenderTexture(desc);
        sdfExtras.filterMode = FilterMode.Point;
        sdfExtras.Create();

        sdfBuffer = new ComputeBuffer(1, System.Runtime.InteropServices.Marshal.SizeOf(typeof(SdfScene)));

        cmd = new CommandBuffer { name = "ChunkRenderer" };
        cmdSdf = new CommandBuffer { name = "SdfRenderer" };
        // Camera.main.AddCommandBuffer(CameraEvent.AfterEverything, cmdSdf);

        RenderUtils.RenderTiles(
            cmd,
            chunkCol,
            chunkExtras,

            chunkRendererMat,
            tiles,

            tileShapes,
            chunkDim,
            new Vector2(tileSizeX, tileSizeY),
            tileDepth,
            tileShapePad,

            envBuffer,

            tileTextureAtlas,
            tileTextureSize,
            tileTexPad,
            tileTextureCount
        );
    }

    void Update()
    {
        compositor.SetBuffer("_Env", envBuffer);
        compositor.SetTexture("_SceneTexture", chunkCol);
        compositor.SetTexture("_SceneExtras", chunkExtras);
        compositor.SetTexture("_SdfTexture", sdfCol);
        compositor.SetTexture("_SdfExtras", sdfExtras);
    }
}

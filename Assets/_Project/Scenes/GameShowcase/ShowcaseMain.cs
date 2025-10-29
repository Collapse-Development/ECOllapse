using System;
using RenderStructs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShowcaseMain : MonoBehaviour
{
    public Texture2D heightmap;
    public GameObject cubePrefab;
    public GameObject player;
    public float spacing = 1f;
    public float heightMultiplier = 5f;



    public float moveSpeed = 5f;
    public float jumpForce = 5f;



    private Rigidbody playerRb;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool isGrounded;
    private Keyboard keyboard;



    void Start()
    {
        SetupScene();
        SetupInput();
    }


    SdfRenderObject[] sdfObjects;
    void Update() // OnRenderObject
    {
        Vector4 cam = new Vector4(
            player.transform.position.x / ShowcaseStore.chunkDim,
            player.transform.position.y / ShowcaseStore.chunkDim,
            player.transform.position.z / ShowcaseStore.chunkDim,
            0.25f
        );

        if (sdfObjects == null) {
            sdfObjects = new SdfRenderObject[]
            {
                new SdfRenderObject{
                    scene = new SdfScene
                    {
                        shapeIdx = 0,
                        pos = cam,
                        viewportSize = new Vector2(20, 20),
                        depthRange = new Vector2(
                            0, 1
                        ),
                        A = new Vector4(0, 0, 0, 8)
                    }
                }
            };
        }

        float w = ShowcaseStore.S.target.rectTransform.rect.width;
        float h = ShowcaseStore.S.target.rectTransform.rect.height;

        Vector4 screen = new Vector4(
            w,
            h,
            w / h
        );
        ShowcaseStore.S.UpdateEnv(cam, screen);

        // RenderUtils.RenderSdfScenes(
        //     ShowcaseStore.S.cmdSdf,
        //     ShowcaseStore.S.sdfCol,
        //     ShowcaseStore.S.sdfExtras,
        //     ShowcaseStore.S.sdfRendererMat,
        //     sdfObjects,
        //     ShowcaseStore.S.env,
        //     ShowcaseStore.S.envBuffer
        // );


        ShowcaseStore.S.compositor.SetVector("_CameraPos", cam);

        ShowcaseStore.S.compositor.SetVector("_Screen", screen);

        ShowcaseStore.S.compositor.SetVector("_Tiles", new Vector4(
            ShowcaseStore.tileSizeX,
            ShowcaseStore.tileSizeY,
            ShowcaseStore.tileDepth,
            ShowcaseStore.chunkDim
        ));
    // }

    // void Update()
    // {
        HandleKeys();
        UpdateDbg();
        UpdateInput();
    }
    

    
    // SCENE

    void SetupScene()
    {
        int width = ShowcaseStore.chunkDim;
        int depth = ShowcaseStore.chunkDim;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float pixelValue = heightmap.GetPixel(x, z).grayscale;
                float cubeHeight = pixelValue * heightMultiplier;
                float yP = Mathf.Ceil(cubeHeight * 3.0f);
                Vector3 position = new Vector3(x * spacing, yP, z * spacing);
                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity, transform);
                cube.layer = 3;

                if (x == width / 2 && z == depth / 2)
                {
                    player.transform.position = position + new Vector3(0, 4, 0);
                }
                float height = (int)yP % ShowcaseStore.chunkDim;
                for (int y = 0; y <= height; y++)
                {
                    ShowcaseStore.S.tiles[
                        x,
                        y,
                        z
                    ] = new TileData{shapeIdx = 1, textureIdx = y == height ? 3u : 2u};
                }
            }
        }
        
        playerRb = player.GetComponent<Rigidbody>();
        player.layer = 1;
        if (playerRb == null)
            playerRb = player.AddComponent<Rigidbody>();

        Destroy(cubePrefab);
    }
    

    
    // INPUTS

    void SetupInput()
    {
        keyboard = Keyboard.current;
    }
    
    void UpdateInput()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
        playerRb.linearVelocity = new Vector3(move.x, playerRb.linearVelocity.y, move.z);
        if (jumpPressed && isGrounded && Time.time - lastJumpTime >= jumpCooldown)
        {
            playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, jumpForce, playerRb.linearVelocity.z);
            lastJumpTime = Time.time;
            jumpPressed = false;
        }
    }

    void FixedUpdate()
    {
        float radius = 0.5f;
        float distance = 2.2f;
        Vector3 origin = player.transform.position;
        isGrounded = Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hit, distance, 1 << 3);
    }

    public float jumpCooldown = 1f;
    private float lastJumpTime = -Mathf.Infinity;
    public void HandleKeys()
    {
        moveInput.x = 
            (keyboard.dKey.isPressed ? 1f : 0f)
            -
            (keyboard.aKey.isPressed ? 1f : 0f)
            ;
        moveInput.y = 
            (keyboard.wKey.isPressed ? 1f : 0f)
            -
            (keyboard.sKey.isPressed ? 1f : 0f)
            ;
        if (keyboard.spaceKey.wasPressedThisFrame)
            jumpPressed = true;
    }



    // DBG

    public Text debugText;
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
            debugText.text = $"Frame Time: {avgDeltaTime * 1000f:F2} ms\nFPS: {avgFPS:F1}\n{isGrounded}";
            totalDeltaTime = 0f;
            frameCount = 0;
        }
    }
}

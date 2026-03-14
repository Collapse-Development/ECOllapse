using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public static BlockSpawner Instance { get; private set; }

    public Texture2D atlas;

    private Dictionary<BlockType, Mesh> meshCache = new Dictionary<BlockType, Mesh>();
    public Material blocksMaterial;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        foreach (var m in meshCache.Values)
        {
            Destroy(m);
        }
        meshCache.Clear();
    }

    public GameObject SpawnBlock(Vector3 position, BlockType type)
    {
        if (!BlockRegistry.Instance)
        {
            Debug.LogError("[BlockSpawner] BlockRegistry.Instance == null!");
            return null;
        }

        var config = BlockRegistry.Instance.Get(type);

        if (config == null)
        {
            Debug.LogError($"[BlockSpawner] No BlockConfig found for {type}");
            return null;
        }

        GameObject go = new GameObject(type.ToString());
        go.transform.position = position;

        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.material = blocksMaterial;

        mf.mesh = GetOrCreateMesh(config);

        return go;
    }

    private Mesh GetOrCreateMesh(BlockConfig config)
    {
        if (meshCache.TryGetValue(config.id, out Mesh cached)) return cached;

        Mesh mesh = BuildCubeMesh(config);
        meshCache[config.id] = mesh;
        return mesh;
    }

    private Mesh BuildCubeMesh(BlockConfig config)
    {
        Mesh mesh = new Mesh();
        float size = 1f;

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0,size,0), new Vector3(size,size,0), new Vector3(size,size,size), new Vector3(0,size,size),
            new Vector3(0,0,0), new Vector3(size,0,0), new Vector3(size,0,size), new Vector3(0,0,size),
            new Vector3(0,0,size), new Vector3(size,0,size), new Vector3(size,size,size), new Vector3(0,size,size),
            new Vector3(size,0,0), new Vector3(size,0,size), new Vector3(size,size,size), new Vector3(size,size,0),
            new Vector3(size,0,0), new Vector3(0,0,0), new Vector3(0,size,0), new Vector3(size,size,0),
            new Vector3(0,0,0), new Vector3(0,0,size), new Vector3(0,size,size), new Vector3(0,size,0)
        };

        int[] triangles = new int[]
        {
            0,2,1, 0,3,2,
            4,5,6, 4,6,7,
            8,9,10, 8,10,11,
            12,14,13, 12,15,14,
            16,17,18, 16,18,19,
            20,21,22, 20,22,23
        };

        float atlasWidth = atlas.width;
        float atlasHeight = -atlas.height;

        float zoneW = 64;
        float zoneH = 48;

        float zoneX = config.idUV.x * zoneW;
        float zoneY = config.idUV.y * zoneH;

        Vector2 uvTop0 = new Vector2((zoneX + 16) / atlasWidth, zoneY / atlasHeight);
        Vector2 uvTop1 = new Vector2((zoneX + 32) / atlasWidth, (zoneY + 16) / atlasHeight);

        Vector2 uvBottom0 = new Vector2((zoneX + 16) / atlasWidth, (zoneY + 32) / atlasHeight);
        Vector2 uvBottom1 = new Vector2((zoneX + 32) / atlasWidth, (zoneY + 48) / atlasHeight);

        Vector2 uvLeft0 = new Vector2(zoneX / atlasWidth, (zoneY + 16) / atlasHeight);
        Vector2 uvLeft1 = new Vector2((zoneX + 16) / atlasWidth, (zoneY + 32) / atlasHeight);

        Vector2 uvBack0 = new Vector2((zoneX + 16) / atlasWidth, (zoneY + 16) / atlasHeight);
        Vector2 uvBack1 = new Vector2((zoneX + 32) / atlasWidth, (zoneY + 32) / atlasHeight);

        Vector2 uvRight0 = new Vector2((zoneX + 32) / atlasWidth, (zoneY + 16) / atlasHeight);
        Vector2 uvRight1 = new Vector2((zoneX + 48) / atlasWidth, (zoneY + 32) / atlasHeight);

        Vector2 uvFront0 = new Vector2((zoneX + 48) / atlasWidth, (zoneY + 16) / atlasHeight);
        Vector2 uvFront1 = new Vector2((zoneX + 64) / atlasWidth, (zoneY + 32) / atlasHeight);

        Vector2[] uvs = new Vector2[24];

        uvs[0] = new Vector2(uvTop0.x, uvTop1.y);
        uvs[1] = new Vector2(uvTop1.x, uvTop1.y);
        uvs[2] = new Vector2(uvTop1.x, uvTop0.y);
        uvs[3] = new Vector2(uvTop0.x, uvTop0.y);

        uvs[4] = new Vector2(uvBottom0.x, uvBottom1.y);
        uvs[5] = new Vector2(uvBottom1.x, uvBottom1.y);
        uvs[6] = new Vector2(uvBottom1.x, uvBottom0.y);
        uvs[7] = new Vector2(uvBottom0.x, uvBottom0.y);

        uvs[20] = new Vector2(uvLeft0.x, uvLeft1.y);
        uvs[21] = new Vector2(uvLeft1.x, uvLeft1.y);
        uvs[22] = new Vector2(uvLeft1.x, uvLeft0.y);
        uvs[23] = new Vector2(uvLeft0.x, uvLeft0.y);

        uvs[8] = new Vector2(uvFront0.x, uvFront1.y);
        uvs[9] = new Vector2(uvFront1.x, uvFront1.y);
        uvs[10] = new Vector2(uvFront1.x, uvFront0.y);
        uvs[11] = new Vector2(uvFront0.x, uvFront0.y);

        uvs[12] = new Vector2(uvRight0.x, uvRight1.y);
        uvs[13] = new Vector2(uvRight1.x, uvRight1.y);
        uvs[14] = new Vector2(uvRight1.x, uvRight0.y);
        uvs[15] = new Vector2(uvRight0.x, uvRight0.y);

        uvs[16] = new Vector2(uvBack0.x, uvBack1.y);
        uvs[17] = new Vector2(uvBack1.x, uvBack1.y);
        uvs[18] = new Vector2(uvBack1.x, uvBack0.y);
        uvs[19] = new Vector2(uvBack0.x, uvBack0.y);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();

        return mesh;
    }
}
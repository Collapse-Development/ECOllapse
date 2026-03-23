using System.Collections.Generic;
using UnityEngine;

public class BlockRegistry : MonoBehaviour
{
    public static BlockRegistry Instance { get; private set; }

    private Dictionary<BlockType, BlockConfig> blocks;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildRegistry();
    }

    void BuildRegistry()
    {
        blocks = new Dictionary<BlockType, BlockConfig>();
        var loaded = Resources.LoadAll<BlockConfig>("Blocks");

        Debug.Log($"[BlockRegistry] Loaded {loaded.Length} BlockConfigs");

        foreach (var block in loaded)
        {
            Debug.Log($"[BlockRegistry] Adding {block.id}");
            blocks[block.id] = block;
        }
    }

    public BlockConfig Get(BlockType type)
    {
        return blocks[type];
    }
}
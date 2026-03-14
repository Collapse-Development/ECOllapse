using UnityEngine;

public class Setup : MonoBehaviour
{
    void Start()
    {
        int x = 0;
        int z = 0;

        foreach (BlockType blockType in System.Enum.GetValues(typeof(BlockType)))
        {
            Vector3 spawnPos = new Vector3(x, 0, z);
            BlockSpawner.Instance.SpawnBlock(spawnPos, blockType);

            x += 2;
            if (x > 8)
            {
                x = 0;
                z += 2;
            }
        }
    }

    void Update()
    {
        
    }
}

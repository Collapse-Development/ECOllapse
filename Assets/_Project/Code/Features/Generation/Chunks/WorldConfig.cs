using UnityEngine;

[CreateAssetMenu(fileName = "WorldConfig", menuName = "Game/World Config")]
public class WorldConfig : ScriptableObject
{
    [Header("Размер чанка (в мировых единицах)")]
    [Range(1, 1000)]
    public float chunkWidth = 100f;

    [Range(1, 1000)]
    public float chunkHeight = 100f;

    [Header("Сетка чанков")]
    [Range(1, 2048)]
    public int chunksX = 360; // по долготе
    [Range(1, 1024)]
    public int chunksY = 180; // по широте

    [Header("Параметры генерации мира")]
    public int seed = 0;
}

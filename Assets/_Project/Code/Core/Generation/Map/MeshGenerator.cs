using System.Collections;
using System.Collections.Generic;
using _Project.Code.Core.Generation.Objects;
using Code.Core.Chunks;
using UnityEngine;

namespace _Project.Code.Core.Generation.Map
{
    public class MeshGenerator : MonoBehaviour
    {
        [Header("References")]
        public Generator worldGenerator;

        [Header("Mesh Settings")]
        public float tileSize = 1f;
        public float heightMultiplier = 10f;
        public Material chunkMaterial;

        [Header("Biome Colors")]
        [SerializeField] private BiomeColor[] biomeColors;

        [Header("Environment Objects")]
        public bool generateEnvironment = true;
        public EnvironmentProfile defaultProfile;   // основной профиль окружения

        [System.Serializable]
        public struct BiomeColor
        {
            public BiomeType type;    
            public Color color;
        }

        [Header("Debug")]
        public bool autoGenerate = true;

        private int chunkSize;

        private Dictionary<BiomeType, Color> biomeColorMap;

        private void Awake()
        {
            biomeColorMap = new Dictionary<BiomeType, Color>();
            foreach (var bc in biomeColors)
                biomeColorMap[bc.type] = bc.color;
        }

        private IEnumerator Start()
        {
            if (autoGenerate)
                yield return GenerateWorldMeshesWhenReady();
        }

        private IEnumerator GenerateWorldMeshesWhenReady()
        {
            if (worldGenerator == null)
            {
                Debug.LogError("ChunkedMeshGenerator: worldGenerator not assigned!");
                yield break;
            }

            while (!worldGenerator.IsGenerated || worldGenerator.Tiles == null || worldGenerator.Chunks.Count == 0)
            {
                yield return null;
            }

            GenerateWorldMeshesInternal();
        }

        public void GenerateWorldMeshes()
        {
            if (worldGenerator == null)
            {
                Debug.LogError("ChunkedMeshGenerator: worldGenerator not assigned!");
                return;
            }

            GenerateWorldMeshesInternal();
        }
        private Color GetBiomeColor(BiomeType type) => biomeColorMap.TryGetValue(type, out var c) ? c : Color.black;
        private void GenerateWorldMeshesInternal()
        {
            var chunks = worldGenerator.Chunks;
            chunkSize = worldGenerator.ChunkSize;
            var tiles = worldGenerator.Tiles;

            Debug.Log($"[WorldMesh] Generating {chunks.Count} chunk meshes...");

            foreach (var kvp in chunks)
            {
                Vector2Int index = kvp.Key;

                GameObject chunkObject = new GameObject($"Chunk_{index.x}_{index.y}");
                chunkObject.transform.parent = transform;
                chunkObject.transform.position = new Vector3(index.x * chunkSize * tileSize, 0, index.y * chunkSize * tileSize);

                MeshFilter mf = chunkObject.AddComponent<MeshFilter>();
                MeshRenderer mr = chunkObject.AddComponent<MeshRenderer>();
                mr.sharedMaterial = chunkMaterial;

                Mesh mesh = new Mesh();
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

                int quads = chunkSize * chunkSize;

                Vector3[] vertices = new Vector3[quads * 4];
                Vector2[] uvs = new Vector2[quads * 4];
                Color[] colors = new Color[quads * 4];
                int[] triangles = new int[quads * 6];

                int vi = 0;
                int ti = 0;

                for (int y = 0; y < chunkSize; y++)
                {
                    for (int x = 0; x < chunkSize; x++)
                    {
                        int gx = index.x * chunkSize + x;
                        int gy = index.y * chunkSize + y;

                        // clamp по миру
                        int gx0 = Mathf.Clamp(gx, 0, tiles.GetLength(0) - 1);
                        int gy0 = Mathf.Clamp(gy, 0, tiles.GetLength(1) - 1);
                        int gx1 = Mathf.Clamp(gx + 1, 0, tiles.GetLength(0) - 1);
                        int gy1 = Mathf.Clamp(gy + 1, 0, tiles.GetLength(1) - 1);

                        // Биом берём из "левого-нижнего" тайла клетки (как обычно для тайловой карты)
                        var t = tiles[gx0, gy0];

                        Color c;

                        switch (t.HeightType)
                        {
                            case HeightType.River:
                                c = GetBiomeColor(BiomeType.River);
                                break;

                            case HeightType.DeepWater:
                                c = GetBiomeColor(BiomeType.Ocean);
                                break;

                            case HeightType.ShallowWater:
                                c = GetBiomeColor(BiomeType.ShallowWater);
                                break;

                            // суша — красим биомом
                            case HeightType.Shore:
                            case HeightType.Sand:
                            case HeightType.Grass:
                            case HeightType.Forest:
                            case HeightType.Rock:
                            case HeightType.Snow:
                            default:
                                c = GetBiomeColor(t.BiomeType);
                                break;
                        }

                        if (t.BiomeType == BiomeType.Lake && t.HeightType!=HeightType.River)
                        {
                            c = GetBiomeColor(BiomeType.Lake);
                        }


                        // Высоты по углам квадрата (чтобы рельеф был непрерывным)
                        float h00 = tiles[gx0, gy0].HeightValue * heightMultiplier;
                        float h10 = tiles[gx1, gy0].HeightValue * heightMultiplier;
                        float h01 = tiles[gx0, gy1].HeightValue * heightMultiplier;
                        float h11 = tiles[gx1, gy1].HeightValue * heightMultiplier;

                        // 4 вершины (уникальные для ЭТОГО тайла)
                        vertices[vi + 0] = new Vector3((x + 0) * tileSize, h00, (y + 0) * tileSize);
                        vertices[vi + 1] = new Vector3((x + 1) * tileSize, h10, (y + 0) * tileSize);
                        vertices[vi + 2] = new Vector3((x + 0) * tileSize, h01, (y + 1) * tileSize);
                        vertices[vi + 3] = new Vector3((x + 1) * tileSize, h11, (y + 1) * tileSize);

                        // Одинаковый цвет на весь тайл => без “размытия”
                        colors[vi + 0] = c;
                        colors[vi + 1] = c;
                        colors[vi + 2] = c;
                        colors[vi + 3] = c;

                        // UV (если не нужны — можно убрать вообще)
                        uvs[vi + 0] = new Vector2(0, 0);
                        uvs[vi + 1] = new Vector2(1, 0);
                        uvs[vi + 2] = new Vector2(0, 1);
                        uvs[vi + 3] = new Vector2(1, 1);

                        // 2 треугольника
                        triangles[ti + 0] = vi + 0;
                        triangles[ti + 1] = vi + 2;
                        triangles[ti + 2] = vi + 1;

                        triangles[ti + 3] = vi + 1;
                        triangles[ti + 4] = vi + 2;
                        triangles[ti + 5] = vi + 3;

                        vi += 4;
                        ti += 6;
                    }
                }

                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.uv = uvs;
                mesh.colors = colors; // vertex colors задаются этим массивом [web:55]
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                SmoothNormalsByPosition(mesh);

                mf.sharedMesh = mesh;

                // --- Генерация объектов окружения для этого чанка ---
                if (generateEnvironment)
                {
                    SpawnEnvironmentObjectsForChunk(chunkObject, index, tiles);
                }
            }

            Debug.Log("[WorldMesh] Generation complete!");
        }


        private void SmoothNormalsByPosition(Mesh mesh)
        {
            var verts = mesh.vertices;
            var norms = mesh.normals;

            var groups = new Dictionary<Vector3, Vector3>(verts.Length);

            // 1) суммируем нормали по каждой позиции
            for (int i = 0; i < verts.Length; i++)
            {
                Vector3 p = verts[i];
                if (groups.TryGetValue(p, out var sum))
                    groups[p] = sum + norms[i];
                else
                    groups[p] = norms[i];
            }

            // 2) нормализуем сумму и присваиваем обратно всем вершинам в этой позиции
            for (int i = 0; i < verts.Length; i++)
            {
                Vector3 p = verts[i];
                norms[i] = groups[p].normalized;
            }

            mesh.normals = norms;
        }

        private void SpawnEnvironmentObjectsForChunk(GameObject chunkObject, Vector2Int chunkIndex, Tile[,] tiles)
        {
            var profile = defaultProfile;
            if (profile == null) return;

            int chunkSize = this.chunkSize;
            float tileSize = this.tileSize;
            float heightMult = heightMultiplier;

            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    // Проверка плотности
                    if (Random.value > profile.density) continue;

                    int gx = chunkIndex.x * chunkSize + x;
                    int gy = chunkIndex.y * chunkSize + y;

                    int gx0 = Mathf.Clamp(gx, 0, tiles.GetLength(0) - 1);
                    int gy0 = Mathf.Clamp(gy, 0, tiles.GetLength(1) - 1);
                    int gx1 = Mathf.Clamp(gx + 1, 0, tiles.GetLength(0) - 1);
                    int gy1 = Mathf.Clamp(gy + 1, 0, tiles.GetLength(1) - 1);

                    Tile centerTile = tiles[gx0, gy0];

                    // Исключаем воду
                    if (centerTile.HeightType == HeightType.DeepWater ||
                        centerTile.HeightType == HeightType.ShallowWater ||
                        centerTile.HeightType == HeightType.River ||
                        centerTile.BiomeType == BiomeType.Lake)
                        continue;

                    // Высоты углов
                    float h00 = tiles[gx0, gy0].HeightValue * heightMult;
                    float h10 = tiles[gx1, gy0].HeightValue * heightMult;
                    float h01 = tiles[gx0, gy1].HeightValue * heightMult;
                    float h11 = tiles[gx1, gy1].HeightValue * heightMult;

                    // Случайная позиция внутри тайла (u,v) в [0,1]
                    float u = Random.value;
                    float v = Random.value;

                    // Билинейная интерполяция высоты
                    float h = (1 - u) * (1 - v) * h00 +
                               u * (1 - v) * h10 +
                               (1 - u) * v * h01 +
                               u * v * h11;

                    // Локальная позиция внутри чанка
                    Vector3 localPos = new Vector3((x + u) * tileSize, h, (y + v) * tileSize);

                    // Вершины тайла (для вычисления нормали)
                    Vector3 v0 = new Vector3(x * tileSize, h00, y * tileSize);
                    Vector3 v1 = new Vector3((x + 1) * tileSize, h10, y * tileSize);
                    Vector3 v2 = new Vector3(x * tileSize, h01, (y + 1) * tileSize);
                    Vector3 v3 = new Vector3((x + 1) * tileSize, h11, (y + 1) * tileSize);

                    // Нормаль в точке
                    Vector3 normal = CalculateNormal(u, v, v0, v1, v2, v3);
                    if (normal.y < 0) normal = -normal; // гарантируем, что нормаль смотрит вверх
                    float angle = Vector3.Angle(normal, Vector3.up);
                    if (angle > profile.maxSlope) continue;

                    // Выбор префаба
                    GameObject prefabToSpawn = (profile.prefabs != null && profile.prefabs.Length > 0)
                        ? profile.prefabs[Random.Range(0, profile.prefabs.Length)]
                        : null;

                    GameObject obj;
                    if (prefabToSpawn != null)
                    {
                        obj = Instantiate(prefabToSpawn, chunkObject.transform);
                    }
                    else
                    {
                        obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        obj.transform.SetParent(chunkObject.transform);
                        Destroy(obj.GetComponent<Collider>()); // убираем коллайдер
                    }

                    obj.transform.localPosition = localPos;

                    // Случайный поворот вокруг вертикальной оси
                    obj.transform.localRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                    // Масштаб
                    float scale = Random.Range(profile.minScale, profile.maxScale);
                    obj.transform.localScale = Vector3.one * scale;

                    // Определение цвета
                    Color color;
                    if (profile.colors != null && profile.colors.Length > 0)
                    {
                        color = profile.colors[Random.Range(0, profile.colors.Length)];
                    }
                    else
                    {
                        color = GetBiomeColor(centerTile.BiomeType);
                    }

                    // Применяем цвет к материалу
                    MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        // Создаём копию материала, чтобы не изменять общий ассет
                        Material mat = new Material(renderer.sharedMaterial ? renderer.sharedMaterial : chunkMaterial);
                        mat.color = color;
                        renderer.material = mat;
                    }
                }
            }
        }

        /// <summary> Вычисляет нормаль в точке (u,v) внутри тайла, заданного четырьмя вершинами. </summary>
        private Vector3 CalculateNormal(float u, float v, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            if (u + v <= 1f)
            {
                // Треугольник v0, v2, v1 (как в меше)
                Vector3 edge1 = v2 - v0;
                Vector3 edge2 = v1 - v0;
                return Vector3.Cross(edge1, edge2).normalized;
            }
            else
            {
                // Треугольник v1, v2, v3 (как в меше)
                Vector3 edge1 = v2 - v1;
                Vector3 edge2 = v3 - v1;
                return Vector3.Cross(edge1, edge2).normalized;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
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

                        // Ѕиом берЄм из "левого-нижнего" тайла клетки (как обычно дл€ тайловой карты)
                        var t = tiles[gx0, gy0];
                        Color c = GetBiomeColor(t.BiomeType);

                        // ¬ысоты по углам квадрата (чтобы рельеф был непрерывным)
                        float h00 = tiles[gx0, gy0].HeightValue * heightMultiplier;
                        float h10 = tiles[gx1, gy0].HeightValue * heightMultiplier;
                        float h01 = tiles[gx0, gy1].HeightValue * heightMultiplier;
                        float h11 = tiles[gx1, gy1].HeightValue * heightMultiplier;

                        // 4 вершины (уникальные дл€ Ё“ќ√ќ тайла)
                        vertices[vi + 0] = new Vector3((x + 0) * tileSize, h00, (y + 0) * tileSize);
                        vertices[vi + 1] = new Vector3((x + 1) * tileSize, h10, (y + 0) * tileSize);
                        vertices[vi + 2] = new Vector3((x + 0) * tileSize, h01, (y + 1) * tileSize);
                        vertices[vi + 3] = new Vector3((x + 1) * tileSize, h11, (y + 1) * tileSize);

                        // ќдинаковый цвет на весь тайл => без Уразмыти€Ф
                        colors[vi + 0] = c;
                        colors[vi + 1] = c;
                        colors[vi + 2] = c;
                        colors[vi + 3] = c;

                        // UV (если не нужны Ч можно убрать вообще)
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
                mesh.colors = colors; // vertex colors задаютс€ этим массивом [web:55]
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                SmoothNormalsByPosition(mesh);

                mf.sharedMesh = mesh;
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

    }
}

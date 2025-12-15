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
        public Gradient colorGradient;

        [Header("Debug")]
        public bool autoGenerate = true;

        private int chunkSize;

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

        private void GenerateWorldMeshesInternal()
        {
            var chunks = worldGenerator.Chunks;
            chunkSize = worldGenerator.ChunkSize;
            var tiles = worldGenerator.Tiles;

            Debug.Log($"[WorldMesh] Generating {chunks.Count} chunk meshes...");

            foreach (var kvp in chunks)
            {
                Vector2Int index = kvp.Key;
                Chunk chunk = kvp.Value;

                GameObject chunkObject = new GameObject($"Chunk_{index.x}_{index.y}");
                chunkObject.transform.parent = transform;
                chunkObject.transform.position = new Vector3(index.x * chunkSize * tileSize, 0, index.y * chunkSize * tileSize);

                MeshFilter mf = chunkObject.AddComponent<MeshFilter>();
                MeshRenderer mr = chunkObject.AddComponent<MeshRenderer>();
                mr.sharedMaterial = chunkMaterial;

                Mesh mesh = new Mesh();
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

                int width = chunkSize + 1;
                int height = chunkSize + 1;

                Vector3[] vertices = new Vector3[width * height];
                Vector2[] uvs = new Vector2[vertices.Length];
                Color[] colors = new Color[vertices.Length];
                List<int> triangles = new List<int>();

                int v = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int globalX = Mathf.Min(index.x * chunkSize + x, tiles.GetLength(0) - 1);
                        int globalY = Mathf.Min(index.y * chunkSize + y, tiles.GetLength(1) - 1);

                        var t = tiles[globalX, globalY];
                        float worldY = t.HeightValue * heightMultiplier;

                        vertices[v] = new Vector3(x * tileSize, worldY, y * tileSize);
                        uvs[v] = new Vector2((float)x / chunkSize, (float)y / chunkSize);
                        colors[v] = colorGradient.Evaluate(t.HeightValue);
                        v++;
                    }
                }

                for (int y = 0; y < chunkSize; y++)
                {
                    for (int x = 0; x < chunkSize; x++)
                    {
                        int topLeft = y * width + x;
                        int topRight = topLeft + 1;
                        int bottomLeft = (y + 1) * width + x;
                        int bottomRight = bottomLeft + 1;

                        triangles.Add(topLeft);
                        triangles.Add(bottomLeft);
                        triangles.Add(topRight);

                        triangles.Add(topRight);
                        triangles.Add(bottomLeft);
                        triangles.Add(bottomRight);
                    }
                }

                mesh.vertices = vertices;
                mesh.triangles = triangles.ToArray();
                mesh.uv = uvs;
                mesh.colors = colors;

                mesh.RecalculateNormals();
                SmoothSharedBorderNormals(mesh, width, height);

                mesh.RecalculateBounds();
                mf.sharedMesh = mesh;
            }

            Debug.Log("[WorldMesh] Generation complete!");
        }

        private void SmoothSharedBorderNormals(Mesh mesh, int width, int height)
        {
            Vector3[] normals = mesh.normals;
            Vector3[] vertices = mesh.vertices;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int i = y * width + x;

                    Vector3 avgNormal = normals[i];
                    int count = 1;

                    if (x > 0) { avgNormal += normals[i - 1]; count++; }
                    if (x < width - 1) { avgNormal += normals[i + 1]; count++; }
                    if (y > 0) { avgNormal += normals[i - width]; count++; }
                    if (y < height - 1) { avgNormal += normals[i + width]; count++; }

                    normals[i] = (avgNormal / count).normalized;
                }
            }

            mesh.normals = normals;
        }
    }
}

using UnityEngine;
using System.Collections;
namespace _Project.Code.Core.Generation.Objects
{
    public class ObjectGenerator : MonoBehaviour
    {
        [Header("Settings")]
        public EnvironmentProfile defaultProfile; // профиль окружения (можно переопределить для разных биомов)

        /// <summary>
        /// Создаёт объекты окружения для указанного чанка.
        /// </summary>
        /// <param name="chunkObject">Родительский объект чанка</param>
        /// <param name="chunkIndex">Координаты чанка в сетке</param>
        /// <param name="tiles">Двумерный массив всех тайлов мира</param>
        /// <param name="chunkSize">Размер чанка в тайлах</param>
        /// <param name="tileSize">Физический размер одного тайла</param>
        /// <param name="heightMultiplier">Множитель высоты</param>
        /// <param name="getBiomeColorFunc">Функция для получения цвета биома (опционально)</param>
        public void SpawnForChunk(GameObject chunkObject, Vector2Int chunkIndex, Tile[,] tiles,
            int chunkSize, float tileSize, float heightMultiplier, System.Func<BiomeType, Color> getBiomeColorFunc = null)
        {
            if (defaultProfile == null)
            {
                Debug.LogWarning("ObjectGenerator: no profile assigned, skipping spawn.");
                return;
            }

            var profile = defaultProfile;

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
                    float h00 = tiles[gx0, gy0].HeightValue * heightMultiplier;
                    float h10 = tiles[gx1, gy0].HeightValue * heightMultiplier;
                    float h01 = tiles[gx0, gy1].HeightValue * heightMultiplier;
                    float h11 = tiles[gx1, gy1].HeightValue * heightMultiplier;

                    // Случайная позиция внутри тайла
                    float u = Random.value;
                    float v = Random.value;

                    float h = (1 - u) * (1 - v) * h00 +
                               u * (1 - v) * h10 +
                               (1 - u) * v * h01 +
                               u * v * h11;

                    Vector3 localPos = new Vector3((x + u) * tileSize, h, (y + v) * tileSize);

                    // Вершины тайла для вычисления нормали
                    Vector3 v0 = new Vector3(x * tileSize, h00, y * tileSize);
                    Vector3 v1 = new Vector3((x + 1) * tileSize, h10, y * tileSize);
                    Vector3 v2 = new Vector3(x * tileSize, h01, (y + 1) * tileSize);
                    Vector3 v3 = new Vector3((x + 1) * tileSize, h11, (y + 1) * tileSize);

                    Vector3 normal = CalculateNormal(u, v, v0, v1, v2, v3);
                    if (normal.y < 0) normal = -normal;
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
                        Destroy(obj.GetComponent<Collider>());
                    }

                    obj.transform.localPosition = localPos;

                    // Поворот
                    Quaternion rotation;
                    if (profile.alignToSlope)
                    {
                        rotation = Quaternion.FromToRotation(Vector3.up, normal) *
                                   Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                    }
                    else
                    {
                        rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                    }
                    obj.transform.localRotation = rotation;

                    // Масштаб
                    float scale = Random.Range(profile.minScale, profile.maxScale);
                    obj.transform.localScale = Vector3.one * scale;

                    // Цвет
                    Color color;
                    if (profile.colors != null && profile.colors.Length > 0)
                    {
                        color = profile.colors[Random.Range(0, profile.colors.Length)];
                    }
                    else if (getBiomeColorFunc != null)
                    {
                        color = getBiomeColorFunc(centerTile.BiomeType);
                    }
                    else
                    {
                        color = Color.white;
                    }

                    MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        Material mat = new Material(renderer.sharedMaterial ? renderer.sharedMaterial : chunkObject.GetComponent<MeshRenderer>()?.sharedMaterial);
                        mat.color = color;
                        renderer.material = mat;
                    }
                }
            }
        }

        private Vector3 CalculateNormal(float u, float v, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            if (u + v <= 1f)
            {
                Vector3 edge1 = v2 - v0;
                Vector3 edge2 = v1 - v0;
                return Vector3.Cross(edge1, edge2).normalized;
            }
            else
            {
                Vector3 edge1 = v2 - v1;
                Vector3 edge2 = v3 - v1;
                return Vector3.Cross(edge1, edge2).normalized;
            }
        }
    }
}

using UnityEngine;

namespace _Project.Code.Core.Generation.Objects
{
    [CreateAssetMenu(fileName = "NewEnvironmentProfile", menuName = "Environment/Profile")]
    public class EnvironmentProfile : ScriptableObject
    {
        public GameObject[] prefabs;                // массив префабов (трава, камни и т.п.)
        public Color[] colors;                       // если не пусто, объекты красятся случайным цветом из списка
        [Range(0f, 1f)] public float density = 0.2f; // вероятность появления на тайле
        public float minScale = 0.2f;                // мин. масштаб
        public float maxScale = 0.8f;                // макс. масштаб
        [Range(0f, 90f)] public float maxSlope = 30f; // максимальный угол наклона (в градусах)
        public bool alignToSlope = true; // выравнивать ли по наклону
    }
}

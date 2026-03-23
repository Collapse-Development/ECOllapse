using UnityEngine;

namespace _Project.Code.Core.Generation.Objects
{
    [CreateAssetMenu(fileName = "NewEnvironmentRule", menuName = "Environment/Rule")]
    public class EnvironmentRule : ScriptableObject
    {
        public GameObject prefab;                     // объект для спавна
        public Color color = Color.white;              // фиксированный цвет (если не нужен, оставьте белым)
        [Range(0f, 1f)] public float density = 0.1f;   // вероятность появления на тайле
        public float minScale = 0.2f;                   // мин. масштаб
        public float maxScale = 0.8f;                   // макс. масштаб
        [Range(0f, 90f)] public float maxSlope = 30f;   // максимальный угол наклона
        public bool alignToSlope = true;               // выравнивать ли по поверхности
    }
}
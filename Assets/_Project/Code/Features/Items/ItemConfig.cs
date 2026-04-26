using UnityEngine;

namespace _Project.Code.Features.Items
{
    [CreateAssetMenu(fileName = "New ItemConfig", menuName = "Scriptable Objects/Items/ItemConfig")]
    public class ItemConfig : ScriptableObject
    {
        [Header("Inventory Stats")]
        public float Weight = 1f;            // Вес (влияет на скорость)
        public int MaxStackSize = 64;        // Сколько влезает в один слот

        [Header("Physical Properties (GDD)")]
        [Range(0, 10000)] public int MaxDurability = 100; // Прочность
        [Range(0f, 1f)] public float Hardness = 0.5f;     // Твёрдость
        [Range(0f, 1f)] public float Flexibility = 0f;    // Гибкость
        [Range(0f, 1f)] public float Flammability = 0f;   // Воспламеняемость
        [Range(0, 1000)] public int Conductivity = 0;     // Проводимость
    }
}
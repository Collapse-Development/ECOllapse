using UnityEngine;

namespace _Project.Code.Features.Character.MB.InventorySystem
{
    /// <summary>
    /// Параметры конкретного предмета в инвентаре: вес единицы и максимальный стак.
    /// Используется в InventoryConfig как значение словаря id → параметры.
    /// </summary>
    [System.Serializable]
    public class InventoryItemConfig
    {
        [Tooltip("Вес одной единицы предмета")]
        public float WeightPerUnit = 1f;

        [Tooltip("Максимальное количество единиц в одном слоте (стак)")]
        public int MaxStackSize = 1;
    }
}

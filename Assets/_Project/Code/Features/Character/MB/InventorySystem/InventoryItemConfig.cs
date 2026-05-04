using UnityEngine;

namespace _Project.Code.Features.Character.MB.InventorySystem
{
    /// <summary>
    /// Параметры конкретного предмета в инвентаре: вес, стак, использование.
    /// </summary>
    [System.Serializable]
    public class InventoryItemConfig
    {
        [Tooltip("Вес одной единицы предмета")]
        public float WeightPerUnit = 1f;

        [Tooltip("Максимальное количество единиц в одном слоте (стак)")]
        public int MaxStackSize = 1;

        [Tooltip("Конфиг предмета — используется как фабрика для создания Item при использовании")]
        public ItemConfig ItemFactory;

        [Tooltip("Расходуется ли предмет при использовании (удаляется 1 единица)")]
        public bool ConsumedOnUse = true;
    }
}

using System;
using System.Collections.Generic;
using _Project.Code.Features.Character.MB;

namespace _Project.Code.Features.Character.MB.InventorySystem
{
    public interface ICharacterInventorySystem : ICharacterSystem
    {
        float MaxWeight { get; }
        float CurrentWeight { get; }

        /// <summary>Текущая нагрузка от 0 до 1 (CurrentWeight / MaxWeight).</summary>
        float WeightRatio { get; }

        float PickupRange { get; }

        IReadOnlyDictionary<string, int> Items { get; }

        /// <summary>
        /// Добавить count единиц предмета. Возвращает результат с количеством добавленных и отклонённых.
        /// </summary>
        InventoryAddResult TryAddItem(Item item, int count = 1);

        /// <summary>Удалить count единиц предмета по id. Возвращает true если удалось.</summary>
        bool RemoveItem(string itemId, int count = 1);

        /// <summary>Получить конфиг предмета по id из InventoryConfig.</summary>
        InventoryItemConfig GetItemConfig(string itemId);

        event Action<string, int> OnItemAdded;
        event Action<string, int> OnItemRemoved;
        event Action<float> OnWeightChanged;
    }
}

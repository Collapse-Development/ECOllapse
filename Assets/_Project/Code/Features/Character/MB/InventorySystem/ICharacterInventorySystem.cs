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

        /// <summary>Упорядоченный список id предметов (слоты инвентаря).</summary>
        IReadOnlyList<string> Slots { get; }

        /// <summary>Индекс активного слота.</summary>
        int ActiveSlotIndex { get; }

        /// <summary>Id предмета в активном слоте. Null если инвентарь пуст.</summary>
        string ActiveItemId { get; }

        /// <summary>
        /// Добавить count единиц предмета. Возвращает результат с количеством добавленных и отклонённых.
        /// </summary>
        InventoryAddResult TryAddItem(Item item, int count = 1);

        /// <summary>Удалить count единиц предмета по id. Возвращает true если удалось.</summary>
        bool RemoveItem(string itemId, int count = 1);

        /// <summary>Получить конфиг предмета по id из InventoryConfig.</summary>
        InventoryItemConfig GetItemConfig(string itemId);

        /// <summary>Переключить активный слот на следующий.</summary>
        void SelectNextSlot();

        /// <summary>Переключить активный слот на предыдущий.</summary>
        void SelectPreviousSlot();

        /// <summary>Выбрать слот по индексу.</summary>
        void SelectSlot(int index);

        /// <summary>Использовать предмет в активном слоте. Возвращает true если предмет был использован.</summary>
        bool UseActiveItem();

        event Action<string, int> OnItemAdded;
        event Action<string, int> OnItemRemoved;
        event Action<float> OnWeightChanged;

        /// <summary>Вызывается при смене активного слота. Передаёт новый индекс и id предмета.</summary>
        event Action<int, string> OnActiveSlotChanged;
    }
}

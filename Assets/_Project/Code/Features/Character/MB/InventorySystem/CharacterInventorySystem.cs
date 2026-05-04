using System;
using System.Collections.Generic;
using UnityEngine;
using _Project.Code.Features.Character.MB;
using _Project.Code.Features.Character.MB.MovementSystem;
using _Project.Code.Features.Character.MB.EnduranceSystem;

namespace _Project.Code.Features.Character.MB.InventorySystem
{
    /// <summary>
    /// Система инвентаря персонажа.
    ///
    /// Эффекты нагрузки (weightRatio = CurrentWeight / MaxWeight):
    ///   Скорость:
    ///     если груз > 50%: Speed% = (-weightRatio*100 + 150)%
    ///     иначе:           Speed% = 100%
    ///
    ///   Восстановление выносливости:
    ///     EnduranceRegen% = sqrt(-100 * weightRatio*100 + 10000)%
    ///     (т.е. sqrt(10000 - 100*%груза), при 0% груза = 100%, при 100% груза = 0%)
    /// </summary>
    public class CharacterInventorySystem : MonoBehaviour, ICharacterInventorySystem
    {
        [SerializeField] private float _maxWeight = 50f;
        [SerializeField] private float _pickupRange = 2f;

        private InventoryConfig _inventoryConfig;
        private Character _character;
        private ICharacterMovementSystem _movementSystem;
        private ICharacterEnduranceSystem _enduranceSystem;

        // itemId → количество единиц
        private readonly Dictionary<string, int> _items = new();
        // упорядоченный список слотов (уникальные id предметов)
        private readonly List<string> _slots = new();
        private float _currentWeight;
        private int _activeSlotIndex;

        public float MaxWeight => _maxWeight;
        public float CurrentWeight => _currentWeight;
        public float WeightRatio => _maxWeight > 0f ? Mathf.Clamp01(_currentWeight / _maxWeight) : 0f;
        public float PickupRange => _pickupRange;
        public IReadOnlyDictionary<string, int> Items => _items;
        public IReadOnlyList<string> Slots => _slots;
        public int ActiveSlotIndex => _activeSlotIndex;
        public string ActiveItemId => _slots.Count > 0 ? _slots[_activeSlotIndex] : null;

        public event Action<string, int> OnItemAdded;
        public event Action<string, int> OnItemRemoved;
        public event Action<float> OnWeightChanged;
        public event Action<int, string> OnActiveSlotChanged;

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterInventorySystemConfig inventoryCfg) return false;

            _character = character;
            if (!_character.TryRegisterSystem<ICharacterInventorySystem>(this)) return false;

            _maxWeight = inventoryCfg.MaxWeight;
            _pickupRange = inventoryCfg.PickupRange;
            _inventoryConfig = inventoryCfg.InventoryConfig;

            _movementSystem = character.GetSystem<ICharacterMovementSystem>();
            _enduranceSystem = character.GetSystem<ICharacterEnduranceSystem>();

            return true;
        }

        public InventoryAddResult TryAddItem(Item item, int count = 1)
        {
            if (item == null || count <= 0) return InventoryAddResult.Rejected(count);

            var itemCfg = GetItemConfig(item.Id);
            if (itemCfg == null)
            {
                Debug.LogWarning($"InventoryConfig не содержит параметров для предмета '{item.Id}'");
                return InventoryAddResult.Rejected(count);
            }

            int currentStack = _items.TryGetValue(item.Id, out var existing) ? existing : 0;
            int stackSpace = itemCfg.MaxStackSize - currentStack;
            if (stackSpace <= 0) return InventoryAddResult.Rejected(count);

            float remainingWeightCapacity = _maxWeight - _currentWeight;
            int byWeight = itemCfg.WeightPerUnit > 0f
                ? Mathf.FloorToInt(remainingWeightCapacity / itemCfg.WeightPerUnit)
                : count;

            int canAdd = Mathf.Min(count, Mathf.Min(stackSpace, byWeight));
            if (canAdd <= 0) return InventoryAddResult.Rejected(count);

            bool isNew = !_items.ContainsKey(item.Id);
            _items[item.Id] = currentStack + canAdd;
            _currentWeight += itemCfg.WeightPerUnit * canAdd;

            if (isNew)
                _slots.Add(item.Id);

            OnItemAdded?.Invoke(item.Id, canAdd);
            OnWeightChanged?.Invoke(_currentWeight);

            return new InventoryAddResult(canAdd, count - canAdd);
        }

        public bool RemoveItem(string itemId, int count = 1)
        {
            if (!_items.TryGetValue(itemId, out int current) || current < count) return false;

            var itemCfg = GetItemConfig(itemId);
            float weightToRemove = itemCfg != null ? itemCfg.WeightPerUnit * count : 0f;

            int remaining = current - count;
            if (remaining <= 0)
            {
                _items.Remove(itemId);
                int slotIdx = _slots.IndexOf(itemId);
                if (slotIdx >= 0)
                {
                    _slots.RemoveAt(slotIdx);
                    // скорректировать активный индекс
                    if (_slots.Count > 0)
                        _activeSlotIndex = Mathf.Clamp(_activeSlotIndex, 0, _slots.Count - 1);
                    else
                        _activeSlotIndex = 0;
                }
            }
            else
            {
                _items[itemId] = remaining;
            }

            _currentWeight = Mathf.Max(0f, _currentWeight - weightToRemove);

            OnItemRemoved?.Invoke(itemId, count);
            OnWeightChanged?.Invoke(_currentWeight);
            return true;
        }

        public InventoryItemConfig GetItemConfig(string itemId)
        {
            return _inventoryConfig != null ? _inventoryConfig.GetItemConfig(itemId) : null;
        }

        public void SelectNextSlot()
        {
            if (_slots.Count == 0) return;
            _activeSlotIndex = (_activeSlotIndex + 1) % _slots.Count;
            OnActiveSlotChanged?.Invoke(_activeSlotIndex, ActiveItemId);
        }

        public void SelectPreviousSlot()
        {
            if (_slots.Count == 0) return;
            _activeSlotIndex = (_activeSlotIndex - 1 + _slots.Count) % _slots.Count;
            OnActiveSlotChanged?.Invoke(_activeSlotIndex, ActiveItemId);
        }

        public void SelectSlot(int index)
        {
            if (_slots.Count == 0) return;
            _activeSlotIndex = Mathf.Clamp(index, 0, _slots.Count - 1);
            OnActiveSlotChanged?.Invoke(_activeSlotIndex, ActiveItemId);
        }

        public bool UseActiveItem()
        {
            string itemId = ActiveItemId;
            if (itemId == null) return false;

            // Создаём временный Item через конфиг для вызова Use()
            // Предполагается, что ItemConfig хранится в InventoryConfig или доступна отдельно.
            // Здесь вызываем Use напрямую через контекст — конкретная логика в подклассе Item.
            var context = new ItemUseContext(_character);

            // Получаем Item из конфига (если ItemConfig зарегистрирован в InventoryConfig)
            var itemCfg = _inventoryConfig != null ? _inventoryConfig.GetItemConfig(itemId) : null;
            if (itemCfg?.ItemFactory == null)
            {
                Debug.LogWarning($"Нет фабрики предмета для '{itemId}', использование невозможно");
                return false;
            }

            var item = itemCfg.ItemFactory.CreateItem();
            item.Use(context);

            if (itemCfg.ConsumedOnUse)
                RemoveItem(itemId, 1);

            return true;
        }

        private void Update()
        {
            ApplyWeightEffects();
        }

        private void ApplyWeightEffects()
        {
            float ratio = WeightRatio;
            float pct = ratio * 100f;

            if (_movementSystem != null)
            {
                float speedMult = pct > 50f ? (-pct + 150f) / 100f : 1f;
                _movementSystem.ApplyFrameSpeedMultiplier(speedMult);
            }

            if (_enduranceSystem != null)
            {
                float regenMult = Mathf.Sqrt(Mathf.Max(0f, -100f * pct + 10000f)) / 100f;
                float regenLoss = _enduranceSystem.MaxValue * (1f - regenMult) * Time.deltaTime * 0.1f;
                if (regenLoss > 0f)
                    _enduranceSystem.ReduceValue(regenLoss);
            }
        }
    }
}

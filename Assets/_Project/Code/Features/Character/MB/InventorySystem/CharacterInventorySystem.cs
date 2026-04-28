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
        private float _currentWeight;

        public float MaxWeight => _maxWeight;
        public float CurrentWeight => _currentWeight;
        public float WeightRatio => _maxWeight > 0f ? Mathf.Clamp01(_currentWeight / _maxWeight) : 0f;
        public float PickupRange => _pickupRange;
        public IReadOnlyDictionary<string, int> Items => _items;

        public event Action<string, int> OnItemAdded;
        public event Action<string, int> OnItemRemoved;
        public event Action<float> OnWeightChanged;

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

            // Сколько влезает по весу
            float remainingWeightCapacity = _maxWeight - _currentWeight;
            int byWeight = itemCfg.WeightPerUnit > 0f
                ? Mathf.FloorToInt(remainingWeightCapacity / itemCfg.WeightPerUnit)
                : count;

            int canAdd = Mathf.Min(count, Mathf.Min(stackSpace, byWeight));
            if (canAdd <= 0) return InventoryAddResult.Rejected(count);

            _items[item.Id] = currentStack + canAdd;
            _currentWeight += itemCfg.WeightPerUnit * canAdd;

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
                _items.Remove(itemId);
            else
                _items[itemId] = remaining;

            _currentWeight = Mathf.Max(0f, _currentWeight - weightToRemove);

            OnItemRemoved?.Invoke(itemId, count);
            OnWeightChanged?.Invoke(_currentWeight);
            return true;
        }

        public InventoryItemConfig GetItemConfig(string itemId)
        {
            return _inventoryConfig != null ? _inventoryConfig.GetItemConfig(itemId) : null;
        }

        private void Update()
        {
            ApplyWeightEffects();
        }

        /// <summary>
        /// Применяет эффекты нагрузки каждый кадр через ApplyFrameSpeedMultiplier
        /// и модифицирует восстановление выносливости.
        /// </summary>
        private void ApplyWeightEffects()
        {
            float ratio = WeightRatio; // 0..1
            float pct = ratio * 100f;  // 0..100

            // --- Скорость ---
            if (_movementSystem != null)
            {
                float speedMult = pct > 50f ? (-pct + 150f) / 100f : 1f;
                _movementSystem.ApplyFrameSpeedMultiplier(speedMult);
            }

            // --- Восстановление выносливости ---
            // EnduranceRegen% = sqrt(-100 * pct + 10000) / 100
            if (_enduranceSystem != null)
            {
                float regenMult = Mathf.Sqrt(Mathf.Max(0f, -100f * pct + 10000f)) / 100f;

                // КОСТЫЛЬ: ICharacterEnduranceSystem не предоставляет способа масштабировать
                // скорость восстановления напрямую (нет аналога ApplyFrameSpeedMultiplier).
                // Вместо этого мы вычитаем "потерянный реген" через ReduceValue каждый кадр.
                // Коэффициент 0.1f подобран эмпирически, чтобы эффект был ощутимым, но не
                // перекрывал базовое восстановление полностью при средней нагрузке.
                // Правильное решение: добавить ApplyFrameRegenMultiplier в ICharacterEnduranceSystem
                // по аналогии с ICharacterMovementSystem.ApplyFrameSpeedMultiplier.
                float regenLoss = _enduranceSystem.MaxValue * (1f - regenMult) * Time.deltaTime * 0.1f;
                if (regenLoss > 0f)
                    _enduranceSystem.ReduceValue(regenLoss);
            }
        }
    }
}

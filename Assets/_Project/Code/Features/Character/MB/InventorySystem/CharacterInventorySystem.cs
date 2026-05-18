using System;
using System.Collections.Generic;
using _Project.Code.Features.Character.MB.EnduranceSystem;
using _Project.Code.Features.Character.MB.MovementSystem;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.InventorySystem
{
    [Serializable]
    public class InventoryStack
    {
        public InventoryStack(string itemId, int count)
        {
            ItemId = itemId;
            Count = count;
        }

        public string ItemId { get; }
        public int Count { get; private set; }

        public void Add(int count)
        {
            Count += count;
        }

        public void Remove(int count)
        {
            Count -= count;
        }
    }

    public class CharacterInventorySystem : MonoBehaviour, ICharacterInventorySystem
    {
        [SerializeField] private float _maxWeight = 50f;
        [SerializeField] private float _pickupRange = 2f;

        private InventoryConfig _inventoryConfig;
        private Character _character;
        private ICharacterMovementSystem _movementSystem;
        private ICharacterEnduranceSystem _enduranceSystem;

        private readonly Dictionary<string, int> _items = new();
        private readonly List<InventoryStack> _stacks = new();
        private readonly List<string> _slots = new();
        private float _currentWeight;
        private int _activeSlotIndex;

        public float MaxWeight => _maxWeight;
        public float CurrentWeight => _currentWeight;
        public float WeightRatio => _maxWeight > 0f ? Mathf.Clamp01(_currentWeight / _maxWeight) : 0f;
        public float PickupRange => _pickupRange;
        public IReadOnlyDictionary<string, int> Items => _items;
        public IReadOnlyList<string> Slots => _slots;
        public IReadOnlyList<InventoryStack> Stacks => _stacks;
        public int ActiveSlotIndex => _activeSlotIndex;
        public string ActiveItemId => _stacks.Count > 0 ? _stacks[_activeSlotIndex].ItemId : null;

        public event Action<string, int> OnItemAdded;
        public event Action<string, int> OnItemRemoved;
        public event Action<float> OnWeightChanged;
        public event Action<int, string> OnActiveSlotChanged;

        public bool TryRegister(Character character)
        {
            _character = character;
            return _character.TryRegisterSystem<ICharacterInventorySystem>(this);
        }

        public bool TryInitialize(Character character, CharacterSystemConfig cfg)
        {
            if (cfg is not CharacterInventorySystemConfig inventoryCfg) return false;

            _maxWeight = inventoryCfg.MaxWeight;
            _pickupRange = inventoryCfg.PickupRange;
            _inventoryConfig = inventoryCfg.InventoryConfig;

            if (_inventoryConfig != null && _maxWeight <= 0f)
                _maxWeight = _inventoryConfig.MaxWeight;

            return true;
        }

        public bool TryResolveDependencies(Character character)
        {
            _movementSystem = character.GetSystem<ICharacterMovementSystem>();
            _enduranceSystem = character.GetSystem<ICharacterEnduranceSystem>();
            return true;
        }

        public InventoryAddResult TryAddItem(Item item, int count = 1)
        {
            if (item == null || item.Config == null || count <= 0)
                return InventoryAddResult.Rejected(Mathf.Max(0, count));

            var itemCfg = GetItemConfig(item.Id);
            if (itemCfg == null)
            {
                Debug.LogWarning($"InventoryConfig does not contain item '{item.Id}'.");
                return InventoryAddResult.Rejected(count);
            }

            int remaining = Mathf.Min(count, GetWeightCapacityFor(itemCfg));
            int added = 0;

            if (remaining <= 0)
                return InventoryAddResult.Rejected(count);

            foreach (var stack in _stacks)
            {
                if (remaining <= 0) break;
                if (stack.ItemId != item.Id) continue;

                int stackSpace = itemCfg.MaxStackSize - stack.Count;
                if (stackSpace <= 0) continue;

                int toAdd = Mathf.Min(stackSpace, remaining);
                stack.Add(toAdd);
                remaining -= toAdd;
                added += toAdd;
            }

            while (remaining > 0)
            {
                int toAdd = Mathf.Min(itemCfg.MaxStackSize, remaining);
                _stacks.Add(new InventoryStack(item.Id, toAdd));
                remaining -= toAdd;
                added += toAdd;
            }

            if (added <= 0)
                return InventoryAddResult.Rejected(count);

            _currentWeight += itemCfg.Weight * added;
            RebuildViews();

            OnItemAdded?.Invoke(item.Id, added);
            OnWeightChanged?.Invoke(_currentWeight);
            OnActiveSlotChanged?.Invoke(_activeSlotIndex, ActiveItemId);

            return new InventoryAddResult(added, count - added);
        }

        public bool RemoveItem(string itemId, int count = 1)
        {
            if (string.IsNullOrWhiteSpace(itemId) || count <= 0) return false;
            if (!_items.TryGetValue(itemId, out int current) || current < count) return false;

            var itemCfg = GetItemConfig(itemId);
            int remaining = count;

            for (var i = _stacks.Count - 1; i >= 0 && remaining > 0; i--)
            {
                var stack = _stacks[i];
                if (stack.ItemId != itemId) continue;

                int toRemove = Mathf.Min(stack.Count, remaining);
                stack.Remove(toRemove);
                remaining -= toRemove;

                if (stack.Count <= 0)
                    _stacks.RemoveAt(i);
            }

            float weightToRemove = itemCfg != null ? itemCfg.Weight * count : 0f;
            _currentWeight = Mathf.Max(0f, _currentWeight - weightToRemove);

            RebuildViews();

            OnItemRemoved?.Invoke(itemId, count);
            OnWeightChanged?.Invoke(_currentWeight);
            OnActiveSlotChanged?.Invoke(_activeSlotIndex, ActiveItemId);
            return true;
        }

        public ItemConfig GetItemConfig(string itemId)
        {
            return _inventoryConfig != null ? _inventoryConfig.GetItemConfig(itemId) : null;
        }

        public void SelectNextSlot()
        {
            if (_stacks.Count == 0) return;
            _activeSlotIndex = (_activeSlotIndex + 1) % _stacks.Count;
            OnActiveSlotChanged?.Invoke(_activeSlotIndex, ActiveItemId);
        }

        public void SelectPreviousSlot()
        {
            if (_stacks.Count == 0) return;
            _activeSlotIndex = (_activeSlotIndex - 1 + _stacks.Count) % _stacks.Count;
            OnActiveSlotChanged?.Invoke(_activeSlotIndex, ActiveItemId);
        }

        public void SelectSlot(int index)
        {
            if (_stacks.Count == 0) return;
            _activeSlotIndex = Mathf.Clamp(index, 0, _stacks.Count - 1);
            OnActiveSlotChanged?.Invoke(_activeSlotIndex, ActiveItemId);
        }

        public bool UseActiveItem()
        {
            string itemId = ActiveItemId;
            if (itemId == null) return false;

            var itemCfg = GetItemConfig(itemId);
            if (itemCfg == null)
            {
                Debug.LogWarning($"Item config for '{itemId}' not found, item cannot be used.");
                return false;
            }

            if (!itemCfg.CanUse) return false;

            var item = itemCfg.CreateItem();
            if (!item.Use(new ItemUseContext(_character))) return false;

            if (itemCfg.ConsumedOnUse)
                RemoveItem(itemId, 1);

            return true;
        }

        private int GetWeightCapacityFor(ItemConfig itemCfg)
        {
            if (itemCfg.Weight <= 0f) return int.MaxValue;

            float remainingWeightCapacity = _maxWeight - _currentWeight;
            return Mathf.Max(0, Mathf.FloorToInt(remainingWeightCapacity / itemCfg.Weight));
        }

        private void RebuildViews()
        {
            _items.Clear();
            _slots.Clear();

            foreach (var stack in _stacks)
            {
                if (!_items.TryAdd(stack.ItemId, stack.Count))
                    _items[stack.ItemId] += stack.Count;

                _slots.Add(stack.ItemId);
            }

            _activeSlotIndex = _stacks.Count > 0
                ? Mathf.Clamp(_activeSlotIndex, 0, _stacks.Count - 1)
                : 0;
        }

        private void Update()
        {
            ApplyWeightEffects();
        }

        private void ApplyWeightEffects()
        {
            float pct = WeightRatio * 100f;

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

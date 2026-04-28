using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.InventorySystem
{
    /// <summary>
    /// ScriptableObject конфига инвентаря.
    /// Хранит максимальный вес и параметры каждого предмета по его id.
    /// </summary>
    [CreateAssetMenu(
        fileName = "New InventoryConfig",
        menuName = "Scriptable Objects/Character/Inventory/InventoryConfig")]
    public class InventoryConfig : ScriptableObject
    {
        [Tooltip("Максимальный суммарный вес инвентаря")]
        public float MaxWeight = 50f;

        [Tooltip("Список параметров предметов: id предмета → его параметры в инвентаре")]
        [SerializeField] private List<InventoryItemEntry> _entries = new();

        private Dictionary<string, InventoryItemConfig> _lookup;

        /// <summary>
        /// Возвращает параметры предмета по его id. Null если не найден.
        /// </summary>
        public InventoryItemConfig GetItemConfig(string itemId)
        {
            if (_lookup == null) BuildLookup();
            return _lookup.TryGetValue(itemId, out var cfg) ? cfg : null;
        }

        private void BuildLookup()
        {
            _lookup = new Dictionary<string, InventoryItemConfig>(_entries.Count);
            foreach (var entry in _entries)
            {
                if (!string.IsNullOrEmpty(entry.ItemId))
                    _lookup[entry.ItemId] = entry.Config;
            }
        }

        [Serializable]
        private class InventoryItemEntry
        {
            public string ItemId;
            public InventoryItemConfig Config;
        }
    }
}

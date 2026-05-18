using System.Collections.Generic;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.InventorySystem
{
    [CreateAssetMenu(
        fileName = "New InventoryConfig",
        menuName = "Scriptable Objects/Character/Inventory/InventoryConfig")]
    public class InventoryConfig : ScriptableObject
    {
        [Tooltip("Maximum total inventory weight.")]
        public float MaxWeight = 50f;

        [SerializeField] private List<ItemConfig> _items = new();

        private Dictionary<string, ItemConfig> _lookup;

        public IReadOnlyList<ItemConfig> Items => _items;

        public ItemConfig GetItemConfig(string itemId)
        {
            if (_lookup == null) BuildLookup();
            return _lookup.TryGetValue(itemId, out var cfg) ? cfg : null;
        }

        private void BuildLookup()
        {
            _lookup = new Dictionary<string, ItemConfig>(_items.Count);
            foreach (var item in _items)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.Id)) continue;
                _lookup[item.Id] = item;
            }
        }

        private void OnValidate()
        {
            MaxWeight = Mathf.Max(0f, MaxWeight);
            _lookup = null;
        }
    }
}

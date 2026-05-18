using UnityEngine;

namespace _Project.Code.Features.Character.MB.InventorySystem
{
    [CreateAssetMenu(
        fileName = "New InventoryItem",
        menuName = "Scriptable Objects/Character/Inventory/Item")]
    public class ItemConfig : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;
        [SerializeField, Min(0f)] private float _weight = 1f;
        [SerializeField, Min(1)] private int _maxStackSize = 1;
        [SerializeField] private bool _consumedOnUse = true;
        [SerializeField] private bool _canUse = true;

        public string Id => _id;
        public string DisplayName => string.IsNullOrWhiteSpace(_displayName) ? _id : _displayName;
        public Sprite Icon => _icon;
        public float Weight => _weight;
        public int MaxStackSize => _maxStackSize;
        public bool ConsumedOnUse => _consumedOnUse;
        public bool CanUse => _canUse;

        public virtual Item CreateItem()
        {
            return new Item(this);
        }

        public virtual bool Use(ItemUseContext context)
        {
            if (!_canUse) return false;

            Debug.Log($"Item '{DisplayName}' used by {context.User.name}");
            return true;
        }

        private void OnValidate()
        {
            _weight = Mathf.Max(0f, _weight);
            _maxStackSize = Mathf.Max(1, _maxStackSize);
        }
    }
}
